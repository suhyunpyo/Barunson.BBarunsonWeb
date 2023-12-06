using Barunson.DbContext;
using Barunson.DbContext.DbModels.BarShop;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class BaseController : Controller
    {
      //  private BarShopContext BarShopContext { get; set; }
        protected readonly BarShopContext _barshopDb;

        public BaseController(BarShopContext barShopContext)
        {
            _barshopDb = barShopContext;
        }


        public List<SelectListItem> BrandList
        {
            get
            {
                return //new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Text = "비핸즈", Value = "SA" },
                        new SelectListItem { Text = "바른손", Value = "SB" },
                        new SelectListItem { Text = "더카드", Value = "ST" },
                        new SelectListItem { Text = "프리미어", Value = "SS" },
                        new SelectListItem { Text = "바른손몰(B)", Value = "B,C" },
                        new SelectListItem { Text = "바른손몰(H)", Value = "H" },
                        new SelectListItem { Text = "대리점", Value = "D" },
                        new SelectListItem { Text = "지역대리점", Value = "Q" },
                        new SelectListItem { Text = "아웃바운드", Value = "P" },
                        new SelectListItem { Text = "해외영업", Value = "SG" },
                        new SelectListItem { Text = "디얼디어", Value = "SD" },
                        new SelectListItem { Text = "모바일초대장", Value = "BM" },
                    };//, "Value", "Text");
            }
        }


        protected async Task<IEnumerable<SelectListItem>> GetSelectAdminListsAsync(string codeGroup, bool addAll = false, string allValue = "", string allText = "전체관리자")
        {
            var admins = await GetAdminListAsync(codeGroup);
            var items = admins.Select(m => new SelectListItem { Text = m.ADMIN_NAME + ": " + m.ADMIN_ID, Value = m.ADMIN_NAME + ": " + m.ADMIN_ID }).ToList();
            //if (addAll)
                items.Insert(0, new SelectListItem { Text = allText, Value = allValue });

            return new SelectList(items, "Value", "Text");
        }

        protected async Task<IEnumerable<SelectListItem>> GetSelectAdminMentCategorysAsync(string codeGroup, string? codeM, bool addAll = false, string allValue = "", string allText = "전체관리자")
        {
            var admins = await GetAdminMentCategoryAsync(codeGroup, codeM);
            var items = admins.Select(m => new SelectListItem { Text = m.code_value, Value = m.code /* Value = m.code + "_" + m.code_value + "_" + m.use_yorn + "_" + m.etc1*/, Disabled = m.code.Equals(codeM) ? true : false, Selected = m.code.Equals(codeM) ? true : false }).ToList();
            
            //if (addAll)
            //  items.Insert(0, new SelectListItem { Text = allText, Value = allValue });
           items.ForEach(m => m.Selected = true);

            return new SelectList(items, "Value", "Text");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeGroup"></param>
        /// <returns></returns>
        protected async Task<List<ADMIN_LST>> GetAdminListAsync(string codeGroup)
        {
            // "select top 1 clsControl_Use from BBarunson_Config order by id desc"
            var query = (from r in _barshopDb.BBarunson_Config
                        orderby r.id descending
                        select new  { clsControl_Use = r.clsControl_Use}).Take(1);
            var orderItems = await query.ToListAsync();

            if (orderItems[0].clsControl_Use.Equals("Y"))
            {
                //"select admin_name,admin_id from ADMIN_LST where company_seq='1' and NState='1' and cms_id <> '' and not cms_id is null"
               var query2 = from r in _barshopDb.ADMIN_LST
                             where r.COMPANY_SEQ == 1 && r.NState.Equals("1") && r.CMS_ID != "" && r.CMS_ID != null
                             select new ADMIN_LST { ADMIN_NAME =  r.ADMIN_NAME, ADMIN_ID = r.ADMIN_ID };

                return await query2.ToListAsync();
            }
            else
            {
                // "select id, admin_id, admin_name from ADMIN_LST where isCS = '1' and  NState = '1'"
                var query2 = from r in _barshopDb.ADMIN_LST
                             where r.NState.Equals("1") && r.isCS.Equals("1")
                             select new ADMIN_LST  { id= r.id, ADMIN_ID = r.ADMIN_ID, ADMIN_NAME=r.ADMIN_NAME };

                return await query2.ToListAsync();

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeGroup"></param>
        /// <returns></returns>
        protected async Task<IEnumerable<manage_code>> GetAdminMentCategoryAsync(string codeGroup, string? Code)
        {
        
            if (codeGroup == "M") //카테고리 대분류 반환
            {
                var query = (from r in _barshopDb.manage_code
                             where r.code_type.Equals("admin_ment_Category_M")//&&
                           // (!string.IsNullOrEmpty(Code) && r.code.Equals(Code))
                             orderby r.seq ascending
                             select new manage_code
                             {
                                 code = r.code,
                                 code_value = r.code_value,
                                 use_yorn = r.use_yorn,

                                 etc1 = r.etc1 == "csmemo_only" ? "Y" : "N"

                             });

                
                return await query.ToListAsync();
            }
            else if (codeGroup == "S") // 카테고리 소분류 반환
            {

                var query = (from r in _barshopDb.manage_code
                             join m in _barshopDb.manage_code on r.parent_id equals m.code_id
                             where r.code_type.Equals("admin_ment_Category_S") &&
                             (!string.IsNullOrEmpty(Code) && m.code.Equals(Code))
                             orderby m.code ascending, r.seq ascending
                             select new manage_code { code = r.code, code_value = r.code_value, use_yorn = r.use_yorn, etc1 = r.etc1 == "csmemo_only" ? "Y" : "N" });

                return await query.ToListAsync();
            }
            else
            {
                return null;
            }



        }


        //protected  Task<List<SelectListItem>> GetSelectBrandListAsync(string codeGroup,
        //   bool addAll = false, string allValue = "", string allText = "전체")
        //{
        //    // var codes = BrandList;
        //    // var items = BrandList;
        //    var items = new SelectList(
        //            new List<SelectListItem>
        //            {
        //                new SelectListItem { Text = "비핸즈", Value = "SA" },
        //                new SelectListItem { Text = "바른손", Value = "SB" },
        //                new SelectListItem { Text = "더카드", Value = "ST" },
        //                new SelectListItem { Text = "프리미어", Value = "SS" },
        //                new SelectListItem { Text = "바른손몰(B)", Value = "B,C" },
        //                new SelectListItem { Text = "바른손몰(H)", Value = "H" },
        //                new SelectListItem { Text = "대리점", Value = "D" },
        //                new SelectListItem { Text = "지역대리점", Value = "Q" },
        //                new SelectListItem { Text = "아웃바운드", Value = "P" },
        //                new SelectListItem { Text = "해외영업", Value = "SG" },
        //                new SelectListItem { Text = "디얼디어", Value = "SD" },
        //                new SelectListItem { Text = "모바일초대장", Value = "BM" },
        //            }, "Value", "Text").ToList();
        //    if (addAll)
        //        items.Insert(0, new SelectListItem { Text = allText, Value = allValue });

        //    return new SelectList(items, "Value", "Text");
        //}


    }
}
