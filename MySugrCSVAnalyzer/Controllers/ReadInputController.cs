//-----------------------------------------------------------------------
// <copyright file="ReadInputController.cs" company="New River Valley Community Services">
//     Copyright (c) Sprocket Enterprises. All rights reserved.
// </copyright>
// <author>David Merryman</author>
//-----------------------------------------------------------------------
namespace MySugrCSVAnalyzer.Controllers
{
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
    using MySugrCSVAnalyzer.Repositories;

    public class ReadInputController
    {
        private static readonly int DateColumn = 0;
        private static readonly int TimeColumn = 1;
        private static readonly int TagsColumn = 2;
        private static readonly int BloodGlucoseColumn = 3;
        private static readonly int InsulinInjectionMealColumn = 7;
        private static readonly int InsulinInjectionCorrectionColumn = 8;
        private static readonly int MealCarbohydratesColumn = 11;
        private static readonly int MealDescriptionColumn = 12;
        private static readonly int LocationColumn = 18;
        private static readonly int FoodTypesColumn = 23;
        private readonly IInputRepository inputRepository;
        private int numColumns;

        public string FileName { get; set; }

        public ReadInputController()
        {
            inputRepository = new InputRepository();
        }

        public ReadInputController(IInputRepository newInputRepository)
        {
            this.inputRepository = newInputRepository;
        }

        public void ReadInputFile()
        {
            List<Entry> tempLogbook = new List<Entry>();
            using (StreamReader input = new StreamReader(File.OpenRead(FileName)))
            {
                string line;
                if (!input.EndOfStream)
                {
                    line = input.ReadLine();
                    numColumns = line.Split(',').Length;
                }
                while (!input.EndOfStream)
                {
                    line = input.ReadLine();
                    tempLogbook.Add(ParseLineToEntry(line));
                }
            }
            inputRepository.setLogbook(newLogbook: tempLogbook);
        }

        // TODO how to make this private by making it a protected abstract method?
        public Entry ParseLineToEntry(string line)
        {
            string[] pieces = new string[numColumns];
            CSVHelper.DecodeLine(line, out pieces);
            Entry newEntry = new Entry(
                newEntryDateTime: ParseDateAndTimeIntoOne(
                    date: Convert.ToDateTime(pieces[DateColumn]),
                    time: Convert.ToDateTime(pieces[TimeColumn])));
            string[] tags = pieces[TagsColumn].Split(',');
            foreach (var tag in tags)
            {
                newEntry.addTag(tag);
            }
            if (pieces[BloodGlucoseColumn] != "")
            {
                newEntry.bloodGlucoseReading = Int32.Parse(pieces[BloodGlucoseColumn]);
            }
            if (pieces[InsulinInjectionMealColumn] != "")
            {
                newEntry.insulinInjectionMeal = Double.Parse(pieces[InsulinInjectionMealColumn]);
            }
            if (pieces[InsulinInjectionCorrectionColumn] != "")
            {
                newEntry.insulinInjectionCorrection = Double.Parse(pieces[InsulinInjectionCorrectionColumn]);
            }
            if (pieces[MealCarbohydratesColumn] != "")
            {
                newEntry.mealCarbohydrates = Int32.Parse(pieces[MealCarbohydratesColumn]);
            }
            newEntry.mealDescription = pieces[MealDescriptionColumn];
            newEntry.location = pieces[LocationColumn];
            string[] foodTypes = pieces[FoodTypesColumn].Split(',');
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
                        }
                        if (allfound)
                        {
                            averageOfAllReadingsTagged += (int)entry.bloodGlucoseReading;
                            numberOfReadings++;
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

        public int? GetAverageOfReadingsWithAnySpecifiedTags(String[] tags)
        {
            int averageOfAllReadingsTagged = 0;
            int numberOfReadings = 0;
            foreach (var entry in this.inputRepository.GetLogbook())
            {
                if (entry.bloodGlucoseReading != null)
                {
                    foreach (var tag in tags)
                    {
                        if (entry.tags.Contains(tag))
                        {
                            averageOfAllReadingsTagged += (int)entry.bloodGlucoseReading;
                            numberOfReadings++;
                            break;
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

        private DateTime ParseDateAndTimeIntoOne(DateTime date, DateTime time)
        {
            return new DateTime(
                year: date.Year,
                month: date.Month,
                day: date.Day,
                hour: time.Hour,
                minute: time.Minute,
                second: time.Second);
        }
    }
}
