using Abp.Application.Services.Dto;
using Newtonsoft.Json.Linq;
using NUglify.Helpers;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.ProjectTemplates.Dtos;
using Syntaq.Falcon.Tags.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.ProjectTemplates
{
    public class CreateOrEditProjectTemplateViewModel
    {
        public CreateOrEditProjectTemplateDto ProjectTemplate { get; set; }

        public bool IsEditMode => ProjectTemplate.Id.HasValue;

        public List<FormListDto> FormsList { get; set; }

        public PagedResultDto<GetTagEntityTypeForViewDto> Tags { get; set; }
        public PagedResultDto<GetTagEntityForViewDto> EntityTags { get; set; }

        public List<GetProjectTemplateForView> VersionHistory { get; set; }

    }
}
