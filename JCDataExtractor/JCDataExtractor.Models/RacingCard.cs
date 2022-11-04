namespace JCDataExtractor.Models
{
    public class RacingCard
    {
        public DateTime raceDate { get; set; }
        public string raceCourse { get; set; }
        public int raceNo { get; set; }
        public List<RacingCardEntry> RacingCardEntries { get; set; }

    }
}