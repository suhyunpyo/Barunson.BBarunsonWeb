using Barunson.BBarunsonWeb.Models;
using Barunson.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Runtime.CompilerServices;

namespace Barunson.BBarunsonWeb.Controllers
{
	public class ProductController : Controller
	{
		private BarShopContext BarShopContext { get; set; }

		public ProductController(BarShopContext barShopContext)
		{
			this.BarShopContext = barShopContext;
		}

		public async Task<IActionResult> ImgPreView(int CardSeq, string CardImg)
		{
			var model = new ProductImageViewModel();

			if (CardImg == null)
			{
				var query = from a in BarShopContext.S2_CardView
							where a.card_seq == CardSeq
							select a;

				var item = await query.FirstAsync();

				model = new ProductImageViewModel
				{
					ImageUrl = "https://file.barunsoncard.com/common_img/" + item.card_image

				};


			}
			else
			{
				model = new ProductImageViewModel
				{
					ImageUrl = CardImg

				};
			}

			return View("ImgPreView", model);
		}
		public async Task<IActionResult> ChoanManagement(int Pid)
		{
            var model = new ProductImageViewModel();

			var query = from a in BarShopContext.custom_order_plist
						where a.id == Pid
						select a;

			var item = await query.FirstAsync();




            return View();

		}

	}
}
