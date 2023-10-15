using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Numerics;

namespace TheSquid.Numerics.Extensions.Tests
{
    [TestClass]
    public class NthRootExtensionTests
    {
        [TestMethod]
        [DataRow(10000, 1000, 1000)]
        [DataRow(10000, 10000, 100)]
        [DataRow(10000, 100000, 10)]
        public void NthRootRandomTest(int iterationsCount, int maxBasement, int maxExponent)
        {
            var random = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < iterationsCount; ++i)
            {
                var exponent = random.Next(2, maxExponent);
                var basementLess = (BigInteger)random.Next(1, maxBasement);
                var sourceLess = BigInteger.Pow(basementLess, exponent);
                var basementGreater = basementLess + 1;
                var sourceGreater = BigInteger.Pow(basementGreater, exponent);
                var source = random.NextBigInteger(sourceLess, sourceGreater);
                var expectedExactResult = ((source == sourceLess) || (source == sourceGreater));
                var expectedBasement = (source == sourceGreater ? basementGreater : basementLess);
                var actualBasement = source.NthRoot(exponent, out var actualExactResult);
                const string message = "exponent={0}, source={1}, basement={2}, isExactResult={3}";
                Assert.AreEqual(expectedExactResult, actualExactResult, message, exponent, source, expectedBasement, expectedExactResult);
                Assert.AreEqual(expectedBasement, actualBasement, message, exponent, source, expectedBasement, expectedExactResult);
            }
        }

        [TestMethod]
        [DataRow(@"Data\Speed\00*.txt")]
        [DataRow(@"Data\Speed\01*.txt")]
        [DataRow(@"Data\Speed\02*.txt")]
        [DataRow(@"Data\Speed\03*.txt")]
        [DataRow(@"Data\Speed\04*.txt")]
        [DataRow(@"Data\Speed\05*.txt")]
        [DataRow(@"Data\Speed\06*.txt")]
        [DataRow(@"Data\Speed\07*.txt")]
        [DataRow(@"Data\Speed\08*.txt")]
        [DataRow(@"Data\Speed\09*.txt")]
        [DataRow(@"Data\Speed\10*.txt")]
        [DataRow(@"Data\Speed\11*.txt")]
        [DataRow(@"Data\Speed\12*.txt")]
        [DataRow(@"Data\Speed\13*.txt")]
        [DataRow(@"Data\Speed\14*.txt")]
        [DataRow(@"Data\Speed\15*.txt")]
        [DataRow(@"Data\Speed\16*.txt")]
        [DataRow(@"Data\Speed\17*.txt")]
        [DataRow(@"Data\Speed\18*.txt")]
        [DataRow(@"Data\Speed\19*.txt")]
        [DataRow(@"Data\Speed\20*.txt")]
        [DataRow(@"Data\Speed\21*.txt")]
        [DataRow(@"Data\Speed\22*.txt")]
        [DataRow(@"Data\Speed\23*.txt")]
        public void NthRootSpeedTest(string filePathTemplate)
        {
            for (var k = 0; k < 10; ++k)
            {
                var filePath = filePathTemplate.Replace("*", k.ToString());
                var json = File.ReadAllText(filePath);
                var dataSet = JsonConvert.DeserializeObject<DataSet>(json);

                var basement = BigInteger.Parse(dataSet.Basement);
                var exponent = int.Parse(dataSet.Exponent);
                var source = BigInteger.Parse(dataSet.Power);
                var isExactResult = dataSet.IsExactResult;

                var actualBasement = source.NthRoot(exponent, out var actualExactResult);
                const string message = "exponent={0}, source={1}, basement={2}, isExactResult={3}";
                Assert.AreEqual(isExactResult, actualExactResult, message, exponent, source, basement, isExactResult);
                Assert.AreEqual(basement, actualBasement, message, exponent, source, basement, isExactResult);
            }
        }

        [TestMethod]
        [DataRow(@"..\..\..\Data\Speed\00*.txt", 2, 40960)]
        [DataRow(@"..\..\..\Data\Speed\01*.txt", 4, 20480)]
        [DataRow(@"..\..\..\Data\Speed\02*.txt", 8, 10240)]
        [DataRow(@"..\..\..\Data\Speed\03*.txt", 16, 5120)]
        [DataRow(@"..\..\..\Data\Speed\04*.txt", 32, 2560)]
        [DataRow(@"..\..\..\Data\Speed\05*.txt", 64, 1280)]
        [DataRow(@"..\..\..\Data\Speed\06*.txt", 128, 640)]
        [DataRow(@"..\..\..\Data\Speed\07*.txt", 256, 320)]
        [DataRow(@"..\..\..\Data\Speed\08*.txt", 512, 160)]
        [DataRow(@"..\..\..\Data\Speed\09*.txt", 1024, 80)]
        [DataRow(@"..\..\..\Data\Speed\10*.txt", 2048, 40)]
        [DataRow(@"..\..\..\Data\Speed\11*.txt", 4096, 20)]
        [DataRow(@"..\..\..\Data\Speed\12*.txt", 8192, 10)]
        [DataRow(@"..\..\..\Data\Speed\13*.txt", 16384, 5)]
        [DataRow(@"..\..\..\Data\Speed\14*.txt", 127, 229)]
        [DataRow(@"..\..\..\Data\Speed\15*.txt", 131, 227)]
        [DataRow(@"..\..\..\Data\Speed\16*.txt", 137, 223)]
        [DataRow(@"..\..\..\Data\Speed\17*.txt", 139, 211)]
        [DataRow(@"..\..\..\Data\Speed\18*.txt", 149, 199)]
        [DataRow(@"..\..\..\Data\Speed\19*.txt", 151, 197)]
        [DataRow(@"..\..\..\Data\Speed\20*.txt", 157, 193)]
        [DataRow(@"..\..\..\Data\Speed\21*.txt", 163, 191)]
        [DataRow(@"..\..\..\Data\Speed\22*.txt", 167, 181)]
        [DataRow(@"..\..\..\Data\Speed\23*.txt", 173, 179)]
        public void GenerateSpeedTests(string filePathTemplate, int exponentValue, int basementLength)
        {
            var targetDirectory = Path.GetDirectoryName(filePathTemplate);
            Directory.CreateDirectory(targetDirectory);
            var random = new Random(DateTime.Now.Millisecond);
            for (int k = 0; k < 10; ++k)
            {
                var basementInput = random.Next(1, 10).ToString();
                for (int i = 1; i < basementLength; ++i) basementInput += random.Next(0, 10).ToString();

                var basement = BigInteger.Parse(basementInput);
                var power = BigInteger.Pow(basement, exponentValue);

                var dataSet = new DataSet()
                {
                    Exponent = exponentValue.ToString(),
                    Basement = basement.ToString(),
                    Power = power.ToString(),
                    IsExactResult = true,
                };

                var filePath = filePathTemplate.Replace("*", k.ToString());

                var json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Assert.IsTrue(File.Exists(filePath));
            }
        }
    }

    public class DataSet
    {
        public bool IsExactResult { get; set; }
        public string Exponent { get; set; }
        public string Basement { get; set; }
        public string Power { get; set; }
    }
}