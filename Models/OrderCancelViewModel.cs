using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace Barunson.BBarunsonWeb.Models
{
    public class OrderCancelViewModel
    {

        public string OrderName { get; set; }

        public DateTime? SettleDate { get; set; }
        public DateTime? SrcPrintCopyDate { get; set; }
        public string SrcPrintCopyAdminID { get; set; }
        public string ViewType { get; set; }
        public int OrderSeq { get; set; }
        public int Order_g_Seq { get; set; }

        public string AdminId { get; set; }

        public string OrderType { get; set; }

        public bool IsErr { get; set; } = true;

        public string ErrStr { get; set; }

        public string SalesGubun { get; set; }
        public string MemberId { get; set; }
        public int StatusSeq { get; set; }

        public int SettlePrice { get; set; }

        public int RefundPrice { get; set; }
        public string RefundDate { get; set; }

        public string RefundUserName { get; set; }

        public string RefundBankAccount { get; set; }

        public string RefundBank { get; set; }

        public string RefundMsg { get; set; }

        public byte? SettleStatus { get; set; }

        public string SettleMethod { get; set; }

        public string PgShopId { get; set; }

        public string PgTid { get; set; }

        public string DacomTid { get; set; }

        public string ButtonMent { get; set; }

        //public string IsPrintCopy { get; set; }
        public int? CompanySeq { get; set; }

        //public List<OrderCanCelReasonKindModel> CanCelReasonKindModel { set; get; }

        public string CanCelType1 { get; set; }
        public IEnumerable<SelectListItem> CanCelType1s { get; set; }

        public string CanCelType2 { get; set; }
        public IEnumerable<SelectListItem> CanCelType2s { get; set; }

        public string RefundType1 { get; set; }
        public IEnumerable<SelectListItem> RefundType1s { get; set; }


        public string RefundType2 { get; set; }
        public IEnumerable<SelectListItem> RefundType2s { get; set; }


        public List<OrderCancelViewModel> DataModel { set; get; }

        public string CancelReason { get; set; } = "고객 변심";

        public string OrderCancelTypeComment { get; set; }

        public string isCloseCopy { get; set; }

        public bool PgCancel { get; set; }

        public bool RePay { get; set; }

        /// <summary>
        /// </summary>
        public string JumunType { get; set; } = "C";
        public IEnumerable<SelectListItem> JumunTypes
        {
            get
            {
                return new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "초대장", Value = "0" },
                        new SelectListItem { Text = "포스터청첩장", Value = "1" },
                        new SelectListItem { Text = "식권/기타", Value = "2" },
                        new SelectListItem { Text = "샘플", Value = "3" }//,
                        //new SelectListItem { Text = "포토북", Value = "4" },
                        //new SelectListItem { Text = "뷰티박스", Value = "5" }
                    }, "Value", "Text");
            }
        }


        public string SettleName { get; set; }

        public int? CancelType { get; set; }
        public string? CancelTypeComment { get; set; }
        public int? CancelUserType { get; set; }
        public int? CouponDetailSeq { get; set; }
        public string? AdditionCouponSeq { get; set; }

        public int? CouponIssueSeq { get; set; }

        public string RefundCode { get; set; }

        // public string? ActiveYn { get; set; }
    }

    public class OrderCanCelReasonKindModel
    {
        public int Seq { get; set; }
        public string Code { get; set; }

        public string CodeValue { get; set; }

        public string RealCode { get; set; }

    }

    //public class OrderSearchDataModel
    //{

    //}



}
