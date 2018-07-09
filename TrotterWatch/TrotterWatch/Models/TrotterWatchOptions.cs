using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TrotterWatch.Core;
using TrotterWatch.Core.Rbl.Provider;

namespace TrotterWatch.Models
{
    public sealed class TrotterWatchOptions
    {
        public IEnumerable<IRblProvider> RblProviders { get; set; }
        public bool ContinueChecks { get; set; }
        public ILogger Logger { get; set; }
    }
}
