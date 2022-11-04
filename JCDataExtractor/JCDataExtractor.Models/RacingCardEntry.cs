namespace JCDataExtractor.Models
{
    public class RacingCardEntry
    {
        /*
        Horse No.
        Last 6 Runs
        Colour
        Horse
        Brand No.
        Wt.
        Jockey
        Over Wt.
        Draw
        Trainer
        Int'l Rtg.
        Rtg.
        Rtg.+/-
        Horse Wt. (Declaration)
        Wt.+/- (vs Declaration)
        Best Time
        Age
        WFA
        Sex
        Season Stakes
        Priority

        Gear
        Owner
        Sire
        Dam
        Import Cat.
        */

        /// <summary>
        /// 馬匹編號
        /// </summary>
        public int houseNo { get; set; }

        /// <summary>
        /// 6次近績
        /// </summary>
        public string last6Runs { get; set; }

        /// <summary>
        /// 綵衣
        /// </summary>
        public string colour { get; set; }

        /// <summary>
        /// 馬名
        /// </summary>
        public string horseName { get; set; }

        /// <summary>
        /// 烙號
        /// </summary>
        public string BrandNo { get; set; }

        /// <summary>
        /// 負磅
        /// </summary>
        public string takeWeight { get; set; }

        /// <summary>
        /// 騎師
        /// </summary>
        public string jockey { get; set; }

        /// <summary>
        /// 可能超磅
        /// </summary>
        public string probableOverWeight { get; set; }

        /// <summary>
        /// 檔位
        /// </summary>
        public string draw { get; set; }

        /// <summary>
        /// 練馬師
        /// </summary>
        public string trainer { get; set; }

        /// <summary>
        /// 國際評分
        /// </summary>
        public string intlRating { get; set; }

        /// <summary>
        /// 評分
        /// </summary>
        public string rating { get; set; }

        /// <summary>
        /// 評分+/-
        /// </summary>
        public string ratingChange { get; set; }

        /// <summary>
        /// 排位體重
        /// </summary>
        public string horseWeightDeclare { get; set; }

        /// <summary>
        /// 排位體重+/-
        /// </summary>
        public string horseWeightDiff { get; set; }

        /// <summary>
        /// 最佳時間
        /// </summary>
        public string bestTime { get; set; }

        /// <summary>
        /// 馬齡
        /// </summary>
        public string age { get; set; }

        /// <summary>
        /// 分齡讓磅
        /// </summary>
        public string handicapWeight { get; set; }

        /// <summary>
        /// 性別
        /// </summary>
        public string sex { get; set; }

        /// <summary>
        /// 今季獎金
        /// </summary>
        public string seasonStakes { get; set; }

        /// <summary>
        ///  優先參賽次序
        /// </summary>
        public string priority { get; set; }

        /// <summary>
        /// 配備
        /// </summary>
        public string gear { get; set; }

        /// <summary>
        /// 馬主
        /// </summary>
        public string owner { get; set; }

        /// <summary>
        /// 父系
        /// </summary>
        public string sire { get; set; }

        /// <summary>
        /// 母系
        /// </summary>
        public string dam { get; set; }

        /// <summary>
        /// 進口類別
        /// </summary>
        public string importCat { get; set; }
    }
}