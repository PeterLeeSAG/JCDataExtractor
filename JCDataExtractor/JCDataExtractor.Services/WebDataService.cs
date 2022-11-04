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

            row tr: f_tac f_fs13
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

        public static async Task<List<RacingCardEntry>> GetRacingRardEntries(DateTime raceDate, string raceCourse, int raceNo)
        {
            //https://racing.hkjc.com/racing/information/chinese/Racing/RaceCard.aspx?RaceDate=2022/11/06&Racecourse=ST&RaceNo=1
            string url = string.Format(@"https://racing.hkjc.com/racing/information/chinese/Racing/RaceCard.aspx?RaceDate={0}&Racecourse={1}&RaceNo={2}", raceDate.ToString("yyyy/MM/dd"), raceCourse, raceNo.ToString());
            var result = new List<RacingCardEntry>();

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
                    Headless = true, //偵測時可設定false觀察網頁顯示結果(註：非Headless時不能匯出PDF)
                    ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).ExecutablePath
                }))
                {
                    using (var page = await browser.NewPageAsync())
                    {
                        await page.GoToAsync(url);
                        //透過SetViewport控制視窗大小決定抓圖尺寸
                        await page.SetViewportAsync(new ViewPortOptions
                        {
                            Width = 1024,
                            Height = 768
                        });

                        //summary table:
                        //await page.WaitForSelectorAsync(".card_body");

                        //#racecardlist > tbody > tr > td > table > tbody > tr:nth-child(1)
                        //#main > div > div.card > div.card_body > table > tbody > tr:nth-child(1) > td:nth-child(5) > span
                        ///document.querySelector("#main > div > div.card > div.card_body > table > tbody > tr:nth-child(1) > td:nth-child(10) > span")

                        Thread.Sleep(1000);

                        var jsShortTable = @"() => {
                        const selectors = Array.from(document.querySelectorAll('#racecardlist > tbody > tr > td > table > tbody > tr '));
                        return selectors.map( (tr) => { 
                            const tds = Array.from(tr.querySelectorAll('td'));
                            return { houseNo: tds[0].innerHTML, last6Runs: tds[1].innerHTML, Colour: tds[2].querySelector('img').src }});
                        }";

                        var shortResults = await page.EvaluateFunctionAsync<RacingCardEntry[]>(jsShortTable);

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
    }
}