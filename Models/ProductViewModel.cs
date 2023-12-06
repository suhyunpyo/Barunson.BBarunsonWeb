namespace Barunson.BBarunsonWeb.Models
{
	
	public class ProductImageViewModel
	{
		public string ImageUrl { get; set; }
	}

	public class ProductChoanViewModel
	{
		public string ChoanTitle { get; set; }

		public string ChoanImageUrl { get; set; }

		public DateTime ChoanConformDate { get; set; }

        public List<ProductPlistViewModel> ProductPlistViewModel { set; get; }

    }
    public class ProductPlistViewModel
	{
		public string Title { get; set; }

		public int Status { get; set; }

		public DateTime RegDate { get; set; }


	}

}

