using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCDataExtractor.Models
{
    /// <summary>
    /// Race yearly result statistics for jockey
    /// </summary>
    public class TrainerRaceStats
    {
        public int year { get; set; }
        public int count1st { get; set; }
        public int count2nd { get; set; }
        public int count3rd { get; set; }
        public int count4th { get; set; }
        public int count5th { get; set; }
        public int totalRun { get; set; }
        public int stakesWon { get; set; }
    }

    /// <summary>
    /// for html table to list of raceStats
    /// </summary>
    public class TrainerRaceRow
    {
        public string trainerName  { get; set; }
        public string trainerId  { get; set; }
        public int    count1st  { get; set; }
        public int    count2nd  { get; set; }
        public int    count3rd  { get; set; }
        public int    count4th  { get; set; }
        public int    count5th  { get; set; }
        public int    totalRun { get; set; }
        public int    stakesWon { get; set; }
    }
}
