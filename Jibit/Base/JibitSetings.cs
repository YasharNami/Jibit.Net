using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibit.Base;

public class JibitSettings
{
    protected readonly JibitSettings _jibitSettings;
    public JibitSettings(string apiKey, string secretKey)
    {
        ApiKey = apiKey;
        SecretKey = secretKey;
        BaseApiUrl ="https://napi.jibit.ir/ide/v1";
    }

    public string ApiKey { get; set; }
    public string SecretKey { get; set; }
    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public string BaseApiUrl { get; set; }
}