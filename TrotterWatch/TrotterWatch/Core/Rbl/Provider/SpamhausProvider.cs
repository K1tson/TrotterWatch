using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetTools;

namespace TrotterWatch.Core.Rbl.Provider
{
    internal sealed class SpamhausProvider : BaseRblProvider
    {
        private static readonly string _address = "zen.spamhaus.org";
        private static readonly Dictionary<string, IPAddressRange> SpamHausZones;

        static SpamhausProvider()
        {
            var locker = new object();

            lock (locker)
            {
                if (SpamHausZones != null)
                    SpamHausZones = TrotterWatchUtil.SpamhausZones();
            }
        }

        public SpamhausProvider(IPAddress requestIp, IPAddress dnsSrvIp) : base(_address, _address, requestIp, dnsSrvIp)
        {}

        public override async Task<IEnumerable<IRblResult>> CheckProvider()
        {
            var resultsCollection = new List<IRblResult>();

            if (RequestPtrName != null)
                resultsCollection.AddRange(await CheckHostName());

            resultsCollection.AddRange(await CheckIp());

            return resultsCollection;
        }

        private async Task<IEnumerable<IRblResult>> CheckHostName()
        {
            var ipresults = await Dns.GetHostAddressesAsync($"{RequestPtrName.HostName}.{_address}");
            return CheckRblResults(ipresults);
        }

        private async Task<IEnumerable<IRblResult>> CheckIp()
        {
            var ipresults = await Dns.GetHostAddressesAsync($"{RequestIpAddress}.{_address}");
            return CheckRblResults(ipresults);
        }

        private IEnumerable<IRblResult> CheckRblResults(IPAddress[] ipResults)
        {
            var rblResultsArr = new IRblResult[ipResults.Length];

            for (int i = 0; i < ipResults.Length; i++)
            {



            }


        }


        private IRblResult CreateRblResult(IPAddress returnCode)
        {
            var raw = returnCode.ToString();
            var ipAddress = returnCode;
            var listedOn = ListedZone(returnCode);
            var advisoryType = RblAdvisoryType.IllegalExploit;


            return new RblResult();
        }


        private string ListedZone(IPAddress returnCode)
        {
            foreach (var zone in SpamHausZones)
            {
                zone.Value.Contains(returnCode);
            }
        }

    }
}
