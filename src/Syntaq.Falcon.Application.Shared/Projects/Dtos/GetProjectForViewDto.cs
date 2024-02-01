using Syntaq.Falcon.Files.Dtos;
using System;
using System.Collections.Generic;
using static Syntaq.Falcon.Projects.ProjectConsts;
using static Syntaq.Falcon.Records.RecordMatterContributorConsts;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectForViewDto
    {
        public ProjectDto Project { get; set; }
        public ProjectReleaseDto Release { get; set; }

        public string RecordRecordName { get; set;}
        public List<FilesDto> Uploadfiles { get; set; }

        public List<ProjectDeploymentDto> Deployments { get; set; } = new List<ProjectDeploymentDto>();

       
    }
    public class ShareProjectForViewDto
    {
      
        public ProjectStepRole Role { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStepName { get; set; }
        public RecordMatterContributorStatus Status { get; set; }
       // public int StatusCode { get; set; }
        public ProjectStepAction Action { get; set; }
       // public int ActionCode { get; set; }
        public Guid FormId { get; set; }
        public Guid RecordMatterId { get; set; }
        public Guid RecordMatterItemId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string AccessToken { get; set; }


    }
}