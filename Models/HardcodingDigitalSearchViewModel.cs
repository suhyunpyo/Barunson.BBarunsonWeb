using Barunson.DbContext.DbModels.BarShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barunson.BBarunsonWeb.Models
{
    public class HardcodingDigitalSearchViewModel
    {
        public int result { get; set; }
        public HardcodingSearchCardModel? SearchCardModel { get; set; }
    }


    public class HardcodingSearchCardModel
    {
        public int CardSeq { get; set; }
        public string? CardCode { get; set; }
        public string? CardDiv { get; set; }
        public string? CardType { get; set; }
        public string? CardImage { get; set; }
        public string? HardCode { get; set; }
        public int? CardKind { get; set; }

    }
}
