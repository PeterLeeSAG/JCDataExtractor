using Microsoft.VisualStudio.TestTools.UnitTesting;
using JCDataExtractor.Services;
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
        public async Task GetRacingRardEntriesTest()
        {
            var results = await WebDataService.GetRacingRardEntries(DateTime.Parse("2022/11/06"), "st", 1);
            Assert.Fail();
        }
    }
}