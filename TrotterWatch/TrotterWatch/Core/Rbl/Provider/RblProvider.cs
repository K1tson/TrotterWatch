using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TrotterWatch.Core.Rbl.Provider
{
    public class RblProvider : IRblProvider
    {
        private IPHostEntry _requestHostname;
        private HttpContext _context;

        public RblProvider(string name, string url, RblType type)
        {
            ProviderName = name;
            ProviderUrl = url;
            ProviderType = type;
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
        /// <returns></returns>
        public async Task<bool> CheckProvider(HttpContext context)
        {
            _context = context;

            var requestIp = context.Connection.RemoteIpAddress;
            var formattedIp = FormatIp(requestIp);

            if (ProviderType != RblType.Both && ProviderType != RblType.Hostname)
                return await RblHostLookup(formattedIp);

            await PtrLookupIp(requestIp);
            return await RblHostLookup(_requestHostname.HostName);
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
            catch (SocketException)
            {
                //ToDo: Add Logging
            }
        }

        /// <summary>
        /// Checks the request hostname against the remote Rbl database.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> RblHostLookup(string query)
        {
            try
            {
                var ipresult = await Dns.GetHostAddressesAsync($"{query}.{ProviderUrl}");
                return CheckResults(ipresult);
            }
            catch (SocketException)
            {
                return false;
                //ToDo: Add Logging
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
                queryFormat.Append(octetArr);

                if (i != 3)
                {
                    queryFormat.Append('.');
                }
            }

            return queryFormat.ToString();
        }


        private bool CheckResults(IPAddress[] results)
        {
            if (results.Any())
            {
                StampHttpHeader(results);
                return true;
            }

            return false;
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
            }
        }
    }
}
