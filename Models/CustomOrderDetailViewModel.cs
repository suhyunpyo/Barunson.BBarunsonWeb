namespace Barunson.BBarunsonWeb.Models
{
    public class CustomOrderDetailViewModel
    {
        public int OrderSeq { get; set; }
        public int? UpOrderSeq { get; set; }
        public string? OrderType { get; set; }
        public string? OrderTypeName { get; set; }
        public string? SalesGubun { get; set; }
        public string? SalesGubunName { get; set; }
        public string? PayType { get; set; }
        public int? CompanySeq { get; set; }
        public string? CompanyName { get; set; }
        public int StatusSeq { get; set; }
        public string? StatusSeqName { get; set; }
        public byte? SettleStatus { get; set; }
        public string SettleStatusName
        {
            get
            {
                if (SettleStatus == 0)
                {
                    return "미결제";
                }
                else if (SettleStatus == 1)
                {
                    return "가상계좌입금전";
                }
                else if (SettleStatus == 2)
                {
                    return "결제완료";
                }
                else if (SettleStatus == 3 || SettleStatus == 5)
                {
                    return "결제취소";
                }
                else
                {
                    return "";
                }
            }
        }
        public string? MemberId { get; set; }
        public string? OrderName { get; set; }
        public int? CardSeq { get; set; }
        public string? CardCode { get; set; }
        public string? CardName { get; set; }
        public int? OrderCount { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? OrderPrice { get; set; }
        public int? OrderTotalPrice { get; set; }
        public int? LastTotalPrice { get; set; }
        public int? SettlePrice { get; set; }
        public int? EtcPrice { get; set; }
        public string? EtcPriceMent { get; set; }
        public int? DeliveryPrice { get; set; }
        public int? JebonPrice { get; set; }
        public int? StickerPrice { get; set; }
        public int? EnvPrice { get; set; }
        public int? GuestbookPrice { get; set; }
        public int? OptionPrice { get; set; }
        public int? ReducePrice { get; set; }
        public int? FticketPrice { get; set; }
        public int? PrintPrice { get; set; }
        public int? SasikPrice { get; set; }
        public int? EnvInsertPrice { get; set; }
        public int? PointPrice { get; set; }
        public int? AdditionReducePrice { get; set; }
        public int? EnvSpecialPrice { get; set; }
        public int? FlowerPrice { get; set; }
        public int? SealingStickerPrice { get; set; }
        public int? RibbonPrice { get; set; }
        public int? PaperCoverPrice { get; set; }
        public int? Jebon2Price { get; set; }
        public int? PocketPrice { get; set; }
        public int? EnvPremiumPrice { get; set; }
        public int? MaskingTapePrice { get; set; }
        public string? IsSpecial { get; set; }
        public string? OrderAddFlag { get; set; }
        public string? inflow_route { get; set; }
        public string? inflow_route_settle { get; set; }

        public string? PgTid { get; set; }

        public List<CustomOrderItemViewModel>? CustomOrderItem { get; set; }
        public List<CustomOrderPlistViewModel>? CustomOrderPlist { get; set; }
        public List<DeliveryInfoViewModel>? DeliveryInfo { get; set; }
        public List<CustomOrderCopyViewModel>? CustomOrderCopy { get; set; }
        public List<Barunson.DbContext.DbModels.BarShop.custom_order_history>? CustomOrderHistory { get; set; }

        public string OrderSeqPrefix
        {
            get
            {
                if (UpOrderSeq.HasValue == false)
                {
                    if (IsSpecial == "1" || IsSpecial == "2")
                    {
                        return string.Format("특{0}", OrderSeq);
                    }
                    else
                    {
                        return string.Format("{0}", OrderSeq);
                    }
                }
                else
                {
                    if (PayType == "4")
                    {
                        return string.Format("사{0}", OrderSeq);
                    }
                    else if (IsSpecial == "1" || IsSpecial == "2")
                    {
                        return string.Format("특{0}", OrderSeq);
                    }
                    else
                    {
                        if (OrderAddFlag == "1")
                        {
                            return string.Format("수{0}", OrderSeq);
                        }
                        else
                        {
                            return string.Format("기{0}", OrderSeq);
                        }
                    }
                }
            }
        }
    }

    public class CustomOrderItemViewModel
    {
        public int Id { get; set; }
        public int CardSeq { get; set; }
        public string? ItemType { get; set; }
        public int? ItemCount { get; set; }
        public int? ItemPrice { get; set; }
        public double? ItemSalePrice { get; set; }
        public double? DiscountRate { get; set; }
        public string? Memo { get; set; }

        public string? CardCode { get; set; }
        public string? ItemTypeName { get; set; }
        public string? CardName { get; set; }
    }

    public class CustomOrderPlistViewModel
    {
        public long Id { get; set; }
        public string? IsFPrint { get; set; }
        public string? PrintType { get; set; }
        public string? ItemType { get; set; }
        public int CardSeq { get; set; }
        public string? Title { get; set; }
        public int? PrintCount { get; set; }
        public string? IsNotPrint { get; set; }
        public string? IsBasic { get; set; }
        public string? EnvSpecialType { get; set; }

        public string? CardCode { get; set; }
        public string? PrintTypeName { get; set; }
        public string? CardName { get; set; }

        public string? ImgFolder { get; set; }
        public string? ImgName { get; set; }
    }

    public class DeliveryInfoViewModel
    {
        public int Id { get; set; }
        public int OrderSeq { get; set; }
        public int DeliverySeq { get; set; }
        public string? Name { get; set; }
        public string? Addr { get; set; }
        public int? DeliveryMethod { get; set; }
        public string? DeliveryMethodName { get; set; }
        public List<DeliveryInfoDetailViewModel>? DeliveryInfoDetail { get; set; }
    }

    public class DeliveryInfoDetailViewModel
    {
        public int Id { get; set; }
        public int DeliveryId { get; set; }
        public string? ItemType { get; set; }
        public string? ItemTitle { get; set; }
        public int ItemId { get; set; }
        public int ItemCount { get; set; }
        public string? ItemTypeColor { get; set; }
    }

    public class CustomOrderCopyViewModel
    {
        public int OrderSeq { get; set; }
        public byte DeliverySeq { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? JebonStr { get; set; }
        public string? EmboStr { get; set; }
        public string? PrintStr { get; set; }
        public string? IsRisk { get; set; }
        public string? IsQuick { get; set; }

        public List<CustomOrderCopyDetailViewModel>? CustomOrderCopyDetail { get; set; }
    }

    public class CustomOrderCopyDetailViewModel
    {
        public long Id { get; set; }
        public int OrderSeq { get; set; }
        public byte DeliverySeq { get; set; }
        public string? ItemType { get; set; }
        public string? ItemTypeName { get; set; }
        public Int16 ItemSeq { get; set; }
        public string? IsPitem { get; set; }
        public string? ItemTitle { get; set; }
        public string? ItemCode { get; set; }
        public Int16 ItemCount { get; set; }
        public string? Etc { get; set; }
    }
}
