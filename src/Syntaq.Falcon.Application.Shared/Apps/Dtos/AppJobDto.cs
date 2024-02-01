
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Apps.Dtos
{
    public class AppJobDto : EntityDto<Guid>
    {
		public string Name { get; set; }


		 public Guid AppId { get; set; }

		 
    }
}