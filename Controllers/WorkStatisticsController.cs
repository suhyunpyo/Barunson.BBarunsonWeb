using Barunson.BBarunsonWeb.Models;
using Barunson.BBarunsonWeb.Services;
using Barunson.DbContext;
using Barunson.DbContext.DbModels.BarShop;
using Barunson.DbContext.DbModels.DearDeer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Data.Common;
using System.Linq;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class WorkStatisticsController : Controller
    {
        private BarShopContext BarShopContext { get; set; }

        public WorkStatisticsController(BarShopContext barShopContext)
        {
            this.BarShopContext = barShopContext;
        }

        private double IntRound(double Value, int Digit)
        {
            double Temp = Math.Pow(10.0, Digit);
            return Math.Round(Value * Temp) / Temp;
        }

        public async Task<IActionResult> Index(WorkStatisticsSearchModel model)
        {
            //model.StartDate = Convert.ToDateTime("2022-09-01");
            //model.EndDate = Convert.ToDateTime("2022-09-20");

            if (model.MenuGubun == 0) /*초안 비용(월간)*/
            {
                model.BtnClass = "MonthBtn";
                var result = await GetWorkStatisticsMonthAsync(model.StartDate, model.EndDate);
                model.MonthDataModel = result.ToList();
                model.InvitationCntTotal  = result.Sum(x => x.InvitationCnt);
                model.ThanksCntTotal = result.Sum(x => x.ThanksCnt);
                model.DigitalCntTotal = result.Sum(x => x.DigitalCnt);
                model.MapCntTotal = result.Sum(x => x.MapCnt);
                model.TotalCntTot = result.Sum(x => x.TotalCnt);
                model.ChoanTotalPriceTot = result.Sum(x => x.ChoanTotalPrice);
                model.MapTotalPriceTot = result.Sum(x => x.MapTotalPrice);
                model.AddTotalPriceTot = result.Sum(x => x.AddTotalPrice);
                model.TotalPriceTot = result.Sum(x => x.TotalPrice);
                model.SagoPriceTot = result.Sum(x => x.SagoPrice);
                model.LastTotalPriceTot = result.Sum(x => x.LastTotalPrice);
                model.WeekWorkCntTot = result.Sum(x => x.WeekWorkCnt);
                model.WeekIncenTiveSingleSideCardCntTot = result.Sum(x => x.WeekIncenTiveSingleSideCardCnt);
                model.WeekIncenTiveInitialCardCntTot = result.Sum(x => x.WeekIncenTiveInitialCardCnt);
                model.WeekIncenTiveDoubleSideCardCntTot = result.Sum(x => x.WeekIncenTiveDoubleSideCardCnt);
                model.WeekIncenTiveLaserCardCntTot = result.Sum(x => x.WeekIncenTiveLaserCardCnt);
                model.WeekIncenTiveTotalPriceTot = result.Sum(x => x.WeekIncenTiveTotalPrice);

            }
            else if (model.MenuGubun == 1) /*주문일 - 예식일*/
            {
                model.BtnClass = "WeddingBtn";
                var result = await GetWorkStatisticsWeddingAsync(model.StartDate, model.EndDate);
                model.WeddingDataModel = result.ToList();
                model.EventDateTotalCount = result.Sum(x => x.EventDateCount);
            }
            else if(model.MenuGubun == 2) /*초안 등록건수*/
            {
                model.BtnClass = "ChoanJobCountBtn";
                var result = await GetWorkStatisticsChoanJobAsync(model.StartDate, model.EndDate, model.Order_Date_Yorn);
                model.ChoanJobDataModel = result.ToList();
                model.ChoanJobDefaultTot = result.Sum(x => x.DefaultCnt);
                model.ChoanJobSpecialTot = result.Sum(x => x.SpecialCnt);
                model.ChoanJobAddTot = result.Sum(x => x.AddCnt);
                model.ChoanJobTot = result.Sum(x => x.TotCnt);
                model.ChoanJobModTot = result.Sum(x => x.ModTotCnt);
                model.ChoanJobWeddTot = result.Sum(x => x.WeddingTotCnt);
               
            }
            else /*초안 작업자별/ 품목별 작업건수*/
            {
                var result = await GetWorkStatisticsChoanJobCardAsync(model.StartDate, model.EndDate);
                model.BtnClass = "ChoanJobCardCountBtn";
                model.ChoanJobCardDataModel = result.ToList();
                model.SingleSideCardTot = result.Sum(x => x.SingleSideCard);
                model.PhotoCardTot = result.Sum(x => x.PhotoCard);
                model.InitialCardCardTot = result.Sum(x => x.InitialCard);
                model.DoubleSideCardTot = result.Sum(x => x.DoubleSideCard);
                model.LaserCardTot = result.Sum(x => x.LaserCard);
                model.RespectCardTot = result.Sum(x => x.RespectCard);
                model.AddOrderOriginalTot = result.Sum(x => x.AddOrderOriginal);
                model.AddOrderEditedTot = result.Sum(x => x.AddOrderEdited);
                model.TotalCountTot = result.Sum(x => x.TotalCount);

            }
         

            return View(model);
        }


        /// <summary>
        /// 초안 작업자별/ 품목별 작업건수 통계
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>

        private async Task<IEnumerable<WorkStatisticsChoanJobCardSearchDataModel>> GetWorkStatisticsChoanJobCardAsync(DateTime startDate, DateTime endDate)
        {
            List<string> orderTypeList = new List<string>() {"2", "6", "7" };

            var Query = from o in BarShopContext.custom_order
                        join c in BarShopContext.S2_Card on o.card_seq equals c.Card_Seq
                        join co in BarShopContext.S2_CardOption on o.card_seq equals co.Card_Seq
                        join cc in BarShopContext.CARD_COREL on c.Card_Code equals cc.card_code
                        join a in BarShopContext.ADMIN_LST on o.src_compose_admin_id equals a.ADMIN_ID into dd
                        from t in dd.DefaultIfEmpty()
                        where   (o.src_compose_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000"))) &&
                                (o.src_compose_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000"))) && 
                                (o.src_compose_admin_id != null && o.src_compose_admin_id != "") && t.isCS == "0"
                        select new 
                        {
                            SrcComposeAdminId = o.src_compose_admin_id,
                            AdminName22 = t.ADMIN_NAME + "(" + o.src_compose_admin_id + ")",
                            AdminLevel = t.ADMIN_LEVEL,
                            SingleSideCard = o.up_order_seq == null && 
                                            !orderTypeList.Contains(o.order_type) && 
                                            co.isLaser != "1" && 
                                            co.Master_2Color != "1" && 
                                            cc.IsFPrint != "1" ? 1 : 0,
                            RespectCard = o.up_order_seq == null && o.order_type == "2" ? 1 : 0,
                            PhotoCard = o.up_order_seq == null && o.order_type == "6" ? 1 : 0,
                            InitialCard = o.up_order_seq == null && o.order_type == "7" ? 1 : 0,
                            LaserCard = o.up_order_seq == null && !orderTypeList.Contains(o.order_type) && co.isLaser == "1" ? 1 : 0,
                            DoubleSideCard = o.up_order_seq == null && !orderTypeList.Contains(o.order_type) && co.isLaser != "1" && (co.Master_2Color == "1" || cc.IsFPrint == "1") ? 1 : 0,
                            AddOrder_Original = o.up_order_seq != null && o.order_type != "2" && o.order_add_flag == "0" ? 1 : 0 ,
                            AddOrder_Edited = o.up_order_seq != null && o.order_type != "2" && o.order_add_flag == "1" ? 1 : 0
                           

                        };
            
            var QueryItems = await Query.ToListAsync();
            var GroupByItems = QueryItems.GroupBy(x => new { x.SrcComposeAdminId, x.AdminName22, x.AdminLevel })
                 .Select(g => new WorkStatisticsChoanJobCardSearchDataModel
                 {
                    AdminID = g.First().SrcComposeAdminId,
                    AdminName = g.First().AdminName22,
                    AdminLevel = (int)g.First().AdminLevel,
                    SingleSideCard = g.Sum(g => g.SingleSideCard),
                    PhotoCard = g.Sum(g => g.PhotoCard),
                    InitialCard = g.Sum(g => g.InitialCard),
                    DoubleSideCard = g.Sum(g => g.DoubleSideCard),
                    LaserCard = g.Sum(g => g.LaserCard),
                    RespectCard = g.Sum(g => g.RespectCard),
                    AddOrderOriginal = g.Sum(g => g.AddOrder_Original),
                    AddOrderEdited = g.Sum(g => g.AddOrder_Edited),
                    TotalCount = g.Sum(g => g.SingleSideCard + g.PhotoCard + g.InitialCard + g.LaserCard + g.DoubleSideCard + g.RespectCard + g.AddOrder_Original + g.AddOrder_Edited)
                 }).OrderBy(x => x.AdminLevel)
                .ThenBy(x => x.AdminName);

            return GroupByItems;


        }

        /// <summary>
        /// 초안 등록건 통계
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="order_Date_Yorn"></param>
        /// <returns></returns>
        private async Task<IEnumerable<WorkStatisticsChoanJobSearchDataModel>> GetWorkStatisticsChoanJobAsync(DateTime startDate, DateTime endDate, string order_Date_Yorn)
        {
            List<string> isSpecialTypeList = new List<string>() { "1", "2" };

            var UnionQuery1 = from c in BarShopContext.custom_order
                                where (order_Date_Yorn == "Y" ? (
                                                                    c.order_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000")) && 
                                                                    c.order_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000"))
                                                                ) :
                                                                (
                                                                    c.src_compose_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000")) && 
                                                                    c.src_compose_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000"))
                                                                )
                                       )
                                && c.src_compose_date != null && c.src_compose_admin_id != null && c.src_compose_admin_id != ""
                                select new
                                {
                                    C_DEFAULT = !isSpecialTypeList.Contains(c.isSpecial) && c.up_order_seq == null ? 1 : 0, /* 초안등록 일반 */
                                    C_SPECIAL = isSpecialTypeList.Contains(c.isSpecial) && c.up_order_seq == null ? 1 : 0,     /* 초안등록 초특급 */
                                    C_ADD = c.up_order_seq != null && c.order_add_flag == "1" ? 1 : 0,
                                    C_ADD_SPECIAL = 0,
                                    ADMIN_ID = c.src_compose_admin_id
                                };

            var UnionQueryItems1 = await UnionQuery1.ToListAsync();
            var UnionGroupByItems1 = UnionQueryItems1.GroupBy(x => new { x.ADMIN_ID })
                 .Select(g => new
                 {
                     DefaultCnt = g.Sum(g => g.C_DEFAULT),
                     SpecialCnt = g.Sum(g => g.C_SPECIAL),
                     AddCnt = g.Sum(g => g.C_ADD),
                     AddSpecialCnt = g.Sum(g => g.C_ADD_SPECIAL),
                     TotCnt = g.Sum(g => g.C_DEFAULT + g.C_SPECIAL + g.C_ADD + g.C_ADD_SPECIAL),
                     ModDefaultCnt = 0,
                     ModSpecialCnt = 0,
                     ModAddCnt = 0,
                     ModAddSpecialCnt = 0,
                     ModTotCnt = 0,
                     WeddingTotCnt = 0,
                     ADMIN_ID = g.First().ADMIN_ID //g.First().ADMIN_ID == null ? "9999" : "1_" + g.First().ADMIN_ID
                 });//.OrderBy(x => x.EventDate);

            var UnioDataItems1 = UnionGroupByItems1.ToList().Select(g => new 
            {

                DefaultCnt = g.DefaultCnt,
                SpecialCnt = g.SpecialCnt,
                AddCnt = g.AddCnt,
                AddSpecialCnt = g.AddSpecialCnt,
                TotCnt = g.TotCnt,
                ModDefaultCnt = g.ModDefaultCnt,
                ModSpecialCnt = g.ModSpecialCnt,
                ModAddCnt = g.ModAddCnt,
                ModAddSpecialCnt = g.ModAddSpecialCnt,
                ModTotCnt = g.ModTotCnt,
                WeddingTotCnt = g.WeddingTotCnt,
                AdminId = g.ADMIN_ID
            });



            var UnionQuery2 = from c in BarShopContext.custom_order
                         join h in BarShopContext.custom_order_history on c.order_seq equals h.order_seq
                         where h.htype == "초안 수정 등록" &&
                                (order_Date_Yorn == "Y" ?   (
                                                                c.order_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000")) &&
                                                                c.order_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000"))
                                                            ) :
                                                            (
                                                                h.reg_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000")) &&
                                                                h.reg_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000"))
                                                            )
                                )
                                && c.src_compose_mod_date != null && c.src_compose_admin_id != null && c.src_compose_admin_id != ""
                         select new
                         {
                             CMOD_DEFAULT = !isSpecialTypeList.Contains(c.isSpecial) && c.up_order_seq == null ? 1 : 0, /* 초안수정 일반 */
                             CMOD_SPECIAL = isSpecialTypeList.Contains(c.isSpecial) && c.up_order_seq == null ? 1 : 0,     /* 초안수정 초특급 */
                             CMOD_ADD = c.up_order_seq != null ? 1 : 0, /* 초안수정 추가 */
                             CMOD_ADD_SPECIAL = 0,  /* 초안수정 추가 초특급 */
                             SRC_COMPOSE_MOD_ADMIN_ID = c.src_compose_mod_admin_id
                         };

            var UnionQueryItems2 = await UnionQuery2.ToListAsync();
            var UnionGroupByItems2 = UnionQueryItems2.GroupBy(x => new { x.SRC_COMPOSE_MOD_ADMIN_ID })
                                     .Select(g => new
                                     {
                                         DefaultCnt = 0,
                                         SpecialCnt = 0,
                                         AddCnt = 0,
                                         AddSpecialCnt = 0,
                                         TotCnt = 0,
                                         ModDefaultCnt = g.Sum(g => g.CMOD_DEFAULT),
                                         ModSpecialCnt = g.Sum(g => g.CMOD_SPECIAL),
                                         ModAddCnt = g.Sum(g => g.CMOD_ADD),
                                         ModAddSpecialCnt = 0,
                                         ModTotCnt = g.Sum(g => g.CMOD_DEFAULT + g.CMOD_SPECIAL + g.CMOD_ADD + g.CMOD_ADD_SPECIAL),
                                         ADMIN_ID = g.First().SRC_COMPOSE_MOD_ADMIN_ID,
                                         WeddingTotCnt = 0,

                                     });

            var UnionDataItems2 = UnionGroupByItems2.ToList().Select(g => new 
            {
                DefaultCnt = g.DefaultCnt,
                SpecialCnt = g.SpecialCnt,
                AddCnt = g.AddCnt,
                AddSpecialCnt = g.AddSpecialCnt,
                TotCnt = g.TotCnt,
                ModDefaultCnt = g.ModDefaultCnt,
                ModSpecialCnt = g.ModSpecialCnt,
                ModAddCnt = g.ModAddCnt,
                ModAddSpecialCnt = g.ModAddSpecialCnt,
                ModTotCnt = g.ModTotCnt,
                WeddingTotCnt = g.WeddingTotCnt,
                AdminId = g.ADMIN_ID
            });


            var UnionQuery3 = from w in BarShopContext.WeddingHall_Log
                             .Where(
                                        w => w.reg_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000")) &&
                                        w.reg_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000")) &&
                                        w.gubun == "UPDATE_WEDD"
                                    )
                             select new
                             {
                                 ADMIN_ID = w.admin_id

                             };

            var UnionQueryItems3 = await UnionQuery3.ToListAsync();
            var UnionGroupByItems3 = UnionQueryItems3.GroupBy(x => new { x.ADMIN_ID })
                .Select(g => new
                    {
                        DefaultCnt = 0,
                        SpecialCnt = 0,
                        AddCnt = 0,
                        AddSpecialCnt = 0,
                        TotCnt = 0,
                        ModDefaultCnt = 0,
                        ModSpecialCnt = 0,
                        ModAddCnt =0,
                        ModAddSpecialCnt = 0,
                        ModTotCnt = 0,
                        WeddingTotCnt = g.Count(),
                        ADMIN_ID = g.First().ADMIN_ID
                    }
                );

            var UnionDataItems3 = UnionGroupByItems3.ToList().Select(g => new 
            {
                DefaultCnt = g.DefaultCnt,
                SpecialCnt = g.SpecialCnt,
                AddCnt = g.AddCnt,
                AddSpecialCnt = g.AddSpecialCnt,
                TotCnt = g.TotCnt,
                ModDefaultCnt = g.ModDefaultCnt,
                ModSpecialCnt = g.ModSpecialCnt,
                ModAddCnt = g.ModAddCnt,
                ModAddSpecialCnt = g.ModAddSpecialCnt,
                ModTotCnt = g.ModTotCnt,
                WeddingTotCnt = g.WeddingTotCnt,
                AdminId = g.ADMIN_ID
            });

            var FinalDataList = UnioDataItems1.Concat(UnionDataItems2).Concat(UnionDataItems3)
                                .GroupBy(x => x.AdminId)
                                .Select(m => new WorkStatisticsChoanJobSearchDataModel
                                {
                                    DefaultCnt = m.Sum(m => m.DefaultCnt),
                                    SpecialCnt = m.Sum(m => m.SpecialCnt),
                                    AddCnt = m.Sum(m => m.AddCnt),
                                    AddSpecialCnt = m.Sum(m => m.AddCnt),
                                    TotCnt = m.Sum(m => m.TotCnt),

                                    ModDefaultCnt = m.Sum(m => m.ModDefaultCnt),
                                    ModSpecialCnt = m.Sum(m => m.ModSpecialCnt),
                                    ModAddCnt = m.Sum(m => m.ModAddCnt),
                                    ModAddSpecialCnt = m.Sum(m => m.ModAddSpecialCnt),
                                    ModTotCnt = m.Sum(m => m.ModTotCnt),
                                    WeddingTotCnt = m.Sum(m => m.WeddingTotCnt),
                                    AdminId = m.First().AdminId,
                                    OrderTempAdminId = m.First().AdminId == null ? "9999" : "1_" + m.First().AdminId
                                }).OrderBy(m => m.OrderTempAdminId);


            return FinalDataList; 


        }

        /// <summary>
        /// 주문일 - 예식일 통계
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkStatisticsWeddingSearchDataModel>> GetWorkStatisticsWeddingAsync(DateTime startDate, DateTime endDate)
        {
            List<string> OrderTypeList = new List<string>() { "1", "6", "7" };
            List<int> StatusSeqList = new List<int>() { -1, 0, 3, 5 };

            var Query = from o in BarShopContext.custom_order
                        join ow in BarShopContext.custom_order_WeddInfo on o.order_seq equals ow.order_seq
                        where o.order_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000")) && o.order_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000")) &&
                              OrderTypeList.Contains(o.order_type) && o.up_order_seq == null &&
                              !StatusSeqList.Contains(o.status_seq) && ow.event_year != null && ow.event_year != ""
                        select new
                        {
                            EventYmd = ow.event_year + "-" + ow.event_month + "-" + ow.event_Day
                        };


            var QueryItems = await Query.ToListAsync();
            var GroupByItems = QueryItems.GroupBy(x => new { x.EventYmd })
                .Select(g => new WorkStatisticsWeddingSearchDataModel
                {
                    EventDate = string.Format("{0:yyyy-MM-dd}", Convert.ToDateTime(g.First().EventYmd)),
                    EventDateCount = g.Count(),
                   
                }).OrderBy(x => x.EventDate);

           
            return GroupByItems;

        }



        /// <summary>
        /// 초안 비용(월간) 통계 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<WorkStatisticsMonthSearchDataModel>> GetWorkStatisticsMonthAsync(DateTime startDate, DateTime endDate)
        {
            //startDate = Convert.ToDateTime("2023-11-04");
            //endDate = Convert.ToDateTime("2023-11-05");

            List<string> InvitationCnt_OrderTypeList = new List<string>() { "1", "3", "4", "5", "7" };
            List<string> Map_Cnt_OrderTypeList = new List<string>() { "1", "3", "4", "5", "6", "7" };
            List<string> WeekIncenTiveCnt_OrderTypeList = new List<string>() { "1", "2", "3", "4", "5", "6", "7" };

            // DateTime endDate = startDate.AddMonths(1);

            var UnionQuery1 = from o in BarShopContext.custom_order
                        join ow in BarShopContext.custom_order_WeddInfo on o.order_seq equals ow.order_seq
                        where o.src_confirm_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000")) &&
                                o.src_confirm_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000")) &&
                                o.src_compose_admin_id.StartsWith("alba") && o.src_compose_mod_admin_id == o.src_compose_admin_id &&
                                o.status_seq >= 9 && o.settle_status == 2
                        select new
                        {
                            InvitationCnt = (InvitationCnt_OrderTypeList.Contains(o.order_type) ? 1 : 0),
                            ThanksCnt = (o.order_type == "2" ? 1 : 0),
                            DigitalCnt = (o.order_type == "6" ? 1 : 0),
                            MapCnt = ((Map_Cnt_OrderTypeList.Contains(o.order_type) && ow.wedd_idx == 0) ? 1 : 0),
                            Statusseq = o.status_seq,
                            Settlestatus = (int)o.settle_status,
                            SrcconfirmDate = o.src_confirm_date,
                            SrccomposeAdmin_id = o.src_compose_admin_id,
                            ChoanId = Convert.ToInt16(o.src_compose_admin_id.Replace("alba", "")),
                            OrderType = o.order_type

                        };
            var UnionQueryItems1 = await UnionQuery1.ToListAsync();
            var UnionGroupByItems1 = UnionQueryItems1.GroupBy(x => new { x.ChoanId })
                .Select(g => new { ChoanId = g.First().ChoanId,
                    InvitationCnt = g.Sum(g => g.InvitationCnt),
                    ThanksCnt = g.Sum(g => g.ThanksCnt),
                    DigitalCnt = g.Sum(g => g.DigitalCnt),
                    MapCnt = g.Sum(g => g.MapCnt),
                    TotalCnt = g.Sum(g => g.InvitationCnt) + g.Sum(g => g.ThanksCnt) + g.Sum(g => g.DigitalCnt),
                    ChoanTotalPrice = (g.Sum(g => g.InvitationCnt) * 1100) + (g.Sum(g => g.ThanksCnt) * 500) + (g.Sum(g => g.DigitalCnt) * 3000),
                    MapTotalPrice = g.Sum(g => g.MapCnt) * 200,
                    AddTotalPrice = (
                                                                        g.Sum(g => g.InvitationCnt) + g.Sum(g => g.ThanksCnt) + g.Sum(g => g.DigitalCnt) >= 1500 ? 300000 :
                                                                        (
                                                                            (
                                                                                g.Sum(g => g.InvitationCnt) + g.Sum(g => g.ThanksCnt) + g.Sum(g => g.DigitalCnt) < 1500 &&
                                                                                g.Sum(g => g.InvitationCnt) + g.Sum(g => g.ThanksCnt) + g.Sum(g => g.DigitalCnt) >= 1000
                                                                            ) ? 200000 :
                                                                            (
                                                                                g.Sum(g => g.InvitationCnt) + g.Sum(g => g.ThanksCnt) + g.Sum(g => g.DigitalCnt) < 1000 &&
                                                                                g.Sum(g => g.InvitationCnt) + g.Sum(g => g.ThanksCnt) + g.Sum(g => g.DigitalCnt) >= 700 ? 150000 :
                                                                                (
                                                                                    g.Sum(g => g.InvitationCnt) + g.Sum(g => g.ThanksCnt) + g.Sum(g => g.DigitalCnt) < 700 &&
                                                                                    g.Sum(g => g.InvitationCnt) + g.Sum(g => g.ThanksCnt) + g.Sum(g => g.DigitalCnt) >= 500 ? 100000 : 0
                                                                                )
                                                                            )

                                                                        )
                                                                    ),
                    WeekendSingleSideCardWork = 0, //주말인센일반
                    WeekendInitialCardWork = 0, //주말인센박카드
                    WeekendDoubleSideCard = 0, //주말인센양면
                    WeekendLaserCard = 0, //주말인센레이저
                });

            var UnionDataItems1 = UnionGroupByItems1.ToList().Select(g => new WorkStatisticsMonthSearchDataModel
            {
                ChoanId = g.ChoanId,
                AlbaId = "alba" + g.ChoanId.ToString(),
                InvitationCnt = g.InvitationCnt,
                ThanksCnt = g.ThanksCnt,
                DigitalCnt = g.DigitalCnt,
                MapCnt = 0,
                TotalCnt = g.TotalCnt,
                ChoanTotalPrice = g.ChoanTotalPrice,
                MapTotalPrice = 0,
                AddTotalPrice = g.AddTotalPrice,
                TotalPrice = g.ChoanTotalPrice + g.AddTotalPrice,
                SagoPrice = 0,
                LastTotalPrice = Convert.ToInt32(IntRound((g.ChoanTotalPrice + g.AddTotalPrice) * 0.967, -1)), //CAST(ROUND((a.choan_total_price + a.add_total_price) * 0.967, -1) as int)
                WeekWorkCnt = 0,
                WeekIncenTiveSingleSideCardCnt = g.WeekendSingleSideCardWork, //주말인센일반
                WeekIncenTiveInitialCardCnt = g.WeekendInitialCardWork,  //주말인센박카드
                WeekIncenTiveDoubleSideCardCnt = g.WeekendDoubleSideCard,  //주말인센양면
                WeekIncenTiveLaserCardCnt = g.WeekendLaserCard, //주말인센레이저
                WeekIncenTiveTotalPrice = Convert.ToInt32(IntRound((g.ChoanTotalPrice + g.AddTotalPrice) * 0.967, -1))
            }).OrderBy(x => x.ChoanId);

            List<string> orderTypeList = new List<string>() { "2", "6", "7" };


            var UnionQuery2 = from o in BarShopContext.custom_order
                         join c in BarShopContext.S2_Card on o.card_seq equals c.Card_Seq
                         join co in BarShopContext.S2_CardOption on o.card_seq equals co.Card_Seq
                         join cc in BarShopContext.CARD_COREL on c.Card_Code equals cc.card_code
                         join a in BarShopContext.ADMIN_LST on o.src_compose_admin_id equals a.ADMIN_ID into dd
                         from t in dd.DefaultIfEmpty()
                         where (o.src_compose_date >= Convert.ToDateTime(startDate.ToString("yyyy-MM-dd 00:00:00.000"))) &&
                                 (o.src_compose_date <= Convert.ToDateTime(endDate.ToString("yyyy-MM-dd 23:59:59.000"))) &&
                                 (o.src_compose_admin_id != null && o.src_compose_admin_id != "") && t.isCS == "0" && o.src_compose_admin_id.StartsWith("alba")
                         select new
                         {
                             ChoanId = Convert.ToInt16(o.src_compose_admin_id.Replace("alba", "")),
                             SrcComposeDate = o.src_compose_date,
                             OrderType = o.order_type,
                             UpOrderSeq = o.up_order_seq,
                             IsLaser = co.isLaser,
                             Master2Color = co.Master_2Color,
                             IsFprint = cc.IsFPrint

                         };

            var UnionQueryItems2 = await UnionQuery2.ToListAsync();
            var UnionGroupByItems2 = UnionQueryItems2.GroupBy(x => new { x.ChoanId })
                                    .Select(g => new
                                    {
                                        ChoanId = g.First().ChoanId,
                                        InvitationCnt = 0,
                                        ThanksCnt = 0,
                                        DigitalCnt = 0,
                                        MapCnt = 0,
                                        TotalCnt = 0,
                                        ChoanTotalPrice = 0,
                                        MapTotalPrice = 0,
                                        AddTotalPrice = 0,

                                        WeekendSingleSideCardWork = g.Sum(
                                                            g => (
                                                                    (
                                                                        Convert.ToDateTime(g.SrcComposeDate).DayOfWeek == DayOfWeek.Saturday ||
                                                                        Convert.ToDateTime(g.SrcComposeDate).DayOfWeek == DayOfWeek.Sunday
                                                                    ) &&
                                                                    ( 
                                                                        g.UpOrderSeq == null && !orderTypeList.Contains(g.OrderType) && (g.IsLaser != "1" && g.Master2Color != "1" && g.IsFprint != "1")
                                                                    ) 

                                                                ) ? 1 : 0  //주말인센일반
                                                            ),
                                        WeekendInitialCardWork = g.Sum(
                                                            g => (
                                                                    (
                                                                        Convert.ToDateTime(g.SrcComposeDate).DayOfWeek == DayOfWeek.Saturday ||
                                                                        Convert.ToDateTime(g.SrcComposeDate).DayOfWeek == DayOfWeek.Sunday
                                                                    ) &&
                                                                    
                                                                    ( 
                                                                         g.UpOrderSeq == null && g.OrderType == "7"
                                                                    )
                                                                ) ? 1 : 0
                                                            ), //주말인센박카드
                                        WeekendDoubleSideCard = g.Sum(
                                                            g => (
                                                                    (
                                                                        Convert.ToDateTime(g.SrcComposeDate).DayOfWeek == DayOfWeek.Saturday ||
                                                                        Convert.ToDateTime(g.SrcComposeDate).DayOfWeek == DayOfWeek.Sunday
                                                                    ) &&
                                                                    ( //양면
                                                                        g.UpOrderSeq == null && !orderTypeList.Contains(g.OrderType) && g.IsLaser != "1" && (g.Master2Color == "1" || g.IsFprint == "1")
                                                                    )

                                                                ) ? 1 : 0
                                                            ), //주말인센양면
                                        WeekendLaserCard = g.Sum(
                                                            g => (
                                                                    (
                                                                        Convert.ToDateTime(g.SrcComposeDate).DayOfWeek == DayOfWeek.Saturday ||
                                                                        Convert.ToDateTime(g.SrcComposeDate).DayOfWeek == DayOfWeek.Sunday
                                                                    ) &&
                                                                    ( //레이저
                                                                        g.UpOrderSeq == null && !orderTypeList.Contains(g.OrderType) && g.IsLaser == "1"
                                                                    )

                                                                ) ? 1 : 0
                                                            ), //주말인센레이저

                                    });

            var UnionDateItems2 = UnionGroupByItems2.ToList().Select(g => new WorkStatisticsMonthSearchDataModel
            {
                ChoanId = g.ChoanId,
                AlbaId = "alba" + g.ChoanId.ToString(),
                InvitationCnt = g.InvitationCnt,
                ThanksCnt = g.ThanksCnt,
                DigitalCnt = g.DigitalCnt,
                MapCnt = 0,
                TotalCnt = g.TotalCnt,
                ChoanTotalPrice = g.ChoanTotalPrice,
                MapTotalPrice = 0,
                AddTotalPrice = g.AddTotalPrice,
                TotalPrice = g.ChoanTotalPrice + g.AddTotalPrice,
                SagoPrice = 0,
                LastTotalPrice = Convert.ToInt32(IntRound((g.ChoanTotalPrice + g.AddTotalPrice) * 0.967, -1)), //CAST(ROUND((a.choan_total_price + a.add_total_price) * 0.967, -1) as int)
                WeekWorkCnt = g.WeekendSingleSideCardWork + g.WeekendInitialCardWork + g.WeekendDoubleSideCard + g.WeekendLaserCard,
                WeekIncenTiveSingleSideCardCnt = g.WeekendSingleSideCardWork * 2200, //주말인센일반
                WeekIncenTiveInitialCardCnt = g.WeekendInitialCardWork * 2200,  //주말인센박카드
                WeekIncenTiveDoubleSideCardCnt = g.WeekendDoubleSideCard * 2200,  //주말인센양면
                WeekIncenTiveLaserCardCnt = g.WeekendLaserCard * 2200, //주말인센레이저
                WeekIncenTiveTotalPrice =  (g.WeekendSingleSideCardWork * 2200) + (g.WeekendInitialCardWork * 2200)  + (g.WeekendDoubleSideCard * 2200) + (g.WeekendLaserCard * 2200) + 
                                        Convert.ToInt32(IntRound((g.ChoanTotalPrice + g.AddTotalPrice) * 0.967, -1))
            }).OrderBy(x => x.ChoanId);


            var FinalDataList = UnionDataItems1.Concat(UnionDateItems2)
                                .GroupBy(x => x.ChoanId)
                                .Select(m => new WorkStatisticsMonthSearchDataModel
                                {
                                    AlbaId = "alba" + m.First().ChoanId.ToString(),
                                    InvitationCnt = m.Sum(m => m.InvitationCnt),
                                    ThanksCnt = m.Sum(m => m.ThanksCnt),
                                    DigitalCnt = m.Sum(m => m.DigitalCnt),
                                    MapCnt = 0,
                                    TotalCnt = m.Sum(m => m.TotalCnt),
                                    ChoanTotalPrice = m.Sum(m => m.ChoanTotalPrice),
                                    MapTotalPrice = 0,
                                    AddTotalPrice = m.Sum(m => m.AddTotalPrice),
                                    TotalPrice = m.Sum(m => m.ChoanTotalPrice + m.AddTotalPrice),
                                    SagoPrice = 0,
                                    LastTotalPrice = m.Sum(m => m.LastTotalPrice),
                                    WeekWorkCnt = m.Sum(m => m.WeekWorkCnt),
                                    WeekIncenTiveSingleSideCardCnt = m.Sum(m => m.WeekIncenTiveSingleSideCardCnt),
                                    WeekIncenTiveInitialCardCnt = m.Sum(m => m.WeekIncenTiveInitialCardCnt),
                                    WeekIncenTiveDoubleSideCardCnt = m.Sum(m => m.WeekIncenTiveDoubleSideCardCnt),
                                    WeekIncenTiveLaserCardCnt = m.Sum(m => m.WeekIncenTiveLaserCardCnt),
                                    WeekIncenTiveTotalPrice = m.Sum(m=> m.WeekIncenTiveTotalPrice)
                                }).OrderBy(m => m.ChoanId);




            return FinalDataList;

        }



    }
}
