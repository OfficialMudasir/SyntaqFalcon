using System;

namespace Syntaq.Falcon.Documents.Dtos
{
    public class TemplateVersionDto
    {
        public Guid OriginalId { get; set; }
        public int Version { get; set; }
        public string VersionDes { get; set; }
        public string VersionName { get; set; }
        public int CurrentVersion { get; set; }
    }
}