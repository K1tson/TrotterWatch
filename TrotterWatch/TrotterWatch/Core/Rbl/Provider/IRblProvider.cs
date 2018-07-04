using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TrotterWatch.Core.Rbl.Provider
{
    public interface IRblProvider
    {
        string Name { get; }
        string RblProviderUri { get; }
        IPAddress RequestIpAddress { get; }
        IPAddress DnsServerAddress { get; }

    }
}