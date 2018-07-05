using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using NetTools;
using TrotterWatch.Core.Rbl;
using TrotterWatch.Core.Rbl.Provider;

namespace TrotterWatch.Core
{
    internal static class TrotterWatchUtil
    {


        internal static IEnumerable<IRblProvider> GetRblCollection(IPAddress requestIp, IPAddress dnsSrv)
        {
            return new List<IRblProvider>
            {
                new SpamhausProvider(requestIp, dnsSrv)
            };
        }


        internal static IEnumerable<IRblResult> SpamhausResults()
        {
            return new List<IRblResult>
            {
                new RblResult(new List<IPAddressRange>
                {
                    new IPAddressRange(IPAddress.Parse("127.0.0.2"), IPAddress.Parse("127.0.0.3")),
                    new IPAddressRange(IPAddress.Parse("127.0.0.9"))
                },
                    "sbl.spamhaus.org",
                    RblAdvisoryType.SpamService),

                new RblResult(new List<IPAddressRange>
                    {
                        new IPAddressRange(IPAddress.Parse("127.0.0.4"), IPAddress.Parse("127.0.0.7"))
                    },
                    "xbl.spamhaus.org",
                    RblAdvisoryType.IllegalExploit),

                new RblResult(new List<IPAddressRange>
                    {
                        new IPAddressRange(IPAddress.Parse("127.0.0.10"), IPAddress.Parse("127.0.0.11")),
                    },
                    "pbl.spamhaus.org",
                    RblAdvisoryType.SpamService),
            };
        } 
    }
}
