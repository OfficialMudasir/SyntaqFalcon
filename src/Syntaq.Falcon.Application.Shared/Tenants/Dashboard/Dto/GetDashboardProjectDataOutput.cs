using Syntaq.Falcon.Projects.Dtos;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Tenants.Dashboard.Dto
{
    public class GetDashboardProjectDataOutput
    {
        public List<string> ProjectStatuNames { get; set; }
        public List<string> ProjectActionNames { get; set; }
        public List<GetProjectsForDashboardDto> ProjectTemplatesCountList { get; set; }
        public List<GetProjectsForDashboardDto> ProjectTemplatesCountByUserList { get; set; }
        public int ProjectTotal { get; set; }
        public int ArchivedTotal { get; set; }


        public List<GetProjectsForDashboardDto> ProjectsList { get; set; }

        public List<GetProjectContributorForDashboardDto> ProjectContributorsList { get; set; }
        public List<GetProjectsRecentDocumentForDashboardDto> ProjectRecentDocumentsList { get; set; }
       
        public List<GetProjectContributorForDashboardDto> projectsWaitOthers { get; set; }
        public List<GetProjectsRecentTemplatesForDashboardDto> ProjectRecentTemplatesList { get; set; }
    }
}


