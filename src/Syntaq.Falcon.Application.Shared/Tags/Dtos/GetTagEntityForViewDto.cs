using System;

namespace Syntaq.Falcon.Tags.Dtos
{
    public class GetTagEntityForViewDto
    {
        public TagEntityDto TagEntity { get; set; }

        public Guid TagValueId { get; set; }
        public string TagValueValue { get; set; }

    }
}