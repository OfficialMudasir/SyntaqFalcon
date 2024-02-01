using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Users.Dtos
{
    public class GetUserAcceptanceForEditOutput
    {
		public CreateOrEditUserAcceptanceDto UserAcceptance { get; set; }

		public string UserAcceptanceTypeName { get; set;}

		public string UserName { get; set;}

		public string RecordMatterContributorName { get; set;}


    }
}