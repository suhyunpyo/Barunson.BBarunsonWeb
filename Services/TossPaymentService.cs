using Barunson.BBarunsonWeb.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;


namespace Barunson.BBarunsonWeb.Services
{

    public interface ITossPaymentService
    {
        
        Task<TossPayment> CancelAsync(string toss_sk_value, string pgshopid, TossPostPaymentCancel postData, string IdempotencyKey = null);


        /// <summary>
        /// Toss가맹점 Key조회(조회할 상점아이디 / 조회할 key 명 ("MertKey","ClientKey","SecretKey))
        /// </summary>
        /// <param name="strshopid">조회할 상점아이디</param>
        /// <param name="strfindkeyname">조회할 key 명 ("MertKey","ClientKey","SecretKey)</param>
        /// <returns></returns>
        Task<TossPayment> GetTossShopKeyValueAsync(string strshopid, string strfindkeyname);

    }
    public class TossPaymentService : ITossPaymentService
    {
      //  private readonly PgMertInfo _pgMertInfo;
        private readonly Uri _toss_url;
        private readonly Uri _api_url;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment hostingEnv;
        public TossPaymentService(/*List<PgMertInfo> pgMertInfos,*/ IWebHostEnvironment _hostingEnv, IHttpClientFactory httpClientFactory)
        {
            this.hostingEnv = _hostingEnv;
            _toss_url = new Uri("https://api.tosspayments.com");
            _api_url = new Uri("https://privateapi.barunsoncard.com");
            _httpClientFactory = httpClientFactory;
           // _pgMertInfo = pgMertInfos.FirstOrDefault(m => m.Id == "barunsonmcard");
        }

        /// <summary>
        /// API 호출 시 인증 헤더 값
        /// </summary>
        private string Base64EncodedAuthenticationString(string SecretKey)
        {
           // get
            //{
                //if (_pgMertInfo == null)
                //    return string.Empty;
                //else
                //{
                    //시크릿 키 뒤에 :을 추가하고 base64로 인코딩
                    var encData_byte = Encoding.ASCII.GetBytes(SecretKey + ":");
                    return Convert.ToBase64String(encData_byte);
                //}
            //}
        }


        public async Task<TossPayment> GetTossShopKeyValueAsync(string strshopid, string strfindkeyname)
        {
            string environment = hostingEnv.IsProduction() ? "Prod" : "Dev";
            var apiUri = new Uri(_api_url, $"/api/Mert/{environment}/{strshopid}");
            var httpClient = _httpClientFactory.CreateClient();
            TossPayment tossPayment = null;

            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = apiUri;

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var restr = await response.Content.ReadAsStringAsync();
                tossPayment = JsonSerializer.Deserialize<TossPayment>(restr);
            }

            return tossPayment;
        }



        /// <summary>
        /// 결제 취소
        /// 결제 취소에 성공했다면 Payment 객체의 cancels 필드에 취소 객체가 배열로 돌아옵니다.
        /// </summary>
        /// <param name="paymentKey"></param>
        /// <returns></returns>
        public async Task<TossPayment> CancelAsync(string toss_sk_value, string paymentKey, TossPostPaymentCancel postData, string IdempotencyKey = null)
        {
            var apiUri = new Uri(_toss_url, $"/v1/payments/{paymentKey}/cancel");
            var httpClient = _httpClientFactory.CreateClient();
            TossPayment tossPayment = null;

            var bodystr = JsonSerializer.Serialize(postData);
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = apiUri;
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Base64EncodedAuthenticationString(toss_sk_value));
                if (!string.IsNullOrEmpty(IdempotencyKey))
                    request.Headers.Add("Idempotency-Key", IdempotencyKey);

                request.Content = new StringContent(bodystr, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var restr = await response.Content.ReadAsStringAsync();
                tossPayment = JsonSerializer.Deserialize<TossPayment>(restr);
            }

            return tossPayment;
        }
    }
}
