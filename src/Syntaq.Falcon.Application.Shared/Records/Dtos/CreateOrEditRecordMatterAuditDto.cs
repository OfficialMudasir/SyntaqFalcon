using Syntaq.Falcon.Records;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using static Syntaq.Falcon.Records.RecordMatterConsts;

namespace Syntaq.Falcon.Records.Dtos
{
    public class CreateOrEditRecordMatterAuditDto : EntityDto<Guid?>
    {

        public RecordMatterStatus Status { get; set; }

        [StringLength(RecordMatterAuditConsts.MaxDataLength, MinimumLength = RecordMatterAuditConsts.MinDataLength)]
        public string Data { get; set; }

        public long? UserId { get; set; }

        public Guid? RecordMatterId { get; set; }

    }
}