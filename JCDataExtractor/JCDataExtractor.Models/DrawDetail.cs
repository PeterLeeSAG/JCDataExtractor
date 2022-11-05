namespace JCDataExtractor.Models
{
    public class DrawDetail
    {
        /// <summary>
        /// 檔位
        /// </summary>
        public int draw { get; set; }

        /// <summary>
        /// 出賽次數
        /// </summary>
        public int runners { get; set; }

        /// <summary>
        /// 冠
        /// </summary>
        public int win { get; set; }

        /// <summary>
        /// 亞
        /// </summary>
        public int second { get; set; }

        /// <summary>
        /// 季
        /// </summary>
        public int third { get; set; }

        /// <summary>
        /// 殿
        /// </summary>
        public int forth { get; set; }

        /// <summary>
        /// 勝出率%
        /// </summary>
        public int percentW { get; set; }

        /// <summary>
        /// 入Q率%
        /// </summary>
        public int percentQ { get; set; }

        /// <summary>
        /// 上名率%
        /// </summary>
        public int percentP { get; set; }

        /// <summary>
        /// 前4名率%
        /// </summary>
        public int percentF { get; set; }
    }
}