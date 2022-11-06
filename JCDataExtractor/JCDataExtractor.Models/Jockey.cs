using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCDataExtractor.Models
{
    public class Jockey
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<JockeyRaceStats> JockeyRaceStatsList { get; set; }
        public List<RidingRecord> RidingRecords { get; set; }
    }
}
