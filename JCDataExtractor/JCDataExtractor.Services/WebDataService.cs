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

            Target:
            table: racecardlist
            header tr: trBg01 tdAlignC boldFont13
                        <tr class="trBg01 tdAlignC boldFont13">
                            <td width="40px" style="width: 40px;">馬匹編號</td>
                            <td width="80px" style="width: 80px;">6次近績</td>
                            <td width="40px" style="width: 40px;">綵衣</td>
                            <td width="100px" style="width: 100px;">馬名</td>
                            <td width="40px" style="width: 40px; display: none;">烙號</td>
                            <td width="40px" style="width: 40px;">負磅</td>
                            <td width="135px" style="width: 135px;">騎師</td>
                            <td width="40px" style="width: 40px; display: none;">可能超磅</td>
                            <td width="40px" style="width: 40px;">檔位</td>
                            <td width="100px" style="width: 100px;">練馬師</td>
                            <td width="40px" style="width: 40px; display: none;">國際評分</td>
                            <td width="40px" style="width: 40px;">評分</td>
                            <td width="50px" style="width: 50px;">評分+/-</td>
                            <td width="40px" style="width: 40px;">排位體重</td>
                            <td width="50px" style="width: 50px; display: none;">排位體重+/-</td>
                            <td width="40px" style="width: 40px; display: none;">最佳時間</td>
                            <td width="40px" style="width: 40px; display: none;">馬齡</td>
                            <td width="40px" style="width: 40px; display: none;">分齡讓磅</td>
                            <td width="40px" style="width: 40px; display: none;">性別</td>
                            <td width="40px" style="width: 40px; display: none;">今季獎金</td>
                            <td width="50px" style="width: 50px;">優先參賽次序</td>

                            <td width="40px" style="width: 40px;">配備</td>
                            <td width="220px" style="width: 220px; display: none;">馬主</td>
                            <td width="40px" style="width: 40px; display: none;">父系</td>
                            <td width="40px" style="width: 40px; display: none;">母系</td>
                            <td width="40px" style="width: 40px; display: none;">進口類別</td>
                        </tr>

                        <tr class="trBg01 tdAlignC boldFont13">
                            <td width="40px" style="width: 40px;">Horse No.</td>
                            <td width="80px" style="width: 80px;">Last 6 Runs</td>
                            <td width="40px" style="width: 40px;">Colour</td>
                            <td width="100px" style="width: 100px;">Horse</td>
                            <td width="40px" style="width: 40px; display: none;">Brand No.</td>
                            <td width="40px" style="width: 40px;">Wt.</td>
                            <td width="135px" style="width: 135px;">Jockey</td>
                            <td width="40px" style="width: 40px; display: none;">Over Wt.</td>
                            <td width="40px" style="width: 40px;">Draw</td>
                            <td width="100px" style="width: 100px;">Trainer</td>
                            <td width="40px" style="width: 40px; display: none;">Int'l Rtg.</td>
                            <td width="40px" style="width: 40px;">Rtg.</td>
                            <td width="50px" style="width: 50px;">Rtg.+/-</td>
                            <td width="40px" style="width: 40px;">Horse Wt. (Declaration)</td>
                            <td width="50px" style="width: 50px; display: none;">Wt.+/- (vs Declaration)</td>
                            <td width="40px" style="width: 40px; display: none;">Best Time</td>
                            <td width="40px" style="width: 40px; display: none;">Age</td>
                            <td width="40px" style="width: 40px; display: none;">WFA</td>
                            <td width="40px" style="width: 40px; display: none;">Sex</td>
                            <td width="40px" style="width: 40px; display: none;">Season Stakes</td>
                            <td width="50px" style="width: 50px;">Priority</td>

                            <td width="40px" style="width: 40px;">Gear</td><td width="220px" style="width: 220px; display: none;">Owner</td>
                            <td width="40px" style="width: 40px; display: none;">Sire</td>
                            <td width="40px" style="width: 40px; display: none;">Dam</td>
                            <td width="40px" style="width: 40px; display: none;">Import Cat.</td>
                        </tr>

            row html: f_tac f_fs13
                        <tr class="f_tac f_fs13">
                            <td style="width: 40px;">1</td>
                            <td style="width: 80px;">14/11/12/11/8/14</td>
                            <td style="width: 40px;"><img src="/racing/content/Images/RaceColor/G232.gif" alt=""></td>
                            <td style="width: 100px;"><a href="/racing/information/chinese/Horse/Horse.aspx?HorseId=HK_2021_G232" onclick="window.open(this.href, '_blank', 'status=no,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes,scrollbars=yes,left=20,top=20,width=812,height=600'); return false;">美麗新星</a></td>
                            <td style="width: 40px; display: none;">G232</td>
                            <td style="width: 40px;">135</td>
                            <td style="width: 135px;">
                                    <a href="/racing/information/chinese/Jockey/JockeyProfile.aspx?JockeyId=BV" onclick="window.open(this.href, '_blank', 'status=no,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes,scrollbars=yes,left=20,top=20,width=812,height=600'); return false;">波健士</a>
                            </td>
                            <td style="width: 40px; display: none;"></td>
                            <td style="width: 40px;">5</td>
                            <td style="width: 100px;"><a href="/racing/information/chinese/Trainers/TrainerProfile.aspx?TrainerId=SJJ" onclick="window.open(this.href, '_blank', 'status=no,toolbar=no,menubar=no,location=no,scrollbars=yes,resizable=yes,scrollbars=yes,left=20,top=20,width=812,height=600'); return false;">蔡約翰</a></td>
                            <td style="width: 40px; display: none;">-</td>
                            <td style="width: 40px;">40</td>
                            <td style="width: 50px;">-</td>
                            <td style="width: 40px;">1052</td>
                            <td style="width: 50px; display: none;">+1</td>
                            <td style="width: 40px; display: none;"></td>
                            <td style="width: 40px; display: none;">4</td>
                            <td style="width: 40px; display: none;">-</td>
                            <td style="width: 40px; display: none;">閹</td>
                            <td style="width: 40px; display: none;">0</td>
                            <td style="width: 50px;">* 1</td>

                            <td style="width: 40px;">H1/TT</td><td style="width: 220px; display: none;">郭少明</td>
                            <td style="width: 40px; display: none;">Territories</td>
                            <td style="width: 40px; display: none;">Folly Bridge</td>
                            <td style="width: 40px; display: none;">PP</td>
                        </tr>
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

                        //#racecardlist > tbody > tr > td > table > tbody > tr:nth-child(1)
                        //#main > div > div.card > div.card_body > table > tbody > tr:nth-child(1) > td:nth-child(5) > span
                        ///document.querySelector("#main > div > div.card > div.card_body > table > tbody > tr:nth-child(1) > td:nth-child(10) > span")

                        Thread.Sleep(1000);

                        //map html table data to list of object via js querySelector here
                        var jsShortTable = @"() => {
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

                        var raceCardResults = await page.EvaluateFunctionAsync<RaceCardEntry[]>(jsShortTable);

                        if (raceCardResults != null && raceCardResults.Length != 0)
                        {
                            result = raceCardResults.ToList();
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
            table: #innerContent > div.Draw.commContent > div:nth-child(5)
            Info: #innerContent > div.Draw.commContent > table > tbody > tr > td.f_fs13.font_wb <- 06/11/2022 沙田
            table header: #race1 
                          #race2 > td <- 第 2 場 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1200米 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 草地 &nbsp;&nbsp;&nbsp;&nbsp;&nbsp; "C+3" 賽道

                #innerContent > div.Draw.commContent > div.racingNum.top_races.js_racecard_rt_num > table > tbody > tr > td:nth-child(11)  <- count how many race here
                
                #innerContent > div.Draw.commContent > div:nth-child(5) <- table 1
                #innerContent > div.Draw.commContent > div:nth-child(7) <- table 2
                #innerContent > div.Draw.commContent > div:nth-child(9) <- table 3 ...
                
            row:#innerContent > div.Draw.commContent > div:nth-child(5) > table > tbody > tr:nth-child(1)
                #innerContent > div.Draw.commContent > div:nth-child(5) > table > tbody > tr:nth-child(1) > td.f_pr.scale > span.win > img .width
                #innerContent > div.Draw.commContent > div:nth-child(5) > table > tbody > tr:nth-child(1) > td.f_pr.scale > span.placed > img .width (100% = 360px)
                
                #innerContent > div.Draw.commContent > div:nth-child(23) > table > tfoot > tr > td:nth-child(2) > span:nth-child(1) <- hot, total 3 items

            row html:
                    檔位
                    出賽次數
                    冠
                    亞
                    季
                    殿
                    勝出率%
                    入Q率%
                    上名率%
                    前4名率%

                    Draw
                    Runners
                    Win
                    2nd
                    3rd
                    4th
                    W%
                    Q%
                    P%
                    F%
                            
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
                        //#innerContent > div.Draw.commContent > div.racingNum.top_races.js_racecard_rt_num > table > tbody > tr > td <- count td


                        //map html table data to list of object via js querySelector here
                        var jsShortTable = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#racecardlist > tbody > tr > td > table > tbody > tr '));
                        return selectors.map( (tr) => { 
                            const tds = Array.from(tr.querySelectorAll('td'));
                            return { houseNo: tds[0].innerHTML, last6Runs: tds[1].innerHTML, Colour: tds[2].querySelector('img').src }});
                        }";

                        var shortResults = await page.EvaluateFunctionAsync<RaceCardEntry[]>(jsShortTable);

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

                <td width="160px">騎師</td>
                <td width="70px">冠</td>
                <td width="70px">亞</td>
                <td width="70px">季</td>
                <td width="70px">殿</td>
                <td width="70px">第五</td>
                <td width="120px">總出賽次數</td>
                <td>所贏獎金</td>

                row: #innerContent > div.commContent > div.Ranking > div:nth-child(3) > table > tbody:nth-child(2) > tr:nth-child(1) <-first
                     #innerContent > div.commContent > div.Ranking > div:nth-child(3) > table > tbody:nth-child(2) > tr:nth-child(25) <-last

                row html:
                    <tr>
                        <td class="f_fs14 f_tal">
                            <a href="/racing/information/Chinese/Jockey/JockeyPastRec.aspx?JockeyId=PZ&amp;Season=Current">
                                潘頓
                            </a>
                        </td>
                        <td>32</td>
                        <td>18</td>
                        <td>9</td>
                        <td>17</td>
                        <td>10</td>
                        <td>133</td>
                        <td class="f_tar">$42,497,125</td>
                    </tr>
         */

        /*
            https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerRanking.aspx?Season=Current&View=Numbers&Racecourse=ALL

            https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerRanking.aspx?Season=Previous&View=Numbers&Racecourse=ALL

            呢一條係練馬師統計表，同騎師統計表一樣，當中有一個變數，Current代表今季資料，Previous代表上季
         */

        /*
            https://racing.hkjc.com/racing/information/Chinese/Horse/Horse.aspx?HorseId=HK_2021_G232&Option=1
            呢條係馬匹既，檔中既變數就係馬匹編號，HK_2021_G232
            HK_2021係隻馬黎港日期，G232係馬匹烙號
            係排表表馬匹名既超連結入面可以提取到呢個馬匹編號

            https://racing.hkjc.com/racing/information/chinese/Horse/SelectHorsebyChar.aspx?ordertype=2
            house list, ordertype = 2,3,4
         */

        /*
            https://racing.hkjc.com/racing/information/Chinese/Jockey/JockeyPastRec.aspx?JockeyId=BV&Season=Current&PageNum=1
            呢個係騎師既，當中有變數JockeyId、Curent、PageNum
            JockeyId同馬匹一樣，可以係排位表個超連結上面提取到
            最麻煩就係PageNum，1代表第一頁，如果得2頁既話，你輸入3佢都會比繼續比第二頁你，所以我個程式要檢查有無「下一頁」呢個字去判斷有無下一頁。    
         */

        /*
            https://racing.hkjc.com/racing/information/Chinese/Trainers/TrainerPastRec.aspx?TrainerId=SJJ&Season=Current&PageNum=1
            呢個係練馬師既，原理同騎師既一樣
         */
    }
}