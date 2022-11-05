namespace JCDataExtractor.Models
{
    public class RaceCard
    {
        public DateTime raceDate { get; set; }
        public string raceCourse { get; set; }
        public int raceNo { get; set; }
        public List<RaceCardEntry> RaceCardEntries { get; set; }

    }
}