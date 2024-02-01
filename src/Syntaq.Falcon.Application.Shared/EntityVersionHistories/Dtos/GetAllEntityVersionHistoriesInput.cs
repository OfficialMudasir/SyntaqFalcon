using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.EntityVersionHistories.Dtos
{
    public class GetAllEntityVersionHistoriesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string VersionNameFilter { get; set; }

        public string DescriptionFilter { get; set; }

        public string UserNameFilter { get; set; }

    }
}