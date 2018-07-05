using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrotterWatch.Core.Rbl;
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
            //Get Requested IP
            //ToDo: Check if this actually works
            var requestIp = context.Connection.RemoteIpAddress;

            //Instantiate Singleton RBL Providers
            //ToDo: Use custom DNS server or not??
            var rblCollection = TrotterWatchUtil.GetRblCollection(requestIp, IPAddress.Parse("8.8.8.8"));

            bool listed = false;
            List<IRblResult> rblResults = null;

            foreach (var rblProvider in rblCollection)
            {
                //Checks RBL Provider to see if listed
                var result = await rblProvider.CheckProvider();

                if (result.Any())
                {
                    if (listed == false)
                    {
                        listed = true;
                        rblResults = new List<IRblResult>();
                    }
                    
                    //Add's provider results to list.
                    rblResults.AddRange(result);
                }
            }

            //Not Listed in RBL
            if (!listed)
            {
                // Call the next middleware delegate in the pipeline 
                await _next.Invoke(context);
            }
            else
            {
                //Add's X- Headers
                foreach (var result in rblResults)
                {
                    context.Response.Headers.Add("X-TrotterWatch", $"You Plonka! Your IP {requestIp} is listed on {result.ListedOn} due to type \"{result.Type}\"");
                }

                //Add's Forbidden Response
                context.Response.StatusCode = 403;

                //ToDo: Add Logging
            }

           
            //Short-circuits request pipeline
        }


    }
}
