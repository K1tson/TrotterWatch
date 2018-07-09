using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrotterWatch;
using TrotterWatch.Core.Rbl.Provider;
using TrotterWatch.Logging;

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

            var loggerFact = new LoggerFactory();
            loggerFact.AddDebug();
            var logger = loggerFact.CreateLogger("TrotterWatchLogs");
            var trotterLogger = new TrotterLog(logger);

            HttpContext context = new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = legitIp}
            };
            
            var provider = new RblProvider("spfbl.net", "dnsbl.spfbl.net", RblType.Ip);

            var result = await provider.CheckProvider(context, trotterLogger);

            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status200OK);
        }

        /// <summary>
        /// Tests a blocked IP againsts an RBL provider.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task BlockedIp()
        {

            var loggerFact = new LoggerFactory();
            loggerFact.AddDebug();
            var logger = loggerFact.CreateLogger("TrotterWatchLogs");
            var trotterLogger = new TrotterLog(logger);

            var legitIp = IPAddress.Parse("192.99.110.138");

            HttpContext context = new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = legitIp }
            };

            var provider = new RblProvider("spfbl.net", "dnsbl.spfbl.net", RblType.Ip);

            var result = await provider.CheckProvider(context, trotterLogger);

            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status403Forbidden);
        }

        /// <summary>
        /// Tests a clean PTR record (for hostname) and IP against RBL provider. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task LegitPtrHostname()
        {
            var loggerFact = new LoggerFactory();
            loggerFact.AddDebug();
            var logger = loggerFact.CreateLogger("TrotterWatchLogs");
            var trotterLogger = new TrotterLog(logger);

            var legitIp = IPAddress.Parse("8.8.8.8");

            HttpContext context = new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = legitIp }
            };

            var provider = new RblProvider("Zen Spamhaus", "zen.spamhaus.org", RblType.Both);

            var result = await provider.CheckProvider(context, trotterLogger);

            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status200OK);
        }

        /// <summary>
        /// Tests a blocked PTR record (for hostname) and IP against RBL provider. 
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task BlockedPtrHostname()
        {
            var loggerFact = new LoggerFactory();
            loggerFact.AddDebug();
            var logger = loggerFact.CreateLogger("TrotterWatchLogs");
            var trotterLogger = new TrotterLog(logger);

            var legitIp = IPAddress.Parse("74.124.200.143");

            HttpContext context = new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = legitIp }
            };

            var provider = new RblProvider("Barracuda", "b.barracudacentral.org", RblType.Both);

            var result = await provider.CheckProvider(context, trotterLogger);

            Assert.IsTrue(context.Response.StatusCode == StatusCodes.Status403Forbidden);
        }


    }
}
