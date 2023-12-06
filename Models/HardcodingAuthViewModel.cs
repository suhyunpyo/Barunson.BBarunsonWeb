using Barunson.DbContext.DbModels.BarShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barunson.BBarunsonWeb.Models
{
	public class HardcodingAuthViewModel
	{
		public List<AuthType> AuthTypes = new List<AuthType> {
			new AuthType("AUTH_TRUBLE", "사고건수정권한"),
            new AuthType("AUTH_CLOSECOPY", "마감건처리권한"),
            new AuthType("AUTH_OUTSOURCING_DEL", "외주인쇄삭제권한")
        };
		public AuthType AuthType { get; set; }
		public List<ADMIN_LST> HardcodingAdminLsts { get; set; } = new List<ADMIN_LST>();
		public List<ADMIN_LST> AdminLsts { get; set; } = new List<ADMIN_LST>();
	}

	public class AuthType
	{
		public AuthType(string t, string n)
		{
			this.Type = t;
			this.Name = n;
		}
		public string Type { get; set; }
		public string Name { get; set; }
	}
}
