using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Records.Dtos
{
    public class RecordDto : EntityDto<Guid>
    {
		public string RecordName { get; set; }
        public string Comments { get; set; }
        public Guid FolderId { get; set; }

        //public long? UserId { get; set; }
        //public long? OrganizationUnitId { get; set; }

        public List<RecordMatterDto> RecordMatters { get; set; } = new List<RecordMatterDto>();

    }
}