using Jibit.Banking;

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