using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrotterWatch.Core.Rbl.Provider
{
    public interface IRblProvider
    {
        string Name { get; }
        string RblProviderUri { get; }
        IPAddress RequestIpAddress { get; }
        IPAddress DnsServerAddress { get; }
        Task<IEnumerable<IRblResult>> CheckProvider();

    }
}