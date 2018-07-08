using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using TrotterWatch.Core.Rbl.Provider;

namespace TrotterWatch.Models
{
    public sealed class TrotterWatchOptions
    {
        public IEnumerable<IRblProvider> RblProviders { get; }
        public bool ContinueChecks { get; }
    }
}
