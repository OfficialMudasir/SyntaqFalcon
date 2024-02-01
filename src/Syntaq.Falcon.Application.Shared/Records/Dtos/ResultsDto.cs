using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;

namespace Syntaq.Falcon.Records.Dtos
{
    public class ResultsDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }

        public DateTime LastModified { get; set; }

        public string Type { get; set; }

        public string UserACLPermission { get; set; }
    }

    public class ResultsWithMattersDto : EntityDto<Guid>
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Comments { get; set; }

        public DateTime LastModified { get; set; }

        public string Type { get; set; }

        public string UserACLPermission { get; set; }

        public List<RecordMatterDto> RecordMatters { get; set; }

        public bool isArchived { get; set; }
    }
}
