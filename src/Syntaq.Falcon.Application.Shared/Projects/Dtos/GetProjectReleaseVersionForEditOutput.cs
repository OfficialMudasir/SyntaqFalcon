using Syntaq.Falcon.Projects;

using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Syntaq.Falcon.Projects.Dtos
{
    public class GetProjectReleaseVersionForEditOutput  
    {

        public virtual int VersionMajor { get; set; }
        public virtual int VersionMinor { get; set; }
        public virtual int VersionRevision { get; set; }

        public Guid? Id { get; set; }

    }
}