using Barunson.BBarunsonWeb.Models;
using Barunson.DbContext;
using Barunson.DbContext.DbModels.MoSvr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Runtime.CompilerServices;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class CustomQnAController : BaseController
    {
        //private BarShopContext BarShopContext 


        public CustomQnAController(BarShopContext barShopContext)
              : base(barShopContext)
        {

        }

        public async Task<IActionResult> Index(CustomQnaViewModel model)
        {
            if (model.ProductBrands == null || model.ProductBrands.Count == 0)
            {
                model.ProductBrands = BrandList;

            }
            if (model.AdminLists == null || model.AdminLists.Count == 0)
            {
                model.AdminLists = (await GetSelectAdminListsAsync("")).ToList();
                model.AdminLists.ForEach(m => m.Selected = true);
            }

            var result = await GetCustomQnaListAsync(model.KeyWord, model.ProductBrands.Where(m => m.Selected).Select(m => m.Value).ToList(),
                                                    model.Status, model.AdminList, model.StartDate, model.EndDate, model.SearchCase, "", model.PageFrom, model.PageSize);

            model.Count = result.Count;
            model.DataModel = result.Items;

            model.RouteAction = "Index";
            model.RouteController = "CustomQnA";

            return View(model);
        }

        public async Task<(int Count, List<CustomQnaDataModel> Items)> GetCustomQnaListAsync(string keyword, List<string> ProductBrands, string Status, string AdminList, DateTime startDate, DateTime endDate,
            string SearchCase, string OneInquiry, int pageFrom = 0, int? pageSize = null)
        {

            //End Date는 날짜 시간으로 하루 더하여 조건 검색
            endDate = endDate.AddDays(1);
            //총 아이템 수
            int count = 0;
            AdminList = !string.IsNullOrEmpty(AdminList) ? AdminList.Split(":")[1] : "";

            List<CustomQnaDataModel> items = null;

            var QnaQuery = from qna in _barshopDb.view_UsrQnA
                           where qna.A_stat != "S4" && ProductBrands.Contains(qna.sales_gubun) &&
                                (Status == "3" && qna.a_dt >= startDate && qna.a_dt < endDate && qna.A_stat.Equals("S3")) ||
                                (Status == "2" && qna.reg_dt >= startDate && qna.reg_dt < endDate && (qna.A_stat.Equals("S1") || qna.A_stat.Equals("S2"))) ||
                                (Status == "1" && qna.reg_dt >= startDate && qna.reg_dt < endDate && qna.A_stat.Equals("S" + Status)) ||
                                (Status == "0" && qna.reg_dt >= startDate && qna.reg_dt < endDate) &&
                                (!string.IsNullOrEmpty(AdminList) && qna.a_id == AdminList) &&
                                (!string.IsNullOrEmpty(keyword) && SearchCase.Equals(0) && (qna.Q_title.Contains(keyword) || qna.Q_content.Contains(keyword))) ||
                                (!string.IsNullOrEmpty(keyword) && SearchCase.Equals(1) && qna.member_id.Contains(keyword)) ||
                                (!string.IsNullOrEmpty(keyword) && SearchCase.Equals(2) && qna.member_name.Contains(keyword)) ||
                                //Replace(tel_no,'-','') like '%" & Replace(Me.txtSrchKey.Text, "-", "") & "%'"
                                (!string.IsNullOrEmpty(keyword) && SearchCase.Equals(3) && (qna.Q_title.Contains(keyword) || qna.Q_content.Contains(keyword))) ||
                                (!string.IsNullOrEmpty(keyword) && SearchCase.Equals(4) && qna.e_mail.Contains(keyword)) ||
                                (!string.IsNullOrEmpty(keyword) && SearchCase.Equals(5) && qna.order_seq == Convert.ToInt32(keyword))
                           select new CustomQnaDataModel
                           {
                               IsS2 = qna.isS2,
                               Qa_Iid = qna.qa_iid,
                               Sales_Gubun = qna.sales_gubun,
                               Company_Seq = qna.company_seq,
                               Company_Name = qna.company_name,
                               Order_Seq = qna.order_seq,
                               Member_Id = qna.member_id,
                               Member_Name = qna.member_name,
                               E_Mail = qna.e_mail,
                               Tel_No = qna.tel_no,
                               Q_Kind = qna.Q_kind,
                               Q_Title = qna.Q_title,
                               A_Stat = qna.A_stat,
                               Reg_Dt = qna.reg_dt,
                               A_Dt = (DateTime)qna.a_dt,
                               A_Id = qna.a_id,
                               A_Research1 = qna.a_research1,
                               A_Research2 = qna.a_research2,

                               card_Code = qna.CARD_CODE

                           };
            QnaQuery = QnaQuery.OrderBy(m => m.Reg_Dt);

            if (pageSize.HasValue)
            {
                //총 아이템 수
                count = await QnaQuery.CountAsync();
                //페이지 수 만큼 데이터 읽기
                items = await QnaQuery.Skip(pageFrom).Take(pageSize.Value).ToListAsync();
            }
            else
            {
                items = await QnaQuery.ToListAsync();
                count = items.Count;
            }

            return (count, items);



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Order id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CustomQnaInfoAsync(int id, string IsS2)
        {
            CustomQnaDataModel Viewmodel = new CustomQnaDataModel();


           // if (Viewmodel.AdminMentCategorys_M == null || Viewmodel.AdminMentCategorys_M.Count == 0)
            //{
              //  Viewmodel.AdminMentCategorys_M = (await GetSelectAdminMentCategorysAsync("M")).ToList();
             //   Viewmodel.AdminMentCategorys_M.ForEach(m => m.Selected = true);
            //}

            //if (Viewmodel.AdminMentCategorys_S == null || Viewmodel.AdminMentCategorys_S.Count == 0)
           // {
             //   Viewmodel.AdminMentCategorys_S = (await GetSelectAdminMentCategorysAsync("S")).ToList();
              //  Viewmodel.AdminMentCategorys_S.ForEach(m => m.Selected = true);
            //}

            if (IsS2.Equals("0"))
            {
                var query = from s in _barshopDb.SQM_QA_TBL
                        where s.QA_IID == id
                        select new CustomQnaDataModel
                        {
                            Code = "",
                            IsS2 = IsS2,
                            Sales_Gubun = s.sales_gubun,
                            Qa_Iid = s.QA_IID,
                            Order_Seq = s.order_seq,
                            Member_Name = s.MEMBER_NAME,
                            Member_Id = s.MEMBER_ID,
                            E_Mail = s.E_MAIL,
                            Tel_No = s.TEL_NO,
                            Reg_Dt = s.REG_DT,
                            A_Stat = s.A_STAT,
                            Q_Title = s.Q_TITLE,
                            Q_Content = s.Q_CONTENT,
                            A_Content = s.A_CONTENT,
                            A_Dt = s.A_DT,
                            user_upfile1 = s.user_upfile1,
                            user_upfile2 = s.user_upfile2,
                            Q_Kind = ""
                        };
                var model = await query.FirstOrDefaultAsync();
                //model.AdminMentCategorys_M = (await GetSelectAdminMentCategorysAsync("M","" ));
                //model.AdminMentCategorys_S = (await GetSelectAdminMentCategorysAsync("S", ""));

                return View("QnaInfo", model);
            }
            else
            {
                var query = from s in _barshopDb.S2_UserQnA
                            join m1 in _barshopDb.manage_code on new { Code = s.q_kind, Code_Group = "admin_ment_Category_S"} equals new { Code = m1.code, Code_Group = m1.code_type} into sm1
                            from pb in sm1.DefaultIfEmpty()
                            join m2 in _barshopDb.manage_code on pb.parent_id equals m2.code_id into sm2
                            from pb2 in sm2.DefaultIfEmpty()
                            where s.qa_iid == id

                        select new CustomQnaDataModel
                        {
                            Code = pb2.code,
                            IsS2 = IsS2,
                            Sales_Gubun = s.sales_gubun,
                            Qa_Iid = s.qa_iid,
                            Company_Seq = s.company_seq,
                            Order_Seq = s.order_seq,
                            Member_Name = s.member_name,
                            Member_Id = s.member_id,
                            E_Mail = s.e_mail,
                            Tel_No = s.tel_no,
                            Reg_Dt = s.reg_dt,
                            A_Stat = s.a_stat,
                            Q_Title = s.q_title,
                            Q_Content = s.q_content,
                            A_Content = s.a_content,
                            A_Dt = s.a_dt,
                            user_upfile1 = s.user_upfile1,
                            user_upfile2 = s.user_upfile2,
                            Q_Kind= s.q_kind
                        };
                var model = await query.FirstOrDefaultAsync();
                model.AdminMentCategorys_M = (await GetSelectAdminMentCategorysAsync("M", model.Code));
                model.AdminMentCategory_M = model.Code;
                model.AdminMentCategorys_S = (await GetSelectAdminMentCategorysAsync("S", model.Code));
                model.AdminMentCategory_S = model.Q_Kind;

                return View("QnaInfo", model);
            }
          
        }




        public async Task<IActionResult> CateGory(string Code)
        {
            var items = await GetSelectAdminMentCategorysAsync("S", Code);
            return Json(items);
            

        }
    }
}
