using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NetTools;

namespace TrotterWatch.Core
{
    internal static class TrotterWatchUtil
    {
        internal static Dictionary<string, IEnumerable<IPAddressRange>> SpamhausZones()
        {
            return new Dictionary<string, IEnumerable<IPAddressRange>>
            {
                {
                    "sbl.spamhaus.org",
                    new List<IPAddressRange>
                    {
                        new IPAddressRange(IPAddress.Parse("127.0.0.2"), IPAddress.Parse("127.0.0.3")),
                        new IPAddressRange(IPAddress.Parse("127.0.0.9"))
                    }
                },
                {
                    "xbl.spamhaus.org",
                    new List<IPAddressRange>
                    {
                        new IPAddressRange(IPAddress.Parse("127.0.0.4"), IPAddress.Parse("127.0.0.7"))
                    }
                },
                {
                    "pbl.spamhaus.org",
                    new List<IPAddressRange>
                    {
                        new IPAddressRange(IPAddress.Parse("127.0.0.10"), IPAddress.Parse("127.0.0.11")),
                    }
                }
            };
        }
    }
}
