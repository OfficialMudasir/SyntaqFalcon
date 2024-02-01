using Syntaq.Falcon.Folders.Dtos;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Folders
{
    public class CreateOrEditFolderModalViewModel
    {
       public CreateOrEditFolderDto Folder { get; set; }

	   
	   public bool IsEditMode => Folder.Id.HasValue;
    }

    public class FolderLookupTableViewModel
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string FilterText { get; set; }
    }
}