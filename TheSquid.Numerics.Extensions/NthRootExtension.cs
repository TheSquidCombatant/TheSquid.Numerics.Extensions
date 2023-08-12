﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace TheSquid.Numerics
{
    /// <summary>
    /// C# implementation of an extension method to quickly calculate an Nth root
    /// (including square root) for BigInteger value. By Nikolai TheSquid.
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
        /// It returns the exact value, in case of the root is completely extracted, otherwise it returns nearest value from below.
        /// </returns>
        /// <exception cref="ArithmeticException">
        /// The value of the exponent leads to an ambiguous results.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Negative exponent values and negative source values are not supported.
        /// </exception>
        public static BigInteger NthRoot(this ref BigInteger source, int exponent, out bool isExactResult)
        {
            // validation of input parameter values
            const string negativeValuesMessage = "Negative exponent values and negative source values are not supported.";
            if ((source < 0) || (exponent < 0)) throw new ArgumentOutOfRangeException(negativeValuesMessage);
            const string ambiguousResultMessage = "The value of the exponent leads to an ambiguous results.";
            if (exponent == 0) throw new ArithmeticException(ambiguousResultMessage);
            // stub for the case of trivial values of the radical expression
            isExactResult = true;
            if ((source == 0) || (source == 1)) return source;
            // base of the numeral system, the value 10 is best for traceability and easу debugging
            var floor = 10;
            // calculate the worst-case cost for each root extraction method
            var quotient = (int)Math.Ceiling(BigInteger.Log(source, floor) / exponent);
            var digitsRootCount = (int)(0.8 * quotient * (BigInteger.Log(floor, 2) + 1));
            var newtonRootCount = (int)(Math.Log(BigInteger.Log(BigInteger.Pow(floor, quotient) - BigInteger.Pow(floor, quotient - 1), 2), 2) * exponent / 2 + 3);
            // call the fastest root extraction method for current parameters
            var min = new[] { digitsRootCount, newtonRootCount }.Min();
            if (min == digitsRootCount) return GetRootByDigits(ref source, exponent, out isExactResult);
            if (min == newtonRootCount) return GetRootByNewton(ref source, exponent, out isExactResult);
            // stub if something went wrong when extending the functionality
            const string notSupportedMethodMessage = "Not supported nthroot calculation method.";
            throw new NotSupportedException(notSupportedMethodMessage);
        }

        /// <summary>
        /// Method for calculating Nth roots for large N degrees.
        /// </summary>
        /// <remarks>
        /// Digit-by-digit extraction method.
        /// </remarks>
        private static BigInteger GetRootByDigits(this ref BigInteger source, int exponent, out bool isExactResult)
        {
            // calculate how many digits of accuracy are cut off from the radicand value for each digit of root value
            var floor = 10;
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
        /// Method for calculating Nth roots for small N degrees.
        /// </summary>
        /// <remarks>
        /// By Newton simplest extraction method.
        /// </remarks>
        private static BigInteger GetRootByNewton(this ref BigInteger source, int exponent, out bool isExactResult)
        {
            // calculate the initial guess (equal or greater) the root value with accuracy up to one digit
            var floor = 10;
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
    }
}