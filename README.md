# TheSquid.Numerics.Extensions
[![GitHub Status](https://github.com/TheSquidCombatant/TheSquid.Numerics.Extensions/actions/workflows/push-main-not-version.yml/badge.svg)](https://github.com/TheSquidCombatant/TheSquid.Numerics.Extensions)
[![NuGet Version](http://img.shields.io/nuget/v/TheSquid.Numerics.Extensions.svg?style=flat&color=green)](https://www.nuget.org/packages/TheSquid.Numerics.Extensions/)

C# implementation of extension methods for BigInteger data type. Such as extracting an Nth root, generating a random value and exponentiation. By Nikolai TheSquid.

I will be glad to merge your pull requests for improve calculation performance. Even if the improvement affects only individual cases from the range of values.

To use these extensions you will need to add to your code following namespaces: `System.Numerics` and `TheSquid.Numerics.Extensions`.

## NthRootExtension
C# implementation of an extension method to quickly calculate an Nth root (including square root) for BigInteger value.

### How to use
Basicly you can copy class [NthRootExtension](TheSquid.Numerics.Extensions/NthRootExtension.cs) from source repository to your project. Another option is to add the [TheSquid.Numerics.Extensions](https://www.nuget.org/packages/TheSquid.Numerics.Extensions/) package from the nuget repository to your project's dependencies.

Usage example:
```csharp
var source = BigInteger.Parse(Console.ReadLine());
var exponent = int.Parse(Console.ReadLine());
var root = source.NthRoot(exponent, out var isExactResult);
```

### How to test
You can start random NthRoot tests right after clone repository and build solution. You must run generate tests and rebuild solution before start speed Nth root tests.

### How to understand
The extension method uses two root calculation algorithms: well-known Newton's method and digit-by-digit method. As the degree of the root increases, the calculation by the Newton method slows down, and the digit-by-digit method accelerates. With a root radicand value order of 100,000 decimal digits, the dependence of the calculation speed on the degree of the root is as follows:

![root comparison](https://user-images.githubusercontent.com/102874947/256707012-6d63160a-b02c-40dd-85b9-43f7b5f8c9e3.jpg)

## NextBigIntegerExtension
C# implementation of an extension method to generate random BigInteger value within the specified range.

### How to use
Basicly you can copy class [NextBigIntegerExtension](TheSquid.Numerics.Extensions/NextBigIntegerExtension.cs) from source repository to your project. Another option is to add the [TheSquid.Numerics.Extensions](https://www.nuget.org/packages/TheSquid.Numerics.Extensions/) package from the nuget repository to your project's dependencies. 

Usage example:
```csharp
var source = BigInteger.Parse(Console.ReadLine());
var exponent = int.Parse(Console.ReadLine());
var power = source.PowCached(exponent);
```

### How to test
You can start random tests for NextBigInteger extension method using NextBigIntegerExtensionTests class from project TheSquid.Numerics.Extensions.Tests right after clone repository and build solution.

### How to understand
Extension method for the system class Random. Method uses instance of Random class to generate an array of random bytes.

## PowCachedExtension
C# implementation of an extension method for faster calculation of powers with repeated parameters using cache.

### How to use
Basicly you can copy class [PowCachedExtension](TheSquid.Numerics.Extensions/PowCachedExtension.cs) from source repository to your project. Another option is to add the [TheSquid.Numerics.Extensions](https://www.nuget.org/packages/TheSquid.Numerics.Extensions/) package from the nuget repository to your project's dependencies.

Usage example:
```csharp
var min = BigInteger.Parse(Console.ReadLine());
var max = BigInteger.Parse(Console.ReadLine());
var random = new Random(DateTime.Now.Millisecond).NextBigInteger(min, max);
```

Pow cache clears itself automatically. Firstly, if out of memory error occurs when calculating the power. Then the cache will be cleared completely. Secondly, if the number of elements in the cache reaches the number of `int.MaxValue`. Then the cache will be cleared by half. Additionally, you can check the number of elements in the cache and clear it manually, leaving a specified number of elements.

Usage example:
```csharp
var available = PowCachedExtension.ItemsInCache;
var threshold = long.Parse(Console.ReadLine());
if (threshold < available) PowCachedExtension.ShrinkCacheData(threshold);
```

### How to test
You can start random tests for PowCached extension method using PowCachedExtensionTests class from project TheSquid.Numerics.Extensions.Tests right after clone repository and build solution.

### How to understand
Acceleration is achieved by memorizing results of computing degrees, as well as memorizing the intermediate results obtained in calculation progress. With random basement values in range from 0 to 1000, random exponent values in range from 0 to 1000 and iterations count up to 2000000, the dependence of the calculation speed on cache filling is as follows:

![pow comparison](https://user-images.githubusercontent.com/102874947/260506546-5cf3d307-cbab-4da6-a1b0-7409f759e516.jpg)
