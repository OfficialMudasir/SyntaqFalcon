using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Apps.Dtos
{
    public class GetAppJobForEditOutput
    {
		public CreateOrEditAppJobDto AppJob { get; set; }

        public Guid Id { get; set; }
        public string AppJobName { get; set; }
        public string AppName { get; set;}
        public Guid? AppId { get; set; }
        public Guid? EntityId { get; set; }

    }
}