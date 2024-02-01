using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectContributorForDashboardDto
    {
        public string Organization { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStepName { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string Action { get; set; }
        public int ActionCode { get; set; }
        public Guid? FormId { get; set; }
        public Guid RecordMatterId { get; set; }
        public Guid RecordMatterItemId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string AccessToken { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
