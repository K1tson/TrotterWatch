using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TrotterWatch.Core.Rbl.Provider;
using TrotterWatch.Models;

namespace TrotterWatch.Core.Rbl
{
    public static class ProviderFactory
    {
        private static IEnumerable<IRblProvider> _rblItems;
        private static readonly string DefaultRblItems;

        static ProviderFactory()
        {
            _rblItems = null;
            var uri = new UriBuilder(Assembly.GetAssembly(typeof(ProviderFactory)).CodeBase);
            var path = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            DefaultRblItems = Path.Combine(path,"Core", "Rbl", "DefaultRbls", "rbls.json");
        }

        public static IEnumerable<IRblProvider> ReturnRblItems(string rblJson = null)
        {
            if (_rblItems != null)
                return _rblItems;

            var locker = new object();

            lock (locker)
            {
                if (_rblItems != null)
                    return _rblItems;

                return _rblItems = InitiateRblSingle(rblJson);
            }

        }

        private static IEnumerable<IRblProvider> InitiateRblSingle(string rblJson)
        {
            return DeserialiseJson(rblJson ?? DefaultRblItems);
        }

        private static IEnumerable<IRblProvider> DeserialiseJson(string rblJson)
        {
            var text = File.ReadAllText(rblJson);
            var rblItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<RblItem>>(text);
            return rblItems.Select(item => (IRblProvider) Activator.CreateInstance(typeof(RblProvider), item.Name, item.Url, item.Type)).ToList();
        }
       


    }
}
