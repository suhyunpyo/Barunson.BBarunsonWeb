using System.Text;
using System;

namespace Barunson.BBarunsonWeb.Models
{
    public class PaymentViewModel
    {
        public PaymentViewModel() { }

        public readonly Dictionary<string, string> cardMap = new Dictionary<string, string>()
        {
            {"3K", "기업 BC" },
            {"46", "광주은행" },
            {"71", "롯데카드" },
            {"30", "KDB산업은행" },
            {"31", "BC카드" },
            {"51", "삼성카드" },
            {"38", "새마을금고" },
            {"41", "신한카드" },
            {"62", "신협" },
            {"36", "씨티카드" },
            {"33", "우리BC카드(BC 매입)" },
            {"W1", "우리카드(우리 매입)" },
            {"37", "우체국예금보험" },
            {"39", "저축은행중앙회" },
            {"35", "전북은행" },
            {"42", "제주은행" },
            {"15", "카카오뱅크" },
            {"3A", "케이뱅크" },
            {"24", "토스뱅크" },
            {"21", "하나카드" },
            {"61", "현대카드" },
            {"11", "KB국민카드" },
            {"91", "NH농협카드" },
            {"34", "Sh수협은행" },
            {"6D", "다이너스 클럽" },
            {"6I", "디스커버" },
            {"4M", "마스터카드" },
            {"3C", "유니온페이" },
            {"7A", "아메리칸 익스프레스" },
            {"4J", "JCB" },
            {"4V", "VISA" }
        };

        public readonly Dictionary<string, string> bankMap = new Dictionary<string, string>()
        {
            { "39", "경남은행"},
            { "34", "광주은행"},
            { "S8", "교보증권"},
            { "12", "단위농협(지역농축협)"},
            { "SE", "대신증권"},
            { "SK", "메리츠증권"},
            { "S5", "미래에셋증권"},
            { "SM", "부국증권"},
            { "32", "부산은행"},
            { "S3", "삼성증권"},
            { "45", "새마을금고"},
            { "64", "산림조합"},
            { "SN", "신영증권"},
            { "S2", "신한금융투자"},
            { "88", "신한은행"},
            { "48", "신협"},
            { "27", "씨티은행"},
            { "20", "우리은행"},
            { "71", "우체국예금보험"},
            { "S0", "유안타증권"},
            { "SJ", "유진투자증권"},
            { "50", "저축은행중앙회"},
            { "37", "전북은행"},
            { "35", "제주은행"},
            { "90", "카카오뱅크"},
            { "SQ", "카카오페이증권"},
            { "89", "케이뱅크"},
            { "토스머니", "토스머니"},
            { "92", "토스뱅크"},
            { "ST", "토스증권"},
            { "SR", "펀드온라인코리아(한국포스증권)"},
            { "SH", "하나금융투자"},
            { "81", "하나은행"},
            { "S9", "하이투자증권"},
            { "S6", "한국투자증권"},
            { "SG", "한화투자증권"},
            { "SA", "현대차증권"},
            { "54", "홍콩상하이은행"},
            { "SI", "DB금융투자"},
            { "31", "DGB대구은행"},
            { "03", "IBK기업은행"},
            { "06", "KB국민은행"},
            { "S4", "KB증권"},
            { "02", "KDB산업은행"},
            { "SP", "KTB투자증권(다올투자증권)"},
            { "SO", "LIG투자증권"},
            { "11", "NH농협은행"},
            { "SL", "NH투자증권"},
            { "23", "SC제일은행"},
            { "07", "Sh수협은행"},
            { "SD", "SK증권"}
        };

        public string jsonStr { get; set; }
        public string jsonStrBeauty { get; set; }
        public string selectSql { get; set; }
        public string updateSql { get; set; }
        public int totalCancelAmount { get; set; } = 0;

        public Payment? payment { get; set; }
    }

    public class Payment
    {
	    public string mId { get; set; }
        public string lastTransactionKey { get; set; }
        public string paymentKey { get; set; }
        public string orderId { get; set; }
        public string orderName { get; set; }
        public int taxExemptionAmount { get; set; }
        public string status { get; set; }
        public string requestedAt { get; set; }
        public string approvedAt { get; set; }
        public bool useEscrow { get; set; }
        public bool cultureExpense { get; set; }
        public Card card { get; set; }
        public VirtualAccount virtualAccount { get; set; }
        public Transfer transfer { get; set; }
        public MobilePhone mobilePhone { get; set; }
        public GiftCertificate giftCertificate { get; set; }
        public CashReceipt cashReceipt { get; set; }
        public CashReceipts[] cashReceipts { get; set; }
        public Discount discount { get; set; }
        public Cancels[] cancels { get; set; }
        public string secret { get; set; }
        public string type { get; set; }
        public EasyPay easyPay { get; set; }
        public string country { get; set; }
        public string failure { get; set; }
        public bool isPartialCancelable { get; set; }
        public Url receipt { get; set; }
        public Url checkout { get; set; }
        public string currency { get; set; }
        public int totalAmount { get; set; }
        public int balanceAmount { get; set; }
        public int suppliedAmount { get; set; }
        public int vat { get; set; }
        public int taxFreeAmount { get; set; }
        public string method { get; set; }
        public string version { get; set; }

        public string statusStr { 
            get {
                string str = string.Empty;
                switch(status)
                {
                    case "READY":
                        str = "결제생성";
                        break;
                    case "IN_PROGRESS":
                        str = "결제진행중";
                        break;
                    case "WAITING_FOR_DEPOSIT":
                        str = "입금대기";
                        break;
                    case "DONE":
                        str = "결제완료";
                        break;
                    case "CANCELED":
                        str = "결제취소";
                        break;
                    case "PARTIAL_CANCELED":
                        str = "부분취소";
                        break;
                    case "ABORTED":
                        str = "승인실패";
                        break;
                    case "EXPIRED":
                        str = "만료(거래취소)";
                        break;
                    default:
                        str = "알수없음";
                        break;
                }
                return str;
            } 
        }
    }

    public class Card
    {
        public string issuerCode { get; set; }
        public string issuerCodeName { get; set; }
        public string acquirerCode { get; set; }
        public string number { get; set; }
        public int installmentPlanMonths{ get; set; }
        public bool isInterestFree { get; set; }
        public string interestPayer { get; set; }
        public string approveNo { get; set; }
        public bool useCardPoint { get; set; }
        public string cardType { get; set; }
        public string ownerType { get; set; }
        public string acquireStatus { get; set; }
        public int amount { get; set; }

    }

    public class Url
    {
        public string url { get; set; }
    }

    public class EasyPay
    {
        public string provider { get; set; }
        public int amount { get; set; }
        public int discountAmount { get; set; }
    }

    public class CashReceipt
    {
        public string type { get; set; }
        public string receiptKey { get; set; }
        public string issueNumber { get; set; }
        public string receiptUrl { get; set; }
        public int amount { get; set; }
        public int taxFreeAmount { get; set; }
    }

    public class CashReceipts
    {
        public string receiptKey { get; set; }
        public string orderId { get; set; }
        public string orderName { get; set; }
        public string type { get; set; }
        public string issueNumber { get; set; }
        public string receiptUrl { get; set; }
        public string businessNumber { get; set; }
        public string transactionType { get; set; }
        public int amount { get; set; }
        public int taxFreeAmount { get; set; }
        public string issueStatus { get; set; }
        public Failure failure { get; set; }
        public string customerIdentityNumber { get; set; }
        public string requestedAt { get; set; }
    }

    public class Failure
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class VirtualAccount
    {
        public string accountType { get; set; }
        public string accountNumber { get; set; }
        public string bankCode { get; set; }
        public string customerName { get; set; }
        public string dueDate { get; set; }
        public string refundStatus { get; set; }
        public bool expired { get; set; }
        public string settlementStatus { get; set; }
        public string refundReceiveAccount { get; set; }
    }

    public class MobilePhone
    {
        public string customerMobilePhone { get; set; }
        public string settlementStatus { get; set; }
        public string receiptUrl { get; set; }
    }

    public class GiftCertificate
    {
        public string approveNo { get; set; }
        public string settlementStatus { get; set; }
    }

    public class Transfer
    {
        public string bankCode { get; set; }
        public string settlementStatus { get; set; }
    }

    public class Cancels
    {
        public int cancelAmount { get; set; }
        public string cancelReason { get; set; }
        public int taxFreeAmount { get; set; }
        public int taxExemptionAmount { get; set; }
        public int refundableAmount { get; set; }
        public int easyPayDiscountAmount { get; set; }
        public string canceledAt { get; set; }
        public string transactionKey { get; set; }
        public string receiptKey { get; set; }
    }

    public class Discount
    {
        public int amount { get; set; }
    }
}
