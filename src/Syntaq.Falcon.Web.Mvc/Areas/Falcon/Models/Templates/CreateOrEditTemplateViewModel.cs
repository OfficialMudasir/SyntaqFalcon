using Syntaq.Falcon.Documents.Dtos;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Templates
{
	public class CreateOrEditTemplateModalViewModel
	{
		public CreateOrEditTemplateDto Template { get; set; }
		public List<GetTemplateForView> VersionHistory { get; set; }

		public bool IsEditMode { get; set; }
 
    }
}