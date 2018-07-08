using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
            HttpContext context = new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = legitIP}
            };
            
            var provider = new RblProvider("Spamhaus Zen", "zen.spamhaus.org", RblType.Both);

            var result = await provider.CheckProvider(context);

            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status200OK);
        }
    }
}
