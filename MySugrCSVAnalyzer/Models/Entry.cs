using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySugrCSVAnalyzer.Models
{
    public class Entry
    {
        public DateTime entryDateTime { get; set; }
        public List<string> tags { get; private set; }
        public int? bloodGlucoseReading { get; set; }
        public double? insulinInjectionMeal { get; set; }
        public double? insulinInjectionCorrection { get; set; }
        public int? temporaryBasalPercentage { get; set; }
        public int? temporaryBasalRate { get; set; }
        public int? mealCarbohydrates { get; set; }
        public string mealDescription { get; set; }
        public string location { get; set; }
        public List<string> foodTypes { get; private set; }

        public Entry(DateTime newEntryDateTime)
        {
            entryDateTime = newEntryDateTime;
            tags = new List<string>();
            foodTypes = new List<string>();
        }

        public void addTag(string tag)
        {
            tags.Add(tag);
        }

        public void addFoodType(string foodType)
        {
            foodTypes.Add(foodType);
        }
    }
}
