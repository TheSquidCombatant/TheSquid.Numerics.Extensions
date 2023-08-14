using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Numerics;

namespace TheSquid.Numerics.Tests
{
    [TestClass]
    public class NthRootExtensionTests
    {
        [TestMethod]
        [DataRow(10000, 1000, 1000)]
        [DataRow(10000, 10000, 100)]
        [DataRow(10000, 100000, 10)]
        public void NthRootRandomTest(int iterationsCount, int maxBasement, int maxExponent)
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

        [TestMethod]
        [DataRow(@"Data\Speed\00*.txt")]
        [DataRow(@"Data\Speed\01*.txt")]
        [DataRow(@"Data\Speed\02*.txt")]
        [DataRow(@"Data\Speed\03*.txt")]
        [DataRow(@"Data\Speed\04*.txt")]
        [DataRow(@"Data\Speed\05*.txt")]
        [DataRow(@"Data\Speed\06*.txt")]
        [DataRow(@"Data\Speed\07*.txt")]
        [DataRow(@"Data\Speed\08*.txt")]
        [DataRow(@"Data\Speed\09*.txt")]
        [DataRow(@"Data\Speed\10*.txt")]
        [DataRow(@"Data\Speed\11*.txt")]
        [DataRow(@"Data\Speed\12*.txt")]
        [DataRow(@"Data\Speed\13*.txt")]
        [DataRow(@"Data\Speed\14*.txt")]
        [DataRow(@"Data\Speed\15*.txt")]
        [DataRow(@"Data\Speed\16*.txt")]
        [DataRow(@"Data\Speed\17*.txt")]
        [DataRow(@"Data\Speed\18*.txt")]
        [DataRow(@"Data\Speed\19*.txt")]
        [DataRow(@"Data\Speed\20*.txt")]
        [DataRow(@"Data\Speed\21*.txt")]
        [DataRow(@"Data\Speed\22*.txt")]
        [DataRow(@"Data\Speed\23*.txt")]
        public void NthRootSpeedTest(string filePathTemplate)
        {
            for (var k = 0; k < 10; ++k)
            {
                var filePath = filePathTemplate.Replace("*", k.ToString());
                var json = File.ReadAllText(filePath);
                var dataSet = JsonConvert.DeserializeObject<DataSet>(json);

                var basement = BigInteger.Parse(dataSet.Basement);
                var exponent = int.Parse(dataSet.Exponent);
                var source = BigInteger.Parse(dataSet.Power);
                var isExactResult = dataSet.IsExactResult;

                var actualBasement = source.NthRoot(exponent, out var actualExactResult);
                const string message = "exponent={0}, source={1}, basement={2}, isExactResult={3}";
                Assert.AreEqual(isExactResult, actualExactResult, message, exponent, source, basement, isExactResult);
                Assert.AreEqual(basement, actualBasement, message, exponent, source, basement, isExactResult);
            }
        }
    }
}