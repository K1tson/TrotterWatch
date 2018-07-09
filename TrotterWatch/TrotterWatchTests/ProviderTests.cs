using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrotterWatch.Core;
using TrotterWatch.Core.Rbl;
using TrotterWatch.Models;

namespace TrotterWatchTests
{
    [TestClass]
    public class ProviderTests
    {
        /// <summary>
        /// Tests a legitmate IP against an RBL provider. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task LegitIp()
        {
            var legitIp = IPAddress.Parse("8.8.8.8");
            var trotterMiddlware = new TrotterWatchMiddleware(httpContext =>Task.CompletedTask, new TrotterWatchOptions());

            HttpContext context = new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = legitIp}
            };

            await trotterMiddlware.Invoke(context);
            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status200OK);
        }

        /// <summary>
        /// Tests a blocked IP againsts an RBL provider.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task BlockedIp()
        {
            var blockedIp = IPAddress.Parse("192.99.110.138"); //Blocked with SPFBL RBL
            var trotterMiddlware = new TrotterWatchMiddleware(httpContext => Task.CompletedTask, new TrotterWatchOptions());

            HttpContext context = new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = blockedIp }
            };

            await trotterMiddlware.Invoke(context);
            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status403Forbidden);
        }

        /// <summary>
        /// Checks if RBL items are deserialised correctly
        /// </summary>
        [TestMethod]
        public void TestProviderFactoryDefault()
        {
           var results = ProviderFactory.ReturnRblItems();
           Assert.IsTrue(results.Any());
        }

    }
}
