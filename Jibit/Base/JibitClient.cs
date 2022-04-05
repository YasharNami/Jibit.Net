using Jibit.Banking;
using Jibit.Base.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Jibit.Base;

public class JibitClient
{
    protected readonly JibitSettings _jibitSettings;

    public JibitClient(string apiKey,string secretKey)
    {
        _jibitSettings = new JibitSettings(apiKey,secretKey);
        BankingEndpoints = new BankingService(_jibitSettings);
    }
    public BankingService BankingEndpoints { get; set; }

}