using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySugrCSVAnalyzer.Models;

namespace MySugrCSVAnalyzer.Interfaces
{
    public interface IReadInputController
    {
        void readInputFile();
        Entry parseLineToEntry(string line);
        Dictionary<string, int> GetDailyAverages();
        void LoadLogbookByDay();
        string fileName { get; set; }
        int? GetAverageOfAllTaggedHappy();
        int? GetAverageOfReadingsWithSpecifiedTag(string[] tags);
    }
}
