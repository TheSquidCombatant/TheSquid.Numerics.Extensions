using System;
using System.Numerics;

namespace TheSquid.Numerics.Extensions;

/// <summary>
/// C# implementation of an extension method to generate random BigInteger value. By Nikolai
/// TheSquid.
/// </summary>
public static partial class NextBigIntegerExtension
{
    /// <summary>
    /// Random BigInteger value within the specified range.
    /// </summary>
    /// <param name="random">
    /// Basic pseudo-random number generator with defined seed.
    /// </param>
    /// <param name="minValue">
    /// Inclusive lower bound value.
    /// </param>
    /// <param name="maxValue">
    /// Inclusive upper bound value.
    /// </param>
    /// <returns>
    /// It returns BigInteger value equal or greater than <paramref name="minValue"/> and equal
    /// or less than <paramref name="maxValue"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Max value can not be less then min value.
    /// </exception>
    public static BigInteger NextBigInteger
    (
        this Random random,
        BigInteger minValue,
        BigInteger maxValue
    )
    {
        const string maxValueCannotBeLessMessage = "Max value can not be less then min value.";
        if (maxValue < minValue) throw new ArgumentOutOfRangeException(nameof(maxValue), maxValueCannotBeLessMessage);
        var residual = maxValue - minValue;
        if (residual == 0) return maxValue;
        var buffer = residual.ToByteArray();
        random.NextBytes(buffer);
        var multiplier = new BigInteger(buffer);
        if (multiplier < 0) multiplier *= -1;
        if (multiplier > residual) multiplier %= residual;
        return minValue + multiplier;
    }

    /// <summary>
    /// Random positive BigInteger value with the specified decimal length.
    /// </summary>
    /// <param name="random">
    /// Basic pseudo-random number generator with defined seed.
    /// </param>
    /// <param name="minLength">
    /// Minimum length in decimal characters.
    /// </param>
    /// <param name="maxlength">
    /// Maximum length in decimal characters.
    /// </param>
    /// <param name="radix">
    /// Numeral system base where digits corresponding to the first natural numbers including
    /// zero are used.
    /// </param>
    /// <returns>
    /// It returns positive BigInteger value with decimal length equal or greater than
    /// <paramref name="minLength"/> and equal or less than <paramref name="maxlength"/>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Min length cannot be equal or less than zero.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Max length cannot be less than min length.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Radix value cannot be equal or less than one.
    /// </exception>
    public static BigInteger NextBigInteger
    (
        this Random random,
        int minLength,
        int maxlength,
        int radix = 10
    )
    {
        const string minLengthCannotBeLessMessage = "Min length cannot be equal or less than zero.";
        if (minLength <= 0) throw new ArgumentOutOfRangeException(nameof(minLength), minLengthCannotBeLessMessage);
        const string maxLengthCannotBeLessMessage = "Max length cannot be less than min length.";
        if (maxlength < minLength) throw new ArgumentOutOfRangeException(nameof(maxlength), maxLengthCannotBeLessMessage);
        const string radixCannotBeLessMessage = "Radix value cannot be equal or less than one.";
        if (radix <= 1) throw new ArgumentOutOfRangeException(nameof(radix), radixCannotBeLessMessage);
        var minValue = (minLength == 1 ? 0 : BigInteger.Pow(radix, minLength - 1));
        var maxValue = BigInteger.Pow(radix, maxlength) - 1;
        return random.NextBigInteger(minValue, maxValue);
    }
}