using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibit.Banking.Models;

public class CartToIBanResult
{
    public string Number { get; set; }
    public IBanInfo IbanInfo { get; set; }
}

public class Owner
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public class IBanInfo
{
    public string Bank { get; set; }
    public string IBan { get; set; }
    public string status { get; set; }
    public Owner[] Owners { get; set; }

}