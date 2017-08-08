using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySugrCSVAnalyzer.Interfaces;
using MySugrCSVAnalyzer.Models;

namespace MySugrCSVAnalyzer.Controllers
{
    public class ReadInputController : IReadInputController
    {
        public string fileName { get; set; }
        public List<Entry> logbook { get; private set; }
        public Dictionary<string, List<Entry>> logbookByDay { get; private set; }
        private int _numColumns;
        public ReadInputController()
        {
            logbook = new List<Entry>();
        }

        public void readInputFile()
        {
            string line = "";
            using (StreamReader input = new StreamReader(File.OpenRead(fileName)))
            {
                if (!input.EndOfStream)
                {
                    line = input.ReadLine();
                    _numColumns = line.Split(',').Length;
                }
                while (!input.EndOfStream)
                {
                    line = input.ReadLine();
                    logbook.Add(parseLineToEntry(line));
                }
            }
        }

        // TODO how to make this private by making it a protected abstract method?
        public Entry parseLineToEntry(string line)
        {
            string[] pieces = new string[_numColumns];
            CSVHelper.DecodeLine(line, out pieces);
            DateTime entryDate = Convert.ToDateTime(pieces[0]);
            DateTime entryTime = Convert.ToDateTime(pieces[1]);
            DateTime entryDateTime = new DateTime(entryDate.Year, entryDate.Month, entryDate.Day, entryTime.Hour,
                entryTime.Minute, entryTime.Second);
            Entry newEntry = new Entry(entryDateTime);
            string[] tags = pieces[2].Split(',');
            foreach (var tag in tags)
            {
                newEntry.addTag(tag);
            }
            if (pieces[3] != "")
            {
                newEntry.bloodGlucoseReading = Int32.Parse(pieces[3]);
            }
            if (pieces[7] != "")
            {
                newEntry.insulinInjectionMeal = Double.Parse(pieces[7]);
            }
            if (pieces[8] != "")
            {
                newEntry.insulinInjectionCorrection = Double.Parse(pieces[8]);
            }
            if (pieces[11] != "")
            {
                newEntry.mealCarbohydrates = Int32.Parse(pieces[11]);
            }
            newEntry.mealDescription = pieces[12];
            newEntry.location = pieces[18];
            string[] foodTypes = pieces[23].Split(',');
            foreach (var foodType in foodTypes)
            {
                newEntry.addFoodType(foodType);
            }
            return newEntry;
        }

        public Dictionary<string, int> GetDailyAverages()
        {
            Dictionary<string, int> results = new Dictionary<string, int>();
            foreach (var dailyEntry in logbookByDay)
            {
                int numReadings = 0;
                int average = 0;
                foreach (var entry in dailyEntry.Value)
                {
                    if (entry.bloodGlucoseReading != null)
                    {
                        average += (int)entry.bloodGlucoseReading;
                        numReadings += 1;
                    }
                }
                if (numReadings > 0)
                {
                    results[dailyEntry.Key] = average / numReadings;
                }
            }
            return results;
        }

        public void LoadLogbookByDay()
        {
            logbookByDay = new Dictionary<string, List<Entry>>();
            foreach (Entry entry in logbook)
            {
                if (!logbookByDay.ContainsKey(entry.entryDateTime.ToString("MM/dd/yyyy")))
                {
                    logbookByDay[entry.entryDateTime.ToString("MM/dd/yyyy")] = new List<Entry>();
                }
                logbookByDay[entry.entryDateTime.ToString("MM/dd/yyyy")].Add(entry);
            }
        }

        public int? GetAverageOfAllTaggedHappy()
        {
            int averageOfAllReadingsTaggedHappy = 0;
            int numberOfReadings = 0;
            foreach (Entry entry in logbook)
            {
                if (entry.tags.Contains("Happy") && entry.bloodGlucoseReading != null)
                {
                    averageOfAllReadingsTaggedHappy += (int)entry.bloodGlucoseReading;
                    numberOfReadings++;
                }
            }
            if (numberOfReadings > 0)
            {
                return averageOfAllReadingsTaggedHappy / numberOfReadings;
            }
            return null;
        }

        public int? GetAverageOfReadingsWithSpecifiedTag(string[] tags)
        {
            
        }
    }
}
