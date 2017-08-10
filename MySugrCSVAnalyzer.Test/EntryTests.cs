using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySugrCSVAnalyzer;
using MySugrCSVAnalyzer.Models;
using NUnit.Framework;

namespace MySugrCSVAnalyzer.Test
{
    using System.IO;

    [TestFixture]
    public class EntryTests
    {
        [Test]
        public void TestConstructor()
        {
            Entry newEntry = new Entry(DateTime.Now);
            newEntry.bloodGlucoseReading = 100;
            newEntry.insulinInjectionCorrection = 10;
            newEntry.location = "Panera Bread";
            newEntry.mealCarbohydrates = 70;
            newEntry.mealDescription = "Sandwich and soup";
            Assert.AreEqual(100, newEntry.bloodGlucoseReading);
        }

        [Test]
        public void TestAddTags()
        {
            Entry newEntry = new Entry(DateTime.Now);
            newEntry.addTag("Tired");
            newEntry.addTag("Vacation");
            newEntry.addTag("Hyper");
            Assert.AreEqual(3, newEntry.tags.Count);
        }

        [Test]
        public void TestAddFoodTypes()
        {
            Entry newEntry = new Entry(DateTime.Now);
            newEntry.addFoodType("meat");
            newEntry.addFoodType("vegetables");
            newEntry.addFoodType("water");
            Assert.AreEqual(3, newEntry.foodTypes.Count);
        }
    }
}
