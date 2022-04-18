using Jibit.Commons;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jibit.Base.Models;

public class ApiResult
{
    public bool isSuccess { get; set; }
    public ApiResultStatusCode status { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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