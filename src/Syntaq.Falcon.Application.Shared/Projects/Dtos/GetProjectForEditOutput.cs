using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectForEditOutput
    {
		public CreateOrEditProjectDto Project { get; set; }

		public string RecordRecordName { get; set;}


    }
}