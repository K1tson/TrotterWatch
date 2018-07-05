using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NetTools;

[assembly: InternalsVisibleTo("TrotterWatchTests")]
namespace TrotterWatch.Core.Rbl.Provider
{
    internal sealed class SpamhausProvider : BaseRblProvider
    {
        private static readonly string _address = "zen.spamhaus.org";
        private static readonly IEnumerable<IRblResult> SpamHausZones;

        static SpamhausProvider()
        {
            var locker = new object();

            lock (locker)
            {
                if (SpamHausZones != null)
                    SpamHausZones = TrotterWatchUtil.SpamhausResults();
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
            try
            {
                var ipresults = await Dns.GetHostAddressesAsync($"{RequestPtrName.HostName}.{_address}");
                return CheckRblResults(ipresults);
            }
            catch (SocketException)
            {
                return new List<IRblResult>(0);
            }
        }

        private async Task<IEnumerable<IRblResult>> CheckIp()
        {
            try
            {
                var ipresults = await Dns.GetHostAddressesAsync($"{FlipOctets(RequestIpAddress)}.{_address}");
                return CheckRblResults(ipresults);
            }
            catch (SocketException)
            {
                return new List<IRblResult>(0);
            }
        }

        private string FlipOctets(IPAddress requestIp)
        {
            var flippedAddress = requestIp.ToString().Split('.').Reverse().ToArray();
            var formattedIp = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                formattedIp.Append(flippedAddress[i]);

                if(i != 3)
                    formattedIp.Append(".");
            }

            return formattedIp.ToString();
        }

        private IEnumerable<IRblResult> CheckRblResults(IPAddress[] ipResults)
        {
            var rblResultsArr = new IRblResult[ipResults.Length];

            for (int i = 0; i < ipResults.Length; i++)
            {
                rblResultsArr[i] = GetListedZone(ipResults[i]);
            }

            return rblResultsArr;
        }

        private IRblResult GetListedZone(IPAddress returnCode)
        {
            return SpamHausZones.Single(n => n.ReturnCode.Any(range => range.Contains(returnCode)));
        }

    }
}
