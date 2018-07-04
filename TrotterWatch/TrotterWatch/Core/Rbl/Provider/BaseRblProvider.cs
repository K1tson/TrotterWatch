using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TrotterWatch.Core.Rbl.Provider
{
    public abstract class BaseRblProvider : IRblProvider
    {
        protected BaseRblProvider(string name, string rblUri, IPAddress requestIpAddress, IPAddress dnsServerAddress)
        {
            Name = name;
            RblProviderUri = rblUri;
            RequestIpAddress = requestIpAddress;
            DnsServerAddress = dnsServerAddress;
            RequestPtrName = GetPtrRecord();
        }

        public string Name { get; }
        public string RblProviderUri { get; }
        public IPAddress RequestIpAddress { get; }
        public IPHostEntry RequestPtrName { get; }
        public IPAddress DnsServerAddress { get; }

        public abstract Task<IEnumerable<IRblResult>> CheckProvider();

        private IPHostEntry GetPtrRecord()
        {
            //ToDo: Resolve slow response when fails to find PTR
            try
            {
                return Dns.GetHostEntry(RequestIpAddress);
            }
            catch (SocketException)
            {
                //ToDo: Add logging here!
                return null;
            }
        }
    }
}
