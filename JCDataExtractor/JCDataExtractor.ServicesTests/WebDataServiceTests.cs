using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCDataExtractor.Services;
using JCDataExtractor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            Assert.AreEqual(true, (results.Count != 0 ? true : false));
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
                var pageCount = 1;

                //Check if jockey has ride before
                if (jRank.totalRide != 0) {
                    do
                    {
                        var ridingResults = await polly.ExecuteAsync(async () => await WebDataService.GetRidingRecords(jRank.jockeyId, seasonType, pageCount));
                        fullRidingRecords.AddRange(ridingResults);
                        pageCount++;
                    }
                    while (fullRidingRecords.OrderBy(r => r.index).FirstOrDefault().index > 1);
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
    }
}