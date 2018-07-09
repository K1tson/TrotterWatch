using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TrotterWatch.Core.Rbl.Provider;

namespace TrotterWatch.Models
{
    public class RblItem
    {
        public string Name { get; set; }

        public string Url { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RblType Type { get; set; }
    }
}
