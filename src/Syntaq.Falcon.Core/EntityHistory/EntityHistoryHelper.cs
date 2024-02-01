//STA MODIFIED
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Projects;
using System;
using System.Linq;
using Abp.Organizations;
using Syntaq.Falcon.Authorization.Roles;
using Syntaq.Falcon.MultiTenancy;

namespace Syntaq.Falcon.EntityHistory
{
    public static class EntityHistoryHelper
    {
        public const string EntityHistoryConfigurationName = "EntityHistory";

        public static readonly Type[] HostSideTrackedTypes =
        {
            //STQ MODIFIED
            typeof(RecordMatterContributor),
            typeof(Project),
            typeof(OrganizationUnit), typeof(Role), typeof(Tenant)
        };

        public static readonly Type[] TenantSideTrackedTypes =
        {
            //STQ MODIFIED
            typeof(RecordMatterContributor),
            typeof(Project),
            typeof(OrganizationUnit), typeof(Role)
        };

        public static readonly Type[] TrackedTypes =
            HostSideTrackedTypes
                .Concat(TenantSideTrackedTypes)
                .GroupBy(type => type.FullName)
                .Select(types => types.First())
                .ToArray();
    }
}
