using Microsoft.AspNetCore.Mvc.Rendering;

namespace Barunson.BBarunsonWeb.Models
{

    public class WorkStatisticsSearchModel
    {

        public string BtnClass { get; set; }

        public int MenuGubun { get; set; } = 0;

        public DateTime ConfirmDate { get; set; } = DateTime.Now.Date;

        public DateTime StartDate { get; set; } = DateTime.Now.Date;

        public DateTime EndDate { get; set; } = DateTime.Now.Date;



        public string Order_Date_Yorn { get; set; } = "N";

        public IEnumerable<SelectListItem> Order_Date_Yorns
        {
            get
            {
                return new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "초안등록일", Value = "N" },
                        new SelectListItem { Text = "주문일", Value = "Y" }

                    }, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> Designer
        {
            get
            {
                return new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "전체", Value = "0" },
                        new SelectListItem { Text = "바른디자인", Value = "1" },
                        new SelectListItem { Text = "초안팀", Value = "2" }
                        
                    }, "Value", "Text");
            }
        }

        public int EventDateTotalCount { get; set; }
        public int ChoanJobDefaultTot { get; set; }
        public int ChoanJobSpecialTot { get; set; }

        public int ChoanJobAddTot { get; set; }

        public int ChoanJobTot { get; set; }

        public int ChoanJobModTot { get; set; }

        public int ChoanJobWeddTot { get; set; }


        public int SingleSideCardTot { get; set; }

        public int PhotoCardTot { get; set; }

        public int InitialCardCardTot { get; set; }

        public int DoubleSideCardTot { get; set; }

        public int LaserCardTot { get; set; }
        public int RespectCardTot { get; set; }
        public int AddOrderOriginalTot { get; set; }

        public int AddOrderEditedTot { get; set; }
        public int TotalCountTot { get; set; }

        /// <summary>
        /// 바인딩 아이템 목록
        /// </summary>
        public List<WorkStatisticsMonthSearchDataModel> MonthDataModel { set; get; }
        public List<WorkStatisticsWeddingSearchDataModel> WeddingDataModel { set; get; }
        public List<WorkStatisticsChoanJobSearchDataModel> ChoanJobDataModel { set; get; }

        public List<WorkStatisticsChoanJobCardSearchDataModel> ChoanJobCardDataModel { set; get; }


        public int InvitationCntTotal { get; set; }
        public int ThanksCntTotal { get; set; }
        public int DigitalCntTotal { get; set; }
        public int MapCntTotal { get; set; }
        public int TotalCntTot { get; set; }
        public int ChoanTotalPriceTot { get; set; }
        public int MapTotalPriceTot { get; set; }
        public int AddTotalPriceTot { get; set; }
        public int TotalPriceTot { get; set; }
        public int SagoPriceTot { get; set; }
        public int LastTotalPriceTot { get; set; }
        public int WeekWorkCntTot { get; set; }
        public double WeekIncenTiveSingleSideCardCntTot { get; set; } //주말인센일반
        public double WeekIncenTiveInitialCardCntTot { get; set; } //주말인센박카드
        public double WeekIncenTiveDoubleSideCardCntTot { get; set; } //주말인센양면
        public double WeekIncenTiveLaserCardCntTot { get; set; }//주말인센레이저

        public double WeekIncenTiveTotalPriceTot { get; set; }
    }

    public class WorkStatisticsMonthSearchDataModel
    {
        public string AlbaId { get; set; }
        public int InvitationCnt { get; set; }
        public int ThanksCnt { get; set; }
        public int DigitalCnt { get; set; }
        public int MapCnt { get; set; }

        public int TotalCnt { get; set; }

        public int ChoanTotalPrice { get; set; }
        public int MapTotalPrice { get; set; }
        public int AddTotalPrice { get; set; }

        public int TotalPrice { get; set; }
        public int SagoPrice { get; set; }
        public int LastTotalPrice { get; set; }

        public int Statusseq { get; set; }

        public int Settlestatus { get; set; }

        public DateTime? SrcconfirmDate { get; set; }

        public string SrccomposeAdmin_id { get; set; }

        public int ChoanId { get; set; }

        public int WeekWorkCnt { get; set; }


        public double WeekIncenTiveSingleSideCardCnt { get; set; } //주말인센일반
        public double WeekIncenTiveInitialCardCnt { get; set; } //주말인센박카드
        public double WeekIncenTiveDoubleSideCardCnt { get; set; } //주말인센양면

        public double WeekIncenTiveLaserCardCnt { get; set; }//주말인센레이저


        public double WeekIncenTiveTotalPrice { get; set; }
    }


    public class WorkStatisticsWeddingSearchDataModel
    {
        public string EventDate { get; set; }

        public int EventDateCount { get; set; }
    }

    public class WorkStatisticsChoanJobSearchDataModel
    {
        public string AdminId { get; set; }

        public int DefaultCnt { get; set; }

        public int SpecialCnt {  get; set; }

        public int AddCnt { get; set; }
        public int AddSpecialCnt { get; set; }

        public int TotCnt { get; set; }



        public int ModDefaultCnt { get; set; }

        public int ModSpecialCnt { get; set; }


        public int ModAddCnt { get; set; }
        public int ModAddSpecialCnt { get; set; }

        public int ModTotCnt { get; set; }


        public int WeddingTotCnt { get; set; }

        public string OrderTempAdminId { get; set; }

    }

    public class WorkStatisticsChoanJobCardSearchDataModel
    {
        public string AdminID { get; set; }
        public int TotalCount { get; set; }
        public int AdminLevel { get; set; } 
        public string AdminName { get; set; }

        public int SingleSideCard { get; set; }

        public int PhotoCard { get; set; }

        public int InitialCard { get; set; }

        public int DoubleSideCard { get; set; }


        public int LaserCard { get; set; }

        public int RespectCard { get; set; }

        public int AddOrderOriginal { get; set; }

        public int AddOrderEdited { get; set; }

        public int TotalCnt { get; set; }


    }

}
