using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NetTools;

namespace TrotterWatch.Core.Rbl
{
    public struct RblResult : IRblResult
    {
        public RblResult(IEnumerable<IPAddressRange> returnCode, string listedOn, RblAdvisoryType type)
        {
            ListedOn = listedOn;
            ReturnCode = returnCode;
            Type = type;
        }
        public IEnumerable<IPAddressRange> ReturnCode { get; }
        public string ListedOn { get; }
        public RblAdvisoryType Type { get; }
    }
}
