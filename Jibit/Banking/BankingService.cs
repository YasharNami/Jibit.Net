using Jibit.Banking.Models;
using Jibit.Base;
using Jibit.Base.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Jibit.Banking;

public class BankingService 
{
    protected readonly JibitSettings _jibitSettings;
    public BankingService(JibitSettings setting) => _jibitSettings = setting;


    // تبدیل شماره کارت بانکی به شماره شبا
    public CartToIBanResult CardToIBan(string cardNumber)
    {
        CheckAuthentication();
        return Get<CartToIBanResult>($"cards?number={cardNumber}&iban=true");
    }
    public async Task<CartToIBanResult> CardToIBanAsync(string cardNumber)
    {
        await CheckAuthenticationAsync();
        return await GetAsync<CartToIBanResult>($"cards?number={cardNumber}&iban=true");
    }

    // دریافت اطلاعات کارت بانکی از طریق شماره کارت
    public CardInfoResult GetCardinfo(string cardNumber)
    {
        CheckAuthentication();
        return Get<CardInfoResult>($"cards?number={cardNumber}");
    }
    public async Task<CardInfoResult> GetCardinfoAsync(string cardNumber)
    {
        await CheckAuthenticationAsync();
        return await GetAsync<CardInfoResult>($"cards?number={cardNumber}");
    }

    // دریافت اطلاعات کارت بانکی از طریق شماره شبا
    public IBanInfoResult GetIbanInfo(string iBan)
    {
        CheckAuthentication();
        return Get<IBanInfoResult>($"ibans?value=IR{iBan}");
    }
    public async Task<IBanInfoResult> GetIbanInfoAsync(string ibanNumber)
    {
        await CheckAuthenticationAsync();
        return await GetAsync<IBanInfoResult>($"ibans?value=IR{ibanNumber}");
    }

    #region Authentication Methods
    protected AuthenticationResult Authenticate()
    {
        var authenticationResult = Post<AuthenticationResult>("tokens/generate",
            new AuthenthicationRequest(_jibitSettings.ApiKey, _jibitSettings.SecretKey));
        _jibitSettings.CreatedAt = DateTime.Now;
        _jibitSettings.AccessToken = authenticationResult.AccessToken;
        _jibitSettings.RefreshToken = authenticationResult.RefreshToken;
        return authenticationResult;
    }
    protected async Task<AuthenticationResult> AuthenticateAsync()
    {
        var authenticationResult = await PostAsync<AuthenticationResult>("tokens/generate",
            new AuthenthicationRequest(_jibitSettings.ApiKey, _jibitSettings.SecretKey));
        _jibitSettings.CreatedAt = DateTime.Now;
        _jibitSettings.AccessToken = authenticationResult.AccessToken;
        _jibitSettings.RefreshToken = authenticationResult.RefreshToken;
        return authenticationResult;
    }

    protected AuthenticationResult RefreshAuthentication()
    {
        var authenticationResult = Post<AuthenticationResult>("tokens/refresh",
            new RefreshTokenRequest(_jibitSettings.AccessToken, _jibitSettings.RefreshToken));
        _jibitSettings.CreatedAt = DateTime.Now;
        _jibitSettings.AccessToken = authenticationResult.AccessToken;
        _jibitSettings.RefreshToken = authenticationResult.RefreshToken;
        return authenticationResult;
    }
    protected async Task<AuthenticationResult> RefreshAuthenticationAsync()
    {
        var authenticationResult = await PostAsync<AuthenticationResult>("tokens/refresh",
            new RefreshTokenRequest(_jibitSettings.AccessToken, _jibitSettings.RefreshToken));
        _jibitSettings.CreatedAt = DateTime.Now;
        _jibitSettings.AccessToken = authenticationResult.AccessToken;
        _jibitSettings.RefreshToken = authenticationResult.RefreshToken;
        return authenticationResult;
    }

    protected bool IsTokenExpired() => _jibitSettings.CreatedAt.AddHours(24) > DateTime.Now;
    protected bool HasToken() => _jibitSettings.AccessToken is not default(string) && _jibitSettings.RefreshToken is not default(string);

    #endregion

    #region Http Methods
    protected TResult Get<TResult>(string route)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jibitSettings.AccessToken);
        var httpResponseMessage = httpClient.GetAsync($"{_jibitSettings.BaseApiUrl}/{route}");
        string value = httpResponseMessage.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        if (httpResponseMessage.Result.IsSuccessStatusCode)
        {
            var response = JsonConvert.DeserializeObject<TResult>(value);
            return response;
        }
        var errorResult = JsonConvert.DeserializeObject<ErrorResult>(value);
        throw new ArgumentException(errorResult.Code);
    }

    protected async Task<TResult> GetAsync<TResult>(string route)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jibitSettings.AccessToken);
        var httpResponseMessage = await httpClient.GetAsync($"{_jibitSettings.BaseApiUrl}/{route}");
        string value = await httpResponseMessage.Content.ReadAsStringAsync();
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var response = JsonConvert.DeserializeObject<TResult>(value);
            return response;
        }
        var errorResult = JsonConvert.DeserializeObject<ErrorResult>(value);
        throw new ArgumentException(errorResult.Code);
    }

    protected TResult Post<TResult>(string route, object data)
    {
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (_jibitSettings.AccessToken is not default(string))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jibitSettings.AccessToken);

        var httpResponseMessage = httpClient.PostAsJsonAsync($"{_jibitSettings.BaseApiUrl}/{route}", data);
        string value = httpResponseMessage.Result.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        if (httpResponseMessage.Result.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<TResult>(value);

        var errorResult = JsonConvert.DeserializeObject<ErrorResult>(value);
        throw new ArgumentException(errorResult.Code);
    }

    protected async Task<TResult> PostAsync<TResult>(string route, object data)
    {
        using var httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (_jibitSettings.AccessToken is not default(string))
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jibitSettings.AccessToken);

        var httpResponseMessage = await httpClient.PostAsJsonAsync($"{_jibitSettings.BaseApiUrl}/{route}", data);
        string value = await httpResponseMessage.Content.ReadAsStringAsync();
        if (httpResponseMessage.IsSuccessStatusCode)
            return JsonConvert.DeserializeObject<TResult>(value);

        var errorResult = JsonConvert.DeserializeObject<ErrorResult>(value);
        throw new ArgumentException(errorResult.Code);
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