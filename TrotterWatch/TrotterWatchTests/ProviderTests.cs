using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrotterWatch;
using TrotterWatch.Core.Rbl.Provider;

namespace TrotterWatchTests
{
    [TestClass]
    public class ProviderTests
    {
        [TestMethod]
        public async Task SpamhausLegitIP()
        {
            var legitIP = IPAddress.Parse("5.148.97.26");
            var provider = new SpamhausProvider(legitIP, null);

            var result = await provider.CheckProvider();

            Assert.IsTrue(result.Any());
        }
    }
}
