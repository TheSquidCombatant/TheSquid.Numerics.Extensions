# NthRootExtension
C# implementation of an extension method to quickly calculate an Nth root (including square root) for BigInteger value. By Nikolai TheSquid.

## How to use
Basicly you can copy class [NthRootExtension](System.Numerics.Extensions/NthRootExtension.cs) to your project and add using for namespace System.Numerics.Extensions.

## How to test
You can start random and specified Nth root tests right after clone repository and build solution. You must run generate tests and rebuild solution before start speed Nth root tests.

## How to extend
I will be glad to merge your pull requests for improve Nth root calculation performance. Even if the improvement affects only individual cases from the range of values.

## How to understand
The extension method uses two root calculation algorithms: well-known Newton's method and digit-by-digit method. As the degree of the root increases, the calculation by the Newton method slows down, and the digit-by-digit method accelerates. With a root radicand value order of 100,000 decimal digits, the dependence of the calculation speed on the degree of the root is as follows:

<p align="center">
  <img width="50%" height="50%" src="https://github.com/TheSquidCombatant/NthRootExtension/assets/102874947/6d63160a-b02c-40dd-85b9-43f7b5f8c9e3.jpg"/>
</p>
