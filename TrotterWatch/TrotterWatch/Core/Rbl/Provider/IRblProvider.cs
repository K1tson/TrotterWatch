using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrotterWatch.Logging;

namespace TrotterWatch.Core.Rbl.Provider
{
    public interface IRblProvider
    {
        string ProviderName { get; }
        string ProviderUrl { get; }
        RblType ProviderType { get; }
        Task<bool> CheckProvider(HttpContext context, TrotterLog logger);
    }
}