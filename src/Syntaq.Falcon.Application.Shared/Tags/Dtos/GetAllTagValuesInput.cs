using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetAllTagValuesInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string ValueFilter { get; set; }

        public string TagNameFilter { get; set; }

        public Guid? Id { get; set; }

    }
}