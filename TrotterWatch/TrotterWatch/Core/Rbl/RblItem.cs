using System;
using System.Collections.Generic;
using System.Text;

namespace TrotterWatch.Core.Rbl
{
    internal struct RblItem
    {
        public RblItem(string name, string url)
        {
            RblName = name;
            RblUrl = url;
        }

        public string RblName { get; }
        public string RblUrl { get; }
    }
}
