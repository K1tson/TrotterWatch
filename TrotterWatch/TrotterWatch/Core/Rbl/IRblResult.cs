using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NetTools;

namespace TrotterWatch.Core.Rbl
{
    public interface IRblResult
    {
        IEnumerable<IPAddressRange> ReturnCode { get; }
        string ListedOn { get; }
        RblAdvisoryType Type { get; }
    }
}
