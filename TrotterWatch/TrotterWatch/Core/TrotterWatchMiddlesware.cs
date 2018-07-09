using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrotterWatch.Core.Rbl.Provider;
using TrotterWatch.Logging;
using TrotterWatch.Models;

namespace TrotterWatch.Core
{
    public sealed class TrotterWatchMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _continueChecks;
        private readonly IEnumerable<IRblProvider> _rblProviders;
        private readonly TrotterLog _logger;

        public TrotterWatchMiddleware(RequestDelegate next, TrotterWatchOptions options)
        {
            _next = next;
            _continueChecks = options.ContinueChecks;
            _rblProviders = options.RblProviders;
            _logger = new TrotterLog(options.Logger);
        }

        public async Task Invoke(HttpContext context)
        {
            //If IPAddress is private then ignore.
            if (IsInternal(context.Connection.RemoteIpAddress))
            {
                _logger.LogEvent(LogLevel.Information, $"Not checking the remote IP {context.Connection.RemoteIpAddress} as it's private.");
                await _next.Invoke(context);
                return;
            }

            _logger.LogEvent(LogLevel.Information, $"The remote IP {context.Connection.RemoteIpAddress} is public.");

            var isListed = false;

            foreach (var provider in _rblProviders)
            {
                _logger.LogEvent(LogLevel.Information, $"Checking Rbl Provider {provider.ProviderName} against Url {provider.ProviderUrl}...");
                var listed = await provider.CheckProvider(context, _logger);

                if (listed && !_continueChecks)
                {
                    isListed = true;
                    _logger.LogEvent(LogLevel.Information, $"Remote IP/PTR is listed on {provider.ProviderName}");
                    _logger.LogEvent(LogLevel.Information, $"Cancelling further Rbl checks as ContinueChecks is set to false");
                    break;
                }

                _logger.LogEvent(LogLevel.Information, $"Moving onto next provider...");
                isListed = true;
            }

            if (!isListed)
            {
                _logger.LogEvent(LogLevel.Information, $"Remote IP/PTR is NOT listed on any of the Rbl Providers");
                _logger.LogEvent(LogLevel.Information, $"Moving onto the next Middleware Component");
                await _next.Invoke(context);
            }
             
            //Short-circuits request pipeline
        }

        /// <summary>
        /// Check's if an IP address is private or public.
        /// </summary>
        /// <param name="requestIp"></param>
        /// <returns></returns>
        public bool IsInternal(IPAddress requestIp)
        {
            if (requestIp.ToString() == "::1") return true;

            byte[] ip = requestIp.GetAddressBytes();

            switch (ip[0])
            {
                case 10:
                case 127:
                    return true;
                case 172:
                    return ip[1] >= 16 && ip[1] < 32;
                case 192:
                    return ip[1] == 168;
                default:
                    return false;
            }
        }
    }
}
