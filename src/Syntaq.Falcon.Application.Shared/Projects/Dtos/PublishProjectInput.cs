using System;
using System.Collections.Generic;
using System.Text;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class PublishProjectInput
    {
        public Guid ProjectId { get; set; }

        public string? ReleaseName { get; set; }

        public string? ReleaseNotes{ get; set; }

        public bool Required { get; set; }

        public int VersionMajor { get; set; }

        public int VersionMinor { get; set; }

        public int VersionRevision { get; set; }

        public Guid? ProjectEnvironmentId { get; set; }

    }
}
