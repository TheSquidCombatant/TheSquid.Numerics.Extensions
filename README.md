# TheSquid.Numerics.Extensions
[![GitHub Status](https://img.shields.io/github/actions/workflow/status/TheSquidCombatant/TheSquid.Numerics.Extensions/push-main-not-version.yml?cacheSeconds=60)](https://github.com/TheSquidCombatant/TheSquid.Numerics.Extensions)
[![NuGet Version](http://img.shields.io/nuget/v/TheSquid.Numerics.Extensions.svg?style=flat&color=green)](https://www.nuget.org/packages/TheSquid.Numerics.Extensions/)

C# implementation of extension methods for BigInteger data type. Such as extracting an Nth root, generating a random value, and so on. By Nikolai TheSquid.

I will be glad to merge your pull requests for improve calculation performance. Even if the improvement affects only individual cases from the range of values.

## NthRootExtension
C# implementation of an extension method to quickly calculate an Nth root (including square root) for BigInteger value.

### How to use
Basicly you can copy class [NthRootExtension](TheSquid.Numerics.Extensions/NthRootExtension.cs) to your project and add using namespace TheSquid.Numerics.Extensions. Another option is to add the [TheSquid.Numerics.Extensions](https://www.nuget.org/packages/TheSquid.Numerics.Extensions/) package from the nuget repository to your project's dependencies. For an example of calling an extension method, you can look at the code of the corresponding test class [NthRootExtensionTests](TheSquid.Numerics.Extensions.Tests/NthRootExtensionTests.cs).

### How to test
You can start random NthRoot tests right after clone repository and build solution. You must run generate tests and rebuild solution before start speed Nth root tests.

### How to understand
The extension method uses two root calculation algorithms: well-known Newton's method and digit-by-digit method. As the degree of the root increases, the calculation by the Newton method slows down, and the digit-by-digit method accelerates. With a root radicand value order of 100,000 decimal digits, the dependence of the calculation speed on the degree of the root is as follows:

![pow comparison](https://github.com/TheSquidCombatant/NthRootExtension/assets/102874947/6d63160a-b02c-40dd-85b9-43f7b5f8c9e3)

## NextBigIntegerExtension
C# implementation of an extension method to generate random BigInteger value within the specified range.

### How to use
Basicly you can copy class [NextBigIntegerExtension](TheSquid.Numerics.Extensions/NextBigIntegerExtension.cs) to your project and add using namespace TheSquid.Numerics.Extensions. Another option is to add the [TheSquid.Numerics.Extensions](https://www.nuget.org/packages/TheSquid.Numerics.Extensions/) package from the nuget repository to your project's dependencies. For an example of calling an extension method, you can look at the code of the corresponding test class [NextBigIntegerExtensionTests](TheSquid.Numerics.Extensions.Tests/NextBigIntegerExtensionTests.cs).

### How to test
You can start random NextBigInteger tests from project TheSquid.Numerics.Extensions.Tests right after clone repository and build solution.

### How to understand
Extension method for the system class Random. Method uses instance of Random class to generate an array of random bytes.

## PowCachedExtension
C# implementation of an extension method for calculating powers of repeating BigInteger values using a cache.

### How to use
Basicly you can copy class [PowCachedExtension](TheSquid.Numerics.Extensions/PowCachedExtension.cs) to your project and add using namespace TheSquid.Numerics.Extensions. Another option is to add the [TheSquid.Numerics.Extensions](https://www.nuget.org/packages/TheSquid.Numerics.Extensions/) package from the nuget repository to your project's dependencies. For an example of calling an extension method, you can look at the code of the corresponding test class [PowCachedExtensionTests](TheSquid.Numerics.Extensions.Tests/PowCachedExtensionTests.cs).

### How to test
You can start random PowCached tests from project TheSquid.Numerics.Extensions.Tests right after clone repository and build solution.

### How to understand
Acceleration is achieved by memorizing results of computing degrees, as well as memorizing the intermediate results obtained in calculation progress. With random basement values in range from 0 to 1000, random exponent values in range from 0 to 1000 and iterations count up to 2000000, the dependence of the calculation speed on cache filling is as follows:

![pow comparison](https://github.com/TheSquidCombatant/TheSquid.Numerics.Extensions/assets/102874947/5cf3d307-cbab-4da6-a1b0-7409f759e516)
