using Microsoft.AspNetCore.Builder;
using System;
using TrotterWatch.Core.Rbl;
using TrotterWatch.Models;

namespace Microsoft.AspNetCore.Builder
{
    public static class TrotterWatchExtensions
    {
        public static IApplicationBuilder UseTrotterWatch(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseMiddleware<TrotterWatch.Core.TrotterWatchMiddleware>(DefaultOptions());
        }

        public static IApplicationBuilder UseTrotterWatch(this IApplicationBuilder app, TrotterWatchOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

           return app.UseMiddleware<TrotterWatch.Core.TrotterWatchMiddleware>(options);
        }

        private static TrotterWatchOptions DefaultOptions()
        {
            return new TrotterWatchOptions();
        }
    }
}
