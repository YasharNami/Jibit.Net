using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibit.Banking.Models;


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
