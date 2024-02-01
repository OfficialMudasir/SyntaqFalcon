using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class GetAllAsicInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string HTTPRequestsFilter { get; set; }

        public string RequestMethodFilter { get; set; }

        public string ResponseFilter { get; set; }
        public string TenantNameFilter { get; set; }
        public DateTime StartDateFilter { get; set; }
        public DateTime EndDateFilter { get; set; }

    }
}