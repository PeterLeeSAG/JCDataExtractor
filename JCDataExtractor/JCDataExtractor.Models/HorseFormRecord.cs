using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCDataExtractor.Models
{
    public class HorseFormRecord
    {        
        /// <summary>
        /// 場次
        /// </summary>
        public int index { get; set; }
        /// <summary>
        /// Race LINK
        /// </summary>
        public string raceURL { get; set; }

        /// <summary>
        /// 名次
        /// </summary>
        public string placing { get; set; }
        
        /// <summary>
        /// 日期
        /// </summary>
        public string raceDate { get; set; }
        
        /// <summary>
        /// 馬場/跑道/賽道
        /// </summary>
        public string trackCourse { get; set; }
        
        /// <summary>
        /// 途程
        /// </summary>
        public int distance { get; set; }

        /// <summary>
        /// 場地狀況
        /// </summary>
        public string going { get; set; }

        /// <summary>
        /// 賽事班次
        /// </summary>
        public string raceClass { get; set; }

        /// <summary>
        /// 檔位
        /// </summary>
        public int draw { get; set; }      
        
        /// <summary>
        /// 評分
        /// </summary>
        public int rtg { get; set; }        
        
        /// <summary>
        /// 練馬師
        /// </summary>
        public string trainer { get; set; }
        
        /// <summary>
        /// 騎師					 
        /// </summary>
        public string jockey { get; set; }    
        
        /// <summary>
        /// 頭馬距離
        /// </summary>
        public string LBW { get; set; }
        
        /// <summary>
        /// 獨贏賠率
        /// </summary>
        public double winOdds { get; set; } 
        
        /// <summary>
        /// 實際負磅
        /// </summary>
        public int actualWeight { get; set; }
        
        /// <summary>
        /// 沿途走位
        /// </summary>
        public string runningPosition { get; set; }
        
        /// <summary>
        /// 完成時間
        /// </summary>
        public string finishTime { get; set; }

        /// <summary>
        /// 馬匹體重
        /// </summary>
        public int bodyWeight { get; set; }

        /// <summary>
        /// 配備
        /// </summary>
        public string gear { get; set; }


        public RaceInfo GetRaceInfo()
        { 
            return new RaceInfo(this.raceURL);
        }
    }
}
