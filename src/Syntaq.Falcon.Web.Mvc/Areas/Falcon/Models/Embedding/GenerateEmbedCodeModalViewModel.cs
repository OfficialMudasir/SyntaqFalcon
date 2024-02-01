using Syntaq.Falcon.Forms.Dtos;
using System;
using System.Collections.Generic;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Embedding
{
	public class GenerateEmbedCodeModalViewModel
	{
		public Guid? FormId { get; set; }

		public string Type { get; set; }

		public List<GetFormForView> VersionHistory { get; set; }

		public string TenancyName { get; set; }
	}
}
