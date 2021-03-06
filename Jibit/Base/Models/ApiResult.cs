using Jibit.Commons;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jibit.Base.Models;

public class ApiResult
{
    public bool isSuccess { get; set; }
    public ApiResultStatusCode status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string message { get; set; }

    public ApiResult(bool isSuccess, ApiResultStatusCode statusCode, string message = null)
    {
        this.isSuccess = isSuccess;
        this.status = statusCode;
        this.message = message ?? statusCode.ToDisplay();
    }
    public ApiResult()
    {
        isSuccess = true;
        status = ApiResultStatusCode.Success;
        message = status.ToDisplay();
    }

}

public class ApiResult<TData> : ApiResult
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TData Data { get; set; }

    public ApiResult(bool isSuccess, ApiResultStatusCode statusCode, TData data, string message = null)
        : base(isSuccess, statusCode, message)
    {
        Data = data;
    }

    public ApiResult()
    {
        isSuccess = true;
        status = ApiResultStatusCode.Success;
        message = status.ToDisplay();
    }
}

public enum ApiResultStatusCode
{
    [Display(Name = "عملیات با موفقیت انجام شد")]
    Success = (int)HttpStatusCode.OK,

    [Display(Name = "خطایی در سرور رخ داده است")]
    ServerError = (int)HttpStatusCode.InternalServerError,

    [Display(Name = "پارامتر های ارسالی معتبر نیست")]
    BadRequest = (int)HttpStatusCode.BadRequest,

    [Display(Name = "یافت نشد")]
    NotFound = (int)HttpStatusCode.NotFound,

    [Display(Name = "لیست خالی است")]
    ListEmpty = (int)HttpStatusCode.NoContent,

    [Display(Name = "کاربر موردنظر لاگین نمیباشد.")]
    UnAuthorized = (int)HttpStatusCode.Unauthorized,

    [Display(Name = "دسترسی غیرمجاز.")]
    Forbidden = (int)HttpStatusCode.Forbidden
}