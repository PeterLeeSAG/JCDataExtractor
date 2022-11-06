using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCDataExtractor.Services;
using JCDataExtractor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCDataExtractor.Services.Tests
{
    [TestClass()]
    public class WebDataServiceTests
    {
        [TestMethod()]
        public async Task GetRaceCardEntriesTest()
        {
            var results = await WebDataService.GetRaceCardEntries(DateTime.Parse("2022/11/06"), "st", 1);
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
            var results = await WebDataService.GetRidingRecords("PZ", "Current");
            Assert.AreEqual(true, (results.Count != 0 ? true : false));
        }

        [TestMethod()]
        public async Task GetJockeyRankingTableTest()
        {
            var results = await WebDataService.GetJockeyRankingTable("Current");
            Assert.AreEqual(true, results.Count != 0 ? true : false);
        }
    }
}