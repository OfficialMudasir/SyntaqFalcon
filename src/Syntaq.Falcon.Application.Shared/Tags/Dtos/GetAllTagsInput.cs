using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetAllTagsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

    }
}