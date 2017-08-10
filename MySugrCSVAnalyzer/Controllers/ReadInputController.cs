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
    using MySugrCSVAnalyzer.Repositories;

    public class ReadInputController
    {
        public string fileName { get; set; }

        private IInputRepository inputRepository;
        private int _numColumns;
        private static readonly int DateColumn = 0;

        private static readonly int TimeColumn = 1;
        public ReadInputController()
        {
            inputRepository = new InputRepository();
        }

        public ReadInputController(IInputRepository newInputRepository)
        {
            this.inputRepository = newInputRepository;
        }

        public void readInputFile()
        {
            List<Entry> tempLogbook = new List<Entry>();
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
                    tempLogbook.Add(parseLineToEntry(line));
                }
            }
            inputRepository.setLogbook(newLogbook: tempLogbook);
        }

        // TODO how to make this private by making it a protected abstract method?
        public Entry parseLineToEntry(string line)
        {
            string[] pieces = new string[_numColumns];
            CSVHelper.DecodeLine(line, out pieces);
            DateTime entryDate = Convert.ToDateTime(pieces[DateColumn]);
            DateTime entryTime = Convert.ToDateTime(pieces[TimeColumn]);
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
            foreach (var dailyEntry in this.inputRepository.GetLogbookByDay())
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
            Dictionary<string, List<Entry>> tempLogbookByDay = new Dictionary<string, List<Entry>>();
            foreach (Entry entry in this.inputRepository.GetLogbook())
            {
                if (!tempLogbookByDay.ContainsKey(entry.entryDateTime.ToString("MM/dd/yyyy")))
                {
                    tempLogbookByDay[entry.entryDateTime.ToString("MM/dd/yyyy")] = new List<Entry>();
                }
                tempLogbookByDay[entry.entryDateTime.ToString("MM/dd/yyyy")].Add(entry);
            }
            this.inputRepository.setLogbookByDay(tempLogbookByDay);
        }

        public int? GetAverageOfReadingsWithSpecifiedTag(string tag)
        {
            int averageOfAllReadingsTagged = 0;
            int numberOfReadings = 0;
            foreach (var entry in this.inputRepository.GetLogbook())
            {
                if (entry.tags.Contains(tag) && entry.bloodGlucoseReading != null)
                {
                    averageOfAllReadingsTagged += (int) entry.bloodGlucoseReading;
                    numberOfReadings++;
                }
            }
            if (numberOfReadings > 0)
            {
                return averageOfAllReadingsTagged / numberOfReadings;
            }
            return null;
        }

        public int? GetAverageOfReadingsWithSpecifiedTags(String[] tags, bool matchAll)
        {
            int averageOfAllReadingsTagged = 0;
            int numberOfReadings = 0;
            foreach (var entry in this.inputRepository.GetLogbook())
            {
                if (entry.bloodGlucoseReading != null)
                {

                    if (matchAll)
                    {
                        bool allfound = true;
                        foreach (var tag in tags)
                        {
                            if (!entry.tags.Contains(tag))
                            {
                                allfound = false;
                                break;
                            }
                            if (allfound)
                            {
                                averageOfAllReadingsTagged += (int)entry.bloodGlucoseReading;
                                numberOfReadings++;
                            }
                        }
                    }
                    else
                    {
                        foreach (var tag in tags)
                        {
                            bool found = false;
                            if (entry.tags.Contains(tag))
                            {
                                averageOfAllReadingsTagged += (int) entry.bloodGlucoseReading;
                                numberOfReadings++;
                                break;
                            }
                        }
                    }
                }
            }
            if (numberOfReadings > 0)
            {
                return averageOfAllReadingsTagged / numberOfReadings;
            }
            return null;
        }
    }
}
