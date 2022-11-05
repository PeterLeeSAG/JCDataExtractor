using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCDataExtractor.Models
{
    public class DrawStats
    {
        public int raceNo { get; set; }
        public string courseInfo { get; set; }
        public List<DrawDetail> DrawDetails { get; set; }
        public string hotPercentW { get; set; }
        public string hotPercentP { get; set; }
        public string hotPercentF { get; set; }
    }
}
