using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
            var requestIp = context.Connection.RemoteIpAddress;

        
            var result = false;

            if (result)
            {
               
                
                return;
            }

            // Call the next middleware delegate in the pipeline 
            await _next.Invoke(context);

        }


    }
}
