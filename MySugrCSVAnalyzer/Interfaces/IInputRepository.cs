using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySugrCSVAnalyzer.Models;

namespace MySugrCSVAnalyzer.Interfaces
{
    public interface IInputRepository
    {
        List<Entry> GetLogbook();

        Dictionary<string, List<Entry>> GetLogbookByDay();

        void setLogbook(List<Entry> newLogbook);

        void setLogbookByDay(Dictionary<string, List<Entry>> newLogbookByDay);
    }
}
