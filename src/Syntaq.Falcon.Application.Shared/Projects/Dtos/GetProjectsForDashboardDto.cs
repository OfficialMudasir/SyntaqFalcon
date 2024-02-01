using System;
using System.Collections.Generic;
using System.Text;
using static Syntaq.Falcon.Projects.ProjectConsts;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectsForDashboardDto
    {
		public string Name { get; set; }
		public string CreatorUserName { get; set; }
		public Guid ProjectTemplateId { get; set; }

		//public string Description { get; set; }

		//public DateTime CreationTime { get; set; }
		//public long CreatorUserId { get; set; }
		//public String CreatorUserName { get; set; }

		//public DateTime LastModificationTime { get; set; }
		//public long LastModifiedUserId { get; set; }
		//public String LastModifiedUserName { get; set; }
		public int CountByUser { get; set; }
		public int TotalCount { get; set; }
		public ProjectStatus Status { get; set; }

		public ProjectType Type { get; set; }
	}
}
