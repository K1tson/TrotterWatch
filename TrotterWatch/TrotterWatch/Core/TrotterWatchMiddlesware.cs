using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Kitson.SimpleDNS;
using Kitson.SimpleDNS.Models;
using Kitson.SimpleDNS.Packet;
using Kitson.SimpleDNS.Packet.Answer;
using Kitson.SimpleDNS.Packet.Flags;
using Kitson.SimpleDNS.Packet.Question;
using Kitson.SimpleDNS.Transmission;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrotterWatch.CustomException;
using TrotterWatch.Models;

namespace TrotterWatch.Core
{
    public sealed class TrotterWatchMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TrotterWatchOptions _options;

        public TrotterWatchMiddleware(RequestDelegate next, IOptions<TrotterWatchOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress;
            var hostName = await Dns.GetHostEntryAsync(ip);
            var result = false;

            if (result)
            {
               
                
                return;
            }

            // Call the next middleware delegate in the pipeline 
            await _next.Invoke(context);

        }

        private static async Task<IEnumerable<IResource>> QueryRblServer(IPAddress rblAddress, string reverseHostname, IPAddress remoteIpAddress)
        {
            var revResult = await DnsQuery(rblAddress, hostname);

            if (queryResult.Header.Parameters.Response != ResponseCode.Ok) //If error from DNS response
            {
                throw new TrotterWatchException($"RblServer {rblAddress} has responded with {queryResult.Header.Parameters.Response.ToString()}");
            }

            return queryResult.Resources;
        }

        private static async Task<IDnsPacket> DnsQuery(IPAddress rblAddress, string hostname)
        {
            var question = new Question(hostname, QType.A);

            SimpleDnsPacket packet = new SimpleDnsPacket(question, rblAddress); //Only supports UDP - UDP used by default
            var queryResult = await Query.SimpleAsync(packet);
            return queryResult;
        }



    }
}
