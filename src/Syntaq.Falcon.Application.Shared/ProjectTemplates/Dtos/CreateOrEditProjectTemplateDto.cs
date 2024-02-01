using Abp.Application.Services.Dto;
using Ganss.Xss;
 
using Syntaq.Falcon.AccessControlList.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Syntaq.Falcon.ProjectTemplates.Dtos
{
    public class CreateOrEditProjectTemplateDto : EntityDto<Guid?>
    {
        private string _name = String.Empty;
        [Required]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                //// Sanitize the Name property using HtmlSanitizer
                //var sanitizer = new HtmlSanitizer();
                //_name = sanitizer.Sanitize(value);

                string pattern = @"<[^>]*>";
                _name = Regex.Replace(value, pattern, string.Empty);


            }
        }

        private string _description = String.Empty;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                // Sanitize the Name property using HtmlSanitizer
                string pattern = @"<[^>]*>";
                _description = Regex.Replace(value, pattern, string.Empty);
            }
        }
 
        [Required]
        public List<ProjectTemplateStepDto> StepsSchema { get; set; }

        public bool Enabled { get; set; } = true;

        public List<GrantACLDto> Assignees { get; set; } = new List<GrantACLDto>();

        public List<ProjectTagsDto> Tags { get; set; }

        public Guid? ProjectId { get; set; }
        public int Version { get; set; }
        public string VersionDescription { get; set; }
    }

    public class ProjectTagsDto
    {
        public Guid Name { get; set; }
        public string Value { get; set; }
    }
}