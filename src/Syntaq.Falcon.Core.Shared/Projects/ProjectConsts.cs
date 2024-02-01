namespace Syntaq.Falcon.Projects
{
    public class ProjectConsts
    {

		public const int MinNameLength = 0;
		public const int MaxNameLength = 128;
						
		public const int MinDescriptionLength = 0;
		public const int MaxDescriptionLength = 2048;
 
		public enum ProjectStatus { New, InProgress, Completed}
		public enum ProjectType { Template, User, Deployment }		
		public enum ProjectStepStatus { New, Draft, Publish, Final, FinalUnlocked }
		public enum ProjectStepRole { Review, Approve, Share, Author}		
		public enum ProjectStepAction { Review, Contribute }



	}
}