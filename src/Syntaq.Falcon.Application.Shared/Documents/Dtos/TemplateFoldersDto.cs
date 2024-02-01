using Abp.Application.Services.Dto;
using Syntaq.Falcon.Folders.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Documents.Dtos
{
    public class TemplateFoldersDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DocumentName { get; set; }
        public int Version { get; set; }
        public int CurrentVersion { get; set; }
        public string Comments { get; set; }
        public Guid OriginalId { get; set; }
        public DateTime LastModified { get; set; }
        public string Type { get; set; }
        public string UserACLPermission { get; set; }

    }
}
