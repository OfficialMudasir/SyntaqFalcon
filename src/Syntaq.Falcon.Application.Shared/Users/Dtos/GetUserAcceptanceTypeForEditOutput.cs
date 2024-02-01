using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Users.Dtos
{
    public class GetUserAcceptanceTypeForEditOutput
    {
		public CreateOrEditUserAcceptanceTypeDto UserAcceptanceType { get; set; }

		public string TemplateName { get; set;}


    }
}