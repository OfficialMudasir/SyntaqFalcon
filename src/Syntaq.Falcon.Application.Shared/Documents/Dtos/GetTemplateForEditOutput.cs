using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Documents.Dtos
{
	public class GetTemplateForEditOutput
	{
		public CreateOrEditTemplateDto Template { get; set; }
	}
}