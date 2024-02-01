
using System;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Apps.Dtos
{
    public class AppDto : EntityDto<Guid>
    {
		public string Name { get; set; }

		public string Description { get; set; }

        public DateTime LastModified { get; set; }

        public string UserACLPermission { get; set; }
    }
}