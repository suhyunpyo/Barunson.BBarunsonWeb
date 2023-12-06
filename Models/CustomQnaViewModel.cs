using Microsoft.AspNetCore.Mvc.Rendering;

namespace Barunson.BBarunsonWeb.Models
{
    public class CustomQnaViewModel : PageViewModel
    {
        public string KeyWord { get; set; }

        /// <summary>
        /// 시작일
        /// </summary>
        public DateTime StartDate { get; set; } = DateTime.Now.AddDays(-7).Date;
        /// <summary>
        /// 종료일
        /// </summary>
        public DateTime EndDate { get; set; } = DateTime.Now.Date;

        /// <summary>
        /// </summary>
        public string Status { get; set; } = "2";
        public IEnumerable<SelectListItem> Statuss
        {
            get
            {
                return new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "처리상태", Value = "0" },
                        new SelectListItem { Text = "접수중", Value = "1" },
                        new SelectListItem { Text = "접수/처리중", Value = "2" },
                        new SelectListItem { Text = "처리완료", Value = "3" },
                        new SelectListItem { Text = "내부처리", Value = "4" },
                    }, "Value", "Text");
            }
        }


        /// <summary>
        /// </summary>
        public string SearchCase { get; set; } = "0";
        public IEnumerable<SelectListItem> SearchCases
        {
            get
            {
                return new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "제목/질문내용", Value = "0" },
                        new SelectListItem { Text = "회원아이디", Value = "1" },
                        new SelectListItem { Text = "작성자이름", Value = "2" },
                        new SelectListItem { Text = "작성자연락처", Value = "3" },
                        new SelectListItem { Text = "작성자이메일", Value = "4" },
                        new SelectListItem { Text = "주문번호", Value = "5" },
                    }, "Value", "Text");
            }
        }



        public string AdminList { get; set; } = "";
        public List<SelectListItem> ProductBrands { get; set; }

        public List<SelectListItem> AdminLists { get; set; }



        /// <summary>
        /// 1대1문의 여부 
        /// </summary>
        public bool OneInquiry { get; set; } = false;

        /// <summary>
        /// 상위
        /// </summary>
        public bool TopCnt { get; set; } = false;


        /// <summary>
        /// 바인딩 아이템 목록
        /// </summary>
        public List<CustomQnaDataModel> DataModel { set; get; }

        public override Dictionary<string, string> RouteData
        {
            get
            {
                var routeail = new Dictionary<string, string>
                {
                    { nameof(KeyWord), KeyWord }
                };


                return routeail;
            }
        }
    }



    public class CustomQnaDataModel
    {

        public List<CustomQnaViewModel>? DataModel { set; get; }

        public string Code { get; set; }
        public string? IsS2 { get; set; }
        public int Qa_Iid { get; set; }
        public string Sales_Gubun { get; set; }
        public int? Company_Seq { get; set; }
        public string? Company_Name { get; set; }
        public long? Order_Seq { get; set; }

        public string? Member_Id { get; set; }

        public string? Member_Name { get; set; }

        public string? E_Mail { get; set; }

        public string? Tel_No { get; set; }

        public string? Q_Kind { get; set; }

        public string? Q_Title { get; set; }

        public string? Q_Content { get; set; }

        public string? A_Content { get; set; }

        public string? A_Stat { get; set; }

        public DateTime? Reg_Dt { get; set; }

        public DateTime? A_Dt { get; set; }

        public string? A_Id { get; set; }

        public string? A_Research1 { get; set; }
        public string? A_Research2 { get; set; }

        public string? card_Code { get; set; }

        public string? user_upfile1 { get; set; }

        public string? user_upfile2 { get; set; }

        public string? admin_upfile1 { get; set; }


        public string AdminMentCategory_M { get; set; }
        public IEnumerable<SelectListItem> AdminMentCategorys_M { get; set; }


        public string AdminMentCategory_S { get; set; }
        public IEnumerable<SelectListItem> AdminMentCategorys_S { get; set; }

    }

}
