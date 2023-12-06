using Barunson.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Barunson.BBarunsonWeb.Models;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Security.Cryptography;
using Barunson.BBarunsonWeb.Config;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class UtilController : Controller
    {
        private readonly BarShopContext _barShopContext;
        private readonly List<PgMertInfo> _pgMertInfos;
        private readonly IHttpClientFactory _clientFactory;
        private readonly Uri _url;

        public UtilController(BarShopContext barShopContext, List<PgMertInfo> pgMertInfos, IHttpClientFactory httpClientFactory)
        {
            _barShopContext = barShopContext;
            _pgMertInfos = pgMertInfos;
            _clientFactory = httpClientFactory;

            _url = new Uri("https://api.tosspayments.com/");
        }

        public IActionResult TossPayment()
        {
            return View();
        }

        public async Task<IActionResult> TossPaymentDetail(string tid, string? type = null)
        {
            ViewBag.Layout = type;

            PaymentViewModel model = new PaymentViewModel();

            var query = from ord in _barShopContext.custom_order
                        where ord.pg_tid == tid
                        select new
                        {
                            ord.pg_shopid
                        };

            var values = await query.FirstOrDefaultAsync();

            string pgShopId = "";
            if (values != null)
            {
                pgShopId = values.pg_shopid;
            }
            string jsonStr = "";

            string[] arrShipId =
            {
                "bhands",
                "bhands_b",
                "bhands_bm",
                "bhands_c",
                "bhands_cm",
                "bhands_b2b",
                "bhands_samsung"
            };

            try
            {
                if(pgShopId == null || pgShopId == "barunson2" || pgShopId == "")
                {
                    foreach(string shopId in arrShipId)
                    {
                        jsonStr = await GetPaymentJson(shopId, tid);

                        if(jsonStr.Length > 0)  break;
                    }
                }
                else
                {
                    jsonStr = await GetPaymentJson(pgShopId, tid);
                }

                if(jsonStr.Length == 0)
                {
                    return View(model);
                }

                model.jsonStr = jsonStr;

                JsonDocument parsedJson = JsonDocument.Parse(jsonStr);
                string prettyJson = JsonSerializer.Serialize(parsedJson.RootElement, new JsonSerializerOptions { 
                    WriteIndented = true, 
                    AllowTrailingCommas = true, 
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                model.jsonStrBeauty = prettyJson;

                model.payment = JsonSerializer.Deserialize<Payment>(jsonStr);

                if(model.payment?.cancels!=null)
                {
                    Cancels[] cancels = model.payment.cancels;

                    foreach(Cancels c in cancels)
                    {
                        model.totalCancelAmount += c.cancelAmount;
                    }
                }

                StringBuilder sb = new StringBuilder();

                model.selectSql = $"select order_seq, settle_price, settle_status, settle_method, settle_date, pg_shopid, pg_tid, dacom_tid, card_installmonth, card_nointyn, pg_receipt_tid, pg_resultinfo, pg_resultinfo2 from custom_order  where order_seq = {tid.Substring(2)}";

                if(model.payment?.totalAmount > model.totalCancelAmount)
                {
                    if (model.payment.method == "카드")
                    {
                        sb.Append( " update custom_order set ");
                        sb.Append( " settle_status = 2, ");
                        sb.Append($" settle_price = {model.payment.totalAmount}, ");
                        sb.Append( " settle_method = 2, ");
                        sb.Append($" settle_date = convert(datetime,'{Convert.ToDateTime(model.payment.approvedAt).ToString("yyyy-MM-dd HH:mm:ss", null)}'), ");
                        sb.Append($" pg_shopid = '{model.payment.mId}', ");
                        sb.Append($" dacom_tid = '{model.payment.paymentKey}', ");
                        sb.Append($" card_installmonth = '{model.payment.card.installmentPlanMonths}', ");
                        sb.Append($" card_nointyn = '{(model.payment.card.installmentPlanMonths > 0 ? "1" : "0")}', ");
                        sb.Append($" pg_resultinfo = '{model.cardMap.GetValueOrDefault(model.payment.card.issuerCode)} {model.payment.card.approveNo}', ");
                        sb.Append( " pg_resultinfo2 = '', ");
                        sb.Append($" receiptUrl = '{model.payment.receipt.url}'");
                        sb.Append($" where pg_tid = '{model.payment.orderId}'");

                        model.updateSql = sb.ToString();
                    }
                    else if (model.payment.method == "계좌이체")
                    {
                        sb.Append( " update custom_order set ");
                        sb.Append( " settle_status = 2, ");
                        sb.Append($" settle_price = {model.payment.totalAmount}, ");
                        sb.Append( " settle_method = 1, ");
                        sb.Append($" settle_date = convert(datetime,'{Convert.ToDateTime(model.payment.approvedAt).ToString("yyyy-MM-dd HH:mm:ss", null)}'), ");
                        sb.Append($" pg_shopid = '{model.payment.mId}', ");
                        sb.Append($" dacom_tid = '{model.payment.paymentKey}', ");
                        sb.Append( " card_installmonth = '', ");
                        sb.Append( " card_nointyn = '0', ");
                        sb.Append($" pg_resultinfo = '{model.bankMap.GetValueOrDefault(model.payment.transfer.bankCode)}', ");
                        sb.Append( " pg_resultinfo2 = '', ");
                        sb.Append($" receiptUrl='{(model.payment.cashReceipt != null ? model.payment.cashReceipt.receiptUrl : model.payment.receipt.url)}' ");
                        sb.Append($" where pg_tid = '{model.payment.orderId}'");

                        model.updateSql = sb.ToString ();
                    }
                    else if (model.payment.method == "가상계좌")
                    {
                        sb.Append( " update custom_order set ");
                        sb.Append( " settle_status = 2, ");
                        sb.Append($" settle_price = {model.payment.totalAmount}, ");
                        sb.Append( " settle_method = 3, ");
                        sb.Append($" settle_date = convert(datetime,'{Convert.ToDateTime(model.payment.approvedAt).ToString("yyyy-MM-dd HH:mm:ss", null)}'), ");
                        sb.Append($" pg_shopid = '{model.payment.mId}', ");
                        sb.Append($" dacom_tid = '{model.payment.paymentKey}', ");
                        sb.Append( " card_installmonth = '', ");
                        sb.Append( " card_nointyn = '0', ");
                        sb.Append($" pg_resultinfo='{model.bankMap.GetValueOrDefault(model.payment.virtualAccount.bankCode)} {model.payment.virtualAccount.accountNumber}',");
                        sb.Append( " pg_resultinfo2 = '', ");
                        sb.Append($" receiptUrl='{(model.payment.cashReceipt != null ? model.payment.cashReceipt.receiptUrl : model.payment.receipt.url)}' ");
                        sb.Append($" where pg_tid = '{model.payment.orderId}'");

                        model.updateSql = sb.ToString();
                    }
                    else if (model.payment.method == "간편결제")
                    {
                        sb.Append( " update custom_order set ");
                        sb.Append( " settle_status = 2, ");
                        sb.Append($" settle_price = {model.payment.totalAmount}, ");
                        sb.Append( " settle_method = 9, ");
                        sb.Append($" settle_date = convert(datetime,'{Convert.ToDateTime(model.payment.approvedAt).ToString("yyyy-MM-dd HH:mm:ss", null)}'), ");
                        sb.Append($" pg_shopid = '{model.payment.mId}', ");
                        sb.Append($" dacom_tid = '{model.payment.paymentKey}', ");
                        sb.Append( " card_installmonth = '', ");
                        sb.Append( " card_nointyn = '0', ");
                        sb.Append($" pg_resultinfo='{model.payment.method} {model.payment.easyPay.provider}',");
                        sb.Append( " pg_resultinfo2 = '', ");
                        sb.Append($" receiptUrl='{model.payment.receipt.url}' ");
                        sb.Append($" where pg_tid = '{model.payment.orderId}'");

                        model.updateSql = sb.ToString();
                    }
                }
            }
            catch(Exception e)
            {
                string msg = e.Message;
            }

            return View(model);
        }

        private async Task<string> GetPaymentJson(string shopId, string tId)
        {
            var httpClient = _clientFactory.CreateClient();

            string jsonStr = "";
            try
            {
                var pgInfo = _pgMertInfos.FirstOrDefault(m => m.Id == shopId);
                if (pgInfo == null)
                {
                    return jsonStr;
                }
                var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.ASCII.GetBytes(pgInfo.SecretKey + ":"));

                var apiUri = new Uri(_url, $"/v1/payments/orders/{tId}");

                string responseText = string.Empty;

                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Get;
                    request.RequestUri = apiUri;
                    //인증헤더는 아래와 같이...
                    request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);

                    var response = await httpClient.SendAsync(request);
                    //웹 호출시 예외가 발생하면 에러를 발생 함.
                    response.EnsureSuccessStatusCode();

                    jsonStr = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e) { }

            return jsonStr;
        }
    }
}
