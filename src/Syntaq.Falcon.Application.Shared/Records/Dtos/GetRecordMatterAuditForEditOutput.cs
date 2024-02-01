using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Records.Dtos
{
    public class GetRecordMatterAuditForEditOutput
    {
        public CreateOrEditRecordMatterAuditDto RecordMatterAudit { get; set; }

        public string UserName { get; set; }

        public string RecordMatterRecordMatterName { get; set; }

    }
}