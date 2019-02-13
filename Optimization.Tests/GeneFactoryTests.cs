﻿using GeneticSharp.Domain.Randomizations;
using NUnit.Framework;
using Optimization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimization.Tests
{
    [TestFixture()]
    public class GeneFactoryTests
    {

        [SetUp]
        public void Setup()
        { 
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            RandomizationProvider.Current = new BasicRandomization();
        }

        [Test()]
        public void InitializeTest()
        {
            GeneFactory.Initialize(new GeneConfiguration[0]);
            Assert.IsNotNull(GeneFactory.Config);
        }

        [Test()]
        public void RandomBetweenTest()
        {
            var actual = GeneFactory.RandomBetween(0, 1);
            Assert.IsTrue(actual < 2);
        }

        [Test()]
        public void RandomBetweenCanBeMaximumTest()
        {
            bool canBeMaximum = false;
            for (var i = 0; i < 10000; i++)
            {
                var actual = GeneFactory.RandomBetween(0, 1);
                if (actual == 1m) { canBeMaximum = true; break;  }
            }

            Assert.IsTrue(canBeMaximum);
        }

        [Test()]
        public void RandomBetweenPrecisionTest()
        {
            var actual = GeneFactory.RandomBetween(1.1m, 1.2m, 1);
            Assert.IsTrue(actual >= 1.1m && actual <= 1.2m);
        }

        [Test()]
        public void RandomBetweenPrecisionCanBeMaximumTest()
        {
            bool canBeMaximum = false;
            for (var i = 0;i < 10000;i++)
            {
                var actual = GeneFactory.RandomBetween(1.1m, 1.2m, 1);
                if (actual == 1.2m) { canBeMaximum = true; break; }
            }

            Assert.IsTrue(canBeMaximum);
        }

        [Test()]
        public void RandomBetweenPrecisionCanBeNegativeTest()
        {
            bool canBeNegative = false;
            for (var i = 0; i < 10000; i++)
            {
                var actual = GeneFactory.RandomBetween(-1m, 1m, 0);
                if (actual == -1) { canBeNegative = true; break; }
            }

            Assert.IsTrue(canBeNegative);
        }

        [Test()]
        public void GenerateTest()
        {
            var config = new[] { new GeneConfiguration { Key = "slow", ActualInt = 200 }, new GeneConfiguration { Key = "take", Precision = 2, MaxDecimal= 0.06m,
                MinDecimal = 0.04m, ActualDecimal = 0.05m } };

            RandomizationProvider.Current = new BasicRandomization();
            GeneFactory.Initialize(config);

            var actual = GeneFactory.Generate(config[0], true);
            Assert.AreEqual(200, (int)((KeyValuePair<string, object>)actual.Value).Value);

            RandomizationProvider.Current = new BasicRandomization();
            actual = GeneFactory.Generate(config[1], false);
            decimal parsed;
            Assert.IsTrue(decimal.TryParse(((KeyValuePair<string, object>)actual.Value).Value.ToString(), out parsed));
            Assert.AreEqual(2, GeneFactory.GetPrecision(parsed));

        }
    }
}