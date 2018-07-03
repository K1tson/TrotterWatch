using System;
using System.Collections.Generic;
using System.Text;

namespace TrotterWatch.CustomException
{
    public class TrotterWatchException : Exception
    {
        public TrotterWatchException(string message)  : base(message)
        {}
    }
}
