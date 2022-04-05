using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jibit.Base.Models;

public class ErrorResult
{
    public string Code { get; set; }
    public string Message { get; set; }
    public List<ErrorViewModel> Errors { get; set; }
}

public class ErrorViewModel
{
    public string Code { get; set; }
    public string Message { get; set; }
}
