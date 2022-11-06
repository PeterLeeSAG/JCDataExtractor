using JCDataExtractor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;

namespace JCDataExtractor.Services.Tests
{
    [TestClass()]
    public class WebDataServiceTests
    {
        [TestMethod()]
        public async Task GetRaceCardEntriesTest()
        {
            var polly = Policy
              .Handle<Exception>()
              .RetryAsync(3, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetRaceCardEntries(DateTime.Parse("2022/11/06"), "st", 1));
            Assert.AreEqual(true, (results.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetDrawStatsListTest()
        {
            var results = await WebDataService.GetDrawStatsList();
            Assert.AreEqual(true, (results.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetRidingRecordsTest()
        {            
            var polly = Policy
            .Handle<Exception>()
            .RetryAsync(3, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetRidingRecords("PZ", "Current"));

            Assert.AreEqual(true, (results.Item1.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetJockeyRankingTableTest()
        {
            var seasonType = "Current";
            var polly = Policy
           .Handle<Exception>()
           .RetryAsync(3, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetJockeyRankingTable(seasonType));

            Assert.AreEqual(true, results.Count != 0 ? true : false);
        }

        [TestMethod()]
        public async Task GetMultipleJockeyTest()
        {
            var seasonType = "Current";
            var JockeyList = new List<Jockey>();

            var polly = Policy
           .Handle<Exception>()
           .RetryAsync(3, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var jockeyRanks = await polly.ExecuteAsync(async () => await WebDataService.GetJockeyRankingTable(seasonType));

            //foreach (var jRank in jockeyRanks)
            for(int i = 0; i < 3; i++) //test only top 3 here
            {
                var jRank = jockeyRanks[i];
                var jockey = new Jockey();
                jockey.id = jRank.jockeyId;
                jockey.name = jRank.jockeyName;

                var fullRidingRecords = new List<RidingRecord>();

                //Check if jockey has ride before
                if (jRank.totalRide != 0) {
                    var ridingResults = await polly.ExecuteAsync(async () => await WebDataService.GetFullRidingRecords(jRank.jockeyId, seasonType));
                    fullRidingRecords.AddRange(ridingResults);
                }

                //Map ranking to yearly race stats
                var jStats = new JockeyRaceStats();
                jStats.year = (seasonType == "Current" ? DateTime.Now.Year : DateTime.Now.Year - 1);
                jStats.count1st = jRank.count1st;
                jStats.count2nd = jRank.count2nd;
                jStats.count3rd = jRank.count3rd;
                jStats.count4th = jRank.count4th;
                jStats.count5th = jRank.count5th;
                jStats.totalRide = jRank.totalRide;
                jStats.stakesWon = jRank.stakesWon;

                jockey.JockeyRaceStatsList = new List<JockeyRaceStats>();
                jockey.JockeyRaceStatsList.Add(jStats);
                jockey.RidingRecords = fullRidingRecords;

                JockeyList.Add(jockey);
            }

            Assert.AreEqual(true, JockeyList.Count != 0 ? true : false);
        }

        [TestMethod()]
        public async Task GetRunnerRecordsTest()
        {
            var polly = Policy
            .Handle<Exception>()
            .RetryAsync(3, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetRunnerRecords("CAS", "Current"));

            Assert.AreEqual(true, (results.Item1.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetTrainerRankingTableTest()
        {
            var seasonType = "Current";
            var polly = Policy
           .Handle<Exception>()
           .RetryAsync(3, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetTrainerRankingTable(seasonType));

            Assert.AreEqual(true, results.Count != 0 ? true : false);
        }

        [TestMethod()]
        public async Task GetMultipleTrainerTest()
        {
            var seasonType = "Current";
            var trainerList = new List<Trainer>();

            var polly = Policy
           .Handle<Exception>()
           .RetryAsync(3, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var trainerRanks = await polly.ExecuteAsync(async () => await WebDataService.GetTrainerRankingTable(seasonType));

            if (trainerRanks != null && trainerRanks.Count >= 3)
            {
                //foreach (var jRank in jockeyRanks)
                for (int i = 0; i < 3; i++) //test only top 3 here
                {
                    var tRank = trainerRanks[i];
                    var trainer = new Trainer();
                    trainer.id = tRank.trainerId;
                    trainer.name = tRank.trainerName;

                    var fullRunnerRecords = new List<RunnerRecord>();

                    //Check if jockey has ride before
                    if (tRank.totalRun != 0)
                    {
                        var ridingResults = await polly.ExecuteAsync(async () => await WebDataService.GetFullRunnerRecords(tRank.trainerId, seasonType));
                        fullRunnerRecords.AddRange(ridingResults);
                    }

                    //Map ranking to yearly race stats
                    var tStats = new TrainerRaceStats();
                    tStats.year = (seasonType == "Current" ? DateTime.Now.Year : DateTime.Now.Year - 1);
                    tStats.count1st = tRank.count1st;
                    tStats.count2nd = tRank.count2nd;
                    tStats.count3rd = tRank.count3rd;
                    tStats.count4th = tRank.count4th;
                    tStats.count5th = tRank.count5th;
                    tStats.totalRun = tRank.totalRun;
                    tStats.stakesWon = tRank.stakesWon;

                    trainer.TrainerRaceStatsList = new List<TrainerRaceStats>();
                    trainer.TrainerRaceStatsList.Add(tStats);
                    trainer.RunnerRecords = fullRunnerRecords;

                    trainerList.Add(trainer);
                }
            }            

            Assert.AreEqual(true, trainerList.Count != 0 ? true : false);
        }
    }
}