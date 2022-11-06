using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCDataExtractor.Models
{
    public class Trainer
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<TrainerRaceStats> TrainerRaceStatsList { get; set; }
        public List<RunnerRecord> RunnerRecords { get; set; }
    }
}
