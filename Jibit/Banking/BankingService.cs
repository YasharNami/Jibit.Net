using Jibit.Banking.Resources;
using Jibit.Base;
using Jibit.Base.Models;
using Jibit.Commons;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Resources;
using System.Text.Json;

namespace Jibit.Banking;

public class BankingService 
{
    protected readonly JibitSettings _jibitSettings;
    public BankingService(JibitSettings setting) => _jibitSettings = setting;


    /// <summary>
    ///  تبدیل شماره کارت بانکی به شماره شبا
    /// </summary>
    public ApiResult<CartToIBanResult> CardToIBan(string cardNumber)
    {
        CheckAuthentication();
        return Get<CartToIBanResult>($"cards?number={cardNumber}&iban=true");
    }
    public async Task<ApiResult<CartToIBanResult>> CardToIBanAsync(string cardNumber)
    {
        await CheckAuthenticationAsync();
        return await GetAsync<CartToIBanResult>($"cards?number={cardNumber}&iban=true");
    }

    /// <summary>
    /// دریافت اطلاعات کارت بانکی از طریق شماره کارت
    /// </summary>
    public ApiResult<CardInfoResult> GetCardInfo(string cardNumber)
    {
        CheckAuthentication();
        return Get<CardInfoResult>($"cards?number={cardNumber}");
    }
    public async Task<ApiResult<CardInfoResult>> GetCardInfoAsync(string cardNumber)
    {
        await CheckAuthenticationAsync();
        return await GetAsync<CardInfoResult>($"cards?number={cardNumber}");
    }
    /// <summary>
    /// دریافت اطلاعات کارت بانکی از طریق شماره شبا
    /// </summary>
    public ApiResult<IBanInfoResult> GetIbanInfo(string ibanNumber)
    {
        CheckAuthentication();
        return Get<IBanInfoResult>($"ibans?value=IR{ibanNumber}");
    }
    public async Task<ApiResult<IBanInfoResult>> GetIbanInfoAsync(string ibanNumber)
    {
        await CheckAuthenticationAsync();
        return await GetAsync<IBanInfoResult>($"ibans?value=IR{ibanNumber}");
    }
    /// <summary>
    ///  بررسی مطابقت شماره شبا و نام و نام خانوادگی
    /// </summary>
    public ApiResult<IBanAndFullNameMatchModel> IsIBanAndFullNameMatch(string ibanNumber, string fullName)
    {
        CheckAuthentication();
        return Get<IBanAndFullNameMatchModel>($"services/matching?iban=IR{ibanNumber}&name={fullName}");
    }
    public async Task<ApiResult<IBanAndFullNameMatchModel>> IsIBanAndFullNameMatchAsync(string ibanNumber,string fullName)
    {
        await CheckAuthenticationAsync();
        return await GetAsync<IBanAndFullNameMatchModel>($"services/matching?iban=IR{ibanNumber}&name={fullName}");
    }
    /// <summary>
    ///  بررسی مطابقت شماره کارت و نام و نام خانوادگی
    /// </summary>
    public ApiResult<CardNumberAndFullNameMatchModel> IsCardNumberAndFullNameMatch(string cardNumber, string fullName)
    {
        CheckAuthentication();
        return Get<CardNumberAndFullNameMatchModel>($"services/matching?cardNumber={cardNumber}&name={fullName}");
    }
    public async Task<ApiResult<CardNumberAndFullNameMatchModel>> IsCardNumberAndFullNameMatchAsync(string cardNumber, string fullName)
    {
        await CheckAuthenticationAsync();
        return await GetAsync<CardNumberAndFullNameMatchModel>($"services/matching?cardNumber={cardNumber}&name={fullName}");
    }

    #region Authentication Methods
    protected AuthenticationResult Authenticate()
    {
        var authenticationResult = Post<AuthenticationResult>("tokens/generate",
            new AuthenthicationRequest(_jibitSettings.ApiKey, _jibitSettings.SecretKey));
        _jibitSettings.CreatedAt = DateTime.Now;
        _jibitSettings.AccessToken = authenticationResult.Data.AccessToken;
        _jibitSettings.RefreshToken = authenticationResult.Data.RefreshToken;
        return authenticationResult.Data;
    }
    protected async Task<AuthenticationResult> AuthenticateAsync()
    {
        var authenticationResult = await PostAsync<AuthenticationResult>("tokens/generate",
            new AuthenthicationRequest(_jibitSettings.ApiKey, _jibitSettings.SecretKey));
        _jibitSettings.CreatedAt = DateTime.Now;
        _jibitSettings.AccessToken = authenticationResult.Data.AccessToken;
        _jibitSettings.RefreshToken = authenticationResult.Data.RefreshToken;
        return authenticationResult.Data;
    }

    protected AuthenticationResult RefreshAuthentication()
    {
        var authenticationResult = Post<AuthenticationResult>("tokens/refresh",
            new RefreshTokenRequest(_jibitSettings.AccessToken, _jibitSettings.RefreshToken));
        _jibitSettings.CreatedAt = DateTime.Now;
        _jibitSettings.AccessToken = authenticationResult.Data.AccessToken;
        _jibitSettings.RefreshToken = authenticationResult.Data.RefreshToken;
        return authenticationResult.Data;
    }
    protected async Task<AuthenticationResult> RefreshAuthenticationAsync()
    {
        var authenticationResult = await PostAsync<AuthenticationResult>("tokens/refresh",
            new RefreshTokenRequest(_jibitSettings.AccessToken, _jibitSettings.RefreshToken));
        _jibitSettings.CreatedAt = DateTime.Now;
        _jibitSettings.AccessToken = authenticationResult.Data.AccessToken;
        _jibitSettings.RefreshToken = authenticationResult.Data.RefreshToken;
        return authenticationResult.Data;
    }

    protected bool IsTokenExpired() => _jibitSettings.CreatedAt.AddHours(24) > DateTime.Now;
    protected bool HasToken() => _jibitSettings.AccessToken is not default(string) && _jibitSettings.RefreshToken is not default(string);

    #endregion

    #region Http Methods
    protected ApiResult<TResult> Get<TResult>(string route)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jibitSettings.AccessToken);
        var httpResponseMessage = httpClient.GetAsync($"{_jibitSettings.BaseApiUrl}/{route}");
        string value = httpResponseMessage.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var response = JsonSerializer.Deserialize<TResult>(value);
        var result = new ApiResult<TResult>(true,ApiResultStatusCode.Success, response);

        if (!httpResponseMessage.Result.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ErrorResult>(value);
            result.isSuccess = false;
            ResourceManager myManager = new ResourceManager(typeof(BankingErrors));
            result.message = myManager.GetString(error.Code) is not default(string) ? myManager.GetString(error.Code) : ApiResultStatusCode.BadRequest.ToDisplay();
            result.status = ApiResultStatusCode.BadRequest; return result;
        }
        return result;
    }

    protected async Task<ApiResult<TResult>> GetAsync<TResult>(string route)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jibitSettings.AccessToken);
        var httpResponseMessage = await httpClient.GetAsync($"{_jibitSettings.BaseApiUrl}/{route}");
        string value = await httpResponseMessage.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<TResult>(value);
        var result = new ApiResult<TResult>(true, ApiResultStatusCode.Success, response);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ErrorResult>(value);
            result.isSuccess = false;
            ResourceManager myManager = new ResourceManager(typeof(BankingErrors));
            result.message = myManager.GetString(error.Code) is not default(string) ? myManager.GetString(error.Code) : ApiResultStatusCode.BadRequest.ToDisplay();
            result.status = ApiResultStatusCode.BadRequest;
        }
        return result;
    }

    protected ApiResult<TResult> Post<TResult>(string route, object data)
    {
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (_jibitSettings.AccessToken is not default(string))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jibitSettings.AccessToken);

        var httpResponseMessage = httpClient.PostAsJsonAsync($"{_jibitSettings.BaseApiUrl}/{route}", data);
        string value = httpResponseMessage.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        var response = JsonSerializer.Deserialize<TResult>(value);
        var result = new ApiResult<TResult>(true, ApiResultStatusCode.Success, response);

        if (!httpResponseMessage.Result.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ErrorResult>(value);
            result.isSuccess = false;
            ResourceManager myManager = new ResourceManager(typeof(BankingErrors));
            result.message = myManager.GetString(error.Code) is not default(string) ? myManager.GetString(error.Code) : ApiResultStatusCode.BadRequest.ToDisplay(); 
            result.status = ApiResultStatusCode.BadRequest;
        }
        return result;
    }

    protected async Task<ApiResult<TResult>> PostAsync<TResult>(string route, object data)
    {
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (_jibitSettings.AccessToken is not default(string))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jibitSettings.AccessToken);

        var httpResponseMessage = await httpClient.PostAsJsonAsync($"{_jibitSettings.BaseApiUrl}/{route}", data);
        string value = await httpResponseMessage.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<TResult>(value);
        var result = new ApiResult<TResult>(true, ApiResultStatusCode.Success, response);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<ErrorResult>(value);
            result.isSuccess = false;
            ResourceManager myManager = new ResourceManager(typeof(BankingErrors));
            result.message = myManager.GetString(error.Code) is not default(string) ? myManager.GetString(error.Code) : ApiResultStatusCode.BadRequest.ToDisplay(); result.status = ApiResultStatusCode.BadRequest;
        }
        return result;
    }

    protected async void CheckAuthentication()
    {
        if (!HasToken()) Authenticate();
        else if (IsTokenExpired()) RefreshAuthentication();
    }

    protected async Task CheckAuthenticationAsync()
    {
        if (!HasToken()) await AuthenticateAsync();
        else if (IsTokenExpired()) await RefreshAuthenticationAsync();
    }

    #endregion
}