using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;
using System.Text;

namespace TheSquid.Numerics.Extensions.Tests;

[TestClass]
public class NextBigIntegerExtensionTests
{
    /// <summary>
    /// Checks that all generated values are within the specified range.
    /// </summary>
    [TestMethod]
    [DataRow(100, 1)]
    [DataRow(100, 10)]
    [DataRow(100, 100)]
    [DataRow(100, 1000)]
    public void NextBigIntegerRandomRangeTest(
        int iterationsCount,
        int borderValueLength)
    {
        var random = new Random(DateTime.Now.Millisecond);

        for (int i = 0; i < iterationsCount; ++i)
        {
            var minValueLength = random.Next(1, borderValueLength);
            var minValueRaw = new StringBuilder(minValueLength);
            if (minValueLength > 1) minValueRaw.Append(random.Next(1, 10));
            for (int k = 0; k < minValueLength; ++k) minValueRaw.Append(random.Next(0, 10));
            var minValue = BigInteger.Parse(minValueRaw.ToString());
            if (random.Next(2) == 1) minValue *= -1;

            var maxValueLength = random.Next(1, borderValueLength);
            var maxValueRaw = new StringBuilder(maxValueLength);
            if (maxValueLength > 1) maxValueRaw.Append(random.Next(1, 10));
            for (int k = 0; k < maxValueLength; ++k) maxValueRaw.Append(random.Next(0, 10));
            var maxValue = BigInteger.Parse(maxValueRaw.ToString());
            if (random.Next(2) == 1) maxValue *= -1;

            if (minValue > maxValue)
            {
                var tmp = minValue;
                minValue = maxValue;
                maxValue = tmp;
            }

            var resultValue = random.NextBigInteger(minValue, maxValue);
            const string message = "minValue={0}, maxValue={1}, resultValue={2}";
            Assert.IsTrue(minValue <= resultValue, message, minValue, maxValue, resultValue);
            Assert.IsTrue(resultValue <= maxValue, message, minValue, maxValue, resultValue);
        }
    }

    /// <summary>
    /// Checks that any value within a given range can be generated.
    /// </summary>
    [TestMethod]
    [DataRow(1000000, 10, 100)]
    [DataRow(1000000, 100, 100)]
    [DataRow(1000000, 1000, 100)]
    public void NextBigIntegerRandomEquiprobabilityTest(
        int iterationsCount,
        int randomRangeSize,
        int maxDigitsCount)
    {
        var random = new Random(DateTime.Now.Millisecond);
        var maxValueRaw = new StringBuilder(random.Next(1, 10).ToString(), maxDigitsCount);

        for (int i = 1; i < maxDigitsCount; ++i)
        {
            maxValueRaw.Append(random.Next(0, 10));
        }

        var maxValue = BigInteger.Parse(maxValueRaw.ToString());
        var minValue = maxValue - randomRangeSize + 1;
        var resultCounter = new int[randomRangeSize];

        for (int i = 0; i < iterationsCount; ++i)
        {
            var result = random.NextBigInteger(minValue, maxValue);
            var index = (int)(result - minValue);
            ++resultCounter[index];
        }

        const string message = "value={0}, count={1}";

        for (int i = 0; i < randomRangeSize; ++i)
        {
            Assert.IsTrue(resultCounter[i] > 0, message, i + minValue, resultCounter[i]);
        }
    }

    /// <summary>
    /// Checks that all generated values are with the specified decimal length.
    /// </summary>
    [TestMethod]
    [DataRow(1000, 1, 10)]
    [DataRow(1000, 10, 100)]
    [DataRow(1000, 100, 1000)]
    public void NextBigIntegerRandomLengthTest(
        int iterationsCount,
        int minLength,
        int maxLength)
    {
        var random = new Random(DateTime.Now.Millisecond);

        for (int i = 0; i < iterationsCount; ++i)
        {
            var result = random.NextBigInteger(minLength, maxLength);
            var resultLength = result.ToString().Length;
            const string message = "minLength={0}, maxLength={1}, resultLength={2}";
            Assert.IsTrue(minLength <= resultLength, message, minLength, maxLength, resultLength);
            Assert.IsTrue(resultLength <= maxLength, message, minLength, maxLength, resultLength);
        }
    }
}