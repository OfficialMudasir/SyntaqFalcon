using Syntaq.Falcon.Apps.Dtos;
using System;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.AppJobs
{
    public class CreateOrEditAppJobModalViewModel
    {
       public CreateOrEditAppJobDto AppJob { get; set; }
        public Guid Id { get; set; }
        public Guid? AppId { get; set; }
        public Guid? EntityId { get; set; }
        public string AppName { get; set;}
        public string AppJobName { get; set; }

        public bool IsEditMode => AppJob.Id.HasValue;
    }
}