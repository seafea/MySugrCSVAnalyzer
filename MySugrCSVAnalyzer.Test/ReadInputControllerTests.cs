using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using MySugrCSVAnalyzer.Controllers;
using MySugrCSVAnalyzer.Models;

namespace MySugrCSVAnalyzer.Test
{
    using MySugrCSVAnalyzer.Controllers;
    using MySugrCSVAnalyzer.Interfaces;

    using NUnit.Framework.Constraints;

    [TestFixture]
    public class ReadInputControllerTests
    {
        private Entry nowEntry = new Entry(DateTime.Now);
        private Entry anHourAgoEntry = new Entry(DateTime.Now.AddHours(-1));
        private Entry yesterdayEntry = new Entry(DateTime.Now.AddDays(-1));
        private Entry yesterdayEntry2 = new Entry(DateTime.Now.AddDays(-1));

        [Test]
        public void TestParseLineToEntry_NormalUsage_Success()
        {
            ReadInputController testReadInputController = new ReadInputController();
            string line =
                "\"Aug 5, 2017\",\"6:46:29 PM\",\"Eating out,Happy,Before the meal,Carbs guess,Dinner\",\"142\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"40\",\"1/2 sandwich \",\"\",\"\",\"\",\"\",\"\",\"Jimmy John's\",\"\",\"\",\"\",\"\",\"\",\"\"";
            Entry testEntry = testReadInputController.ParseLineToEntry(line);
            Assert.AreEqual(142, testEntry.bloodGlucoseReading);
            Assert.Contains("Dinner", testEntry.tags);
        }

        [Test]
        public void TestGetAverageForSingleTag_NormalUsage_Success()
        {
            Mock<IInputRepository> testInputRepository = new Mock<IInputRepository>();
            List<Entry> logbook = new List<Entry>();
            nowEntry.bloodGlucoseReading = 100;
            anHourAgoEntry.bloodGlucoseReading = 180;
            yesterdayEntry.bloodGlucoseReading = 225;
            this.yesterdayEntry2.bloodGlucoseReading = 190;
            this.nowEntry.addTag("Happy");
            this.anHourAgoEntry.addTag("Happy");
            this.anHourAgoEntry.addTag("Dinner");
            this.yesterdayEntry.addTag("Dinner");
            logbook.Add(this.nowEntry);
            logbook.Add(this.anHourAgoEntry);
            logbook.Add(this.yesterdayEntry);
            testInputRepository.Setup(x => x.GetLogbook()).Returns(logbook);
            ReadInputController testReadInputController = new ReadInputController(testInputRepository.Object);
            Assert.AreEqual(140, testReadInputController.GetAverageOfReadingsWithSpecifiedTag("Happy"));
        }

        [Test]
        public void TestGetAverageForSpecifiedTagsNotAll_NormalUsage_Success()
        {
            Mock<IInputRepository> testInputRepository = new Mock<IInputRepository>();
            List<Entry> logbook = new List<Entry>();
            nowEntry.bloodGlucoseReading = 100;
            anHourAgoEntry.bloodGlucoseReading = 180;
            yesterdayEntry.bloodGlucoseReading = 225;
            this.nowEntry.addTag("Happy");
            this.anHourAgoEntry.addTag("Happy");
            this.anHourAgoEntry.addTag("Dinner");
            this.yesterdayEntry.addTag("Dinner");
            logbook.Add(this.nowEntry);
            logbook.Add(this.anHourAgoEntry);
            logbook.Add(this.yesterdayEntry);
            testInputRepository.Setup(x => x.GetLogbook()).Returns(logbook);
            ReadInputController testReadInputController = new ReadInputController(testInputRepository.Object);
            Assert.AreEqual(140, testReadInputController.GetAverageOfReadingsWithSpecifiedTags(new []
                                                                                                   {
                                                                                                       "Happy"
                                                                                                   }, false));
        }

        [Test]
        public void TestGetAverageForSpecifiedTagsAll_NormalUsage_Success()
        {
            Mock<IInputRepository> testInputRepository = new Mock<IInputRepository>();
            List<Entry> logbook = new List<Entry>();
            nowEntry.bloodGlucoseReading = 100;
            anHourAgoEntry.bloodGlucoseReading = 180;
            yesterdayEntry.bloodGlucoseReading = 225;
            this.yesterdayEntry2.bloodGlucoseReading = 190;
            this.nowEntry.addTag("Happy");
            this.anHourAgoEntry.addTag("Happy");
            this.anHourAgoEntry.addTag("Dinner");
            this.yesterdayEntry.addTag("Dinner");
            this.yesterdayEntry2.addTag("Happy");
            this.yesterdayEntry2.addTag("Dinner");
            logbook.Add(this.nowEntry);
            logbook.Add(this.anHourAgoEntry);
            logbook.Add(this.yesterdayEntry);
            logbook.Add(this.yesterdayEntry2);
            testInputRepository.Setup(x => x.GetLogbook()).Returns(logbook);
            ReadInputController testReadInputController = new ReadInputController(testInputRepository.Object);
            Assert.AreEqual(185, testReadInputController.GetAverageOfReadingsWithSpecifiedTags(new[]
                                                                                                   {
                                                                                                       "Happy",
                                                                                                       "Dinner"
                                                                                                   }, true));
        }
    }
}
