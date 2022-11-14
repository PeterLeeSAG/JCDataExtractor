using JCDataExtractor.Models;
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
                            return {    horseNo:            tds[0].innerHTML, 
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
        public static async Task<List<TrainerRaceRow>> GetTrainerRankingTable(string seasonType)
        {
            string url = string.Format(@"https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerRanking.aspx?Season=" + seasonType + @"&View=Numbers&Racecourse=ALL");
            var result = new List<TrainerRaceRow>();

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
                        var jsTrainerRank = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#innerContent > div.commContent > div.Ranking > div:nth-child(3) > table > tbody:nth-child(2) > tr '));
                        return selectors.map( (tr) => { 
                                    const tds = Array.from(tr.querySelectorAll('td'));
                                    return { 
                                             trainerName: tds[0].querySelector('a').innerHTML,
                                             trainerId:   tds[0].querySelector('a').href.split('=')[1].split('&')[0],
                                             count1st:    tds[1].innerHTML,
                                             count2nd:    tds[2].innerHTML,
                                             count3rd:    tds[3].innerHTML,
                                             count4th:    tds[4].innerHTML,
                                             count5th:    tds[5].innerHTML,
                                             totalRun:    tds[6].innerHTML,
                                             stakesWon:   tds[7].innerHTML.replace('$','').replaceAll(',','')
                                            }
                                        });
                                }";

                        var trainerRaceRows = await page.EvaluateFunctionAsync<TrainerRaceRow[]>(jsTrainerRank);
                        result = trainerRaceRows.ToList();
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
            5. https://racing.hkjc.com/racing/information/Chinese/Horse/Horse.aspx?HorseId=HK_2021_G232&Option=1
            呢條係馬匹既，檔中既變數就係馬匹編號，HK_2021_G232
            HK_2021係隻馬黎港日期，G232係馬匹烙號
            係排表表馬匹名既超連結入面可以提取到呢個馬匹編號
         */
        public static async Task<Tuple<Horse, bool>> GetHorseRecord(string horseID)
        {
            //https://racing.hkjc.com/racing/information/Chinese/Horse/Horse.aspx?HorseId=HK_2021_G232&Option=1
            string url = string.Format(@"https://racing.hkjc.com/racing/information/Chinese/Horse/Horse.aspx?HorseId={0}&Option=1"
                , horseID
                );
            var result = new Horse();
            var isNextPage = false;

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
                        /*
                         *  Left
                            1出生地 / 馬齡	:	英國 / 4
                            2毛色 / 性別	:	棗 / 閹
                            3進口類別	:	自購馬
                            4今季獎金*	:	$28,350
                            5總獎金*	:	$83,300
                            6冠-亞-季-總出賽次數*	:	0-0-0-14
                            7最近十個賽馬日
                            出賽場數	:	1
                            8現在位置
                            (到達日期)	:	從化
                            (08/11/2022)
                            
                            Right
                            9練馬師	:	蔡約翰
                            10馬主	:	郭少明
                            11現時評分	:	38
                            12季初評分	:	40
                            13父系	:	Territories
                            14母系	:	Folly Bridge
                            15外祖父	:	Avonbridge
                            16同父系馬	: 紅運泰斗
 
                         */
                        //Left column
                        //#innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(2) > table > tbody > tr:nth-child(1) > td:nth-child(3)
                        //#innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(2) > table > tbody > tr:nth-child(2) > td:nth-child(3)
                        //#innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(2) > table > tbody > tr:nth-child(8) > td:nth-child(3)

                        //Right column (a: 1,2,5)
                        //#innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(3) > table > tbody > tr:nth-child(1) > td:nth-child(3) > a
                        //#innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(3) > table > tbody > tr:nth-child(7) > td:nth-child(3)
                        //#SameSire > option:nth-child(2)

                        var jsCols = @"()=>{                        
                        var selectors = Array.from(document.querySelectorAll('#innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(2) > table > tbody > tr'));
                        var arr = [];
                        
                        //Left
                        for (j=0; j<selectors.length; j++)
                        {
                            var row = selectors[j];
                            var target = row.querySelector('td:nth-of-type(3)');
                            arr.push(target);
                        };

                        //Right
                        selectors = Array.from(document.querySelectorAll('#innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(3) > table > tbody > tr'));
                        for (i=0; i<selectors.length; i++)
                        {
                            var row = selectors[i];
                            var target = row.querySelector('td:nth-of-type(3)');
                            arr.push(target);
                        };

                        if (arr != null && arr.length)
                        {
                            var pArr = arr[5].innerHTML.split('-');
                            var options = Array.from(document.querySelectorAll('#SameSire > option'));
                            var sameSireArr = options.map( (option) => { return option.innerHTML });
                            return {
                                countryOrigin : arr[0].innerHTML.split('/')[0].trim(),   
                                age           : arr[0].innerHTML.split('/')[1].trim(),      
                                colour        : arr[1].innerHTML.split('/')[0].trim(),     
                                sex	          : arr[1].innerHTML.split('/')[1].trim(),
                                importType	  : arr[2].innerHTML,        
                                seasonStakes  : arr[3].innerHTML.replace('$','').replace(',',''),           
                                totalStakes   : arr[4].innerHTML.replace('$','').replace(',',''),           
                                countFirstThreeAndStarts : [pArr[0], pArr[1], pArr[2], pArr[3]],
                                countStartsInPast10Races : arr[6].innerHTML, 
                                currentStableLocation    : arr[7].innerHTML.split('(')[0].trim(),
                                arrivalDate              : arr[7].innerHTML.split('(')[1].replace(')','').trim(),
                                trainer                  : arr[8].querySelector('a').innerHTML,
                                owner                    : arr[9].querySelector('a').innerHTML,
                                currentRating	         : arr[10].innerHTML, 
                                startofSeasonRating	     : arr[11].innerHTML,
                                sire                     : arr[12].querySelector('a').innerHTML,
                                dam	                     : arr[13].innerHTML,   
                                damSire	                 : arr[14].innerHTML,
                                sameSire                 : sameSireArr
                                };
                        };
                        return null;
                        }";

                        var horseInfo = await page.EvaluateFunctionAsync<HorseInfo>(jsCols);
                        result.info = horseInfo;

                        /*場次,名次,日期,馬場/跑道/賽道,途程,場地狀況,賽事班次,檔位,評分,練馬師,騎師,頭馬距離,獨贏賠率,實際負磅,沿途走位,完成時間,排位體重,配備*/
                        //#innerContent > div.commContent > div:nth-child(1) > table.bigborder > tbody > tr:nth-child(3)
                        //#innerContent > div.commContent > div:nth-child(1) > table.bigborder > tbody > tr:nth-child(3) > td:nth-child(1)
                        //#innerContent > div.commContent > div:nth-child(1) > table.bigborder > tbody > tr:nth-child(3) > td:nth-child(1) > a
                        //#innerContent > div.commContent > div:nth-child(1) > table.bigborder > tbody > tr:nth-child(3) > td:nth-child(2)
                        //#innerContent > div.commContent > div:nth-child(1) > table.bigborder > tbody > tr:nth-child(3) > td:nth-child(18)
                        //#innerContent > div.commContent > div:nth-child(1) > table.bigborder > tbody > tr:nth-child(3) > td:nth-child(15) > span
                        var jsHorseFormRecords = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#innerContent > div.commContent > div:nth-child(1) > table.bigborder > tbody > tr'));
                        return selectors.map( (tr) => { 
                                    const tds = Array.from(tr.querySelectorAll('td'));
                                    if (tds.length == 19 && tds[4].innerHTML != '途程') {
                                        return { 
                                             index        :tds[0].querySelector('a').innerHTML,
                                             raceURL      :tds[0].querySelector('a').href,
                                             placing      :tds[1].querySelector('span').innerHTML,
                                             raceDate     :tds[2].innerHTML,
                                             trackCourse  :tds[3].innerHTML,
                                             distance     :tds[4].innerHTML,           
                                             going        :tds[5].innerHTML,
                                             raceClass    :tds[6].innerHTML,
                                             draw         :tds[7].innerHTML,
                                             rtg          :tds[8].innerHTML,
                                             trainer      :tds[9].querySelector('a').innerHTML,
                                             jockey       :tds[10].querySelector('a').innerHTML,
                                             LBW          :tds[11].querySelector('span').innerHTML,
                                             winOdds      :tds[12].innerHTML,
                                             actualWeight :tds[13].innerHTML,
                                             runningPosition:tds[14].innerHTML,
                                             finishTime   :tds[15].innerHTML,
                                             bodyWeight   :tds[16].innerHTML,
                                             gear         :tds[17].innerHTML,
                                            }
                                        }
                                    });
                                }";

                        var horseFromRecords = await page.EvaluateFunctionAsync<HorseFormRecord[]>(jsHorseFormRecords);
                        if (horseFromRecords != null && horseFromRecords.Length != 0)
                        {
                            result.horseFormRecords = horseFromRecords.Where(r => r != null).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return new Tuple<Horse, bool>(result, isNextPage);
        }

        /*
         * table 1- > tr -> 3rd td: #innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(2) > table > tr > td[2]
         * table 2- > tr -> 3rd td: #innerContent > div.commContent > div:nth-child(1) > table.horseProfile > tbody > tr > td:nth-child(3) > table > tr > td[2]
         * #SameSire > option:nth-child(1) > value (id), innerHTML (name)
         */

        /*
            https://racing.hkjc.com/racing/information/chinese/Horse/SelectHorsebyChar.aspx?ordertype=2
            horse list, ordertype = 2,3,4
         */
        public static async Task<List<Horse>> GetHorseIDNameList(int chineseCount = 2)
        {
            List<Horse> horses = new List<Horse>();
            string url = string.Format(@"https://racing.hkjc.com/racing/information/chinese/Horse/SelectHorsebyChar.aspx?ordertype={0}"
                , chineseCount
                );

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
                        //#innerContent > div.commContent > p > table > tbody > tr:nth-child(2) > td > table > tbody > tr:nth-child(24) > td:nth-child(5) > table > tbody > tr > td.table_text_two > li > a
                        //#innerContent > div.commContent > p > table > tbody > tr:nth-child(2) > td > table > tbody > tr:nth-child(1) > td:nth-child(1) > table > tbody > tr > td.table_text_two > li > a

                        var jsHorseRecords = @"() => {
                            var arr = [];
                            const selectors = Array.from(document.querySelectorAll('#innerContent > div.commContent > p > table > tbody > tr:nth-child(2) > td > table > tbody > tr '));
                            for (j=0; j<selectors.length; j++)
                            {
                                var row = selectors[j];
                                var tds = Array.from(row.querySelectorAll('td'));
                                for (k=0; k<tds.length; k++)
                                {
                                    if (tds[k] != null && tds[k].querySelector('table > tbody > tr > td.table_text_two > li') != null)
                                    {
                                        var item = tds[k].querySelector('table > tbody > tr > td.table_text_two > li');
                                        arr.push( { 
                                                    horseName    :item.querySelector('a').innerHTML,
                                                    horseID      :item.querySelector('a').href.split('=')[1],
                                                    });
                                    }                                    
                                }
                            };
                            return arr;
                        }";

                        var horseRecords = await page.EvaluateFunctionAsync<HorseList[]>(jsHorseRecords);
                        if (horseRecords != null && horseRecords.Length != 0)
                        {
                            var list = horseRecords.Where(r => r != null).ToList();
                            foreach (var h in list)
                            {
                                horses.Add(new Horse { id = h.horseID, name = h.horseName });
                            }
                        }
                        else
                        {
                            throw new Exception("No riding records found!");
                        }

                        return horses;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return horses;
        }


        /*
            6. https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyPastRec.aspx?JockeyId=BV&Season=Current&PageNum=1
            呢個係騎師既，當中有變數JockeyId、Curent、PageNum
            JockeyId同馬匹一樣，可以係排位表個超連結上面提取到
            最麻煩就係PageNum，1代表第一頁，如果得2頁既話，你輸入3佢都會比繼續比第二頁你，所以我個程式要檢查有無「下一頁」呢個字去判斷有無下一頁。    
        */
            public static async Task<Tuple<List<RidingRecord>, bool>> GetRidingRecords(string jockeyID, string seasonType, int pageID = 1)
            {
            //https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyPastRec.aspx?JockeyId=BV&Season=Current&PageNum=1
            string url = string.Format(@"https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyPastRec.aspx?JockeyId={0}&Season={1}&PageNum={2}"
                , jockeyID
                , seasonType
                , pageID
                );
            var result = new List<RidingRecord>();
            var isNextPage = false;

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

                        //#innerContent > div.jockeyPastRec.commContent > div.ridingRec > p.f_clear.f_fs13 > span.f_fr.page_pre_head > a
                        var jsNextPage = @"()=>{                        
                        const selector = document.querySelectorAll('#innerContent > div.jockeyPastRec.commContent > div.ridingRec > p.f_clear.f_fs13 > span.f_fr.page_pre_head > a ');
                        if (selector != null)
                        {
                            return selector.innerHTML;
                        }
                        return '';
                        }";

                        var nextPage = await page.EvaluateFunctionAsync<string>(jsNextPage);
                        if (!string.IsNullOrEmpty(nextPage) && nextPage.Trim() == "下一頁")
                        {
                            isNextPage = true;
                        }

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
                                             horse        :tds[6].innerHTML,
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
                        
                        return new Tuple<List<RidingRecord>,bool>(result, isNextPage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return new Tuple<List<RidingRecord>, bool>(result, isNextPage);
        }

        /*
            7. https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerPastRec.aspx?TrainerId=SJJ&Season=Current&PageNum=1
            呢個係練馬師既，原理同騎師既一樣
         */
        public static async Task<Tuple<List<RunnerRecord>,bool>> GetRunnerRecords(string trainerID, string seasonType, int pageID = 1)
        {
            //https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerPastRec.aspx?TrainerId=SJJ&Season=Current&PageNum=1
            string url = string.Format(@"https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerPastRec.aspx?TrainerId={0}&Season={1}&PageNum={2}"
                , trainerID
                , seasonType
                , pageID
                );
            var result = new List<RunnerRecord>();
            var isNextPage = false;

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

                        //Check nextpage -> #innerContent > div.trainerPastRec.commContent > div.runnerRec > p:nth-child(1) > span.f_fr.page_pre_head > a
                        var jsCheckNextPage = @"() => {
                        const selector = document.querySelector('#innerContent > div.trainerPastRec.commContent > div.runnerRec > p:nth-child(1) > span.f_fr.page_pre_head > a ');
                        if (selector != null)
                        {
                            return selector.innerHTML;
                        }                        
                        return '';
                        }";

                        var nextPage = await page.EvaluateFunctionAsync<string>(jsCheckNextPage);

                        if (!string.IsNullOrEmpty(nextPage) && nextPage.Trim() == "下一頁")
                        {
                            isNextPage = true;
                        }

                        var jsRunnerRecords = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#innerContent > div.trainerPastRec.commContent > div.runnerRec > table > tbody > tr '));
                        return selectors.map( (tr) => { 
                                    const tds = Array.from(tr.querySelectorAll('td'));
                                    if (tds.length == 16) {
                                        return { 
                                             index        :tds[0].querySelector('a').innerHTML,
                                             raceURL      :tds[0].querySelector('a').href,
                                             horse        :tds[1].innerHTML,
                                             placing      :tds[2].innerHTML,
                                             trackCourse  :tds[3].innerHTML,
                                             distance     :tds[4].innerHTML,
                                             going        :tds[5].innerHTML,
                                             draw         :tds[6].innerHTML,
                                             rtg          :tds[7].innerHTML,
                                             winOdds      :tds[8].innerHTML,                                             
                                             jockey       :tds[9].innerHTML,
                                             gear         :tds[10].innerHTML,
                                             bodyWeight   :tds[11].innerHTML,
                                             actualWeight :tds[12].innerHTML,
                                             horseFirst   :tds[13].innerHTML,
                                             horseSecond  :tds[14].innerHTML,
                                             horseThird   :tds[15].innerHTML
                                            }
                                        }
                                    });
                                }";

                        var runnerRecords = await page.EvaluateFunctionAsync<RunnerRecord[]>(jsRunnerRecords);
                        if (runnerRecords != null && runnerRecords.Length != 0)
                        {
                            result = runnerRecords.Where(r => r != null).ToList();
                        }
                        else
                        {
                            throw new Exception("No runner records found!");
                        }

                        return new Tuple<List<RunnerRecord>, bool>(result, isNextPage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

            return new Tuple<List<RunnerRecord>, bool>(result, isNextPage);
        }

        /// <summary>
        /// Get the full list of Runner Records with multiple page ID
        /// </summary>
        /// <param name="trainerID"></param>
        /// <param name="seasonType"></param>
        /// <returns></returns>
        public static async Task<List<RunnerRecord>> GetFullRunnerRecords(string trainerID, string seasonType)
        {
            var result = new List<RunnerRecord>();
            var pageID = 1;
            var isNextPage = false;

            do {
                var tResult = await GetRunnerRecords(trainerID, seasonType, pageID);
                isNextPage = tResult.Item2;
                result.AddRange(tResult.Item1);
                pageID++;
            } while (isNextPage);

            return result;
        }

        /// <summary>
        /// Get the full list of Runner Records with multiple page ID
        /// </summary>
        /// <param name="jockeyID"></param>
        /// <param name="seasonType"></param>
        /// <returns></returns>
        public static async Task<List<RidingRecord>> GetFullRidingRecords(string jockeyID, string seasonType)
        {
            var result = new List<RidingRecord>();
            var pageID = 1;
            var isNextPage = false;

            do
            {
                var tResult = await GetRidingRecords(jockeyID, seasonType, pageID);
                isNextPage = tResult.Item2;
                result.AddRange(tResult.Item1);
                pageID++;
            } while (isNextPage);

            return result;
        }
    }
}