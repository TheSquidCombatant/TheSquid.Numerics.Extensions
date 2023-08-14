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
        [DataRow(10000, 100, 100)]
        [DataRow(100000, 100, 100)]
        [DataRow(1000000, 100, 100)]
        public void PowCachedRandomTest(int iterationsCount, int maxBasement, int maxExponent)
        {
            var random = new Random(DateTime.Now.Millisecond);
            var stopwatchPow = new Stopwatch();
            var stopwatchPowCached = new Stopwatch();
            PowCachedExtension.ShrinkCacheData(0);

            for (long i = 0; i < iterationsCount; ++i)
            {
                BigInteger basement = random.Next(0, maxBasement);
                int exponent = random.Next(0, maxExponent);

                stopwatchPow.Start();
                var pow = basement.Pow(exponent);
                stopwatchPow.Stop();

                stopwatchPowCached.Start();
                var powCached = basement.PowCached(exponent);
                stopwatchPowCached.Stop();

                const string messagePow = "\npow={0}\npowCached={1}";
                Assert.IsTrue(pow == powCached, messagePow, pow, powCached);
            }

            var powElapsed = stopwatchPow.ElapsedMilliseconds;
            var powCachedElapsed = stopwatchPowCached.ElapsedMilliseconds;
            const string messageElapsed = "\npowElapsed={0}\npowCachedElapsed={1}";
            Assert.IsTrue(powCachedElapsed <= powElapsed, messageElapsed, powElapsed, powCachedElapsed);
        }
    }
}