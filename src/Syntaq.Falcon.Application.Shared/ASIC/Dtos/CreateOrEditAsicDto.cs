using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.ASIC.Dtos
{
    public class CreateOrEditAsicDto : EntityDto<Guid?>
    {
        public string HTTPRequests { get; set; }

        public AsicConsts.RequestMethod RequestMethod { get; set; }

        public string Response { get; set; }
        public virtual string Data { get; set; }

        public virtual string AccessToken { get; set; }

        public virtual int? RequestId { get; set; }
        public virtual Guid RecordId { get; set; }
        public virtual Guid RecordMatterId { get; set; }
        public virtual Guid RecordMatterItemId { get; set; }
        public virtual Guid SubmissionId { get; set; }
        public string Status { get; set; }
    }
}