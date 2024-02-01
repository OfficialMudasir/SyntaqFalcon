using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class AsicDto : EntityDto<Guid>
    {
        public int? TenantId { get; set; }

        public virtual int? RequestId { get; set; } //should be changed as int

        public string Status { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastModificationTime { get; set; }
    }

    public class GetCertificateDownloadDto : EntityDto<Guid>
    {
        public byte[] Document { get; set; }
        public string DocumentName { get; set; }
    }

    public class GetDocumentResultDto
    {
        public int? TenantId { get; set; }
        public Guid? FormId { get; set; }
        public Guid? RecordMatterId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }

        public Guid SubmissionId { get; set; }
        public string SubmissionStatus { get; set; }

        public List<DocumentList201Dto> DocList { get; set; }
    }

    public class DocumentList201Dto
    {
        public Guid RecordmatterItemId { get; set; }
        public string DocumentName { get; set; }
        public virtual string DocumentTemplateId { get; set; }
        public virtual bool LockOnBuild { get; set; } = false;
        public string AllowedFormats { get; set; }
       
    }

    

}