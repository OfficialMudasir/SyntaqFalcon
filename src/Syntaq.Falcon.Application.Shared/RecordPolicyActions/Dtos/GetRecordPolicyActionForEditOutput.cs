using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.RecordPolicyActions.Dtos
{
    public class GetRecordPolicyActionForEditOutput
    {
        public CreateOrEditRecordPolicyActionDto RecordPolicyAction { get; set; }

        public string RecordPolicyName { get; set; }

    }
}