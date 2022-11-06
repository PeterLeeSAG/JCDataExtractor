namespace JCDataExtractor.Models
{
    public class RaceInfo
    {
        /// <summary>
        /// pass url to split and find the details of race
        /// </summary>
        /// <param name="url"></param>
        public RaceInfo(string url)
        {
            if (string.IsNullOrEmpty(url) && url.Contains("?"))
            {
                //example: https://racing.hkjc.com/racing/information/Chinese/Racing/LocalResults.aspx?RaceDate=2022/10/30&Racecourse=HV&RaceNo=10
                var parameters = url.Split("?")[1].Split("&");
                if (parameters.Length == 3)
                {
                    DateTime date;
                    DateTime.TryParse(parameters[0].Split("=")[1], out date);
                    this.date = date;
                    course = parameters[1];
                    raceNo = int.Parse(parameters[2]);
                }
            }
        }

        public DateTime date { get; set; }
        public string course { get; set; }
        public int raceNo { get; set; }
    }
}
