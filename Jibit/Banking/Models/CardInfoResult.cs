

namespace Jibit.Banking;


public class CardInfoResult
{
    public string Number { get; set; }
    public CardInfo CardInfo { get; set; }

}
public class CardInfo
{
    public string Bank { get; set; }
    public string Type { get; set; }
    public string OwnerName { get; set; }
    public string DepositNumber { get; set; }
}
