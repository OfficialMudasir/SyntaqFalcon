using System;

namespace Syntaq.Falcon.Folders.Dtos
{
    public class MoveFolderDto
    {
        public Guid DraggableId { get; set; }
        public string DraggableType { get; set; }
        public Guid Id { get; set; }
        public string FolderType { get; set; }
        public long? UserId { get; set; }
    }

    public class FormVersionDto
    {
        public Guid OriginalId { get; set; }
        public int Version { get; set; }
        public string VersionDes { get; set; }
        public string VersionName { get; set; }
    }

    public class FormScriptDto
    {
        public Guid Id { get; set; }
        public string Script { get; set; }
    }

    public class FormRulesDto
    {
        public Guid Id { get; set; }
        //public string Rules { get; set; }
        public string RulesScript { get; set; }
    }
}
