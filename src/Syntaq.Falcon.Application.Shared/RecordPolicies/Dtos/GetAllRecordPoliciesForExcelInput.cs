using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.RecordPolicies.Dtos
{
    public class GetAllRecordPoliciesForExcelInput
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

    }
}