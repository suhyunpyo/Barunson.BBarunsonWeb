using Barunson.DbContext.DbModels.BarShop;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barunson.BBarunsonWeb.Models
{
	public class HardcodingInitialViewModel
	{
		public List<HardcodingInitialModel> HardcodingList { get; set; } = new List<HardcodingInitialModel>();

		public HardcodingInitialSearchParam Param { get; set; } = new HardcodingInitialSearchParam();
	}

    [Keyless]
    public class HardcodingInitialModel {
		public int CardSeq { get; set; }

		public string? CardCode { get; set; }

		public string? PrintMethod { get; set; }

		public string? PmValue { get; set; }

		public string? Pm { get; set; }

		public string? Pm1 { get; set; }

		public string? Pm2 { get; set; }

		public string? SdUse { get; set; }

		public string? HgUse { get; set; }

		public string? SgUse { get; set; }

		public string? Barunson { set; get; }

		public string? Mall { set; get; }

		public string? Premier { set; get; }
	}

	public class HardcodingInitialSearchParam
	{
		//public int? Jumun { set; get; } = 1;

		//public int? Display { set; get; } = 1;

		public string CompanySeq { set; get; } = "5000,5001,5003";

		public string? SdYn { set; get; }

		public string? HgYn { set; get; }

		public string? SgYn { set; get; }
	}

	public class HardcodingInitialParam
	{
		public string CardCode { set; get; }

		public string Factory { set; get; }
	}
}
