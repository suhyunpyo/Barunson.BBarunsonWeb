using Barunson.DbContext.DbModels.BarShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barunson.BBarunsonWeb.Models
{
    public class HardcodingDigitalViewModel
    {
        public List<DigitalType> DigitalTypes = new List<DigitalType> { 
            new DigitalType("DigitalCardCode", "디지털부속병합"),
            new DigitalType("DigitalCardExclude", "디지털병합제외"),
        };
        public DigitalType DigitalType { get; set; }
        public List<HardcodingDigitalModel> HardcodingDigitalList { get; set; } = new List<HardcodingDigitalModel>();
    }

    public class HardcodingDigitalModel
    {
        public string? CardCode { get; set; }
        public int? CardSeq { get; set; }
        public string? CardType { get; set; }
        public string? IsDigit { get; set; }
        public string? CardImage { get; set; }
    }

    public class DigitalType
    {
        public DigitalType (string t, string n)
        {
            this.Type = t;
            this.Name = n;
        }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
