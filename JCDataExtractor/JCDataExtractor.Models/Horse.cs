using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCDataExtractor.Models
{
    public class Horse
    {
        /*
            出生地 / 馬齡	:	澳洲 / 6
            毛色 / 性別	:	棗 / 閹
            進口類別	:	自購新馬
            今季獎金*	:	$2,850,000
            總獎金*	:	$42,906,190
            冠-亞-季-總出賽次數*	:	11-1-0-17
            最近十個賽馬日
            出賽場數	:	1
            現在位置 :	香港
            (到達日期)	(09/09/2019)
            練馬師	:	高伯新
            馬主	:	鄭永安先生及夫人與鄭文昌
            現時評分	:	128
            季初評分	:	123
            父系	:	All Too Hard
            母系	:	Mihiri
            外祖父	:	More Than Ready
            同父系馬	:	美麗滿滿
 
            Country of Origin / Age	:	AUS / 6
            Colour / Sex	:	Bay / Gelding
            Import Type	:	PPG
            Season Stakes*	:	$2,850,000
            Total Stakes*	:	$42,906,190
            No. of 1-2-3-Starts*	:	11-1-0-17
            No. of starts in past 10
            race meetings	:	1
            Current Stable Location :	Hong Kong
            (Arrival Date)	(09/09/2019)
            Trainer	:	R Gibson
            Owner	:	Mr & Mrs Michael Cheng Wing On and Jeffrey Cheng Man Cheong
            Current Rating	:	128
            Start of
            Season Rating	:	123
            Sire	:	All Too Hard
            Dam	:	Mihiri
            Dam's Sire	:	More Than Ready
            Same Sire	:	ALL BEAUTY
         */
        public string id { get; set; }
        public string name { get; set; }
        public List<JockeyRaceStats> JockeyRaceStatsList { get; set; }
        public List<HorseFormRecord> horseFormRecords { get; set; }
    }

    public class HorseList
    {
        public string horseName { get; set; }
        public string horseID { get; set; }
    }
}
