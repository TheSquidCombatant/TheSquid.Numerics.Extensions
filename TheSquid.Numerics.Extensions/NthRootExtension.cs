using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace TheSquid.Numerics.Extensions;

/// <summary>
/// C# implementation of an extension method to quickly calculate an Nth root (including square
/// root) for BigInteger value. By Nikolai TheSquid.
/// </summary>
public static partial class NthRootExtension
{
    /// <summary>
    /// Nth root for non negative BigInteger values.
    /// </summary>
    /// <param name="source">
    /// Root radicand value.
    /// </param>
    /// <param name="exponent">
    /// Root degree value.
    /// </param>
    /// <param name="isExactResult">
    /// True value for exact result or False value for approximate result.
    /// </param>
    /// <returns>
    /// It returns the exact value, in case of the root is completely extracted, otherwise it
    /// returns nearest value from below.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Negative source values are not supported.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Negative exponent values are not supporte.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value of the exponent leads to an ambiguous results.
    /// </exception>
    public static BigInteger NthRoot(
        this ref BigInteger source,
        int exponent,
        out bool isExactResult)
    {
        // validation of input parameter values
        const string sourceNegativeValuesMessage = "Negative source values are not supported.";
        if (source < 0) throw new ArgumentOutOfRangeException(nameof(source), sourceNegativeValuesMessage);
        const string exponentNegativeValuesMessage = "Negative exponent values are not supported.";
        if (exponent < 0) throw new ArgumentOutOfRangeException(nameof(exponent), exponentNegativeValuesMessage);
        const string exponentAmbiguousResultMessage = "The value of the exponent leads to an ambiguous results.";
        if (exponent == 0) throw new ArgumentOutOfRangeException(nameof(exponent), exponentAmbiguousResultMessage);
        // stub for the case of trivial values of the radical expression
        isExactResult = true;
        if ((source == 0) || (source == 1)) return source;
        // calculate the worst-case cost for each root extraction method
        var digitsRootWeight = ByDigits.GetWeight(ref source, exponent, out var isDigitsApplicable);
        var newtonRootWeight = ByNewton.GetWeight(ref source, exponent, out var isNewtonApplicable);
        var doubleRootWeight = ByDouble.GetWeight(ref source, exponent, out var isDoubleApplicable);
        // choose the fastest root extraction method for current parameters
        var min = new[] { digitsRootWeight, newtonRootWeight, doubleRootWeight }.Min();
        // call the fastest root extraction method for current parameters
        if ((min == digitsRootWeight) && isDigitsApplicable) return ByDigits.GetRoot(ref source, exponent, out isExactResult);
        if ((min == newtonRootWeight) && isNewtonApplicable) return ByNewton.GetRoot(ref source, exponent, out isExactResult);
        if ((min == doubleRootWeight) && isDoubleApplicable) return ByDouble.GetRoot(ref source, exponent, out isExactResult);
        // stub if something went wrong when extending the functionality
        const string notSupportedMethodMessage = "Not supported nthroot calculation method.";
        throw new NotSupportedException(notSupportedMethodMessage);
    }

    internal static class ByDigits
    {
        /// <summary>
        /// Method for calculating Nth roots for large N degrees.
        /// </summary>
        /// <remarks>
        /// Digit-by-digit extraction method.
        /// <summary>
        /// Method for calculating Nth roots for large N degrees.
        /// </summary>
        /// <remarks>
        /// Digit-by-digit extraction method.
        /// </remarks>
        /// </remarks>
        public static BigInteger GetRoot(
            ref BigInteger source,
            int exponent,
            out bool isExactResult)
        {
            // calculate how many digits of accuracy are cut off from the radicand value for each digit of root value
            const int floor = 10;
            var digitsShift = BigInteger.Pow(floor, exponent);
            var currentSource = source;
            var intermediateResults = new LinkedList<BigInteger>();
            intermediateResults.AddLast(currentSource);
            // remember the values of the radical expression intermediate in accuracy
            while (currentSource >= digitsShift)
            {
                currentSource = currentSource / digitsShift;
                intermediateResults.AddLast(currentSource);
            }
            // initial setting for the digits-by-digits root extraction method
            isExactResult = false;
            var minResult = new BigInteger(1);
            var maxResult = new BigInteger(floor);
            var sourceNode = intermediateResults.Last;
            BigInteger currentResult = 0, currentPower = 0;
            // looking for the root one by one digit starting from the most significant digit
            while (sourceNode != null)
            {
                // initial setting for the current iteration of digits-by-digits extraction
                currentSource = sourceNode.Value;
                isExactResult = false;
                // followed by an optional, but almost zero-cost optimization
                if (sourceNode != intermediateResults.Last)
                {
                    // use data from previous iteration
                    currentResult *= floor;
                    currentPower *= digitsShift;
                    // build a tangent (y=k*x+b) to the point of the previous root value 
                    var k = exponent * currentPower / currentResult;
                    var b = currentPower - k * currentResult;
                    var x = (currentSource - b) / k + 1;
                    // reduces approximately 20% of iterations
                    if (x < maxResult) maxResult = x;
                }
                // initial setting for the binary search method
                currentResult = (minResult + maxResult) / 2;
                BigInteger previousResult = 0;
                // looking for the new last digit of the root using the binary search
                while (previousResult != currentResult)
                {
                    currentPower = BigInteger.Pow(currentResult, exponent);
                    if (currentPower == currentSource) { isExactResult = true; break; }
                    previousResult = currentResult;
                    if (currentPower < currentSource) minResult = currentResult; else maxResult = currentResult;
                    currentResult = (minResult + maxResult) / 2;
                }
                // shift digits to the left for the next iteration
                minResult = currentResult * floor;
                maxResult = (currentResult + 1) * floor;
                sourceNode = sourceNode.Previous;
            }
            // return accumulated root value
            return currentResult;
        }

        /// <summary>
        /// Method for calculating weight of digit-by-digit extraction method.
        /// </summary>
        /// <remarks>
        /// The formula for calculating weight is very approximate and relative, so I will be grateful if someone can clarify.
        /// </remarks>
        public static long GetWeight(
            ref BigInteger source,
            int exponent,
            out bool isApplicableMethod)
        {
            const int floor = 10;
            isApplicableMethod = true;
            var quotient = (int)Math.Ceiling(BigInteger.Log(source, floor) / exponent);
            var weight = (long)(0.8 * quotient * (BigInteger.Log(floor, 2) + 1));
            return weight;
        }
    }

    internal static class ByNewton
    {
        /// <summary>
        /// Method for calculating Nth roots for small N degrees.
        /// </summary>
        /// <remarks>
        /// By Newton simplest extraction method.
        /// </remarks>
        public static BigInteger GetRoot(
            ref BigInteger source,
            int exponent,
            out bool isExactResult)
        {
            // calculate the initial guess (equal or greater) the root value with accuracy up to one digit
            const int floor = 10;
            var quotient = (int)Math.Ceiling(BigInteger.Log(source, floor) / exponent);
            var currentResult = BigInteger.Pow(floor, quotient);
            // initial setting for applying Newton's method
            BigInteger previousResult = 0;
            BigInteger delta = 0;
            // looking for the root by averaging the maximum and minimum values by Newton's method
            while ((previousResult != currentResult) && (delta >= 0))
            {
                var counterweight = BigInteger.Pow(currentResult, (exponent - 1));
                previousResult = currentResult;
                currentResult = (((exponent - 1) * currentResult) + (source / counterweight)) / exponent;
                delta = previousResult - currentResult;
            }
            // on any condition loop end, previousResult contains the desired value
            currentResult = previousResult;
            // check if the last obtained approximation is the exact value of the root
            isExactResult = (BigInteger.Pow(currentResult, exponent) == source);
            // return accumulated root value
            return currentResult;
        }

        /// <summary>
        /// Method for calculating weight of Newton simplest extraction method.
        /// </summary>
        /// <remarks>
        /// The formula for calculating weight is very approximate and relative, so I will be grateful if someone can clarify.
        /// </remarks>
        public static long GetWeight(
            ref BigInteger source,
            int exponent,
            out bool isApplicableMethod)
        {
            const int floor = 10;
            isApplicableMethod = true;
            var quotient = (int)Math.Ceiling(BigInteger.Log(source, floor) / exponent);
            var weight = (long)(Math.Log(BigInteger.Log(BigInteger.Pow(floor, quotient) - BigInteger.Pow(floor, quotient - 1), 2), 2) * exponent / 2 + 3);
            return weight;
        }
    }

    internal static class ByDouble
    {
        /// <summary>
        /// Method for calculating Nth roots for doubling N degrees.
        /// </summary>
        /// <remarks>
        /// Inner well optimized square root extraction method was relased by Ryan Scott White.
        /// </remarks>
        public static BigInteger GetRoot(
            ref BigInteger source,
            int exponent,
            out bool isExactResult)
        {
            var basement = source;
            var power = exponent;
            for (; power > 1; power >>= 1) basement = NewtonPlusSqrt(basement);
            var target = BigInteger.Pow(basement, exponent);
            isExactResult = (target == source);
            return basement;
            // below is an adaptation of Ryan's method for .NET Standard
            BigInteger NewtonPlusSqrt(BigInteger x)
            {
                // 1.448e17 = ~1<<57
                if (x < 144838757784765629)
                {
                    uint vInt = (uint)Math.Sqrt((ulong)x);
                    // 4.5e15 = ~1<<52
                    if ((x >= 4503599761588224) && ((ulong)vInt * vInt > (ulong)x)) vInt--;
                    return vInt;
                }
                double xAsDub = (double)x;
                // 8.5e37 is long.max * long.max
                if (xAsDub < 8.5e37)
                {
                    ulong vInt = (ulong)Math.Sqrt(xAsDub);
                    BigInteger v = (vInt + ((ulong)(x / vInt))) >> 1;
                    return (v * v <= x) ? v : v - 1;
                }
                if (xAsDub < 4.3322e127)
                {
                    BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
                    v = (v + (x / v)) >> 1;
                    if (xAsDub > 2e63) v = (v + (x / v)) >> 1;
                    return (v * v <= x) ? v : v - 1;
                }
                int xLen = (int)BigInteger.Log(BigInteger.Abs(x), 2) + 1;
                int wantedPrecision = (xLen + 1) / 2;
                int xLenMod = xLen + (xLen & 1) + 1;
                // do the first sqrt on hardware
                long tempX = (long)(x >> (xLenMod - 63));
                double tempSqrt1 = Math.Sqrt(tempX);
                ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
                if (valLong == 0) valLong = 1UL << 53;
                // classic Newton iterations
                BigInteger val = ((BigInteger)valLong << (53 - 1)) + (x >> xLenMod - (3 * 53)) / valLong;
                int size = 106;
                for (; size < 256; size <<= 1) val = (val << (size - 1)) + (x >> xLenMod - (3 * size)) / val;
                if (xAsDub > 4e254)
                {
                    // 1 << 845
                    int numOfNewtonSteps = (int)BigInteger.Log((uint)(wantedPrecision / size), 2) + 2;
                    // apply starting size
                    int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
                    int needToShiftBy = size - wantedSize;
                    val >>= needToShiftBy;
                    size = wantedSize;
                    do
                    {
                        // Newton plus iterations
                        int shiftX = xLenMod - (3 * size);
                        BigInteger valSqrd = (val * val) << (size - 1);
                        BigInteger valSU = (x >> shiftX) - valSqrd;
                        val = (val << size) + (valSU / val);
                        size *= 2;
                    } while (size < wantedPrecision);
                }
                // there are a few extra digits here, lets save them
                int oversidedBy = size - wantedPrecision;
                BigInteger saveDroppedDigitsBI = val & ((BigInteger.One << oversidedBy) - 1);
                int downby = (oversidedBy < 64) ? (oversidedBy >> 2) + 1 : (oversidedBy - 32);
                ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);
                // shrink result to wanted precision
                val >>= oversidedBy;
                // detect a round-ups
                if ((saveDroppedDigits == 0) && (val * val > x)) val--;
                return val;
            }
        }

        /// <summary>
        /// Method for calculating weight of optimized doubling extraction method.
        /// </summary>
        /// <remarks>
        /// The formula for calculating weight is very approximate and relative, so I will be grateful if someone can clarify.
        /// </remarks>
        public static long GetWeight(
            ref BigInteger source,
            int exponent,
            out bool isApplicableMethod)
        {
            const int floor = 10;
            var isPowerOfTwo = (exponent != 0 && ((exponent & (exponent - 1)) == 0));
            isApplicableMethod = false;
            if (!isPowerOfTwo) return long.MaxValue;
            isApplicableMethod = true;
            var quotient = (int)Math.Ceiling(BigInteger.Log(source, floor) / exponent);
            var weight = (long)(0.2 * quotient * (BigInteger.Log(floor, 2) + 1));
            return weight;
        }
    }
}