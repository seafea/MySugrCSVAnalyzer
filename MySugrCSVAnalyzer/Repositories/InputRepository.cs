using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySugrCSVAnalyzer.Models;

namespace MySugrCSVAnalyzer.Repositories
{
    using MySugrCSVAnalyzer.Interfaces;
    class InputRepository : IInputRepository
    {
        private List<Entry> logbook;

        private Dictionary<string, List<Entry>> logbookByDay;

        public List<Entry> GetLogbook()
        {
            return logbook;
        }

        public Dictionary<string, List<Entry>> GetLogbookByDay()
        {
            return logbookByDay;
        }

        public void setLogbook(List<Entry> newLogbook)
        {
            logbook = newLogbook;
        }

        public void setLogbookByDay(Dictionary<string, List<Entry>> newLogbookByDay)
        {
            logbookByDay = newLogbookByDay;
        }
    }
}
