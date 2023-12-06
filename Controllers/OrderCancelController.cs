using Barunson.BBarunsonWeb.Models;
using Barunson.BBarunsonWeb.Services;
using Barunson.DbContext;
using Barunson.DbContext.DbModels.BarShop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System.Collections.Generic;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class OrderCancelController : Controller
    {
        private BarShopContext BarShopContext { get; set; }
        private readonly Uri _url;
        private readonly ITossPaymentService _tossPay;

        public OrderCancelController(BarShopContext barShopContext, ITossPaymentService tossPaymentService)
        {
         //   _url = new Uri("https://api.tosspayments.com");
            this.BarShopContext = barShopContext;
            this._tossPay = tossPaymentService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(int order_seq, string order_type, string? admin_id, string viewType)
        {
            List<OrderCancelViewModel>? items = null;
            OrderCancelViewModel model = new OrderCancelViewModel();

            model.ViewType = viewType;
    
            if (order_type.ToUpper() == "W" || order_type.ToUpper() == "") //청첩장
            {
                // 취소 타입 가져오기 
                model.CanCelType1s = await GetSelectListsCommonCodesAsync("CanCelType1s", true, "", "-------선택하세요------");
                model.CanCelType2s = await GetSelectListsCommonCodesAsync("CanCelType2s", true, "", "-------선택하세요------");
          
                order_type = "W";
            }
            
            model.RefundType1s = await GetSelectListsCommonCodesAsync("RefundType1s", false, "", "");
            model.RefundType2s = await GetSelectListsCommonCodesAsync("RefundType2s", false, "", "");

            var result = await GetOrderCancelAsync(order_seq, order_type, admin_id);

            if (result.Count > 0) { model.IsErr = false; } else { model.IsErr = true; model.ErrStr = "존재하지 않는 주문입니다."; }

            if (result.Count > 0) { model.IsErr = false; }
            else
            {
                model.IsErr = true; model.ErrStr = "존재하지 않는 주문입니다.";
            }

            model.DataModel = result.ToList();

            //if (items.StatusSeq == 5)
            //{
            //    items.IsErr = true;
            //    items.ErrStr = "이미 처리된 주문건 입니다.";

            //}

            return View(model);
        }

        /// <summary>
        /// 공통 코드 SelectList 출력
        /// </summary>
        /// <param name="codeGroup"></param>
        /// <param name="addAll">All 추가 여부</param>
        /// <param name="allValue"></param>
        /// <param name="allText"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<SelectListItem>> GetSelectListsCommonCodesAsync(string codeGroup,
            bool addAll = false, string allValue = "", string allText = "전체")
        {
            var codes = await GetOrderCanCelReasonCodeAsync(codeGroup);
            var items = codes.Select(m => new SelectListItem { Text = m.CodeValue, Value = m.Code }).ToList();
            if (addAll)
                items.Insert(0, new SelectListItem { Text = allText, Value = allValue });

            return new SelectList(items, "Value", "Text");
        }

        /// <summary>
        /// 공통 코드 목록 - 코드 그룹
        /// </summary>
        /// <param name="codeGroup"></param>
        /// <returns></returns>
        protected async Task<List<OrderCanCelReasonKindModel>> GetOrderCanCelReasonCodeAsync(string? codeGroup)
        {
            if (codeGroup == "CanCelType1s")
            {

                var query = from m in BarShopContext.manage_code
                            where (m.code_type == "order_cancel_type") && ((Convert.ToInt32(m.code) % 100) == 0)
                            select new OrderCanCelReasonKindModel
                            {
                                Code = m.code,
                                CodeValue = m.code_value

                            };
                return await query.OrderBy(x => x.Code).ToListAsync();
            }
            else if (codeGroup == "CanCelType2s")
            {
                var query = from m in BarShopContext.manage_code
                            where (m.code_type == "order_cancel_type") && ((Convert.ToInt32(m.code) % 100) != 0)
                            select new OrderCanCelReasonKindModel
                            {
                                Code = m.code,
                                CodeValue = m.code_value

                            };
                return await query.OrderBy(x => x.Code).ToListAsync();
            }
            else if (codeGroup == "RefundType1s")
            {
                var query = from m in BarShopContext.manage_code
                            where (m.code_type == "REFUND_TYPE_CODE")// && m.etc2 == "환입"
                            select new OrderCanCelReasonKindModel
                            {
                                Code = m.etc1,
                                CodeValue = m.etc2

                            };
                return await query.Distinct().OrderBy(x => x.Code).ToListAsync();
              
            }
            else if (codeGroup == "RefundType2s")
            {
                var query = from m in BarShopContext.manage_code
                            where (m.code_type == "REFUND_TYPE_CODE")// && m.etc2 == "환입"
                            select new OrderCanCelReasonKindModel
                            {
                                Seq = (int)m.seq,
                                Code = m.etc2 + "_" + m.seq + "_" + m.code,
                                CodeValue = m.code_value,
                                RealCode = m.code
                            };
                return await query.ToListAsync();

            }
            else
                {
                    var query = from m in BarShopContext.manage_code
                                where (m.code_type == "order_cancel_type") && ((Convert.ToInt32(m.code) % 100) != 0)
                                select new OrderCanCelReasonKindModel
                                {
                                    Code = m.code,
                                    CodeValue = m.code_value

                                };
                    return await query.OrderBy(x => x.Code).ToListAsync();
                }

        }

        public async Task<List<OrderCancelViewModel>> GetOrderCancelAsync(int order_seq, string order_type, string? id)
        {
            List<OrderCancelViewModel> items = null;

            if (order_type.ToUpper() == "W" || order_type.ToUpper() == "") //청첩장
            {
                order_type = "W";

                var order_query = from o in BarShopContext.custom_order
                                  where o.order_seq == order_seq
                                  select new OrderCancelViewModel
                                  {
                                      SalesGubun = o.sales_Gubun,
                                      OrderSeq = o.order_seq,
                                      Order_g_Seq = o.order_g_seq == null ? 0 : (int)o.order_g_seq,
                                      MemberId = o.member_id,
                                      OrderName = o.order_name,
                                      CompanySeq = o.company_seq,
                                      SettlePrice = o.settle_price.ToString() == null ? 0 : (int)o.settle_price,
                                      SettleStatus = o.settle_status,
                                      SettleMethod = o.settle_method == null ? "" : o.settle_method,
                                      SettleDate = o.settle_date.ToString() == null ? null : (DateTime)o.settle_date,
                                      PgShopId = o.pg_shopid,
                                      PgTid = o.pg_tid,
                                      DacomTid = o.dacom_tid == null ? "" : o.dacom_tid,
                                      StatusSeq = o.status_seq,
                                      isCloseCopy = o.isPrintCopy,
                                      OrderType = order_type,
                                      AdminId = id,
                                      SrcPrintCopyDate = o.src_printCopy_date == null ? null : o.src_printCopy_date,
                                      SrcPrintCopyAdminID = o.src_PrintCopy_admin_id == null ? "" : o.src_PrintCopy_admin_id,
                                      CancelType = o.cancel_type,
                                      CancelTypeComment = o.cancel_type_comment,
                                      CancelUserType = o.cancel_user_type,
                                      AdditionCouponSeq = o.addition_couponseq
                                      //SettleName = GetCustomSettleName(o.settle_method == null ? "" : o.settle_method)

                                  };
                items = await order_query.ToListAsync();
                items.ForEach(m =>
                {
                    m.SettleName = GetCustomSettleName(m.SettleMethod);

                });


            }
            else if (order_type.ToUpper() == "S")
            {
                var order_query = from o in BarShopContext.CUSTOM_SAMPLE_ORDER
                                    where o.sample_order_seq == order_seq
                                    select new OrderCancelViewModel
                                    {
                                        SalesGubun = o.SALES_GUBUN,
                                        OrderSeq = o.sample_order_seq,
                                        Order_g_Seq = o.order_g_seq == null ? 0 : (int)o.order_g_seq,
                                        MemberId = o.MEMBER_ID,
                                        OrderName = o.MEMBER_NAME,
                                        CompanySeq = o.COMPANY_SEQ,
                                        SettlePrice = o.SETTLE_PRICE.ToString() == null ? 0 : (int)o.SETTLE_PRICE,
                                        SettleStatus = (byte?)o.STATUS_SEQ,
                                        SettleMethod = o.SETTLE_METHOD == null ? "" : o.SETTLE_METHOD,
                                        SettleDate = (DateTime)o.SETTLE_DATE,
                                        PgShopId = o.PG_MERTID,
                                        PgTid = o.PG_TID,
                                        DacomTid = o.DACOM_TID == null ? "" : o.DACOM_TID,
                                        StatusSeq = o.STATUS_SEQ,
                                        isCloseCopy = "0",
                                        OrderType = order_type,
                                        AdminId = id//,
                                        //SettleName = GetCustomSettleName(o.SETTLE_METHOD == null ? "" : o.SETTLE_METHOD)

                                    };

                items = await order_query.ToListAsync();
                items.ForEach(m =>
                {
                    m.SettleName = GetCustomSettleName(m.SettleMethod);

                });


            }
            else if (order_type.ToUpper() == "F")
            {
                var order_query = from o in BarShopContext.CUSTOM_ETC_ORDER
                                    where o.order_seq == order_seq
                                    select new OrderCancelViewModel
                                    {
                                        SalesGubun = o.sales_gubun,
                                        CompanySeq = o.company_Seq,
                                        StatusSeq = (int)o.status_seq,
                                        SettleStatus = (byte?)o.status_seq,
                                        OrderSeq = o.order_seq,
                                        Order_g_Seq = o.order_g_seq == null ? 0 : (int)o.order_g_seq,
                                        OrderName = o.order_name,
                                        MemberId = o.member_id,

                                        SettlePrice = o.settle_price.ToString() == null ? 0 : (int)o.settle_price,
                                        SettleDate = (DateTime)o.settle_date,
                                        SettleMethod = o.settle_method == null ? "" : o.settle_method,
                                        PgShopId = o.pg_shopid,
                                        PgTid = o.pg_tid,
                                        DacomTid = o.dacom_tid == null ? "" : o.dacom_tid,

                                        isCloseCopy = "0",
                                        OrderType = order_type,
                                        AdminId = id//,
                                        //SettleName = GetCustomSettleName(o.settle_method == null ? "" : o.settle_method)

                                    };
                items = await order_query.ToListAsync();
                items.ForEach(m =>
                {
                    m.SettleName = GetCustomSettleName(m.SettleMethod);

                });

            }

            return items;
        }




    /// <summary>
    /// 주문 취소 + 환불 처리 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<IActionResult> RefundSaveAsync(OrderCancelViewModel model)
    {
        var result = new JsonResultModel { status = false, message = "", code = "" };
        string RefundOrderYn = "T";
        result.code = "0000";

        using (var transaction = await BarShopContext.Database.BeginTransactionAsync())
        {
            try
            {
                if (model.ViewType == "1") // 결재 취소 
                {
                    model.StatusSeq = model.StatusSeq > 9 ? 9 : model.StatusSeq;

                    string cancel_reason = model.CancelReason;

                    StringBuilder LogMent = new StringBuilder();
                    var now = DateTime.Now;

                    #region 청첩장일 경우 로그 DB 기록 Start
                    if (model.OrderType == "W")
                    {
                        cancel_reason = model.OrderCancelTypeComment;

                        var order_query = from o in BarShopContext.custom_order
                                            where o.order_seq == model.OrderSeq
                                            select new OrderCancelViewModel
                                            {
                                                StatusSeq = o.status_seq,
                                                SettleStatus = o.settle_status,
                                                SettleMethod = o.settle_method == null ? "" : o.settle_method,
                                                SettlePrice = o.settle_price.ToString() == null ? 0 : (int)o.settle_price,
                                                PgShopId = o.pg_shopid,
                                                DacomTid = o.dacom_tid == null ? "" : o.dacom_tid

                                            };


                        var items = await order_query.FirstAsync();


                        LogMent.Append($"로그 취소 시작 - {Environment.NewLine}" + ",")
                        .Append($"PG 취소 요청 : {model.PgCancel}{Environment.NewLine}" + ",")
                        .Append($"재결제 요청 : {model.RePay}{Environment.NewLine}" + ",")
                        .Append($"넘겨받은 status_seq : {model.StatusSeq}{Environment.NewLine}" + ",")
                        .Append($"넘겨받은 settle_status : {model.SettleStatus}{Environment.NewLine}" + ",")
                        .Append($"실 DB STATUS_SEQ : {items.StatusSeq}{Environment.NewLine}" + ",")
                        .Append($"실 DB SETTLE_STATUS : {items.SettleStatus}{Environment.NewLine}" + ",")
                        .Append($"실 DB SETTLE_METHOD : {items.SettleStatus}{Environment.NewLine}" + ",")
                        .Append($"실 DB SETTLE_PRICE : {items.SettlePrice}{Environment.NewLine}" + ",")
                        .Append($"실 DB PG_SHOPID : {items.PgShopId}{Environment.NewLine}" + ",")
                        .Append($"실 DB DACOM_TID : {items.DacomTid}{Environment.NewLine}" + ",");


                        LOG_MST mst = new LOG_MST();

                        mst.GUID = model.OrderSeq.ToString();
                        mst.SITE = "빠른손웹";
                        mst.LOCATION = "RefundSaveAsync";
                        mst.SUB_LOCATION = "";
                        mst.LOG_TYPE_NAME = "주문 취소 시작";
                        mst.MSG = LogMent.ToString();
                        mst.USER_ID = model.AdminId;
                        mst.REG_DATE = now;
                        BarShopContext.Add(mst);
                        await BarShopContext.SaveChangesAsync();

                    }
                        #endregion 청첩장일 경우 로그 DB 기록 End


                    #region PG Shop ID 가공 - tbarunson2 -> barunson2 
                    if (!string.IsNullOrEmpty(model.PgShopId) && model.PgShopId != "")
                    {
                        model.PgShopId = model.PgShopId.Substring(0, 1) == "t" ? model.PgShopId.Substring(1, model.PgShopId.Length - 1) : model.PgShopId;
                    }
                    #endregion

                    #region PG 결제 취소
                    if (model.PgCancel) // PG 결제 취소
                    {
                        // -휴대폰/계좌이체/신용카드/카카오/간편결제
                        if (model.SettleMethod == "5" || model.SettleMethod == "1" || model.SettleMethod == "2" || model.SettleMethod == "6" || model.SettleMethod == "9")
                        {
                            string keyValue = "";
                            var toss_sk_value = await _tossPay.GetTossShopKeyValueAsync(model.PgShopId, "");
                            if (toss_sk_value.status == "404")
                            {
                                keyValue = "not_found_key";

                            }
                            else if (toss_sk_value.status == "400")
                            {
                                keyValue = "no_permissions";
                            }
                            else
                            {
                                keyValue = toss_sk_value.secretKey;
                            }

                            var response = await _tossPay.CancelAsync(keyValue, model.DacomTid, new TossPostPaymentCancel
                            {
                                cancelReason = cancel_reason
                            }); ;
                            if (response == null || response.cancels == null)
                            {
                                result.code = "9999";
                                result.message = response?.message;
                                // throw new Exception($"결제 취소요청이 실패하였습니다.(Code: {response?.code}, message: {response?.message})");
                            }
                            else
                            {
                                result.code = "0000";
                            }

                        }
                        else
                        {
                            result.code = "9999";
                        }

                    }
                    else
                    {
                        result.code = "0000";
                    }
                    #endregion

                    //using (var transaction = await BarShopContext.Database.BeginTransactionAsync())
                    //{
                    //try
                    //{
                    //PG연동 성공이거나 PG연동 없이 취소인 경우
                    if (result.code == "0000")
                    {
                        // 부가상품인경우 order_type "F"->"E"로 변경 (DB에서는 E로 구분함 etc테이블이어서..)
                        if (model.OrderType == "F")
                        {
                            model.OrderType = "E";
                        }
                        string NpgTid = model.PgTid;
                        int Next_Settle_Status;
                        int Next_Statue_Seq;
                        int Next_Settle_Status_g;
                        int Next_Statue_Seq_g;
                        int Pcheck = 0;

                        #region 청첩장 취소 처리 로직1(SP_CUSTOM_ORDER_PAY_CANCEL 프로시저 내용 )
                        // 
                        if (model.OrderType == "W")
                        {
                            //custom_order_group 기본 상태값
                            Next_Settle_Status_g = 5;
                            Next_Statue_Seq = 5;

                            //custom_order 기본 상태값
                            Next_Settle_Status = 5;
                            Next_Statue_Seq = 5;

                            // 재결제 요청 시 pg_tid 생성 
                            if (model.RePay)
                            {
                                switch (model.PgTid.Substring(0, 2))
                                {
                                    case "IC":
                                        NpgTid = $"ID{model.OrderSeq}";
                                        break;
                                    case "ID":
                                        NpgTid = $"IE{model.OrderSeq}";
                                        break;
                                    case "IE":
                                        NpgTid = $"IF{model.OrderSeq}";
                                        break;
                                    case "IT":
                                        NpgTid = $"IH{model.OrderSeq}";
                                        break;
                                    default:
                                        NpgTid = $"IG{model.OrderSeq}";
                                        break;
                                }
                                Next_Settle_Status = 0;
                                Next_Statue_Seq = model.StatusSeq;

                                if (model.PgCancel)
                                {
                                    Next_Settle_Status_g = 0;
                                    Next_Statue_Seq_g = model.StatusSeq;
                                }


                            }
                            else
                            {
                                if (!model.PgCancel && model.SettleStatus != 2)
                                {
                                    Next_Settle_Status = 3;
                                    Next_Statue_Seq = 3;
                                }
                            }
                            model.CancelReason = model.OrderCancelTypeComment;
                            #endregion
                        }
                        else
                        {
                            if (model.PgCancel)
                            {
                                Next_Settle_Status = 5;
                                Next_Statue_Seq = 5;
                            }
                            else
                            {
                                if (model.SettleStatus >= 4)
                                {
                                    Next_Settle_Status = 5;
                                    Next_Statue_Seq = 5;
                                }
                                else
                                {
                                    Next_Settle_Status = 3;
                                    Next_Statue_Seq = 3;
                                }
                            }


                        }

                        #region 청첩장 취소 처리 로직2(SP_COUPON_CANCEL 프로시저 내용 )
                        if (model.OrderType == "W")
                        {
                            //바른손카드, 프리미어페이퍼,비핸즈카드,더카드인 경우 쿠폰 취소 처리
                            if ((model.CompanySeq == 5001 || model.CompanySeq == 5003 || model.CompanySeq == 5006 || model.CompanySeq == 5007) && model.MemberId != "")
                            {
                                //    EXEC SP_COUPON_CANCEL @order_seq, @member_id
                                if (model.SalesGubun == "B" || model.SalesGubun == "H" || model.SalesGubun == "C")
                                {
                                    /* 중복쿠폰만 원복 */
                                    if (model.AdditionCouponSeq != "")
                                    {
                                        var updateMyCoupon = await (from m in BarShopContext.S4_MyCoupon
                                                                    where m.coupon_code == model.AdditionCouponSeq
                                                                            && m.uid == model.MemberId
                                                                            && (m.company_seq == 5006 || m.company_seq == model.CompanySeq)
                                                                            && m.isMyYN == "N"
                                                                    select m).ToListAsync();

                                        if (updateMyCoupon != null)
                                        {
                                            foreach (var item in updateMyCoupon)
                                            {
                                                item.isMyYN = "Y";
                                                await BarShopContext.SaveChangesAsync();

                                            }


                                        }

                                    }
                                }
                                else
                                {
                                    /*  
                                        2018.02.01   
                                        요청자      : 온라인사업1팀 정미진
                                        요청사항    : 바른손카드 취소건일 경우 발급한 식전영상쿠폰을 삭제하고, 사용전 상태로 변경한다.
                                    */
                                    if (model.SalesGubun == "SB")
                                    {
                                        var couponDetailQuery = from ci in BarShopContext.COUPON_ISSUE
                                                                join cd in BarShopContext.COUPON_DETAIL on ci.COUPON_DETAIL_SEQ equals cd.COUPON_DETAIL_SEQ
                                                                where ci.UID == model.MemberId && cd.COUPON_MST_SEQ == 301 && ci.ACTIVE_YN == "Y"
                                                                select new OrderCancelViewModel
                                                                {
                                                                    CouponDetailSeq = ci.COUPON_DETAIL_SEQ
                                                                };
                                        var couponDetailitems = await couponDetailQuery.ToListAsync();

                                        if (couponDetailitems != null)
                                        {
                                            foreach (var item in couponDetailitems)
                                            {
                                                var deletecouponissue = await (from c in BarShopContext.COUPON_ISSUE where c.COUPON_DETAIL_SEQ == item.CouponDetailSeq select c).FirstOrDefaultAsync();
                                                if (deletecouponissue != null)
                                                {
                                                    //DELETE COUPON_ISSUE
                                                    //WHERE COUPON_DETAIL_SEQ IN(SELECT COUPON_DETAIL_SEQ FROM @T_COUPON_DETAIL)

                                                    BarShopContext.Remove(deletecouponissue);
                                                    await BarShopContext.SaveChangesAsync();
                                                }

                                            }
                                        }
                                    }

                                    var coupon_IssueQuery = from cp in BarShopContext.CUSTOM_ORDER_COUPON
                                                            join cd in BarShopContext.custom_order on cp.ORDER_SEQ equals cd.order_seq
                                                            where cp.ORDER_SEQ == model.OrderSeq && cd.member_id == model.MemberId
                                                            select new OrderCancelViewModel
                                                            {
                                                                CouponIssueSeq = cp.COUPON_ISSUE_SEQ
                                                            };
                                    var coupon_Issueitems = await coupon_IssueQuery.ToListAsync();
                                    if (coupon_Issueitems != null)
                                    {
                                        foreach (var item in coupon_Issueitems)
                                        {
                                            //쿠폰보관함에 쿠폰을 사용상태로 변경
                                            var updatecouponissue = await (from c in BarShopContext.COUPON_ISSUE where c.COUPON_ISSUE_SEQ == item.CouponIssueSeq select c).FirstOrDefaultAsync();
                                            if (updatecouponissue != null)
                                            {
                                                updatecouponissue.ACTIVE_YN = "Y";
                                                await BarShopContext.SaveChangesAsync();
                                            }

                                        }
                                    }

                                }


                            }

                            var updateItem = await (from m in BarShopContext.custom_order where m.order_seq == model.OrderSeq select m).FirstAsync();
                            updateItem.status_seq = Next_Statue_Seq;
                            updateItem.src_cancel_date = now;
                            updateItem.src_cancel_admin_id = model.AdminId;
                            updateItem.settle_status = (byte)Next_Settle_Status;
                            updateItem.settle_cancel_date = now;
                            updateItem.pg_tid = NpgTid;
                            updateItem.isPrintCopy = (model.isCloseCopy == "1" ? "0" : model.isCloseCopy);
                            updateItem.src_printCopy_date = (model.isCloseCopy == "1" ? null : model.SrcPrintCopyDate);
                            updateItem.src_print_admin_id = (model.isCloseCopy == "1" ? null : model.SrcPrintCopyAdminID);
                            updateItem.cancel_type = model.CancelType;
                            updateItem.cancel_type_comment = model.OrderCancelTypeComment;
                            updateItem.cancel_user_type = model.CancelUserType;
                            
                            await BarShopContext.SaveChangesAsync();

                            #region PG 취소 & 재결재 요청
                            if (model.PgCancel)
                            {
                                if (model.RePay) // 재결제 요청시 'africa' 동의 삭제
                                {
                                    var africaAgree = await (from c in BarShopContext.custom_order_agreement where c.order_seq == model.OrderSeq && c.agreement_type.ToUpper() == "africa" select c).ToListAsync();
                                    if (africaAgree != null)
                                    {
                                        foreach (var item2 in africaAgree)
                                        {
                                            BarShopContext.Remove(africaAgree);
                                            await BarShopContext.SaveChangesAsync();
                                        }
                                    }

                                    LogMent.Remove(0, LogMent.Length)
                                    .Append($"(PG결제취소 & 재결제처리)  {model.CancelReason}");

                                }
                                else
                                {
                                    LogMent.Remove(0, LogMent.Length)
                                    .Append($"(PG결제취소)  {model.CancelReason}");
                                }

                                Pcheck = 5;
                            }
                            else
                            {
                                if (model.RePay)
                                {
                                    LogMent.Remove(0, LogMent.Length)
                                    .Append($"(주문취소 & 재결제처리)  {model.CancelReason}");

                                }
                                else
                                {
                                    LogMent.Remove(0, LogMent.Length)
                                    .Append($"(주문취소)  {model.CancelReason}");

                                }
                                Pcheck = 3;

                            }
                            #endregion

                            CUSTOM_ORDER_ADMIN_MENT admin_memt_item = new CUSTOM_ORDER_ADMIN_MENT
                            {
                                ISWOrder = "1",
                                ORDER_SEQ = model.OrderSeq,
                                MENT = LogMent.ToString(),
                                PCHECK = (byte)Pcheck,
                                ADMIN_ID = model.AdminId,
                                Mtype = "AM01",
                                Category = "AMC0701",
                                REG_DATE = now,
                              
                                intype = 0,
                                isJumun = "1",
                                sgubun = "",
                                stype = "기타",
                                STATUS = 0

                            };

                            BarShopContext.Add(admin_memt_item);
                            await BarShopContext.SaveChangesAsync();

                        }
                        #endregion
                        else if (model.OrderType == "E" || model.OrderType == "S")
                        {
                            // 그룹 주문
                            if (model.PgCancel && model.Order_g_Seq > 0)
                            {
                                var group_updateItem = await (from m in BarShopContext.Custom_order_Group where m.order_g_seq == model.Order_g_Seq select m).ToListAsync();
                                foreach (var item in group_updateItem)
                                {
                                    item.status_seq = 5;
                                    item.src_cancel_admin_id = model.AdminId;
                                    item.settle_status = 5;
                                    item.settle_cancel_date = now;
                                    await BarShopContext.SaveChangesAsync();
                                }
                            }
                            // 부가상품 업데이트 
                            if (model.OrderType == "E")
                            {
                                var etc_updateItem = await (from m in BarShopContext.CUSTOM_ETC_ORDER where m.order_seq == model.OrderSeq select m).FirstAsync();
                                etc_updateItem.status_seq = (byte)Next_Statue_Seq;
                                etc_updateItem.settle_Cancel_Date = now;
                                etc_updateItem.admin_id = model.AdminId;

                                await BarShopContext.SaveChangesAsync();

                            }
                            else if (model.OrderType == "S")
                            {
                                var sample_updateItem = await (from m in BarShopContext.CUSTOM_SAMPLE_ORDER where m.sample_order_seq == model.OrderSeq select m).ToListAsync();

                                foreach (var item in sample_updateItem)
                                {
                                    item.STATUS_SEQ = @Next_Statue_Seq;
                                    item.CANCEL_DATE = now;
                                    item.CANCEL_REASON = model.CancelReason;
                                    item.ADMIN_ID = model.AdminId;

                                    await BarShopContext.SaveChangesAsync();
                                }
                               

                            }
                        }

                        #region 지시서 검증 취소 처리 
                        if (model.isCloseCopy == "1")
                        {
                            var delivery_updateItem = await (from m in BarShopContext.DELIVERY_INFO where m.ORDER_SEQ == model.OrderSeq select m).ToListAsync();

                            foreach (var item in delivery_updateItem)
                            {
                                item.savepack_date = null;
                                item.savepack_admin_id = null;
                                await BarShopContext.SaveChangesAsync();
                            }


                            var deleteItem = await (from m in BarShopContext.CUSTOM_ORDER_COPY_DETAIL where m.order_seq == model.OrderSeq select m).ToListAsync();
                            if (deleteItem != null)
                            {
                                foreach (var item in deleteItem)
                                {
                                    BarShopContext.Remove(item);
                                    await BarShopContext.SaveChangesAsync();
                                }

                            }
                            var deleteItem2 = await (from m in BarShopContext.CUSTOM_ORDER_COPY where m.order_seq == model.OrderSeq select m).ToListAsync();
                            if (deleteItem2 != null)
                            {
                                foreach (var item2 in deleteItem2)
                                {
                                    BarShopContext.Remove(item2);
                                    await BarShopContext.SaveChangesAsync();
                                }
                            }


                            custom_order_history custom_order_history = new custom_order_history
                            {
                                order_seq = model.OrderSeq,
                                htype = "검증취소",
                                admin_id = model.AdminId,
                                memo = "지시서 검증 취소",
                                system_sql = "",
                                reg_date = now
                            };

                            BarShopContext.Add(custom_order_history);
                            await BarShopContext.SaveChangesAsync();

                        }
                        #endregion

                        #region 가상계좌 취소 처리 
                        if (model.SettleMethod == "3")
                        {
                            var toss_updateItem = await (from m in BarShopContext.toss_vaccount where m.order_seq == model.OrderSeq && m.toss_orderid == model.PgTid select m).FirstOrDefaultAsync();
                            toss_updateItem.status = 4;

                            await BarShopContext.SaveChangesAsync();

                        }
                        #endregion

                    }

                    //await transaction.CommitAsync();
                    //}
                    //catch (Exception ex)
                    //{
                    //    result.message = ex.ToString();
                    //    result.code = "9999";
                    //await transaction.RollbackAsync();
                    //}
                    // }


                    #region 청첩장일 경우 로그 DB 기록 Start
                    if (model.OrderType == "W")
                    {

                        var order_query = from o in BarShopContext.custom_order
                                            where o.order_seq == model.OrderSeq
                                            select new OrderCancelViewModel
                                            {
                                                StatusSeq = o.status_seq,
                                                SettleStatus = o.settle_status,
                                                SettleMethod = o.settle_method == null ? "" : o.settle_method,
                                                SettlePrice = o.settle_price.ToString() == null ? 0 : (int)o.settle_price,
                                                PgShopId = o.pg_shopid,
                                                DacomTid = o.dacom_tid == null ? "" : o.dacom_tid

                                            };


                        var items = await order_query.FirstAsync();

                        LogMent.Remove(0, LogMent.Length)
                        .Append($"로그 취소 끝 -  {Environment.NewLine}" + ",")
                        .Append($"데이콤 취소 요청 : {model.PgCancel}{Environment.NewLine}" + ",")
                        .Append($"재결제 요청 : {model.RePay}{Environment.NewLine}" + ",")
                        .Append($"넘겨받은 status_seq : {model.StatusSeq}{Environment.NewLine}" + ",")
                        .Append($"넘겨받은 settle_status : {model.SettleStatus}{Environment.NewLine}" + ",")
                        .Append($"실 DB STATUS_SEQ : {items.StatusSeq}{Environment.NewLine}" + ",")
                        .Append($"실 DB SETTLE_STATUS : {items.SettleStatus}{Environment.NewLine}" + ",")
                        .Append($"실 DB SETTLE_METHOD : {items.SettleMethod}{Environment.NewLine}" + ",")
                        .Append($"실 DB SETTLE_PRICE : {items.SettlePrice}{Environment.NewLine}" + ",")
                        .Append($"실 DB PG_SHOPID : {items.PgShopId}{Environment.NewLine}" + ",")
                        .Append($"실 DB DACOM_TID : {items.DacomTid}{Environment.NewLine}" + ",");

                        LOG_MST mst = new LOG_MST();

                        mst.GUID = model.OrderSeq.ToString();
                        mst.SITE = "빠른손";
                        mst.LOCATION = "RefundSaveAsync";
                        mst.SUB_LOCATION = "";
                        mst.LOG_TYPE_NAME = "주문 취소 끝";
                        mst.MSG = LogMent.ToString();
                        mst.USER_ID = model.AdminId;

                        BarShopContext.Add(mst);
                        await BarShopContext.SaveChangesAsync();


                    }
                    #endregion 청첩장일 경우 로그 DB 기록 End

                }

                if (model.SettleMethod == "3" || model.ViewType == "2") //  환불 등록
                {
                    if (model.JumunType == "0") //청첩장
                    {
                        var custom_orderItem = await (from o in BarShopContext.custom_order where o.order_seq == model.OrderSeq select o).ToListAsync();

                        if (custom_orderItem.Count == 0)
                        {
                            RefundOrderYn = "F";
                        }
                    }
                    else if (model.JumunType == "1" || model.JumunType == "2") //포스터 청첩장 && 식권/기타
                    {
                        var etc_orderItem = await (from o in BarShopContext.CUSTOM_ETC_ORDER where o.order_seq == model.OrderSeq && o.order_type != "B" select o).ToListAsync();

                        if (etc_orderItem.Count == 0)
                        {
                            RefundOrderYn = "F";
                        }
                    }
                    else if (model.JumunType == "3") //샘플
                    {
                        var sample_orderItem = await (from o in BarShopContext.CUSTOM_SAMPLE_ORDER where o.sample_order_seq == model.OrderSeq select o).ToListAsync();

                        if (sample_orderItem.Count == 0)
                        {
                            RefundOrderYn = "F";
                        }
                    }
                    else if (model.JumunType == "5") //포토북
                    {
                        var photobook_orderItem = await (from o in BarShopContext.PHOTOBOOK_ORDER where o.id == model.OrderSeq select o).ToListAsync();

                        if (photobook_orderItem.Count == 0)
                        {
                            RefundOrderYn = "F";
                        }
                    }
                    else if (model.JumunType == "6") //뷰티박스
                    {
                        var etc_orderItem = await (from o in BarShopContext.CUSTOM_ETC_ORDER where o.order_seq == model.OrderSeq && o.order_type == "B" select o).ToListAsync();
                        if (etc_orderItem.Count == 0)
                        {
                            RefundOrderYn = "F";
                        }
                    }

                    if (RefundOrderYn == "T" && result.code == "0000")
                    {
                        //using (var transaction = await BarShopContext.Database.BeginTransactionAsync())
                        //{
                        //try
                        //{
                        //환불 정보 등록
                        custom_order_refund custom_order_refund = new custom_order_refund
                        {
                            sales_gubun = model.SalesGubun,
                            company_seq = model.CompanySeq,
                            order_tbl = model.JumunType,
                            order_seq = model.OrderSeq,
                            order_name = model.OrderName,
                            settle_date = model.SettleDate == null ? "" : model.SettleDate?.ToString("yyyy-MM-dd"),
                            settle_method = model.SettleMethod,
                            settle_price = model.SettlePrice,
                            refund_price = model.RefundPrice,
                            refund_date = model.RefundDate,
                            refund_msg = model.RefundMsg == null ? "" : model.RefundMsg,
                            refund_gubun = "",
                            bank_name = model.RefundBank,
                            bank_account = model.RefundBankAccount,
                            bank_user = model.RefundUserName,
                            admin_id = model.AdminId,
                            REFUND_TYPE_CODE = model.RefundCode,
                            reg_date = DateTime.Now


                        };

                        BarShopContext.Add(custom_order_refund);
                        await BarShopContext.SaveChangesAsync();
                        //await transaction.CommitAsync();
                        result.code = "0000";
                        //}
                        //catch (Exception ex)
                        //{
                        //    result.message = ex.ToString();
                        //    //await transaction.RollbackAsync();
                        //    result.code = "7777";
                        //}
                        //}
                    }
                    else { result.code = "8888"; }
                }

                if (result.code == "0000")
                {
                    await transaction.CommitAsync();
                }
                else
                {
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                result.code = "8888";
                result.message = ex.ToString();
                await transaction.RollbackAsync();
            }

        }
        
            return Json(result);
    }

    private string GetCustomSettleName(string SettleStatus)
    {
        string ReturnStr = "";

        switch (SettleStatus)
        {
            case "1":
                ReturnStr = "계좌이체";
                break;
            case "2":
                ReturnStr = "신용카드";
                break;
            case "5":
                ReturnStr = "휴대폰 결제";
                break;
            case "6":
                ReturnStr = "카카오";
                break;
            case "9":
                ReturnStr = "간편결제";
                break;
            case "3":
                ReturnStr = "가상계좌";
                break;
            default:
                break;
        }
        return ReturnStr;
    }


    }

}
