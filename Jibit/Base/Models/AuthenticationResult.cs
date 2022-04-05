using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibit.Base.Models;

public class AuthenthicationRequest
{
    public AuthenthicationRequest(string apiKey, string secretKey)
    {
       ApiKey = apiKey;
       SecretKey = secretKey;
    }

    public string ApiKey { get; set; }
    public string SecretKey { get; set; }
}

public class RefreshTokenRequest
{
    public RefreshTokenRequest(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class AuthenticationResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}