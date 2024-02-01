using Syntaq.Falcon.Forms.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Forms
{
	public class CreateOrEditFormModalViewModel
	{
		public CreateOrEditFormDto Form { get; set; }
		public List<GetFormForView> VersionHistory { get; set; }
		public bool IsEditMode => Form.Id.HasValue;
		public string Type { get; set; }
	}
}