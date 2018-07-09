using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using TrotterWatch.Core;

namespace TrotterWatch.Logging
{
    public class TrotterLog
    {
        private readonly ILogger _logger;

        public TrotterLog(ILogger logger)
        {
            _logger = logger;
        }

        public void LogEvent(LogLevel level, string message)
        {
            _logger?.Log(level,$"TrotterWatch {level}: {message}");
        }

    }
}
