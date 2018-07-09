using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TrotterWatch.Logging;

namespace TrotterWatch.Core.Rbl.Provider
{
    public class RblProvider : IRblProvider
    {
        private IPHostEntry _requestHostname;
        private HttpContext _context;
        private bool _isListed;
        private TrotterLog _logger;

        public RblProvider(string name, string url, RblType type)
        {
            ProviderName = name;
            ProviderUrl = url;
            ProviderType = type;
            _isListed = false;
        }

        /// <summary>
        /// Rbl Provider Name
        /// </summary>
        public string ProviderName { get; }

        /// <summary>
        /// Rbl Provider Url
        /// </summary>
        public string ProviderUrl { get; }

        /// <summary>
        /// Rbl Provier Type e.g. Hostname == Rbl checks hostname
        /// </summary>
        public RblType ProviderType { get; }

        /// <summary>
        /// Checks requests IP and/or Hostname against the Rbl's database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public async Task<bool> CheckProvider(HttpContext context, TrotterLog logger)
        {
            _logger = logger;
            _context = context;

            var requestIp = context.Connection.RemoteIpAddress;
            var formattedIp = FormatIp(requestIp);

            if (ProviderType == RblType.Both || ProviderType == RblType.Hostname)
            {
                _logger.LogEvent(LogLevel.Information, "Initiating PTR Lookup & Hostname check...");
                await PtrLookupIp(requestIp);
                await RblHostLookup(_requestHostname.HostName);
                _logger.LogEvent(LogLevel.Information, "PTR Lookup & Hostname check completed!");
            }

            _logger.LogEvent(LogLevel.Information, "Initiating IP Lookup against RBL");
            await RblHostLookup(formattedIp);
         
            _logger.LogEvent(LogLevel.Information, $"RBL Lookup has completed for {ProviderName}");
            return _isListed;
        }

        /// <summary>
        /// Check's if the request IP has a valid PTR record. 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private async Task PtrLookupIp(IPAddress ip)
        {
            try
            {
                _requestHostname = await Dns.GetHostEntryAsync(ip);
            }
            catch (SocketException ex)
            {
                _logger.LogEvent(LogLevel.Error, $"PTR Lookup Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks the request hostname against the remote Rbl database.
        /// </summary>
        /// <returns></returns>
        private async Task RblHostLookup(string query)
        {
            try
            {
                var ipresult = await Dns.GetHostAddressesAsync($"{query}.{ProviderUrl}");
                CheckResults(ipresult);
            }
            catch (SocketException ex)
            {
                _logger.LogEvent(LogLevel.Error, $"RBL Lookup Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Formats the IPAddress into the format needed for the Rbl IP query
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private string FormatIp(IPAddress ip)
        {
            StringBuilder queryFormat = new StringBuilder();
            var octetArr = ip.ToString().Split('.').Reverse().ToArray();

            for (int i = 0; i < octetArr.Length; i++)
            {
                queryFormat.Append(octetArr[i]);

                if (i != 3)
                {
                    queryFormat.Append('.');
                }
            }

            return queryFormat.ToString();
        }

        /// <summary>
        /// Checks if the Rbl request returned any results
        /// </summary>
        /// <param name="results"></param>
        private void CheckResults(IPAddress[] results)
        {
            if (results.Any())
            {
                StampHttpHeader(results);
                _isListed = true;
            }
        }

        /// <summary>
        /// Stamps the Http Response Header with a reason as to why the requested IP is blocked. 
        /// </summary>
        /// <param name="results"></param>
        private void StampHttpHeader(IPAddress[] results)
        {
            foreach (var result in results)
            {
                var requestIp = _context.Connection.RemoteIpAddress;
                var key = "X-TrotterWatch";
                var value = $"{requestIp} is listed on {ProviderName}({ProviderUrl}) due to {result}";
                _context.Response.Headers.Add(key, value);
                _context.Response.StatusCode = StatusCodes.Status403Forbidden;
                _logger.LogEvent(LogLevel.Information, "RBL has been stamped to header");
            }
        }
    }
}
