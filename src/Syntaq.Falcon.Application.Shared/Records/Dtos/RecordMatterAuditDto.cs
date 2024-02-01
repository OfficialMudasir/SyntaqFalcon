using Syntaq.Falcon.Records;

using System;
using Abp.Application.Services.Dto;
using static Syntaq.Falcon.Records.RecordMatterConsts;

namespace Syntaq.Falcon.Records.Dtos
{
    public class RecordMatterAuditDto : EntityDto<Guid>
    {
        public DateTime CreationTime { get; set; }

        public RecordMatterStatus Status { get; set; }

        public string Data { get; set; }

        public long? UserId { get; set; }
        public String UserName { get; set; }

        public Guid? RecordMatterId { get; set; }

    }

}