using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TrotterWatch.Core.Rbl
{
    public interface IRblResult
    {
        string Raw { get; }
        IPAddress ReturnCode { get; }
        string ListedOn { get; }
        RblAdvisoryType Type { get; }
    }
}
