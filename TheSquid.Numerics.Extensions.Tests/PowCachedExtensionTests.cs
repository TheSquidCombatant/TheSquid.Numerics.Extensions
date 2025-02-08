using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Numerics;

namespace TheSquid.Numerics.Extensions.Tests;

[TestClass]
public class PowCachedExtensionTests
{
    /// <summary>
    /// Checks that any value calculated using caching exponentiation matches the value calculated by the standard method.
    /// </summary>
    [TestMethod]
    [DataRow(10000, 100, 100)]
    [DataRow(100000, 100, 100)]
    [DataRow(1000000, 100, 100)]
    public void PowCachedRandomTest(
        int iterationsCount,
        int maxBasement,
        int maxExponent)
    {
        // initial set up
        var random = new Random(DateTime.Now.Millisecond);
        var stopwatchPow = new Stopwatch();
        var stopwatchPowCached = new Stopwatch();
        var stopwatchWarmup = new Stopwatch();
        const int warmupTimeMillisecond = 1000;

        // initial warm up
        stopwatchWarmup.Start();
        while (stopwatchWarmup.Elapsed.TotalMilliseconds < warmupTimeMillisecond)
        {
            BigInteger basement = random.Next(0, maxBasement);
            int exponent = random.Next(0, maxExponent);

            var powValue = basement.Pow(exponent);
            var powCachedValue = basement.PowCached(exponent);

            const string messagePow = "\npowValue={0}\npowCachedValue={1}";
            Assert.IsTrue(powValue == powCachedValue, messagePow, powValue, powCachedValue);
        }
        stopwatchWarmup.Stop();
        PowCachedExtension.ShrinkCacheData(0);

        // main run
        for (long i = 0; i < iterationsCount; ++i)
        {
            BigInteger basement = random.Next(0, maxBasement);
            int exponent = random.Next(0, maxExponent);

            stopwatchPow.Start();
            var powValue = basement.Pow(exponent);
            stopwatchPow.Stop();

            stopwatchPowCached.Start();
            var powCachedValue = basement.PowCached(exponent);
            stopwatchPowCached.Stop();

            const string messagePow = "BigInteger.Pow and BigInteger.PowCached show different results.";
            if (powValue != powCachedValue) throw new ArithmeticException(messagePow);
        }

        // check results
        var powElapsed = stopwatchPow.Elapsed.TotalMilliseconds;
        var powCachedElapsed = stopwatchPowCached.Elapsed.TotalMilliseconds;
        const string messageElapsed = "BigInteger.Pow elapsed {0} ms, BigInteger.PowCached elapsed {1} ms.";
        Assert.IsTrue(powCachedElapsed <= powElapsed, messageElapsed, powElapsed, powCachedElapsed);
        Console.WriteLine(string.Format(messageElapsed, powElapsed, powCachedElapsed));
    }
}