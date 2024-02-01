using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Tenants.Dashboard.Dto
{
	public class DashboardRecentSubmissions
	{
		public Guid Id { get; set; }
		public string Display { get; set; }
		public string Status { get; set; }
		public DateTime Time { get; set; }
	}
}
