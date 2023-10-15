using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace TheSquid.Numerics.Extensions
{
    /// <summary>
    /// C# implementation of an extension method for calculating powers 
    /// of repeating BigInteger values using a cache. By Nikolai TheSquid.
    /// </summary>
    public static partial class PowCachedExtension
    {
        /// <summary>
        /// Cach with format: basement, exponent, power, counter. 
        /// </summary>
        private static Dictionary<ValueTuple<BigInteger, int>, ValueTuple<BigInteger, long>> powCache;

        /// <summary>
        /// Life time counter for sort items by age.
        /// </summary>
        private static long itemsLifetime;

        /// <summary>
        /// Number of items in the cache.
        /// </summary>
        public static long ItemsInCache => powCache.LongCount();

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static PowCachedExtension()
        {
            powCache = new Dictionary<ValueTuple<BigInteger, int>, ValueTuple<BigInteger, long>>();
            itemsLifetime = 0;
        }

        /// <summary>
        /// Wrapper over the standard exponentiation method.
        /// </summary>
        /// <param name="basement">
        /// Power base value.
        /// </param>
        /// <param name="exponent">
        /// Power degree value.
        /// </param>
        /// <returns>
        /// The result of raising basement to the exponent power.
        /// </returns>
        public static BigInteger Pow(this ref BigInteger basement, int exponent)
        {
            return BigInteger.Pow(basement, exponent);
        }

        /// <summary>
        /// Exponentiation method with result caching.
        /// </summary>
        /// <param name="basement">
        /// Power base value.
        /// </param>
        /// <param name="exponent">
        /// Power degree value.
        /// </param>
        /// <returns>
        /// The result of raising basement to the exponent power.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static BigInteger PowCached(this ref BigInteger basement, int exponent)
        {
            try
            {
                const string negativeExponentMessage = "Negative exponent values are not supported.";
                if (exponent < 0) throw new ArgumentOutOfRangeException(negativeExponentMessage);
                if (itemsLifetime >= int.MaxValue) ShrinkCacheData(powCache.LongCount() / 2);
                return CalculateNewValue(ref basement, exponent);
            }
            catch (OutOfMemoryException)
            {
                if (powCache.LongCount() > 0) ShrinkCacheData(0);
                return CalculateNewValue(ref basement, exponent);
            }
        }

        /// <summary>
        /// Get power result from cache.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetValue(ref BigInteger basement, int exponent, out BigInteger power)
        {
            power = BigInteger.Zero;
            var key = new ValueTuple<BigInteger, int>(basement, exponent);
            if (!powCache.TryGetValue(key, out var value)) return false;
            value.Item2 = ++itemsLifetime;
            power = value.Item1;
            return true;
        }

        /// <summary>
        /// Put power result to cache.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CacheNewValue(ref BigInteger basement, int exponent, BigInteger power)
        {
            var key = new ValueTuple<BigInteger, int>(basement, exponent);
            var value = new ValueTuple<BigInteger, long>(power, ++itemsLifetime);
            powCache.Add(key, value);
        }

        /// <summary>
        /// Calculate new power value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static BigInteger CalculateNewValue(ref BigInteger basement, int exponent)
        {
            // results for well known parameter values
            if (exponent == 0) return 1;
            if (basement == 0) return 0;
            if (exponent == 1) return basement;
            if (TryGetValue(ref basement, exponent, out var power)) return power;
            // devide power for using exponentiation with large number of cache hits
            var left = exponent / 2;
            var right = exponent - left;
            // recursive use of the exponentiation schema
            var result = CalculateNewValue(ref basement, left) * CalculateNewValue(ref basement, right);
            CacheNewValue(ref basement, exponent, result);
            return result;
        }

        /// <summary>
        /// Shrink the dataset of cached power values.
        /// </summary>
        /// <param name="itemsInCache">
        /// The number of newest elements left in the cache. Input zero to clear the cache completely.
        /// </param>
        /// <remarks>
        /// Associated items count value in the ItemsInCache property.
        /// </remarks>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void ShrinkCacheData(long itemsInCache)
        {
            if (itemsInCache == 0)
            {
                if (powCache.LongCount() == 0) return;
                itemsLifetime = 0;
                powCache.Clear();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            else
            {
                itemsLifetime = 0;
                powCache = powCache
                    .OrderBy(p => p.Value.Item2)
                    .Skip((int)Math.Min(powCache.LongCount() - itemsInCache, int.MaxValue))
                    .ToDictionary(p => p.Key, p => new ValueTuple<BigInteger, long>(p.Value.Item1, ++itemsLifetime));
            }
        }
    }
}