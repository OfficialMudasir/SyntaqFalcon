using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Syntaq.Falcon.RecordPolicies.Dtos
{
    public class GetRecordPolicyForEditOutput
    {
        public CreateOrEditRecordPolicyDto RecordPolicy { get; set; }
        public List<TenantIdNameListDto> TenantList { get; set; }
    }
    public class TenantIdNameListDto
    {
        public string Name { get; set; }
        public int? Id { get; set; }
    }
}