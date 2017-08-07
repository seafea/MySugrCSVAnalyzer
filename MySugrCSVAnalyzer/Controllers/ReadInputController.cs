using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySugrCSVAnalyzer.Models;

namespace MySugrCSVAnalyzer.Controllers
{
    public class ReadInputController
    {
        public string fileName { get; set; }
        public Dictionary<string, Models.Entry> logbook { get; private set; }
        public ReadInputController()
        {
            logbook = new Dictionary<string, Entry>();
        }

        public void readInputFile()
        {
            string line = "";
            int numColumns = -1;
            using (StreamReader input = new StreamReader(File.OpenRead(fileName)))
            {
                if (!input.EndOfStream)
                {
                    line = input.ReadLine();
                    numColumns = line.Split(',').Length;
                }
                while (!input.EndOfStream)
                {
                    line = input.ReadLine();
                    MessageBox.Show(parseLineToEntry(line, numColumns).bloodGlucoseReading.ToString());
                }
            }
        }

        public Entry parseLineToEntry(string line, int numColumns)
        {
            string[] pieces = new string[numColumns];
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
    }
}
