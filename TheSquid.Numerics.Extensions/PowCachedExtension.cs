using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace TheSquid.Numerics
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
        private static Dictionary<BigInteger, Dictionary<int, ValueTuple<BigInteger, long>>> powCache;

        /// <summary>
        /// Cache access synchronization object.
        /// </summary>
        private static Object syncObject;

        /// <summary>
        /// Life time counter for shrink data when it is needed.
        /// </summary>
        public static long ItemsLifetime { get; private set; }

        /// <summary>
        /// Static constuctor.
        /// </summary>
        static PowCachedExtension()
        {
            powCache = new Dictionary<BigInteger, Dictionary<int, ValueTuple<BigInteger, long>>>();
            syncObject = new Object();
            ItemsLifetime = 0;
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
        public static BigInteger PowCached(this ref BigInteger basement, int exponent)
        {
            lock (syncObject)
            {
                try
                {
                    if (ItemsLifetime >= int.MaxValue) ShrinkCacheData(ItemsLifetime / 2);
                    return CalculateNewValue(ref basement, exponent);
                }
                catch (OutOfMemoryException)
                {
                    if (powCache.Count > 0) ShrinkCacheData(0);
                    return CalculateNewValue(ref basement, exponent);
                }
            }
        }

        /// <summary>
        /// Get power result from cache.
        /// </summary>
        private static bool TryGetValue(ref BigInteger basement, int exponent, out BigInteger power)
        {
            if (!powCache.TryGetValue(basement, out var termCache)) return false;
            if (!termCache.TryGetValue(exponent, out var itemCache)) return false;
            power = itemCache.Item1;
            return true;
        }

        /// <summary>
        /// Put power result to cache.
        /// </summary>
        private static void CacheNewValue(ref BigInteger basement, int exponent, BigInteger power)
        {
            if (!powCache.TryGetValue(basement, out var termCache))
            {
                termCache = new Dictionary<int, ValueTuple<BigInteger, long>>();
                powCache.Add(basement, termCache);
            }
            if (!termCache.TryGetValue(exponent, out var itemCache))
            {
                itemCache = new ValueTuple<BigInteger, long>(power, ++ItemsLifetime);
                termCache.Add(exponent, itemCache);
            }
            else itemCache.Item2 = ++ItemsLifetime;
        }

        /// <summary>
        /// Calculate new power value.
        /// </summary>
        private static BigInteger CalculateNewValue(ref BigInteger basement, int exponent)
        {
            if (exponent == 0) return 1;
            if (basement == 0) return 0;
            if (exponent == 1) return basement;
            if (TryGetValue(ref basement, exponent, out var power)) return power;

            var left = exponent / 2;
            var right = exponent - left;

            var result = CalculateNewValue(ref basement, left) * CalculateNewValue(ref basement, right);
            CacheNewValue(ref basement, exponent, result);
            return result;
        }

        /// <summary>
        /// Shrink the dataset of cached power values.
        /// </summary>
        /// <param name="lifetimeLimit">
        /// Input lifetime value, all elements less than which will be considered obsolete
        /// and removed. Or input a zero to clear the cache completely.
        /// </param>
        /// <remarks>
        /// Associated lifetime values in the ItemsLifetime property.
        /// </remarks>
        public static void ShrinkCacheData(long lifetimeLimit)
        {
            lock (syncObject)
            {
                if (lifetimeLimit == 0)
                {
                    if (ItemsLifetime == 0) return;
                    ItemsLifetime = 0;
                    powCache.Clear();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Debug.WriteLine($"{nameof(TheSquid.Numerics)}.{nameof(PowCachedExtension)}.{nameof(ShrinkCacheData)}");
                }
                else
                {
                    var newPowCache = new Dictionary<BigInteger, Dictionary<int, ValueTuple<BigInteger, long>>>();
                    ItemsLifetime = 0;
                    foreach (var p in powCache)
                    {
                        var newTermCache = p.Value
                            .Where(t => t.Value.Item2 > lifetimeLimit)
                            .Select(t => new KeyValuePair<int, ValueTuple<BigInteger, long>>(t.Key, new ValueTuple<BigInteger, long>(t.Value.Item1, ++ItemsLifetime)))
                            .ToDictionary(t => t.Key, t => t.Value);
                        if (newTermCache.Count > 0) newPowCache.Add(p.Key, newTermCache);
                    }
                    powCache = newPowCache;
                }
            }
        }
    }
}