using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Numerics;

namespace TheSquid.Numerics.Tests
{
    [TestClass]
    public class PowCachedExtensionTests
    {
        [TestMethod]
        [DataRow(1000000, 100, 100)]
        [DataRow(100000, 100, 1000)]
        [DataRow(100000, 1000, 100)]
        [DataRow(10000, 1000, 1000)]
        public void PowChachedRandomTest(int iterationsCount, int maxBasement, int maxExponent)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var stopwatchPow = new Stopwatch();
            var stopwatchPowCached = new Stopwatch();

            for (long i = 0; i < iterationsCount; ++i)
            {
                BigInteger basement = random.Next(1, maxBasement);
                int exponent = random.Next(1, maxExponent);

                stopwatchPow.Start();
                var pow = basement.Pow(exponent);
                stopwatchPow.Stop();

                stopwatchPowCached.Start();
                var powChached = basement.PowChached(exponent);
                stopwatchPowCached.Stop();

                var messagePow = $"pow={pow}\npowChached={powChached}";
                Assert.IsTrue(pow == powChached, messagePow);
            }

            var powElapsed = stopwatchPow.ElapsedMilliseconds;
            var powCachedElapsed = stopwatchPowCached.ElapsedMilliseconds;
            var messageElapsed = $"powElapsed={powElapsed}\npowCachedElapsed={powCachedElapsed}";
            Assert.IsTrue(powCachedElapsed <= powElapsed, messageElapsed);
        }
    }
}