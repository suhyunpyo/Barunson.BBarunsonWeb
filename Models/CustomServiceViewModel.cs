using Microsoft.AspNetCore.Mvc.Rendering;

namespace Barunson.BBarunsonWeb.Models
{
    public class CustomServiceViewModel : PageViewModel
    {
        public int OrderSeq { get; set; }

        /// <summary>
        /// 차수 select Box
        /// </summary>
        public string PChasu { get; set; } = "1";
        public IEnumerable<SelectListItem> PChasus
        {
            get
            {
                return new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "1차", Value = "1" },
                        new SelectListItem { Text = "2차", Value = "2" },
                        new SelectListItem { Text = "3차", Value = "3" },
                        new SelectListItem { Text = "4차", Value = "4" },
                        new SelectListItem { Text = "5차", Value = "5" },
                        new SelectListItem { Text = "6차", Value = "6" },
                        new SelectListItem { Text = "7차", Value = "7" },
                    }, "Value", "Text");
            }
        }


        /// <summary>
        /// </summary>
        public string Jong { get; set; } = "C";
        public IEnumerable<SelectListItem> Jongs
        {
            get
            {
                return new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "카드", Value = "C" },
                        new SelectListItem { Text = "봉투", Value = "E" },
                        new SelectListItem { Text = "내지", Value = "I" },
                        new SelectListItem { Text = "미니", Value = "M" },
                    }, "Value", "Text");
            }
        }

        /// <summary>
        /// 시작일
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        /// <summary>
        /// 종료일
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Now.Date;

        public Uri ChoanImgUrl { get; set; }


        public List<CustomServiceSearchDataModel> DataModel { set; get; }
    }



    public class CustomServiceSearchDataModel
    {
        //A.oseq,B.id,A.order_seq,B.print_type,title,imgFolder, imgName
        public int OSeq { get; set; }
        
        public int Id { get; set; }

        public int OrderSeq { get; set; }

        public string PrintType { get; set; }

        public string Title { get; set; }


        public string ImgFolder { get; set; }

        public string imgName { get; set; }

    }

}
