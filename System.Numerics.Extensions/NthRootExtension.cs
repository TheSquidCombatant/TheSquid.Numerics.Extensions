using System.Collections.Generic;
using System.Linq;

namespace System.Numerics.Extensions
{
    /// <summary>
    /// C# implementation of an extension method to quickly calculate an Nth root for BigInteger value. By Nikolai TheSquid.
    /// </summary>
    public static partial class NthRootExtension
    {
        /// <summary>
        /// Nth root for big non negative integer values.
        /// </summary>
        /// <param name="source">Root radicand value.</param>
        /// <param name="exponent">Root degree value.</param>
        /// <param name="wishExactResult">If the root is not extracted completely: False for approximate result or True for null.</param>
        /// <returns>By default, it returns the exact integer value, in case of the root is completely extracted, otherwise it returns null.</returns>
        /// <exception cref="ArithmeticException">The value of the exponent leads to an ambiguous results.</exception>
        /// <exception cref="NotSupportedException">You don't have to pay attention to it.</exception>
        public static BigInteger? NthRoot(this ref BigInteger source, int exponent, bool wishExactResult = true)
        {
            const string negativeValuesMessage = "Negative exponent values and negative source values are not supported.";
            if ((source < 0) || (exponent < 0)) throw new ArgumentOutOfRangeException(negativeValuesMessage);
            const string ambiguousResultMessage = "The value of the exponent leads to an ambiguous results.";
            if (exponent == 0) throw new ArithmeticException(ambiguousResultMessage);

            if ((source == 0) || (source == 1)) return source;

            var floor = 10;

            var quotient = (int)Math.Ceiling(BigInteger.Log(source, floor) / exponent);
            var digitsRootCount = (int)(0.8 * quotient * (BigInteger.Log(floor, 2) + 1));
            var newtonRootCount = (int)(Math.Log2(BigInteger.Log(BigInteger.Pow(floor, quotient) - BigInteger.Pow(floor, quotient - 1), 2)) * exponent / 2 + 3);

            var min = new[] { digitsRootCount, newtonRootCount }.Min();

            if (min == digitsRootCount) return GetRootByDigits(ref source, exponent, wishExactResult);
            if (min == newtonRootCount) return GetRootByNewton(ref source, exponent, wishExactResult);

            const string notSupportedMethodMessage = "Not supported nthroot calculation method.";
            throw new NotSupportedException(notSupportedMethodMessage);
        }

        /// <summary>
        /// Method for calculating Nth roots for large N degrees.
        /// </summary>
        /// <remarks>
        /// Digit by digit extraction method.
        /// </remarks>
        private static BigInteger? GetRootByDigits(this ref BigInteger source, int exponent, bool wishExactResult = true)
        {
            var ambiguousResultMessage = "The value of the exponent leads to an ambiguous results.";
            if (exponent == 0) throw new ArithmeticException(ambiguousResultMessage);
            if ((source == 0) || (source == 1)) return source;

            var floor = 10;

            var digitsShift = BigInteger.Pow(floor, exponent);
            var currentSource = source;
            var intermediateResults = new LinkedList<BigInteger>();
            intermediateResults.AddLast(currentSource);

            while (currentSource >= digitsShift)
            {
                currentSource = currentSource / digitsShift;
                intermediateResults.AddLast(currentSource);
            }

            var minResult = new BigInteger(1);
            var maxResult = new BigInteger(floor);
            var isExactValue = false;
            var sourceNode = intermediateResults.Last;
            BigInteger currentResult = 0, currentPower = 0;

            while (sourceNode != null)
            {
                currentSource = sourceNode.Value;
                isExactValue = false;

                if (sourceNode != intermediateResults.Last)
                {
                    currentResult *= floor;
                    currentPower *= digitsShift;

                    var k = exponent * currentPower / currentResult;
                    var b = currentPower - k * currentResult;
                    var x = (currentSource - b) / k + 1;

                    if (x < maxResult) maxResult = x;
                }

                currentResult = (minResult + maxResult) / 2;
                BigInteger? previousResult = null;

                while (previousResult != currentResult)
                {
                    currentPower = BigInteger.Pow(currentResult, exponent);
                    if (currentPower == currentSource) { isExactValue = true; break; }
                    previousResult = currentResult;
                    if (currentPower < currentSource) minResult = currentResult; else maxResult = currentResult;
                    currentResult = (minResult + maxResult) / 2;
                }

                minResult = currentResult * floor;
                maxResult = (currentResult + 1) * floor;
                sourceNode = sourceNode.Previous;
            }

            return (isExactValue || !wishExactResult ? currentResult : null);
        }

        /// <summary>
        /// Method for calculating Nth roots for small N degrees.
        /// </summary>
        /// <remarks>
        /// By Newton simplest extraction method.
        /// </remarks>S
        private static BigInteger? GetRootByNewton(this ref BigInteger source, int exponent, bool wishExactResult = true)
        {
            var ambiguousResultMessage = "The value of the exponent leads to an ambiguous results.";
            if (exponent == 0) throw new ArithmeticException(ambiguousResultMessage);
            if ((source == 0) || (source == 1)) return source;

            var floor = 10;

            var quotient = (int)Math.Ceiling(BigInteger.Log(source, floor) / exponent);
            var currentResult = BigInteger.Pow(floor, quotient);

            BigInteger? previousPreviousResult = null;
            BigInteger? previousResult = null;

            while ((previousResult != currentResult) && (previousPreviousResult != currentResult))
            {
                var counterweight = BigInteger.Pow(currentResult, (exponent - 1));
                previousPreviousResult = previousResult;
                previousResult = currentResult;
                currentResult = (((exponent - 1) * currentResult) + (source / counterweight)) / exponent;
            }

            var isExactValue = (BigInteger.Pow(currentResult, exponent) == source);

            return (isExactValue || !wishExactResult ? currentResult : null);
        }
    }
}