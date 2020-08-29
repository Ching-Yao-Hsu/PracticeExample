using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WpfApp1.Data.DTO
{
    public class VegFruitMarketSupplyPrice
    {
        [Display(Name = "日期")]
        public string date { get; set; }
        [Display(Name = "市場")]
        public string market { get; set; }
        [Display(Name = "小代號")]
        public string smallCode { get; set; }
        [Display(Name = "種類")]
        public string className { get; set; }
        [Display(Name = "品名代號")]
        public string productCode { get; set; }
        [Display(Name = "品名名稱")]
        public string productName { get; set; }
        [Display(Name = "等級")]
        public string Level { get; set; }
        [Display(Name = "數量")]
        public string amount { get; set; }
        [Display(Name = "淨重(公斤)")]
        public string heavy { get; set; }
        [Display(Name = "單價(元/公斤)")]
        public string dollar { get; set; }
        [Display(Name = "總價(元)")]
        public string total { get; set; }
        [Display(Name = "標章別")]
        public string typeName { get; set; }
    }
}
