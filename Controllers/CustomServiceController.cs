using Barunson.BBarunsonWeb.Models;
using Barunson.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Barunson.BBarunsonWeb.Controllers
{
    public class CustomServiceController : Controller
    {
        private BarShopContext BarShopContext { get; set; }


        public CustomServiceController(BarShopContext barShopContext)
        {
            this.BarShopContext = barShopContext;
        }


        public async Task<IActionResult> Index(CustomServiceViewModel model)
        {
            var result = await GetOrderListAsync(model.StartDate, model.PChasu, model.Jong);
            model.Count = result.Count;
            model.DataModel = result.Items;

            return View(model);
        }



        private async Task<(int Count, List<CustomServiceSearchDataModel> Items)> GetOrderListAsync(DateTime startDate, string Chasu, string Jong)
        {
            //총 아이템 수
            int count = 0;

            List<CustomServiceSearchDataModel>? items = null;
            

            var orderQuery = from o in BarShopContext.CUSTOM_ORDER_CHASU
                             join c in BarShopContext.custom_order_plist on o.order_seq equals c.order_seq
                             where o.pdate.Equals(startDate.ToString("yyyy-MM-dd")) && o.pseq == Convert.ToInt32(Chasu) && c.print_type.Equals(Jong)  && c.imgFolder != null
                             //where o.pdate.Equals(startDate.ToString("yyyy-MM-dd")) &&  o.pseq == 1 && c.print_type == "C" // && !string.IsNullOrEmpty(c.imgFolder)
                             select new CustomServiceSearchDataModel
                             {
                                 OSeq = (int)o.oseq,
                                 Id = (int)c.id,
                                 OrderSeq = o.order_seq,
                                 PrintType = c.print_type,
                                 Title = c.title,
                                 ImgFolder = c.imgFolder,
                                 imgName = c.imgName
                             };
            orderQuery = orderQuery.OrderBy(m => m.OSeq).ThenBy(m => m.PrintType);

            items = await orderQuery.ToListAsync();
            count = items.Count;
            return (count, items);
        }
    }
}
