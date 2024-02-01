using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class GetAllAsicForExcelInput
    {
        public string Filter { get; set; }

        public string HTTPRequestsFilter { get; set; }

        public string RequestMethodFilter { get; set; }

        public string ResponseFilter { get; set; }

    }
}