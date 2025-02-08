using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Numerics;

namespace TheSquid.Numerics.Extensions.Tests;

[TestClass]
public class NthRootExtensionTests
{
    /// <summary>
    /// Checks that any value obtained when extracting the root coincides with the value previously raised to a power.
    /// </summary>
    [TestMethod]
    [DataRow(10000, 1000, 1000)]
    [DataRow(10000, 10000, 100)]
    [DataRow(10000, 100000, 10)]
    public void NthRootRandomTest(
        int iterationsCount,
        int maxBasement,
        int maxExponent)
    {
        var random = new Random(DateTime.Now.Millisecond);

        for (int i = 0; i < iterationsCount; ++i)
        {
            var exponent = random.Next(2, maxExponent);
            var basementLess = (BigInteger)random.Next(1, maxBasement);
            var sourceLess = BigInteger.Pow(basementLess, exponent);
            var basementGreater = basementLess + 1;
            var sourceGreater = BigInteger.Pow(basementGreater, exponent);
            var source = random.NextBigInteger(sourceLess, sourceGreater);
            var expectedExactResult = ((source == sourceLess) || (source == sourceGreater));
            var expectedBasement = (source == sourceGreater ? basementGreater : basementLess);
            var actualBasement = source.NthRoot(exponent, out var actualExactResult);
            const string message = "exponent={0}, source={1}, basement={2}, isExactResult={3}";
            Assert.AreEqual(expectedExactResult, actualExactResult, message, exponent, source, expectedBasement, expectedExactResult);
            Assert.AreEqual(expectedBasement, actualBasement, message, exponent, source, expectedBasement, expectedExactResult);
        }
    }
}