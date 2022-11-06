﻿using JCDataExtractor.Models;
using PuppeteerSharp;
using System.Reflection;

namespace JCDataExtractor.Services
{
    public class WebDataService
    {
        public WebDataService()
        {
        }

        /*
            1. https://racing.hkjc.com/racing/information/chinese/Racing/RaceCard.aspx?RaceDate=2022/11/06&Racecourse=ST&RaceNo=1
            呢條係排位表既Link，所有基本資料都係呢個排位表道黎。
            其實條Link 上面有一D變數，日期、場地、同埋場次
            其餘既就全部唔會變
         */
        public static async Task<List<RaceCardEntry>> GetRaceCardEntries(DateTime raceDate, string raceCourse, int raceNo)
        {
            //https://racing.hkjc.com/racing/information/chinese/Racing/RaceCard.aspx?RaceDate=2022/11/06&Racecourse=ST&RaceNo=1
            string url = string.Format(@"https://racing.hkjc.com/racing/information/chinese/Racing/RaceCard.aspx?RaceDate={0}&Racecourse={1}&RaceNo={2}", raceDate.ToString("yyyy/MM/dd"), raceCourse, raceNo.ToString());
            var result = new List<RaceCardEntry>();

            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
                {
                    Path = path + @"\.local-chromium"
                });

                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
                {
                    Headless = true,
                    ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).ExecutablePath
                }))
                {
                    using (var page = await browser.NewPageAsync())
                    {
                        await page.GoToAsync(url);
                        await page.SetViewportAsync(new ViewPortOptions
                        {
                            Width = 1024,
                            Height = 768
                        });

                        Thread.Sleep(1000);

                        //map html table data to list of object via js querySelector here
                        var jsRaceCard = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#racecardlist > tbody > tr > td > table > tbody > tr '));
                        return selectors.map( (tr) => { 
                            const tds = Array.from(tr.querySelectorAll('td'));
                            return {    houseNo:            tds[0].innerHTML, 
                                        last6Runs:          tds[1].innerHTML, 
                                        colour:             tds[2].querySelector('img').src,
                                        horseName:          tds[3].querySelector('a').innerHTML,
                                        brandNo:            tds[4].innerHTML,
                                        takeWeight:         tds[5].innerHTML,
                                        jockey:             tds[6].querySelector('a').innerHTML,
                                        probableOverWeight: tds[7].innerHTML,
                                        draw:               tds[8].innerHTML,
                                        tranier:            tds[9].querySelector('a').innerHTML,
                                        intlRating:         tds[10].innerHTML,
                                        rating:             tds[11].innerHTML,
                                        ratingChange:       tds[12].innerHTML,
                                        horseWeightDeclare: tds[13].innerHTML,
                                        horseWeightDiff:    tds[14].innerHTML,
                                        bestTime:           tds[15].innerHTML,
                                        age:                tds[16].innerHTML,
                                        handicapWeight:     tds[17].innerHTML,
                                        sex:                tds[18].innerHTML,
                                        seasonStakes:       tds[19].innerHTML,
                                        priority:           tds[20].innerHTML,
                                        gear:               tds[21].innerHTML,
                                        owner:              tds[22].innerHTML,
                                        sire:               tds[23].innerHTML,
                                        dam:                tds[24].innerHTML,
                                        importCat:          tds[25].innerHTML
                                }
                            });
                        }";

                        var raceCardResults = await page.EvaluateFunctionAsync<RaceCardEntry[]>(jsRaceCard);

                        if (raceCardResults != null && raceCardResults.Length != 0)
                        {
                            result = raceCardResults.ToList();
                        }
                        else {
                            throw new Exception("No race card result found!");
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return result;
        }

        /*
            2. https://racing.hkjc.com/racing/information/Chinese/racing/Draw.aspx
            呢一條係當日賽事所有場次既檔位統計，基本上係一頁Show晒咁多場，所以條Link 無變數。            
         */
        /// <summary>
        /// Get the list of Draw Statistics for the coming race
        /// </summary>
        /// <returns></returns>
        public static async Task<List<DrawStats>> GetDrawStatsList()
        {
            //https://racing.hkjc.com/racing/information/Chinese/racing/Draw.aspx
            string url = string.Format(@"https://racing.hkjc.com/racing/information/Chinese/racing/Draw.aspx");
            var result = new List<DrawStats>();

            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
                {
                    Path = path + @"\.local -chromium"
                });

                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
                {
                    Headless = true,
                    ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).ExecutablePath
                }))
                {
                    using (var page = await browser.NewPageAsync())
                    {
                        await page.GoToAsync(url);
                        await page.SetViewportAsync(new ViewPortOptions
                        {
                            Width = 1024,
                            Height = 768
                        });

                        Thread.Sleep(1000);

                        //check how many race on the page                        
                        var jsRaceCount = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#innerContent > div.Draw.commContent > div.racingNum.top_races.js_racecard_rt_num > table > tbody > tr > td '));
                        return selectors.length;}";

                        var raceCount = await page.EvaluateFunctionAsync<int>(jsRaceCount);
                        raceCount--;

                        if (raceCount != 0)
                        {
                            for (int i = 1; i <= raceCount; i++)
                            {
                                Thread.Sleep(500);

                                var draw = new DrawStats();
                                draw.raceNo = i;
                                draw.courseInfo = await page.EvaluateFunctionAsync<string>(@"() => { const selector = document.querySelector('#race" + i + " > td'); return selector.innerHTML; }");
                                draw.courseInfo = draw.courseInfo.Replace("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", string.Empty); //remove html space tag
                                draw.DrawDetails = new List<DrawDetail>();
                                
                                var childNodeNo = 3 + (i * 2);
                                //map html table data to list of DrawDetails via js querySelector here
                                var jsDrawDetails = @"() => {
                                const selectors = Array.from(document.querySelectorAll('#innerContent > div.Draw.commContent > div:nth-child(" + childNodeNo + @") > table > tbody > tr '));
                                return selectors.map( (tr) => { 
                                    const tds = Array.from(tr.querySelectorAll('td'));
                                    return { draw:      tds[0].innerHTML, 
                                             runners:   tds[1].innerHTML, 
                                             win:       tds[2].innerHTML,
                                             second:    tds[3].innerHTML,
                                             third:     tds[4].innerHTML,
                                             forth:     tds[5].innerHTML,
                                             percentW:  tds[6].innerHTML,
                                             percentQ:  tds[7].innerHTML,
                                             percentP:  tds[8].innerHTML,
                                             percentF:  tds[9].innerHTML
                                            };
                                        });
                                }";

                                var drawDetails = await page.EvaluateFunctionAsync<DrawDetail[]>(jsDrawDetails);
                                if (drawDetails != null && drawDetails.Length != 0) {
                                    draw.DrawDetails.AddRange(drawDetails.ToList());
                                }

                                //Hot percents
                                var jsHotPercents = @"() => {
                                const selectors = Array.from(document.querySelectorAll('#innerContent > div.Draw.commContent > div:nth-child(" + childNodeNo + @") > table > tfoot > tr > td:nth-child(2) > span '));
                                return selectors.map( (span) => span.innerHTML);
                                }";

                                var hotPercents = await page.EvaluateFunctionAsync<string[]>(jsHotPercents);

                                draw.hotPercentW = hotPercents[0];
                                draw.hotPercentP = hotPercents[1];
                                draw.hotPercentF = hotPercents[2];

                                //Add drawStat object to the result
                                result.Add(draw);
                            }
                        }

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return result;
        }

        /*
         3. https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyRanking.aspx?Season=Current&View=Numbers&Racecourse=ALL
            https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyRanking.aspx?Season=Previous&View=Numbers&Racecourse=ALL
            呢一條係騎師統計表，當中有變數，Current代表今季資料，Previous代表上季
         */
        public static async Task<List<JockeyRaceRow>> GetJockeyRankingTable(string seasonType)
        {
            string url = string.Format(@"https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyRanking.aspx?Season=" + seasonType + @"&View=Numbers&Racecourse=ALL");
            var result = new List<JockeyRaceRow>();

            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
                {
                    Path = path + @"\.local -chromium"
                });

                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
                {
                    Headless = true,
                    ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).ExecutablePath
                }))
                {
                    using (var page = await browser.NewPageAsync())
                    {
                        await page.GoToAsync(url);
                        await page.SetViewportAsync(new ViewPortOptions
                        {
                            Width = 1024,
                            Height = 768
                        });

                        Thread.Sleep(1000);

                        //check how many race on the page                        
                        var jsJockeyRank = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#innerContent > div.commContent > div.Ranking > div:nth-child(3) > table > tbody:nth-child(2) > tr '));
                        return selectors.map( (tr) => { 
                                    const tds = Array.from(tr.querySelectorAll('td'));
                                    return { 
                                             jockeyName: tds[0].querySelector('a').innerHTML.replaceAll('\n','').replace(/ /g,''),
                                             jockeyId:   tds[0].querySelector('a').href.split('=')[1].split('&')[0],
                                             count1st:   tds[1].innerHTML,
                                             count2nd:   tds[2].innerHTML,
                                             count3rd:   tds[3].innerHTML,
                                             count4th:   tds[4].innerHTML,
                                             count5th:   tds[5].innerHTML,
                                             totalRide:  tds[6].innerHTML,
                                             stakesWon:  tds[7].innerHTML.replace('$','').replaceAll(',','')
                                            }
                                        });
                                }";

                        var JockeyRaceRows = await page.EvaluateFunctionAsync<JockeyRaceRow[]>(jsJockeyRank);
                        result = JockeyRaceRows.ToList();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return result;
        }

        /*
            4. https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerRanking.aspx?Season=Current&View=Numbers&Racecourse=ALL
               https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerRanking.aspx?Season=Previous&View=Numbers&Racecourse=ALL

            呢一條係練馬師統計表，同騎師統計表一樣，當中有一個變數，Current代表今季資料，Previous代表上季
         */

        /*
            5. https://racing.hkjc.com/racing/information/Chinese/Horse/Horse.aspx?HorseId=HK_2021_G232&Option=1
            呢條係馬匹既，檔中既變數就係馬匹編號，HK_2021_G232
            HK_2021係隻馬黎港日期，G232係馬匹烙號
            係排表表馬匹名既超連結入面可以提取到呢個馬匹編號

            https://racing.hkjc.com/racing/information/chinese/Horse/SelectHorsebyChar.aspx?ordertype=2
            house list, ordertype = 2,3,4
         */

        /*
            6. https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyPastRec.aspx?JockeyId=BV&Season=Current&PageNum=1
            呢個係騎師既，當中有變數JockeyId、Curent、PageNum
            JockeyId同馬匹一樣，可以係排位表個超連結上面提取到
            最麻煩就係PageNum，1代表第一頁，如果得2頁既話，你輸入3佢都會比繼續比第二頁你，所以我個程式要檢查有無「下一頁」呢個字去判斷有無下一頁。    
         */
        public static async Task<List<RidingRecord>> GetRidingRecords(string jockeyID, string seasonType, int pageID = 1)
        {
            //https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyPastRec.aspx?JockeyId=BV&Season=Current&PageNum=1
            string url = string.Format(@"https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyPastRec.aspx?JockeyId={0}&Season={1}&PageNum={2}"
                , jockeyID
                , seasonType
                , pageID
                );
            var result = new List<RidingRecord>();

            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
                {
                    Path = path + @"\.local -chromium"
                });

                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
                {
                    Headless = true,
                    ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).ExecutablePath
                }))
                {
                    using (var page = await browser.NewPageAsync())
                    {
                        await page.GoToAsync(url);
                        await page.SetViewportAsync(new ViewPortOptions
                        {
                            Width = 1024,
                            Height = 768
                        });

                        Thread.Sleep(1000);

                        var jsRidingRecords = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#innerContent > div.jockeyPastRec.commContent > div.ridingRec > table > tbody > tr '));
                        return selectors.map( (tr) => { 
                                    const tds = Array.from(tr.querySelectorAll('td'));
                                    if (tds.length == 13) {
                                        return { 
                                             index        :tds[0].querySelector('a').innerHTML,
                                             raceURL      :tds[0].querySelector('a').href,
                                             placing      :tds[1].innerHTML,
                                             trackCourse  :tds[2].innerHTML,
                                             distance     :tds[3].innerHTML,
                                             raceClass    :tds[4].innerHTML,
                                             going        :tds[5].innerHTML,
                                             house        :tds[6].innerHTML,
                                             draw         :tds[7].innerHTML,
                                             rtg          :tds[8].innerHTML,
                                             trainer      :tds[9].innerHTML,
                                             gear         :tds[10].innerHTML,
                                             bodyWeight   :tds[11].innerHTML,
                                             actualWeight :tds[12].innerHTML
                                            }
                                        }
                                    });
                                }";

                        var ridingRecords = await page.EvaluateFunctionAsync<RidingRecord[]>(jsRidingRecords);
                        if (ridingRecords != null && ridingRecords.Length != 0)
                        {
                            result = ridingRecords.Where(r => r != null).ToList();
                        }
                        else
                        {
                            throw new Exception("No riding records found!");
                        }
                        
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return result;
        }

        /*
            7. https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerPastRec.aspx?TrainerId=SJJ&Season=Current&PageNum=1
            呢個係練馬師既，原理同騎師既一樣
         */
    }
}