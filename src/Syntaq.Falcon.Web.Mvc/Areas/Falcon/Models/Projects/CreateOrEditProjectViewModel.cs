using Syntaq.Falcon.Projects.Dtos;

using Abp.Extensions;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Projects
{
    public class CreateOrEditProjectViewModel
    {
       public CreateOrEditProjectDto Project { get; set; }

	   		public string RecordRecordName { get; set;}
         

       
	   public bool IsEditMode => Project.Id.HasValue;
    }
}