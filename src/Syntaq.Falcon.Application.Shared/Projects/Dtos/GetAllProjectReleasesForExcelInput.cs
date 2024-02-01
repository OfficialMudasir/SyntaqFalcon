using Abp.Application.Services.Dto;
using System;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetAllProjectReleasesForExcelInput
    {
        public string Filter { get; set; }

        public string NameFilter { get; set; }

        public string NotesFilter { get; set; }

        public Guid? ProjectIdFilter { get; set; }

        public int? RequiredFilter { get; set; }

        public int? MaxVersionMajorFilter { get; set; }
        public int? MinVersionMajorFilter { get; set; }

        public int? MaxVersionMinorFilter { get; set; }
        public int? MinVersionMinorFilter { get; set; }

        public int? MaxVersionRevisionFilter { get; set; }
        public int? MinVersionRevisionFilter { get; set; }

        public int? ReleaseTypeFilter { get; set; }

        public string ProjectEnvironmentNameFilter { get; set; }

    }
}