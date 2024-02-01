using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Forms.Dtos
{
    public class GetAllFormRulesInput : PagedAndSortedResultRequestDto
    {
        public Guid Id { get; set; }

        public bool IsEnabledFilter { get; set; }

        //public string RuleFilter { get; set; }

        //public string RuleScriptFilter { get; set; }

        //public string FormNameFilter { get; set; }
    }
}