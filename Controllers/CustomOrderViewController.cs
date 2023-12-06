using Barunson.BBarunsonWeb.Models;
using Barunson.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class CustomOrderViewController : Controller
    {
        private BarShopContext BarShopContext { get; set; }

        public CustomOrderViewController(BarShopContext barShopContext)
        {
            this.BarShopContext = barShopContext;
        }

        /// <summary>
        /// 상품구성정보, 판정보, 배송정보 등을 자세히 확인할 수 있는 화면을 보여줍니다.
        /// </summary>
        /// <param name="id">주문번호</param>
        /// <param name="uid">빠른손 관리자ID</param>
        /// <param name="otp">one time password</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(int? id, string uid, string otp)
        {
            ViewBag.Id = id;
            ViewData["Title"] = "청첩장 주문 상세보기";

            CustomOrderDetailViewModel ? model;

            if (id.HasValue)
            {
                var query =
                    from co in BarShopContext.custom_order
                    join c in BarShopContext.S2_Card on co.card_seq equals c.Card_Seq
                    join com in BarShopContext.COMPANY on co.company_seq equals com.COMPANY_SEQ
                    where co.order_seq == id 
                    select new CustomOrderDetailViewModel
                    {
                        OrderSeq = co.order_seq,
                        UpOrderSeq = co.up_order_seq,
                        CompanySeq = co.company_seq,
                        OrderType = co.order_type,
                        MemberId = co.member_id,
                        OrderName = co.order_name,
                        CardSeq = co.card_seq,
                        CardCode = c.Card_Code,
                        CardName = c.Card_Name,
                        OrderCount = co.order_count,
                        OrderDate = co.order_date,
                        StatusSeq = co.status_seq,
                        SettleStatus = co.settle_status,
                        SalesGubun = co.sales_Gubun,
                        OrderPrice = co.order_price,
                        OrderTotalPrice = co.order_total_price,
                        LastTotalPrice = co.last_total_price,
                        SettlePrice = co.settle_price,
                        EtcPrice = co.etc_price,
                        EtcPriceMent = co.etc_price_ment,
                        AdditionReducePrice = co.addition_reduce_price,
                        RibbonPrice = co.ribbon_price,
                        SealingStickerPrice = co.sealing_sticker_price,
                        FlowerPrice = co.flower_price,
                        EnvSpecialPrice = co.EnvSpecial_Price,
                        PointPrice = co.point_price,
                        EnvInsertPrice = co.envInsert_price,
                        SasikPrice = co.sasik_price,
                        PrintPrice = co.print_price,
                        JebonPrice = co.jebon_price,
                        DeliveryPrice = co.delivery_price,
                        EnvPremiumPrice = co.EnvPremium_price,
                        EnvPrice = co.env_price,
                        FticketPrice = co.fticket_price,
                        GuestbookPrice = co.guestbook_price,
                        Jebon2Price = co.jebon2_price,
                        MaskingTapePrice = co.MaskingTape_price,
                        OptionPrice = co.option_price,
                        PaperCoverPrice = co.paperCover_price,
                        PocketPrice = co.Pocket_price,
                        ReducePrice = co.reduce_price,
                        StickerPrice = co.sticker_price,
                        OrderAddFlag = co.order_add_flag,
                        PayType = co.pay_Type,
                        IsSpecial = co.isSpecial,
                        inflow_route = co.inflow_route,
                        inflow_route_settle = co.inflow_route_settle,
                        SalesGubunName = BarShopContext.manage_code.Where(m => m.code_type == "sales_gubun" && m.code == co.sales_Gubun).Select(m => m.code_value).FirstOrDefault(),
                        OrderTypeName = BarShopContext.manage_code.Where(m => m.code_type == "worder_type" && m.code == co.order_type).Select(m => m.code_value).FirstOrDefault(),
                        CompanyName = com.COMPANY_NAME,
                        StatusSeqName = BarShopContext.manage_code.Where(m => m.code_type == "status_seq" && m.code == co.status_seq.ToString()).Select(m => m.code_value).FirstOrDefault(),
                        PgTid = co.pg_tid
                    };

                model = await query.FirstOrDefaultAsync();

                if (model != null)
                {
                    var queryCustomOrderItem =
                        from coi in BarShopContext.custom_order_item
                        join c in BarShopContext.S2_Card on coi.card_seq equals c.Card_Seq into gj
                        where coi.order_seq == id
                        from coic in gj.DefaultIfEmpty()                        
                        select new CustomOrderItemViewModel
                        {
                            Id = coi.id,
                            CardSeq = coi.card_seq,
                            CardCode = coic == null ? "알수없음" : coic.Card_Code,
                            CardName = coic == null ? "알수없음" : coic.Card_Name,
                            ItemType = coi.item_type,
                            ItemCount = coi.item_count,
                            DiscountRate = coi.discount_rate,
                            ItemSalePrice = coi.item_sale_price,
                            ItemPrice = coi.item_price,
                            Memo = coi.memo1,
                            ItemTypeName = BarShopContext.manage_code.Where(m => m.code_type == "item_type" && m.code == coi.item_type).Select(m => m.code_value).FirstOrDefault()
                        };
                    model.CustomOrderItem = await queryCustomOrderItem.ToListAsync();

                    var queryCustomOrderPlist =
                        from cop in BarShopContext.custom_order_plist
                        join c in BarShopContext.S2_Card on cop.card_seq equals c.Card_Seq into gj
                        where cop.order_seq == id
                        from copc in gj.DefaultIfEmpty()
                        select new CustomOrderPlistViewModel
                        {
                            Id = cop.id,
                            CardSeq = cop.card_seq,
                            CardName = copc == null ? "알수없음" : copc.Card_Name,
                            EnvSpecialType = cop.EnvSpecialType,
                            IsBasic = cop.isBasic,
                            IsFPrint = cop.isFPrint,
                            IsNotPrint = cop.isNotPrint,
                            PrintCount = cop.print_count,
                            PrintType = cop.print_type,
                            Title = cop.title,
                            CardCode = copc == null ? "알수없음" : copc.Card_Code,
                            ImgFolder = cop.imgFolder,
                            ImgName = cop.imgName,
                            PrintTypeName = BarShopContext.manage_code.Where(m => m.code_type == "print_type" && m.code == cop.print_type).Select(m => m.code_value).FirstOrDefault(),
                            ItemType = queryCustomOrderItem.Where(m => m.CardSeq == cop.card_seq).Select(m => m.ItemType).FirstOrDefault()
                        };
                    model.CustomOrderPlist = await queryCustomOrderPlist.ToListAsync();

                    var queryDeliveryInfo =
                        from di in BarShopContext.DELIVERY_INFO
                        where di.ORDER_SEQ == id
                        select new DeliveryInfoViewModel
                        {
                            Id = di.ID,
                            OrderSeq = di.ORDER_SEQ,
                            DeliverySeq = di.DELIVERY_SEQ,
                            Name = di.NAME,
                            Addr = di.ADDR,
                            DeliveryMethod = di.DELIVERY_METHOD,
                            DeliveryMethodName = BarShopContext.manage_code.Where(m => m.code_type == "delivery_method" && m.code == di.DELIVERY_METHOD.ToString()).Select(m => m.code_value).FirstOrDefault(),
                            DeliveryInfoDetail = (
                                from did in BarShopContext.DELIVERY_INFO_DETAIL
                                where did.delivery_id == di.ID
                                select new DeliveryInfoDetailViewModel
                                {
                                    Id = did.ID,
                                    DeliveryId = did.delivery_id,
                                    ItemCount = did.item_count,
                                    ItemId = did.item_id,
                                    ItemTitle = did.item_title,
                                    ItemType = did.item_type,
                                    ItemTypeColor = did.item_type == "P" ? queryCustomOrderPlist.Where(m => m.Id == did.item_id).Select(m => m.ItemType).FirstOrDefault() :
                                        queryCustomOrderItem.Where(m => m.Id == did.item_id).Select(m => m.ItemType).FirstOrDefault()
                                }).ToList()
                        };

                    model.DeliveryInfo = await queryDeliveryInfo.ToListAsync();

                    var queryCustomOrderCopy =
                        from coc in BarShopContext.CUSTOM_ORDER_COPY
                        where coc.order_seq == id
                        select new CustomOrderCopyViewModel
                        {
                            OrderSeq = coc.order_seq,
                            DeliverySeq = coc.delivery_seq,
                            DeliveryMethod= coc.delivery_method,
                            EmboStr = coc.embo_str,
                            JebonStr = coc.jebon_str,
                            PrintStr = coc.print_str,
                            IsQuick = coc.isQuick,
                            IsRisk = coc.isRisk,
                            CustomOrderCopyDetail = (
                                from cocd in BarShopContext.CUSTOM_ORDER_COPY_DETAIL
                                where cocd.order_seq == id && cocd.delivery_seq == coc.delivery_seq
                                select new CustomOrderCopyDetailViewModel
                                {
                                    DeliverySeq = cocd.delivery_seq,
                                    Etc = cocd.etc1,
                                    Id = cocd.id,
                                    IsPitem = cocd.isPItem,
                                    ItemCode = cocd.item_code,
                                    ItemCount = cocd.item_count,
                                    ItemSeq = cocd.item_seq,
                                    ItemTitle = cocd.item_title,
                                    ItemType = cocd.item_type,
                                    OrderSeq = cocd.order_seq,
                                    ItemTypeName = BarShopContext.manage_code.Where(m => m.code_type == "item_type" && m.code == cocd.item_type).Select(m => m.code_value).FirstOrDefault()
                                }).ToList()                                               
                        };

                    model.CustomOrderCopy = await queryCustomOrderCopy.ToListAsync();

                    var customOrderHistory =
                        from cocd in BarShopContext.custom_order_history
                        where cocd.order_seq == id
                        orderby cocd.id
                        select cocd;

                    model.CustomOrderHistory = await customOrderHistory.ToListAsync();

                    ViewData["Title"] = string.Format("{0} 청첩장 주문 상세보기", model.OrderSeqPrefix);
                }
                else
                {
                    model = new CustomOrderDetailViewModel();
                }
            }
            else
            {
                model = new CustomOrderDetailViewModel();
            }
            
            return View(model);
        }
    }
}
