using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;
using System.Text;

namespace TheSquid.Numerics.Tests
{
    [TestClass]
    public class NextBigIntegerExtensionTests
    {
        [TestMethod]
        [DataRow(100, 10000)]
        [DataRow(1000, 1000)]
        [DataRow(10000, 100)]
        [DataRow(100000, 10)]
        [DataRow(1000000, 1)]
        public void NextBigIntegerRandomTest(int iterationsCount, int borderValueLength)
        {
            var random = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < iterationsCount; ++i)
            {
                var minValueLength = random.Next(1, borderValueLength);
                var minValueRaw = new StringBuilder(minValueLength);
                if (minValueLength > 1) minValueRaw.Append(random.Next(1, 10));
                for (int k = 0; k < minValueLength; ++k) minValueRaw.Append(random.Next(0, 10));
                var min = BigInteger.Parse(minValueRaw.ToString());
                if (random.Next(2) == 1) min *= -1;

                var maxValueLength = random.Next(1, borderValueLength);
                var maxValueRaw = new StringBuilder(maxValueLength);
                if (maxValueLength > 1) maxValueRaw.Append(random.Next(1, 10));
                for (int k = 0; k < maxValueLength; ++k) maxValueRaw.Append(random.Next(0, 10));
                var max = BigInteger.Parse(maxValueRaw.ToString());
                if (random.Next(2) == 1) max *= -1;

                if (min > max)
                {
                    var tmp = min;
                    min = max;
                    max = tmp;
                }

                var result = random.NextBigInteger(min, max);
                const string message = "min={0}, max={1}, result={2}";
                Assert.IsTrue(min <= result, message, min, max, result);
                Assert.IsTrue(result <= max, message, min, max, result);
            }
        }
    }
}