using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Users.Dtos
{
    public class GetAllUserAcceptanceTypesForExcelInput
    {
		public string Filter { get; set; }

		public string NameFilter { get; set; }

		public int ActiveFilter { get; set; }


		 public string TemplateNameFilter { get; set; }

		 
    }
}