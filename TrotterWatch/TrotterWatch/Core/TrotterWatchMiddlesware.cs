using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrotterWatch.Core.Rbl.Provider;
using TrotterWatch.Models;

namespace TrotterWatch.Core
{
    public sealed class TrotterWatchMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _continueChecks;
        private readonly IEnumerable<IRblProvider> _rblProviders;

        public TrotterWatchMiddleware(RequestDelegate next, IOptions<TrotterWatchOptions> options)
        {
            _next = next;
            _continueChecks = options.Value.ContinueChecks;
            _rblProviders = options.Value.RblProviders;
        }

        public async Task Invoke(HttpContext context)
        {
            var isListed = false;

            foreach (var provider in _rblProviders)
            {
                var listed = await provider.CheckProvider(context);

                if (listed && !_continueChecks)
                {
                    isListed = true;
                    break;
                }

                isListed = true;
            }

            if (!isListed)
              await _next.Invoke(context);

            //Short-circuits request pipeline
        }



    }
}
