using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TrotterWatch.Core.Rbl
{
    public struct RblResult : IRblResult
    {
        public RblResult(string raw, IPAddress returnCode, string listedOn, RblAdvisoryType type)
        {
            Raw = raw;
            ListedOn = listedOn;
            ReturnCode = returnCode;
            Type = type;
        }

        public string Raw { get; }
        public IPAddress ReturnCode { get; }
        public string ListedOn { get; }
        public RblAdvisoryType Type { get; }
    }
}
