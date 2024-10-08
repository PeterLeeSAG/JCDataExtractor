﻿using JCDataExtractor.Services;
using JCDataExtractor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace JCDataExtractor.Services.Tests
{
    [TestClass()]
    public class WebDataServiceTests
    {
        [TestMethod()]
        public async Task GetRaceCardTest()
        {
            var polly = Policy
              .Handle<Exception>()
              .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetRunnerRecordsByRace("https://bet.hkjc.com/ch/racing/wpq/2024-08-11/S1/3")); //The date sb changed for available date
            //SHOW json here:
            //Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));
            Assert.AreEqual(true, (results.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetRaceCardEntriesTest()
        {
            var polly = Policy
              .Handle<Exception>()
              .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetRaceCardEntries(DateTime.Parse("2023/03/11"), "st", 1)); //The date sb changed for available date
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));
            Assert.AreEqual(true, (results.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetDrawStatsListTest()
        {
            var polly = Policy
              .Handle<Exception>()
              .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetDrawStatsList());
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));
            Assert.AreEqual(true, (results.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetRidingRecordsTest()
        {
            var polly = Policy
            .Handle<Exception>()
            .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetRidingRecords("PZ", "Current"));
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));
            Assert.AreEqual(true, (results.Item1.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetJockeyRankingTableTest()
        {
            var seasonType = "Current";
            var polly = Policy
           .Handle<Exception>()
           .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetJockeyRankingTable(seasonType));
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));

            seasonType = "Previous";
            results = await polly.ExecuteAsync(async () => await WebDataService.GetJockeyRankingTable(seasonType));
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));

            Assert.AreEqual(true, results.Count != 0 ? true : false);
        }

        [TestMethod()]
        public async Task GetMultipleJockeyTest()
        {
            var seasonType = "Current";
            var JockeyList = new List<Jockey>();

            var polly = Policy
           .Handle<Exception>()
           .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var jockeyRanks = await polly.ExecuteAsync(async () => await WebDataService.GetJockeyRankingTable(seasonType));

            //foreach (var jRank in jockeyRanks)
            for (int i = 0; i < 10; i++) //test only top 10 here
            {
                var jRank = jockeyRanks[i];
                var jockey = new Jockey();
                jockey.id = jRank.jockeyId;
                jockey.name = jRank.jockeyName;

                var fullRidingRecords = new List<RidingRecord>();

                //Check if jockey has ride before
                if (jRank.totalRide != 0)
                {
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
                //SHOW json here:
                Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(jockey)));
            }

            Assert.AreEqual(true, JockeyList.Count != 0 ? true : false);
        }

        [TestMethod()]
        public async Task GetRunnerRecordsTest()
        {
            var polly = Policy
            .Handle<Exception>()
            .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetRunnerRecords("CAS", "Current"));
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));
            Assert.AreEqual(true, (results.Item1.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetTrainerRankingTableTest()
        {
            var seasonType = "Current";
            var polly = Policy
           .Handle<Exception>()
           .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetTrainerRankingTable(seasonType));
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));
            Assert.AreEqual(true, results.Count != 0 ? true : false);
        }

        [TestMethod()]
        public async Task GetMultipleTrainerTest()
        {
            var seasonType = "Current";
            var trainerList = new List<Trainer>();

            var polly = Policy
           .Handle<Exception>()
           .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var trainerRanks = await polly.ExecuteAsync(async () => await WebDataService.GetTrainerRankingTable(seasonType));

            if (trainerRanks != null && trainerRanks.Count >= 3)
            {
                //foreach (var jRank in jockeyRanks)
                for (int i = 0; i < 10; i++) //test only top 10 here
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
                    //SHOW json here:
                    Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(trainer)));
                }
            }

            Assert.AreEqual(true, trainerList.Count != 0 ? true : false);
        }

        [TestMethod()]
        public async Task GetHorseIDNameListTest()
        {
            var wordCount = 2;
            var polly = Policy
               .Handle<Exception>()
               .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var results = await polly.ExecuteAsync(async () => await WebDataService.GetHorseIDNameList(wordCount));
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(results)));
            Assert.AreEqual(true, results.Count != 0 ? true : false);
        }

        [TestMethod()]
        public async Task GetHorseRecordTest()
        {
            //https://racing.hkjc.com/racing/information/Chinese/Horse/Horse.aspx?HorseId=HK_2020_E131&Option=1
            var horseID = "HK_2020_E131"; //"TODO: HK_2020_E131" is retired, need to update handling js
            var polly = Policy
               .Handle<Exception>()
               .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            var result = await polly.ExecuteAsync(async () => await WebDataService.GetHorseRecord(horseID));
            //SHOW json here:
            Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(result)));
            Assert.AreEqual(true, result != null ? true : false);
        }

        [TestMethod()]
        public async Task GetMultipleHorseRecordsTest()
        {
            var horses = new List<Horse>();
            var polly = Policy
               .Handle<Exception>()
               .RetryAsync(5, (exception, retryCount, context) => Console.WriteLine($"try: {retryCount}, Exception: {exception.Message}"));

            int[] wordCounts = { 2, 3, 4 };

            foreach (var wordCount in wordCounts)
            {
                var horseList = await polly.ExecuteAsync(async () => await WebDataService.GetHorseIDNameList(wordCount));
                var count = 0;

                foreach (var horse in horseList)
                {
                    if (count >= 10)
                    {
                        break;
                    }

                    Console.WriteLine("Testing: " + horse.id);
                    var horseResult = await polly.ExecuteAsync(async () => await WebDataService.GetHorseRecord(horse.id));
                    if (horseResult.Item2)
                    {
                        var horseData = horseResult.Item1;
                        horseData.name = horse.name;
                        horseData.id = horse.id;

                        horses.Add(horseData);
                        //SHOW json here:
                        Console.WriteLine(String.Format("DATA: {0}", JsonConvert.SerializeObject(horseResult.Item1)));
                    }
                    count++;
                }
            }

            Assert.AreEqual(true, horses != null && horses.Count != 0 ? true : false);
        }
    }
}