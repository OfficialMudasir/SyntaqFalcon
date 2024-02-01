namespace Syntaq.Falcon.Projects
{
    public class ProjectEnvironmentConsts
    {

        public const int MinNameLength = 0;
        public const int MaxNameLength = 128;

        public const int MinDescriptionLength = 0;
        public const int MaxDescriptionLength = 1024;

        public enum ProjectEnvironmentType { Dev, Test, PreProduction, Production}
    }
}