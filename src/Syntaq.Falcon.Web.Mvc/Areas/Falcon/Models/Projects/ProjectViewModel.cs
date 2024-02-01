using Syntaq.Falcon.Projects.Dtos;
using System;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Projects
{
    public class ProjectViewModel : GetProjectForViewDto
    {
        public Guid? RecordMatterId;
    }
}