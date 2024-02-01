using Abp;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUglify.JavaScript.Syntax;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Authorization.Users.Dto;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.Features;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Tenants.Dashboard.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Xml;

namespace Syntaq.Falcon.Tenants.Dashboard
{
    [DisableAuditing]
    [AbpAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
    public class TenantDashboardAppService : FalconAppServiceBase, ITenantDashboardAppService
    {
        private readonly SubmissionManager _submissionManager;
        private readonly IRepository<Submission, Guid> _submissionRepository;
        private readonly IRepository<User, long> _userRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly IRepository<Project, Guid> _projectRepository;

        private readonly IRepository<ProjectRelease, Guid> _projectReleaseRepository;
        private readonly IRepository<ProjectDeployment, Guid> _projectDeploymentRepository;
        private readonly IRepository<ProjectEnvironment, Guid> _projectEnvironmentRepository;

        private readonly IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
        private readonly IRepository<RecordMatterItem, Guid> _lookup_recordMatterItemRepository;



        public TenantDashboardAppService(
            SubmissionManager submissionManager
            , IRepository<Submission, Guid> submissionRepository
            , IRepository<User, long> userRepository
            , IUnitOfWorkManager unitOfWorkManager
            , IRepository<Project, Guid> projectRepository

            , IRepository<ProjectRelease, Guid> projectReleaseRepository
            , IRepository<ProjectDeployment, Guid> projectDeploymentRepository
            , IRepository<ProjectEnvironment, Guid> projectEnvironmentRepository

            , IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository
            , IRepository<RecordMatter, Guid> recordMatterRepository
            , IRepository<RecordMatterItem, Guid> lookup_recordMatterItemRepository

        )
        {
            _submissionManager = submissionManager;
            _submissionRepository = submissionRepository;
            _userRepository = userRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _projectRepository = projectRepository;

            _projectReleaseRepository = projectReleaseRepository;
            _projectDeploymentRepository = projectDeploymentRepository;
            _projectEnvironmentRepository = projectEnvironmentRepository;

            _recordMatterContributorRepository = recordMatterContributorRepository;
            _recordMatterRepository = recordMatterRepository;
            _lookup_recordMatterItemRepository = lookup_recordMatterItemRepository;

        }

        public async Task<GetDashboardDataOutput> GetDashboardData(GetDashboardDataInput input)
        {
            int submissionLimit = Convert.ToInt32(await FeatureChecker.GetValueAsync(AppFeatures.SubmissionLimitAmount));
            int currentSubmissions = GetMonthlySubmissions(input);
            var output = new GetDashboardDataOutput
            {
                //TotalProfit = DashboardRandomDataGenerator.GetRandomInt(5000, 9000),
                SubmissionLimit = submissionLimit,
                CurrentSubmissions = currentSubmissions,
                SubmissionUsagePercent = (int)Math.Round((double)(100 * currentSubmissions) / submissionLimit),
                YesterdaysSubmissions = GetYesterdaysSubmissions(input),
                TodaysSubmissions = GetTodaysSubmissions(input),
                NewUsers = GetNewUsers(input),
                RecentSubmissions = GetRecentSubmissions(input),
                //NewFeedbacks = DashboardRandomDataGenerator.GetRandomInt(1000, 5000),
                //NewOrders = DashboardRandomDataGenerator.GetRandomInt(100, 900),
                //NewUsers = DashboardRandomDataGenerator.GetRandomInt(50, 500),
                //SalesSummary = DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod),
                //Expenses = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                //Growth = DashboardRandomDataGenerator.GetRandomInt(5000, 10000),
                //Revenue = DashboardRandomDataGenerator.GetRandomInt(1000, 9000),
                //TotalSales = DashboardRandomDataGenerator.GetRandomInt(10000, 90000),
                //TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                //NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                //BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                //DailySales = DashboardRandomDataGenerator.GetRandomArray(30, 10, 50),
                //ProfitShares = DashboardRandomDataGenerator.GetRandomPercentageArray(3)
            };

            return output;
        }

        //#Refactored 
        public async Task<GetDashboardProjectDataOutput> GetDashboardProjectData(GetDashboardProjectDataInput input)
        {
            #region Old Code
            //using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            //{
            //    if (input.TabType == 'P')
            //    {
            //        return new GetDashboardProjectDataOutput
            //        {
            //            ProjectStatuNames = GetProjectStatusNamesList_OLD(), // DONE
            //            ProjectTotal = GetProjectTotalCount_OLD(input),  // DONE
            //            ProjectsList = GetProjectsList_OLD(input), // DONE
            //            ArchivedTotal = GetArchivedTotalCount_OLD(input) // DONE
            //        };
            //    }
            //    else if (input.TabType == 'C')
            //    {
            //        return new GetDashboardProjectDataOutput
            //        {
            //            ProjectContributorsList = GetContributorsList_OLD(input), // DONE 1
            //            ProjectActionNames = GetProjectActionNamesList_OLD(), // DONE
            //            ProjectRecentDocumentsList = GetRecentProjectStatusNamesList_OLD(input), // DONE 2
            //            projectsWaitOthers = GetWaitOthersList_OLD(input), // DONE 3

            //            ProjectRecentTemplatesList = GetRecentTemplatesList_OLD(input), // DONE 4
            //            ProjectTemplatesCountList = GetProjectTemplatesList_OLD(input), // DONE
            //            ProjectTemplatesCountByUserList = GetProjectTemplatesUserCountList_OLD(input), // DONE
            //            ArchivedTotal = GetArchivedTotalCount_OLD(input) // DONE
            //        };
            //    }
            //    else
            //    {
            //        return new GetDashboardProjectDataOutput
            //        {
            //            ProjectStatuNames = GetProjectStatusNamesList_OLD(),// DONE
            //            ProjectTotal = GetProjectTotalCount_OLD(input),// DONE
            //            ArchivedTotal = GetArchivedTotalCount_OLD(input), // DONE
            //            ProjectsList = GetProjectsList_OLD(input),// DONE
            //            ProjectContributorsList = GetContributorsList_OLD(input), // DONE
            //            ProjectActionNames = GetProjectActionNamesList_OLD(), // DONE
            //            ProjectRecentDocumentsList = GetRecentProjectStatusNamesList_OLD(input),  // DONE 5
            //        };
            //    }
            //}
            #endregion

            #region Refactored Code
            // TURN off Tenant Filter to allow lookup of Release Environment Data
            // ! Must Filter for Tenant manually
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {
                if (input.TabType == 'P')
                {
                    return new GetDashboardProjectDataOutput
                    {
                        ProjectStatuNames = GetProjectStatusNamesList(), // DONE
                        ProjectTotal = GetProjectTotalCount(input),  // DONE
                        ProjectsList = GetProjectsList(input), // DONE
                        ArchivedTotal = GetArchivedTotalCount(input) // DONE
                    };
                }
                else if (input.TabType == 'C')
                {
                    GetDashboardProjectDataOutput getDashboardProjectDataOutput = new GetDashboardProjectDataOutput
                    {
                        ProjectContributorsList = GetContributorsList_ByNew(input), // DONE 1
                        ProjectActionNames = GetProjectActionNamesList(), // DONE
                        ProjectRecentDocumentsList = GetRecentProjectStatusNamesList(input), // DONE 2
                        projectsWaitOthers = GetWaitOthersList(input), // DONE 3

                        ProjectRecentTemplatesList = GetRecentTemplatesList(input), // DONE 4
                        ProjectTemplatesCountList = GetProjectTemplatesList(input), // DONE
                                                                                    // ProjectTemplatesCountList = GetProjectTemplatesList_OLD(input), // DONE
                        ProjectTemplatesCountByUserList = GetProjectTemplatesUserCountList(input), // DONE
                        //ProjectTemplatesCountByUserList = GetProjectTemplatesUserCountList_Demo(input), // DONE
                        ArchivedTotal = GetArchivedTotalCount(input) // DONE
                    };

                    return getDashboardProjectDataOutput;
                }
                else
                {
                    return new GetDashboardProjectDataOutput
                    {
                        ProjectStatuNames = GetProjectStatusNamesList(),// DONE
                        ProjectTotal = GetProjectTotalCount(input),// DONE
                        ArchivedTotal = GetArchivedTotalCount(input), // DONE
                        ProjectsList = GetProjectsList(input),// DONE
                        //ProjectContributorsList = GetContributorsList(input), // DONE
                        ProjectActionNames = GetProjectActionNamesList(), // DONE
                        ProjectRecentDocumentsList = GetRecentProjectStatusNamesList(input),  // DONE 5
                    };
                }
            }
            #endregion
        }

        public async Task<List<ProjectEnvironmentDto>> GetProjectEnvironments()
        {
            // TURN off Tenant Filter to allow lookup of Release Environment Data
            // ! Must Filter for Tenant manually
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
            {

                // Get The Deployments and Environments for all in this Tenancy

                //_projectReleaseRepository = projectReleaseRepository;
                //_projectDeploymentRepository = projectDeploymentRepository;
                //_projectEnvironmentRepository = projectEnvironmentRepository;

                // Get the Release Ids of the Deployments to this Tenant
                var rIds = _projectDeploymentRepository.GetAll().Where(d => d.TenantId == AbpSession.TenantId).Select(d => d.ProjectReleaseId);

                var projectenvironments = _projectReleaseRepository.GetAll().Where(r => rIds.Contains(r.Id))
                    .Select(r => new ProjectEnvironmentDto()
                    {
                        Id = (Guid)r.ProjectEnvironmentId,
                        Name = r.ProjectEnvironmentFk.Name,
                        Description = r.ProjectEnvironmentFk.Description,
                        EnvironmentType = r.ProjectEnvironmentFk.EnvironmentType
                    });

                return projectenvironments.Distinct().ToList();

            }
        }

        private List<GetProjectsRecentTemplatesForDashboardDto> GetRecentTemplatesList_OLD(GetDashboardProjectDataInput input)
        {
            var recentProjectsList = _projectRepository.GetAll()
             .Where(p => p.CreatorUserId == AbpSession.UserId && p.Type == ProjectConsts.ProjectType.User)
             .ToList()
             .Join(_recordMatterRepository.GetAll().Where(rm => rm.TenantId == AbpSession.TenantId),
                 p => p.RecordId,
                 rm => rm.RecordId,
                 (p, rm) => new
                 {
                     p.Id,
                     p.Name,
                     rm.RecordMatterName,
                     rm.LastModificationTime,
                     rm.CreationTime,
                     rm.FormId,
                     RecordMatterId = rm.Id,
                     rm.Status,
                     ProjectStepName = rm.RecordMatterName,
                     Type = 'O'
                 })
             .ToList()
             .Join(_lookup_recordMatterItemRepository.GetAll()
                     .Where(rmi => !string.IsNullOrEmpty(rmi.DocumentName) && rmi.TenantId == AbpSession.TenantId),
                 rm => rm.RecordMatterId,
                 rmi => rmi.RecordMatterId,
                 (rm, rmi) => new GetProjectsRecentTemplatesForDashboardDto
                 {
                     ProjectId = rm.Id,
                     ProjectName = rm.Name,
                     DocumentName = rmi.DocumentName,
                     LastModificationTime = rm.LastModificationTime ?? rm.CreationTime,
                     RecordMatterId = rmi.Id,
                     FormId = rm.FormId,
                     ProjectStepName = rm.ProjectStepName,
                     Status = Convert.ToString(rm.Status),
                     RecordMatterItemId = rmi.Id,
                     AllowedFormats = rmi.AllowedFormats
                 })
             .OrderByDescending(p => p.LastModificationTime)
             .Take(25)
             .ToList();

            return recentProjectsList;

        }

        private List<GetProjectsRecentTemplatesForDashboardDto> GetRecentTemplatesList(GetDashboardProjectDataInput input)
        {
            var recentProjectsList = (
                from p in _projectRepository.GetAll()
                join rm in _recordMatterRepository.GetAll().Where(rm => rm.TenantId == AbpSession.TenantId)
                    on p.RecordId equals rm.RecordId
                join rmi in _lookup_recordMatterItemRepository.GetAll().Where(rmi => !string.IsNullOrEmpty(rmi.DocumentName) && rmi.TenantId == AbpSession.TenantId)
                    on rm.Id equals rmi.RecordMatterId
                where p.CreatorUserId == AbpSession.UserId && p.Type == ProjectConsts.ProjectType.User
                orderby rm.LastModificationTime ?? rm.CreationTime descending
                select new GetProjectsRecentTemplatesForDashboardDto
                {
                    ProjectId = p.Id,
                    ProjectName = p.Name,
                    DocumentName = rmi.DocumentName,
                    LastModificationTime = rm.LastModificationTime ?? rm.CreationTime,
                    RecordMatterId = rmi.RecordMatterId,
                    FormId = rm.FormId,
                    ProjectStepName = rm.RecordMatterName,
                    Status = rm.Status.ToString(),
                    RecordMatterItemId = rmi.Id,
                    AllowedFormats = rmi.AllowedFormats
                }
            ).Take(25).ToList();

            return recentProjectsList;
        }

        private List<GetProjectContributorForDashboardDto> GetContributorsList_OLD(GetDashboardProjectDataInput input)
        {
            try
            {
                List<GetProjectContributorForDashboardDto> ContributorsList = new List<GetProjectContributorForDashboardDto>();
                var contributors = _recordMatterContributorRepository.GetAll()
                    .Where(c => c.UserId == AbpSession.UserId && c.Enabled == true)
                    .ToList()
                    .Join(_recordMatterRepository.GetAll().Where(rm => rm.TenantId == AbpSession.TenantId),
                    RecordMatterContributor => RecordMatterContributor.RecordMatterId,
                    RecordMatter => RecordMatter.Id,
                    (RecordMatterContributor, RecordMatter) => new
                    {
                        Organization = RecordMatterContributor.OrganizationName,
                        //RecordMatterContributor.Name,
                        Role = RecordMatterContributor.StepRole,
                        //RecordMatterContributor.Email,
                        RecordMatterContributor.CreatorUserId,
                        RecordMatter.RecordId,
                        ProjectStepName = RecordMatter.RecordMatterName,
                        Status = RecordMatterContributor.Status,
                        Action = RecordMatterContributor.StepAction, //maybe useless
                        CreatedTime = RecordMatterContributor.CreationTime,
                        RecordMatterContributor.AccessToken,
                        RecordMatterContributor.RecordMatterId
                    }
                    ).ToList()

                    .Join(_projectRepository.GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(p => p.TenantId == AbpSession.TenantId && p.Type == ProjectConsts.ProjectType.User),
                    RecordMatterContributor => RecordMatterContributor.RecordId,
                    Project => Project.RecordId,
                    (RecordMatterContributor, Project) => new
                    {
                        RecordMatterContributor.Organization,
                        //RecordMatterContributor.Name,
                        RecordMatterContributor.Role,
                        //RecordMatterContributor.Email,
                        RecordMatterContributor.CreatorUserId,
                        ProjectName = Project.Name,
                        ProjectId = Project.Id,
                        RecordMatterContributor.ProjectStepName,
                        RecordMatterContributor.Status,
                        RecordMatterContributor.Action,
                        RecordMatterContributor.CreatedTime,
                        RecordMatterContributor.AccessToken,
                        RecordMatterContributor.RecordMatterId
                    }
                    ).ToList()

                    .Join(_userRepository.GetAll(),
                    RecordMatterContributor => RecordMatterContributor.CreatorUserId,
                    User => User.Id,
                    (RecordMatterContributor, User) => new
                    {
                        Organization = User.Entity,
                        CreatedUserName = User.Name,
                        RecordMatterContributor.Role,
                        CreatedUserEmail = User.EmailAddress,
                        RecordMatterContributor.CreatorUserId,
                        RecordMatterContributor.ProjectName,
                        RecordMatterContributor.ProjectId,
                        RecordMatterContributor.ProjectStepName,
                        RecordMatterContributor.Status,
                        RecordMatterContributor.Action,
                        RecordMatterContributor.CreatedTime,
                        RecordMatterContributor.AccessToken,
                        RecordMatterContributor.RecordMatterId
                    })
                    .OrderByDescending(i => i.CreatedTime)
                    .ToList();

                contributors.ForEach(c =>
                {
                    ContributorsList.Add(new GetProjectContributorForDashboardDto()
                    {
                        Organization = c.Organization,
                        Name = c.CreatedUserName,
                        Role = Enum.GetName(typeof(ProjectConsts.ProjectStepRole), c.Role),
                        Email = c.CreatedUserEmail,
                        ProjectName = c.ProjectName,
                        ProjectStepName = c.ProjectStepName,
                        Status = Enum.GetName(typeof(RecordMatterContributorConsts.RecordMatterContributorStatus), c.Status),
                        StatusCode = (int)c.Status,
                        Action = Enum.GetName(typeof(ProjectConsts.ProjectStepAction), c.Action),
                        ActionCode = (int)c.Role,
                        CreatedTime = c.CreatedTime,
                        AccessToken = c.AccessToken,
                        RecordMatterId = (Guid)c.RecordMatterId,
                        ProjectId = c.ProjectId,
                    });

                });

                return ContributorsList;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private List<GetProjectContributorForDashboardDto> GetContributorsList1(GetDashboardProjectDataInput input)
        {
            try
            {
                List<GetProjectContributorForDashboardDto> contributors = (from recordMatterContributor in _recordMatterContributorRepository.GetAll()
                                                                           join recordMatter in _recordMatterRepository.GetAll()
                                                                           on recordMatterContributor.RecordMatterId equals recordMatter.Id
                                                                           join p in _projectRepository.GetAll()
                                                                           on recordMatter.RecordId equals p.RecordId
                                                                           where recordMatterContributor.UserId == AbpSession.UserId
                                                                           && recordMatterContributor.Enabled == true
                                                                           && p.TenantId == AbpSession.TenantId
                                                                           && p.Type == ProjectConsts.ProjectType.User
                                                                           && (input.EnvironmentId == null || p.ProjectEnvironmentId == input.EnvironmentId)
                                                                           join u in _userRepository.GetAll()
                                                                           on recordMatterContributor.CreatorUserId equals u.Id
                                                                           orderby recordMatterContributor.CreationTime descending

                                                                           select new GetProjectContributorForDashboardDto()
                                                                           {
                                                                               Organization = u.Entity,
                                                                               Name = u.Name,
                                                                               Role = Enum.GetName(typeof(ProjectConsts.ProjectStepRole), recordMatterContributor.StepRole),
                                                                               //Role = Enum.GetName(typeof(ProjectConsts.ProjectStepRole), recordMatterContributor.StepRole),
                                                                               Email = u.EmailAddress,
                                                                               ProjectName = p.Name,
                                                                               ProjectStepName = recordMatter.RecordMatterName,
                                                                               Status = Enum.GetName(typeof(RecordMatterContributorConsts.RecordMatterContributorStatus), recordMatterContributor.Status),
                                                                               StatusCode = (int)recordMatterContributor.Status,
                                                                               Action = Enum.GetName(typeof(ProjectConsts.ProjectStepAction), recordMatterContributor.StepAction),
                                                                               ActionCode = (int)recordMatterContributor.StepAction,
                                                                               CreatedTime = recordMatterContributor.CreationTime,
                                                                               AccessToken = recordMatterContributor.AccessToken,
                                                                               RecordMatterId = (Guid)recordMatterContributor.RecordMatterId,
                                                                               ProjectId = p.Id
                                                                           }).ToList();


                return contributors;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private List<GetProjectContributorForDashboardDto> GetContributorsList_OLD1(GetDashboardProjectDataInput input)
        {
            try
            {
                var roleType = typeof(ProjectConsts.ProjectStepRole);
                var statusType = typeof(RecordMatterContributorConsts.RecordMatterContributorStatus);
                var actionType = typeof(ProjectConsts.ProjectStepAction);

                List<GetProjectContributorForDashboardDto> contributors = (from recordMatterContributor in _recordMatterContributorRepository.GetAll()
                                                                           join recordMatter in _recordMatterRepository.GetAll()
                                                                           on recordMatterContributor.RecordMatterId equals recordMatter.Id
                                                                           join p in _projectRepository.GetAll()
                                                                           on recordMatter.RecordId equals p.RecordId
                                                                           where recordMatterContributor.UserId == AbpSession.UserId
                                                                           && recordMatterContributor.Enabled == true
                                                                           && p.TenantId == AbpSession.TenantId
                                                                           && p.Type == ProjectConsts.ProjectType.User
                                                                           && (input.EnvironmentId == null || p.ProjectEnvironmentId == input.EnvironmentId)
                                                                           join u in _userRepository.GetAll()
                                                                           on recordMatterContributor.CreatorUserId equals u.Id
                                                                           orderby recordMatterContributor.CreationTime descending

                                                                           select new GetProjectContributorForDashboardDto()
                                                                           {
                                                                               Organization = u.Entity,
                                                                               Name = u.Name,
                                                                               Role = Enum.GetName(roleType, recordMatterContributor.StepRole),
                                                                               Email = u.EmailAddress,
                                                                               ProjectName = p.Name,
                                                                               ProjectStepName = recordMatter.RecordMatterName,
                                                                               Status = Enum.GetName(statusType, recordMatterContributor.Status),
                                                                               StatusCode = (int)recordMatterContributor.Status,
                                                                               Action = Enum.GetName(actionType, recordMatterContributor.StepAction),
                                                                               ActionCode = (int)recordMatterContributor.StepAction,
                                                                               CreatedTime = recordMatterContributor.CreationTime,
                                                                               AccessToken = recordMatterContributor.AccessToken,
                                                                               RecordMatterId = (Guid)recordMatterContributor.RecordMatterId,
                                                                               ProjectId = p.Id
                                                                           }).ToList();

                return contributors;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static GetProjectContributorForDashboardDto MapToContributorDto(
            RecordMatterContributor rmc, Project p, User u)
        {
            return new GetProjectContributorForDashboardDto
            {
                Organization = u.Entity,
                Name = u.Name,
                Role = Enum.GetName(typeof(ProjectConsts.ProjectStepRole), rmc.StepRole),
                Email = u.EmailAddress,
                ProjectName = p.Name,
               // ProjectStepName = rmc.RecordMatterName,
                Status = Enum.GetName(
                    typeof(RecordMatterContributorConsts.RecordMatterContributorStatus),
                    rmc.Status),
                StatusCode = (int)rmc.Status,
                Action = Enum.GetName(typeof(ProjectConsts.ProjectStepAction), rmc.StepAction),
                ActionCode = (int)rmc.StepAction,
                CreatedTime = rmc.CreationTime,
                AccessToken = rmc.AccessToken,
                RecordMatterId = (Guid)rmc.RecordMatterId,
                ProjectId = p.Id
            };
        }

        private List<GetProjectContributorForDashboardDto> GetContributorsList_ByNew(GetDashboardProjectDataInput input)
        {
            try
            {
                var userId = AbpSession.UserId;
                var tenantId = AbpSession.TenantId;

                var contributors = (
                    from rmc in _recordMatterContributorRepository.GetAll()
                    join rm in _recordMatterRepository.GetAll() on rmc.RecordMatterId equals rm.Id
                    join p in _projectRepository.GetAll() on rm.RecordId equals p.RecordId
                    join u in _userRepository.GetAll() on rmc.CreatorUserId equals u.Id
                    where rmc.UserId == userId
                        && rmc.Enabled
                        && p.TenantId == tenantId
                        && p.Type == ProjectConsts.ProjectType.User
                        && (input.EnvironmentId == null || p.ProjectEnvironmentId == input.EnvironmentId)
                    orderby rmc.CreationTime descending
                    select MapToContributorDto(rmc, p, u)
                ).ToList();

                return contributors;
            }
            catch (Exception ex)
            {
                return null;
            }
        }





        private List<GetProjectContributorForDashboardDto> GetWaitOthersList_OLD(GetDashboardProjectDataInput input)
        {
            List<GetProjectContributorForDashboardDto> ContributorsList = new List<GetProjectContributorForDashboardDto>();
            var contributors = _recordMatterContributorRepository.GetAll()
                .Where(c => c.CreatorUserId == AbpSession.UserId)
                .ToList()
                .Join(_recordMatterRepository.GetAll().Where(rm => rm.TenantId == AbpSession.TenantId),
                RecordMatterContributor => RecordMatterContributor.RecordMatterId,
                RecordMatter => RecordMatter.Id,
                (RecordMatterContributor, RecordMatter) => new
                {
                    Organization = RecordMatterContributor.OrganizationName,
                    sName = RecordMatterContributor.Name,
                    Role = RecordMatterContributor.StepRole,
                    sEmail = RecordMatterContributor.Email,
                    RecordMatterContributor.CreatorUserId,
                    RecordMatter.RecordId,
                    ProjectStepName = RecordMatter.RecordMatterName,
                    Status = RecordMatterContributor.Status,
                    Action = RecordMatterContributor.StepAction, //maybe useless
                    CreatedTime = RecordMatterContributor.CreationTime,
                    RecordMatterContributor.AccessToken,
                    RecordMatterContributor.RecordMatterId,
                    formId = RecordMatter.FormId
                }
                ).ToList()
                .Join(_projectRepository.GetAll()
                .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                .Where(p => p.TenantId == AbpSession.TenantId && p.Type == ProjectConsts.ProjectType.User),
                RecordMatterContributor => RecordMatterContributor.RecordId,
                Project => Project.RecordId,
                (RecordMatterContributor, Project) => new
                {
                    RecordMatterContributor.Organization,
                    RecordMatterContributor.sName,
                    RecordMatterContributor.Role,
                    RecordMatterContributor.sEmail,
                    RecordMatterContributor.CreatorUserId,
                    ProjectName = Project.Name,
                    RecordMatterContributor.ProjectStepName,
                    RecordMatterContributor.Status,
                    RecordMatterContributor.Action,
                    RecordMatterContributor.CreatedTime,
                    RecordMatterContributor.AccessToken,
                    RecordMatterContributor.RecordMatterId,
                    RecordMatterContributor.formId,
                    ProjectId = Project.Id,
                })
                .OrderByDescending(i => i.CreatedTime)
                .ThenBy(x => x.Status)
                .ToList();

            contributors.ForEach(c =>
            {
                ContributorsList.Add(new GetProjectContributorForDashboardDto()
                {
                    Organization = c.Organization,
                    Name = c.sName,
                    Role = Enum.GetName(typeof(ProjectConsts.ProjectStepRole), c.Role),
                    Email = c.sEmail,
                    ProjectName = c.ProjectName,
                    ProjectStepName = c.ProjectStepName,
                    Status = Enum.GetName(typeof(RecordMatterContributorConsts.RecordMatterContributorStatus), c.Status),
                    StatusCode = (int)c.Status,
                    Action = Enum.GetName(typeof(ProjectConsts.ProjectStepAction), c.Action),
                    ActionCode = (int)c.Action,
                    CreatedTime = c.CreatedTime,
                    AccessToken = c.AccessToken,
                    RecordMatterId = (Guid)c.RecordMatterId,
                    FormId = c.formId,
                    ProjectId = c.ProjectId,
                });

            });

            return ContributorsList;

        }

        private List<GetProjectsForDashboardDto> GetProjectTemplatesList_OLD(GetDashboardProjectDataInput input)
        {
            List<GetProjectsForDashboardDto> ProjectsList = new List<GetProjectsForDashboardDto>();
            var user = _userRepository.GetAll().Where(u => u.Id == AbpSession.UserId).FirstOrDefault();
            // admin can see all projects, tenant user can see own projects
            List<Project> Projects = _projectRepository.GetAll()
                      .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                      .Where(i => i.TenantId == AbpSession.TenantId && i.Type == ProjectConsts.ProjectType.Template)
                .ToList();

            Projects.ForEach(i =>
            {
                ProjectsList.Add(new GetProjectsForDashboardDto()
                {
                    Name = i.Name,
                    Status = i.Status,
                    Type = i.Type,
                    ProjectTemplateId = i.Id,
                    TotalCount = _projectRepository.GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(c => c.TenantId == AbpSession.TenantId && c.Type == ProjectConsts.ProjectType.User && c.ProjectTemplateId == i.Id).Count()
                });
            });

            return ProjectsList;
        }

        private List<GetProjectContributorForDashboardDto> GetWaitOthersList1(GetDashboardProjectDataInput input)
        {
            try
            {
                var contributors = from rc in _recordMatterContributorRepository.GetAll()
                                   join rm in _recordMatterRepository.GetAll()
                                   on rc.RecordMatterId equals rm.Id
                                   join p in _projectRepository.GetAll()
                                   on rm.RecordId equals p.RecordId
                                   where rc.CreatorUserId == AbpSession.UserId
                                       && rm.TenantId == AbpSession.TenantId
                                       && p.TenantId == AbpSession.TenantId
                                       && p.Type == ProjectConsts.ProjectType.User
                                       && (input.EnvironmentId == null || p.ProjectEnvironmentId == input.EnvironmentId)
                                   orderby rc.CreationTime descending, rc.Status
                                   select new GetProjectContributorForDashboardDto
                                   {
                                       Organization = rc.OrganizationName,
                                       Name = rc.Name,
                                       Role = Enum.GetName(typeof(ProjectConsts.ProjectStepRole), rc.StepRole),
                                       Email = rc.Email,
                                       ProjectName = p.Name,
                                       ProjectStepName = rm.RecordMatterName,
                                       Status = Enum.GetName(typeof(RecordMatterContributorConsts.RecordMatterContributorStatus), rc.Status),
                                       StatusCode = (int)rc.Status,
                                       Action = Enum.GetName(typeof(ProjectConsts.ProjectStepAction), rc.StepAction),
                                       ActionCode = (int)rc.StepAction,
                                       CreatedTime = rc.CreationTime,
                                       AccessToken = rc.AccessToken,
                                       RecordMatterId = (Guid)rc.RecordMatterId,
                                       FormId = rm.FormId,
                                       ProjectId = p.Id,
                                   };

                return contributors.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private List<GetProjectContributorForDashboardDto> GetWaitOthersList(GetDashboardProjectDataInput input)
        {
            try
            {
                var contributors = from rc in _recordMatterContributorRepository.GetAll()
                                   join rm in _recordMatterRepository.GetAll()
                                   on rc.RecordMatterId equals rm.Id
                                   join p in _projectRepository.GetAll()
                                   on rm.RecordId equals p.RecordId
                                   where rc.CreatorUserId == AbpSession.UserId
                                       && rm.TenantId == AbpSession.TenantId
                                       && p.TenantId == AbpSession.TenantId
                                       && p.Type == ProjectConsts.ProjectType.User
                                       && (input.EnvironmentId == null || p.ProjectEnvironmentId == input.EnvironmentId)
                                   orderby rc.CreationTime descending, rc.Status
                                   select new GetProjectContributorForDashboardDto
                                   {
                                       Organization = rc.OrganizationName,
                                       Name = rc.Name,
                                       Role = rc.StepRole.ToString(),
                                       Email = rc.Email,
                                       ProjectName = p.Name,
                                       ProjectStepName = rm.RecordMatterName,
                                       Status = rc.Status.ToString(),
                                       StatusCode = (int)rc.Status,
                                       Action = rc.StepAction.ToString(),
                                       ActionCode = (int)rc.StepAction,
                                       CreatedTime = rc.CreationTime,
                                       AccessToken = rc.AccessToken,
                                       RecordMatterId = (Guid)rc.RecordMatterId,
                                       FormId = rm.FormId,
                                       ProjectId = p.Id,
                                   };

                return contributors.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private List<GetProjectsForDashboardDto> GetProjectTemplatesList_old_old(GetDashboardProjectDataInput input)
        {
            try
            {
                User user = _userRepository.FirstOrDefault(u => u.Id == AbpSession.UserId);
                var ProjetListWithUsageCount = new List<GetProjectsForDashboardDto>();
                var projects = _projectRepository.GetAll() // proj -- 22 -- 22 ids har ek id match reasleas 
                     .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(i => i.TenantId == AbpSession.TenantId && i.Type == ProjectConsts.ProjectType.Template)
                    .ToList();

                //Calculate the total usage Count
                foreach (var project in projects)
                {
                    var usedReleaseIDs = _projectReleaseRepository.GetAll()
                    .Where(r => r.ProjectId == project.Id)
                    .Select(r => r.Id)
                    .ToList(); // 3

                    var projectwithUsageCount = new GetProjectsForDashboardDto
                    {
                        Name = project.Name,
                        Status = project.Status,
                        Type = project.Type,
                        ProjectTemplateId = project.Id,
                        TotalCount = _projectRepository.GetAll()
                        .Where(p => usedReleaseIDs.Contains(p.ProjectTemplateId.Value))
                        .Count()
                    };
                    ProjetListWithUsageCount.Add(projectwithUsageCount);
                }
                return ProjetListWithUsageCount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //10361_Dashboard Widget Enhancements Resolved and Refactored code
        private List<GetProjectsForDashboardDto> GetProjectTemplatesList(GetDashboardProjectDataInput input)
        {
            try
            {
                var userId = AbpSession.UserId;
                var tenantId = AbpSession.TenantId;
                var projects = _projectRepository.GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(i => i.TenantId == tenantId && (i.Type == ProjectConsts.ProjectType.Template || i.Type == ProjectConsts.ProjectType.User))
                    .ToList();
                var projectListWithUsageCount = projects.Select(project => new GetProjectsForDashboardDto
                {
                    Name = project.Name,
                    Status = project.Status,
                    Type = project.Type,
                    ProjectTemplateId = project.Id,
                    TotalCount = _projectRepository.GetAll()
                        .Count(p => _projectReleaseRepository
                            .GetAll()
                            .Where(r => r.ProjectId == project.Id)
                            .Select(r => r.Id)
                            .Contains(p.ProjectTemplateId.Value))
                }).ToList();
                return projectListWithUsageCount;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<GetProjectsForDashboardDto> GetProjectTemplatesUserCountList_OLD(GetDashboardProjectDataInput input)
        {
            var users = _userRepository.GetAll()
                .Where(ur => ur.TenantId == AbpSession.TenantId).ToList();
            List<GetProjectsForDashboardDto> ProjectTemplatesCountByUserList = new List<GetProjectsForDashboardDto>();
            users.ForEach(o =>
            {
                ProjectTemplatesCountByUserList.Add(new GetProjectsForDashboardDto()
                {
                    CreatorUserName = o.UserName,
                    CountByUser = _projectRepository.GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(c => c.TenantId == AbpSession.TenantId && c.Type == ProjectConsts.ProjectType.User && c.CreatorUserId == o.Id && c.ProjectTemplateId.ToString() == input.ProjectTemplateId).Count(),
                });
            });

            return ProjectTemplatesCountByUserList;
        }

        //10361_Dashboard Widget Enhancements Resolved and Refactored code
        private List<GetProjectsForDashboardDto> GetProjectTemplatesUserCountList(GetDashboardProjectDataInput input)
        {
            try
            {
                var users = _userRepository
                    .GetAll()
                    .Where(ur => ur.TenantId == AbpSession.TenantId)
                    .ToList();
                var userListNew = users.Select(user => new GetProjectsForDashboardDto
                {
                    CreatorUserName = user.UserName,
                    CountByUser = input.ProjectTemplateId != null
                                    ? _projectRepository.GetAll()
                                        .Count(p => _projectReleaseRepository.GetAll()
                                            .Where(r => r.ProjectId == Guid.Parse(input.ProjectTemplateId))
                                            .Select(r => r.Id)
                                            .Contains(p.ProjectTemplateId.Value) && p.CreatorUserId == user.Id)
                                    : _projectRepository.GetAll()
                                        .Count(p => p.TenantId == AbpSession.TenantId &&
                                                    p.Type == ProjectConsts.ProjectType.User &&
                                                    p.CreatorUserId == user.Id &&
                                                    (input.EnvironmentId == null || p.ProjectEnvironmentId == input.EnvironmentId))
                }).ToList();
                return userListNew;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<GetProjectsForDashboardDto> GetProjectsList_OLD(GetDashboardProjectDataInput input)
        {
            List<GetProjectsForDashboardDto> ProjectsList = new List<GetProjectsForDashboardDto>();
            var user = _userRepository.GetAll().Where(u => u.Id == AbpSession.UserId).FirstOrDefault();
            // admin can see all projects, tenant user can see own projects
            if (user.UserName == "admin")
            {
                List<Project> Projects = _projectRepository.GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(i => i.TenantId == AbpSession.TenantId && i.Type == ProjectConsts.ProjectType.User).ToList();
                Projects.ForEach(i =>
                {
                    ProjectsList.Add(new GetProjectsForDashboardDto()
                    {
                        Name = i.Name,
                        Status = i.Status,
                        Type = i.Type
                    });
                });

                return ProjectsList;

            }
            else
            {
                List<Project> Projects = _projectRepository
                    .GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(i => i.CreatorUserId == AbpSession.UserId && i.Type == ProjectConsts.ProjectType.User).ToList();
                Projects.ForEach(i =>
                {
                    ProjectsList.Add(new GetProjectsForDashboardDto()
                    {
                        Name = i.Name,
                        Status = i.Status,
                        Type = i.Type
                    });
                });

                return ProjectsList;
            }
        }

        // Get the list of projects
        private List<GetProjectsForDashboardDto> GetProjectsList(GetDashboardProjectDataInput input)
        {
            User user = _userRepository.FirstOrDefault(u => u.Id == AbpSession.UserId);

            var projects = _projectRepository.GetAll()
                .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                .Where(i => i.Type == ProjectConsts.ProjectType.User);

            // admin can see all projects, tenant user can see own projects
            projects = (user.UserName == "admin") ? projects.Where(i => i.TenantId == AbpSession.TenantId) :
                projects.Where(i => i.CreatorUserId == AbpSession.UserId);

            return projects.Select(i => new GetProjectsForDashboardDto
            {
                Name = i.Name,
                Status = i.Status,
                Type = i.Type
            }).ToList();
        }

        private int GetArchivedTotalCount_OLD(GetDashboardProjectDataInput input)
        {
            var user = _userRepository.GetAll().Where(u => u.Id == AbpSession.UserId).FirstOrDefault();
            if (user.UserName == "admin")
            {
                return _projectRepository.GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(i => i.TenantId == AbpSession.TenantId && i.Type == ProjectConsts.ProjectType.User && i.Archived == true).Count();
            }
            else
            {
                return _projectRepository.GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(i => i.CreatorUserId == AbpSession.UserId && i.Type == ProjectConsts.ProjectType.User && i.Archived == true).Count();
            }
        }

        // Get Archived Total Count
        private int GetArchivedTotalCount(GetDashboardProjectDataInput input)
        {
            User user = _userRepository.FirstOrDefault(u => u.Id == AbpSession.UserId);

            var projects = _projectRepository.GetAll()
                .WhereIf(input.EnvironmentId != null, p => p.ProjectEnvironmentId == input.EnvironmentId)
                .Where(p => p.Type == ProjectConsts.ProjectType.User && p.Archived);

            if (user.UserName == "admin")
            {
                return projects.Where(p => p.TenantId == AbpSession.TenantId).Count();
            }
            else
            {
                return projects.Where(p => p.CreatorUserId == AbpSession.UserId).Count();
            }
        }

        private int GetProjectTotalCount_OLD(GetDashboardProjectDataInput input)
        {
            var user = _userRepository.GetAll().Where(u => u.Id == AbpSession.UserId).FirstOrDefault();
            if (user.UserName == "admin")
            {

                return _projectRepository.GetAll()
                    .Where(i => i.TenantId == AbpSession.TenantId && i.Type == ProjectConsts.ProjectType.User)
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Count();

            }
            else
            {
                return _projectRepository.GetAll()
                          .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                          .Where(i => i.CreatorUserId == AbpSession.UserId && i.Type == ProjectConsts.ProjectType.User).Count();
            }

        }

        private int GetProjectTotalCount(GetDashboardProjectDataInput input)
        {
            var user = _userRepository.FirstOrDefault(u => u.Id == AbpSession.UserId);
            var projects = _projectRepository.GetAll()
                .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId);

            if (user.UserName == "admin")
            {
                projects = projects.Where(i => i.TenantId == AbpSession.TenantId && i.Type == ProjectConsts.ProjectType.User);
            }
            else
            {
                projects = projects.Where(i => i.CreatorUserId == AbpSession.UserId && i.Type == ProjectConsts.ProjectType.User);
            }

            return projects.Count();
        }

        private List<string> GetProjectStatusNamesList_OLD()
        {
            List<string> ProjectStatusList = new List<string>();

            foreach (string Statu in Enum.GetNames(typeof(ProjectConsts.ProjectStatus)))
            {
                var status = Statu;
                if (Statu == "InProgress")
                {
                    status = "In Progress";
                }
                ProjectStatusList.Add(status);
            }

            return ProjectStatusList;
        }

        // Get List of Project Status Name 
        private List<string> GetProjectStatusNamesList()
        {
            return Enum.GetNames(typeof(ProjectConsts.ProjectStatus))
                .Select(name => name == "InProgress" ? "In Progress" : name).ToList();
        }

        ////Recent Activities
        //private List<GetProjectsRecentDocumentForDashboardDto> GetRecentProjectStatusNamesList(GetDashboardProjectDataInput input)
        //{
        //	List<GetProjectsRecentDocumentForDashboardDto> RecentProjectsList = new List<GetProjectsRecentDocumentForDashboardDto>();
        //	var user = _userRepository.GetAll().Where(i => i.Id == AbpSession.UserId && i.TenantId == AbpSession.TenantId).FirstOrDefault();
        //	// 1. check own project   Project join RecordMatter
        //	var ownProjects = _projectRepository.GetAll()
        //		.WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
        //		.Where(i => i.CreatorUserId == AbpSession.UserId && i.Type == ProjectConsts.ProjectType.User && i.Status != 0)
        //		.ToList()

        //		.Join(_recordMatterRepository.GetAll().Where(rm => rm.TenantId == AbpSession.TenantId && rm.Status != 0),
        //		Project => Project.RecordId,
        //		RecordMatter => RecordMatter.RecordId,
        //		(Project, RecordMatter) => new
        //		{
        //			ProjectId = Project.Id,
        //			Project.Name,
        //			Project.Status,
        //			RecordMatter.RecordMatterName,
        //			RecordMatter.LastModificationTime,
        //			RecordMatter.CreationTime,
        //			RecordMatter.FormId,
        //			RecordMatterId = RecordMatter.Id,
        //			Type = 'O'
        //		}
        //		).ToList();


        //	List<Guid> invalidsteps = new List<Guid>();

        //	ownProjects.ForEach(o =>
        //	{
        //		if (!ValidateRecordMatterStep(o.RecordMatterId))
        //		{
        //			invalidsteps.Add(o.RecordMatterId);
        //		}

        //	});

        //	foreach (Guid invalidstep in invalidsteps)
        //	{
        //		ownProjects.RemoveAll(o => o.RecordMatterId == invalidstep);

        //	}

        //	//LastModificationTime = (DateTime)o.LastModificationTime.GetValueOrDefault(),
        //	ownProjects.ForEach(o =>
        //	{


        //		RecentProjectsList.Add(new GetProjectsRecentDocumentForDashboardDto()
        //		{
        //			ProjectId = o.ProjectId,
        //			ProjectName = o.Name,
        //			Status = Enum.GetName(typeof(ProjectConsts.ProjectStatus), o.Status),
        //			DocumentName = o.RecordMatterName,
        //			LastModificationTime = (DateTime)o.LastModificationTime.GetValueOrDefault(),
        //			CreationTime = (DateTime)o.CreationTime,
        //			RecordMatterId = o.RecordMatterId,
        //			FormId = o.FormId,

        //		});
        //	});

        //	var RecentDocumentsList = RecentProjectsList.OrderByDescending(p => p.LastModificationTime).Take(10).ToList();
        //	RecentDocumentsList.ForEach(d =>
        //	{
        //		var recordmatteritem = _lookup_recordMatterItemRepository.GetAll().Where(e => e.RecordMatterId == d.RecordMatterId).FirstOrDefault();
        //		if (recordmatteritem != null)
        //		{
        //			d.FormId = (Guid)recordmatteritem.FormId;
        //			d.RecordMatterItemId = recordmatteritem.Id;
        //		}
        //		else
        //		{
        //			d.RecordMatterItemId = new Guid();
        //		}
        //	});

        //	return RecentDocumentsList;

        //}

        private List<GetProjectsRecentDocumentForDashboardDto> GetRecentProjectStatusNamesList_OLD(GetDashboardProjectDataInput input)
        {
            List<GetProjectsRecentDocumentForDashboardDto> RecentProjectsList = new List<GetProjectsRecentDocumentForDashboardDto>();
            var user = _userRepository.GetAll().Where(i => i.Id == AbpSession.UserId && i.TenantId == AbpSession.TenantId).FirstOrDefault();

            var ownProjects = _projectRepository.GetAll()
                .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                .Where(i => i.CreatorUserId == AbpSession.UserId && i.Type == ProjectConsts.ProjectType.User && i.Status != 0)
                .ToList()
                .Join(_recordMatterRepository.GetAll()
                .Where(rm => rm.TenantId == AbpSession.TenantId && rm.CreatorUserId == AbpSession.UserId && rm.Status != 0),
                    Project => Project.RecordId,
                    RecordMatter => RecordMatter.RecordId,
                    (Project, RecordMatter) => new
                    {
                        ProjectId = Project.Id,
                        Project.Name,
                        Project.Status,
                        RecordMatter.RecordMatterName,
                        RecordMatter.LastModificationTime,
                        RecordMatter.CreationTime,
                        RecordMatter.FormId,
                        RecordMatterId = RecordMatter.Id,
                        Type = 'O'
                    }
                ).ToList();

            //List<Guid> invalidsteps = new List<Guid>();

            //ownProjects.ForEach(o =>
            //{
            //    if (!ValidateRecordMatterStep(o.RecordMatterId))
            //    {
            //        invalidsteps.Add(o.RecordMatterId);
            //    }
            //});

            //foreach (Guid invalidstep in invalidsteps)
            //{
            //    ownProjects.RemoveAll(o => o.RecordMatterId == invalidstep);
            //}

            ownProjects.ForEach(o =>
            {
                RecentProjectsList.Add(new GetProjectsRecentDocumentForDashboardDto()
                {
                    ProjectId = o.ProjectId,
                    ProjectName = o.Name,
                    Status = Enum.GetName(typeof(ProjectConsts.ProjectStatus), o.Status),
                    DocumentName = o.RecordMatterName,
                    LastModificationTime = (DateTime)o.LastModificationTime.GetValueOrDefault(),
                    CreationTime = (DateTime)o.CreationTime,
                    RecordMatterId = o.RecordMatterId,
                    FormId = o.FormId,
                });
            });

            var RecentDocumentsList = RecentProjectsList.OrderByDescending(p => p.LastModificationTime).Take(10).ToList();
            RecentDocumentsList.ForEach(d =>
            {
                var recordmatteritem = _lookup_recordMatterItemRepository.GetAll().Where(e => e.RecordMatterId == d.RecordMatterId).FirstOrDefault();
                if (recordmatteritem != null)
                {
                    d.FormId = (Guid)recordmatteritem.FormId;
                    d.RecordMatterItemId = recordmatteritem.Id;
                }
                else
                {
                    d.RecordMatterItemId = new Guid();
                }
            });

            return RecentDocumentsList;
        }

        private List<GetProjectsRecentDocumentForDashboardDto> GetRecentProjectStatusNamesList(GetDashboardProjectDataInput input)
        {
            try
            {
                var user = _userRepository.FirstOrDefault(i => i.Id == AbpSession.UserId && i.TenantId == AbpSession.TenantId);
                if (user == null) return null;

                var RecentProjectsList = _projectRepository.GetAll()
                                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                                    .Where(i => i.CreatorUserId == AbpSession.UserId && i.Type == ProjectConsts.ProjectType.User && i.Status != 0)
                                    .ToList()
                                    .Join(_recordMatterRepository.GetAll()
                                            .Where(rm => rm.TenantId == AbpSession.TenantId && rm.CreatorUserId == AbpSession.UserId && rm.Status != 0),
                                        project => project.RecordId,
                                        recordMatter => recordMatter.RecordId,
                                        (project, recordMatter) => new GetProjectsRecentDocumentForDashboardDto()
                                        {
                                            ProjectId = project.Id,
                                            ProjectName = project.Name,
                                            Status = Enum.GetName(typeof(ProjectConsts.ProjectStatus), project.Status),
                                            DocumentName = recordMatter.RecordMatterName,
                                            LastModificationTime = recordMatter.LastModificationTime ?? recordMatter.CreationTime,
                                            CreationTime = recordMatter.CreationTime,
                                            RecordMatterId = recordMatter.Id,
                                            FormId = recordMatter.FormId,
                                            Type = 'O'
                                        }
                                    )
                                    .OrderByDescending(p => p.LastModificationTime)
                                    .Take(10)
                                    .ToList();

                RecentProjectsList.ForEach(d =>
                {
                    var recordmatteritem = _lookup_recordMatterItemRepository.FirstOrDefault(e => e.RecordMatterId == d.RecordMatterId);
                    d.FormId = recordmatteritem != null? (Guid)recordmatteritem.FormId : Guid.Empty;
                    d.RecordMatterItemId = recordmatteritem != null ? recordmatteritem.Id : Guid.Empty;
                });

                return RecentProjectsList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private List<GetProjectsRecentDocumentForDashboardDto> GetRecentProjectStatusNamesList1(GetDashboardProjectDataInput input)
        {
            try
            {
                User user = _userRepository.GetAll().FirstOrDefault(i => i.Id == AbpSession.UserId && i.TenantId == AbpSession.TenantId);

                var ownProjects = _projectRepository.GetAll()
                    .WhereIf(input.EnvironmentId != null, i => i.ProjectEnvironmentId == input.EnvironmentId)
                    .Where(i => i.CreatorUserId == AbpSession.UserId && i.Type == ProjectConsts.ProjectType.User && i.Status != 0)
                    .Join(_recordMatterRepository.GetAll()
                    .Where(rm => rm.TenantId == AbpSession.TenantId && rm.CreatorUserId == AbpSession.UserId && rm.Status != 0),
                        Project => Project.RecordId,
                        RecordMatter => RecordMatter.RecordId,
                        (Project, RecordMatter) => new GetProjectsRecentDocumentForDashboardDto()
                        {
                            ProjectId = Project.Id,
                            ProjectName = Project.Name,
                            Status = Enum.GetName(typeof(ProjectConsts.ProjectStatus), Project.Status),
                            DocumentName = RecordMatter.RecordMatterName,
                            LastModificationTime = (DateTime)RecordMatter.LastModificationTime.GetValueOrDefault(),
                            CreationTime = (DateTime)RecordMatter.CreationTime,
                            RecordMatterId = RecordMatter.Id,
                            FormId = RecordMatter.FormId,
                            Type = 'O'
                        }
                    ).ToList();

                var RecentDocumentsList = ownProjects.OrderByDescending(p => p.LastModificationTime).ToList();
                RecentDocumentsList.ForEach(d =>
                {
                    var recordmatteritem = _lookup_recordMatterItemRepository.GetAll().FirstOrDefault(e => e.RecordMatterId == d.RecordMatterId);
                    d.FormId = recordmatteritem?.FormId ?? d.FormId;
                    d.RecordMatterItemId = recordmatteritem?.Id ?? new Guid();
                });

                return RecentDocumentsList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //      private Boolean ValidateRecordMatterStep(Guid recordmatterId)
        //{

        //	var result = true;
        //	var rm = _recordMatterRepository.FirstOrDefault(r => r.Id == recordmatterId);

        //	if (!string.IsNullOrEmpty(rm.Filter))
        //	{
        //		var data = rm.Data;

        //		if (string.IsNullOrEmpty(data))
        //		{
        //			data = "{}";
        //		}

        //		try
        //		{

        //			XmlNode node = null;
        //			try
        //			{
        //				XmlDocument doc = new XmlDocument();
        //				doc.LoadXml(JsonConvert.DeserializeXNode(data, "Data").ToString());
        //				node = doc.DocumentElement.SelectSingleNode(rm.Filter);

        //				if (node == null) result = false;

        //			}
        //			catch (Exception) { result = false; }


        //		}
        //		catch
        //		{
        //			result = true;
        //		}
        //	}
        //	return result;

        //}


        private List<string> GetProjectActionNamesList_OLD()
        {
            List<string> ProjectActionsList = new List<string>();

            foreach (string Action in Enum.GetNames(typeof(ProjectConsts.ProjectStepRole)))
            {
                if (Action == "Review" || Action == "Approve")
                {
                    ProjectActionsList.Add(Action);
                }
            }
            return ProjectActionsList;
        }

        // Get List of Project Action Names
        private List<string> GetProjectActionNamesList()
        {
            string[] validActions = new string[] { "Review", "Approve" };

            return Enum.GetNames(typeof(ProjectConsts.ProjectStepRole))
                .Where(action => validActions.Contains(action))
                .ToList();
        }


        // return all working on project filter by tenant ID
        private List<string> GetProjectContributorData()
        {
            var contributorData = _recordMatterContributorRepository.GetAll().Where(i => i.TenantId == AbpSession.TenantId);

            return new List<string>();
        }


        private int GetMonthlySubmissions(GetDashboardDataInput input)
        {
            return _submissionRepository.GetAll().Where(i => (i.LastModificationTime ?? i.CreationTime) > input.StartDate & (i.LastModificationTime ?? i.CreationTime) < input.EndDate).Count();
        }

        public GetProfitShareOutput GetProfitShare()
        {
            return new GetProfitShareOutput
            {
                ProfitShares = DashboardRandomDataGenerator.GetRandomPercentageArray(3)
            };
        }

        public GetDailySalesOutput GetDailySales()
        {
            return new GetDailySalesOutput
            {
                DailySales = DashboardRandomDataGenerator.GetRandomArray(30, 10, 50)
            };
        }

        public GetSalesSummaryOutput GetSalesSummary(GetSalesSummaryInput input)
        {
            var salesSummary = DashboardRandomDataGenerator.GenerateSalesSummaryData(input.SalesSummaryDatePeriod);
            return new GetSalesSummaryOutput(salesSummary)
            {
                Expenses = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                Growth = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                Revenue = DashboardRandomDataGenerator.GetRandomInt(0, 3000),
                TotalSales = DashboardRandomDataGenerator.GetRandomInt(0, 3000)
            };
        }


        //public GetRegionalStatsOutput GetRegionalStats(GetRegionalStatsInput input)
        //{
        //    return new GetRegionalStatsOutput(DashboardRandomDataGenerator.GenerateRegionalStat());
        //}

        //public GetGeneralStatsOutput GetGeneralStats(GetGeneralStatsInput input)
        //{
        //    return new GetGeneralStatsOutput
        //    {
        //        TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
        //        NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
        //        BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100)
        //    };
        //}

        public GetTopStatsOutput GetTopStats()
        {
            return new GetTopStatsOutput
            {
                TotalProfit = DashboardRandomDataGenerator.GetRandomInt(5000, 9000),
                NewFeedbacks = DashboardRandomDataGenerator.GetRandomInt(1000, 5000),
                NewOrders = DashboardRandomDataGenerator.GetRandomInt(100, 900),
                NewUsers = DashboardRandomDataGenerator.GetRandomInt(50, 500)
            };
        }

        public GetRegionalStatsOutput GetRegionalStats()
        {
            return new GetRegionalStatsOutput(
                DashboardRandomDataGenerator.GenerateRegionalStat()
            );
        }

        public GetGeneralStatsOutput GetGeneralStats()
        {
            return new GetGeneralStatsOutput
            {
                TransactionPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                NewVisitPercent = DashboardRandomDataGenerator.GetRandomInt(10, 100),
                BouncePercent = DashboardRandomDataGenerator.GetRandomInt(10, 100)
            };
        }

        public GetMemberActivityOutput GetMemberActivity()
        {
            return new GetMemberActivityOutput
            (
                DashboardRandomDataGenerator.GenerateMemberActivities()
            );
        }

        //public async Task<GetSubmissionLimitOutput> GetSubmissionLimit_OLD(GetSubmissionLimitInput input)
        //{
        //    int submissionLimit = Convert.ToInt32(await FeatureChecker.GetValueAsync(AppFeatures.SubmissionLimitAmount));
        //    int currentSubmissions = _submissionRepository.GetAll().Where(i => (i.LastModificationTime ?? i.CreationTime) > input.StartDate 
        //    & (i.LastModificationTime ?? i.CreationTime) < input.EndDate).Count();
        //    return new GetSubmissionLimitOutput
        //    {
        //        SubmissionLimit = submissionLimit,
        //        CurrentSubmissions = currentSubmissions,
        //        SubmissionUsagePercent = (int)Math.Round((double)(100 * currentSubmissions) / submissionLimit)
        //    };
        //}

        public async Task<GetSubmissionLimitOutput> GetSubmissionLimit(GetSubmissionLimitInput input)
        {
            int submissionLimit = Convert.ToInt32(await FeatureChecker.GetValueAsync(AppFeatures.SubmissionLimitAmount));
            int currentSubmissions = _submissionRepository.GetAll().Count(i => (i.LastModificationTime ?? i.CreationTime) > input.StartDate
            && (i.LastModificationTime ?? i.CreationTime) < input.EndDate);
            int submissionUsagePercent = submissionLimit > 0 ? (int)Math.Round((double)currentSubmissions / submissionLimit * 100) : 0;

            return new GetSubmissionLimitOutput
            {
                SubmissionLimit = submissionLimit,
                CurrentSubmissions = currentSubmissions,
                SubmissionUsagePercent = submissionUsagePercent
            };
        }

        public GetTodaySubmissionsOutput GetTodaySubmissions(GetTodaySubmissionsInput input)
        {

            int TodaysSubmissions = _submissionRepository.GetAll()
                .Where(i => (i.LastModificationTime ?? i.CreationTime) > input.TodaysStartDate & (i.LastModificationTime ?? i.CreationTime) < input.TodaysEndDate)
                .Count();

            int YesterdaysSubmissions = _submissionRepository.GetAll()
                .Where(i => (i.LastModificationTime ?? i.CreationTime) > input.YesterdaysStartDate & (i.LastModificationTime ?? i.CreationTime) < input.YesterdaysEndDate)
                .Count();

            return new GetTodaySubmissionsOutput
            {
                TodaysSubmissions = TodaysSubmissions,
                YesterdaysSubmissions = YesterdaysSubmissions
            };
        }

        public GetNewUsersOutput GetNewUsersForWidget(GetDashboardDataInput input)
        {
            int NewUsers = _userRepository.GetAll().Where(i => (i.LastModificationTime ?? i.CreationTime) > input.StartDate & (i.LastModificationTime ?? i.CreationTime) < input.EndDate).Count();

            return new GetNewUsersOutput
            {
                NewUsers = NewUsers
            };
        }

        public int GetDocumentsStatusCountForWidget(GetDocumentsStatusCountInput input)
        {
            var user = _userRepository.GetAll().Where(u => u.Id == AbpSession.UserId).FirstOrDefault();
            // Check if a user is admin?  TODO: check roles permission.
            char userType = 'U';
            if (user.UserName == "admin")
            {
                userType = 'A';
            }

            int count = 0;
            switch (input.statusType)
            {
                case 'D':
                    count = GetDocumentsStatusCount(RecordMatterConsts.RecordMatterStatus.Draft, input.EnvironmentId, userType);  // Done by M
                    break;
                case 'F':
                    count = GetDocumentsStatusCount(RecordMatterConsts.RecordMatterStatus.Final, input.EnvironmentId, userType);    // Done by M
                    break;
                case 'N':
                    count = GetDocumentsStatusCount(RecordMatterConsts.RecordMatterStatus.New, input.EnvironmentId, userType);  // Done by M
                    break;
                case 'P':
                    count = GetDocumentsStatusCount(RecordMatterConsts.RecordMatterStatus.Share, input.EnvironmentId, userType);    // Done by M
                    break;
                case 'A':
                    count = GetAllDocumentsStatusCount(input.EnvironmentId, userType);
                    break;
                default:
                    break;
            }
            return count;
        }

        private int GetDocumentsStatusCount_Old(RecordMatterConsts.RecordMatterStatus status, Guid? environmentId, char userType = 'U')
        {
            int count = 0;
            if (userType == 'U')
            {
                count = _projectRepository.GetAll()

                    .WhereIf(environmentId != null, i => i.ProjectEnvironmentId == (Guid)environmentId)
                    .Where(p => p.CreatorUserId == AbpSession.UserId && p.Type == ProjectConsts.ProjectType.User)

                    .ToList()
                    .Join(_recordMatterRepository.GetAll().Where(rm => rm.CreatorUserId == AbpSession.UserId && rm.Status == status),
                   Project => Project.RecordId,
                   RecordMatter => RecordMatter.RecordId,
                   (Project, RecordMatter) => new
                   {
                       RecordMatter.Id
                   }).Count();
            }
            else if (userType == 'A')
            {
                count = _projectRepository.GetAll()
                    .WhereIf(environmentId != null, i => i.ProjectEnvironmentId == (Guid)environmentId)
                    .Where(p => p.TenantId == AbpSession.TenantId && p.Type == ProjectConsts.ProjectType.User)

                    .ToList()
                    .Join(_recordMatterRepository.GetAll().Where(rm => rm.TenantId == AbpSession.TenantId && rm.Status == status),
                   Project => Project.RecordId,
                   RecordMatter => RecordMatter.RecordId,
                   (Project, RecordMatter) => new
                   {
                       RecordMatter.Id
                   }).Count();
            }
            return count;
        }
       
        private int GetDocumentsStatusCount(RecordMatterConsts.RecordMatterStatus status, Guid? environmentId, char userType = 'U')
        {
            try
            {
                var query = userType == 'U'
                     ? _projectRepository.GetAll()
                         .WhereIf(environmentId != null, i => i.ProjectEnvironmentId == environmentId)
                         .Where(p => p.CreatorUserId == AbpSession.UserId && p.Type == ProjectConsts.ProjectType.User)
                         .ToList()
                         .Join(_recordMatterRepository.GetAll()
                             .Where(rm => rm.CreatorUserId == AbpSession.UserId && rm.Status == status),
                             p => p.RecordId,
                             rm => rm.RecordId,
                             (p, rm) => rm.Id)
                     : userType == 'A'
                         ? _projectRepository.GetAll()
                             .WhereIf(environmentId != null, i => i.ProjectEnvironmentId == environmentId)
                             .Where(p => p.TenantId == AbpSession.TenantId && p.Type == ProjectConsts.ProjectType.User)
                             .ToList()
                             .Join(_recordMatterRepository.GetAll()
                                 .Where(rm => rm.TenantId == AbpSession.TenantId && rm.Status == status),
                                 p => p.RecordId,
                                 rm => rm.RecordId,
                                 (p, rm) => rm.Id)
                     : Enumerable.Empty<Guid>();
                return query.Count();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        private int GetAllDocumentsStatusCount_OLD(Guid? environmentId, char userType = 'U')
        {

            int count = 0;

            if (userType == 'U')
            {
                count = _projectRepository.GetAll()

                    .WhereIf(environmentId != null, i => i.ProjectEnvironmentId == (Guid)environmentId)
                   .Where(p => p.CreatorUserId == AbpSession.UserId && p.Type == ProjectConsts.ProjectType.User)

                   .ToList()
                   .Join(_recordMatterRepository.GetAll().Where(rm => rm.CreatorUserId == AbpSession.UserId && rm.Status != null),
                   Project => Project.RecordId,
                   RecordMatter => RecordMatter.RecordId,
                   (Project, RecordMatter) => new
                   {
                       RecordMatter.Id
                   }).Count();
            }
            else if (userType == 'A')
            {
                count = _projectRepository.GetAll()

                    .WhereIf(environmentId != null, i => i.ProjectEnvironmentId == (Guid)environmentId)
                   .Where(p => p.TenantId == AbpSession.TenantId && p.Type == ProjectConsts.ProjectType.User)

                   .ToList()
                   .Join(_recordMatterRepository.GetAll().Where(rm => rm.TenantId == AbpSession.TenantId && rm.Status != null),
                   Project => Project.RecordId,
                   RecordMatter => RecordMatter.RecordId,
                   (Project, RecordMatter) => new
                   {
                       RecordMatter.Id
                   }).Count();
            }

            return count;
        }

        private int GetAllDocumentsStatusCount(Guid? environmentId, char userType = 'U')
        {
            var projects = _projectRepository.GetAll()
                .WhereIf(environmentId != null, i => i.ProjectEnvironmentId == environmentId.Value)
                .Where(p => (userType == 'U' && p.CreatorUserId == AbpSession.UserId) || (userType == 'A' && p.TenantId == AbpSession.TenantId))
                .Where(p => p.Type == ProjectConsts.ProjectType.User)
                .ToList();

            var recordMatters = _recordMatterRepository.GetAll()
                .Where(rm => (userType == 'U' && rm.CreatorUserId == AbpSession.UserId) || (userType == 'A' && rm.TenantId == AbpSession.TenantId))
                .Where(rm => rm.Status != null)
                .ToList();

            var count = projects
                .Join(recordMatters,
                    p => p.RecordId,
                    rm => rm.RecordId,
                    (p, rm) => new { rm.Id })
                .Count();

            return count;
        }

        private int GetYesterdaysSubmissions(GetDashboardDataInput input)
        {
            return _submissionRepository.GetAll().Where(i => (i.LastModificationTime ?? i.CreationTime) > input.YesterdaysStartDate & (i.LastModificationTime ?? i.CreationTime) < input.YesterdaysEndDate).Count();
        }

        private int GetTodaysSubmissions(GetDashboardDataInput input)
        {
            return _submissionRepository.GetAll().Where(i => (i.LastModificationTime ?? i.CreationTime) > input.TodaysStartDate & (i.LastModificationTime ?? i.CreationTime) < input.TodaysEndDate).Count();
        }

        public int GetNewUsers(GetDashboardDataInput input)
        {
            return _userRepository.GetAll().Where(i => (i.LastModificationTime ?? i.CreationTime) > input.StartDate & (i.LastModificationTime ?? i.CreationTime) < input.EndDate).Count();
        }

        //public List<DashboardRecentSubmissions> GetRecentSubmissions_OLD(GetDashboardDataInput input)
        //{

        //    _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

        //    List<DashboardRecentSubmissions> RecentSubmissions = new List<DashboardRecentSubmissions>();
        //    List<Submission> Submissions = _submissionRepository.GetAll()
        //                                    .Include(e => e.RecordFk).Include(e => e.FormFk)
        //                                    .Where(i => (i.LastModificationTime ?? i.CreationTime) > input.TodaysStartDate & (i.LastModificationTime ?? i.CreationTime) < input.TodaysEndDate && i.TenantId == AbpSession.TenantId)
        //                                    .OrderByDescending(i => i.LastModificationTime ?? i.CreationTime).Take(12).ToList();
        //    Submissions.ForEach(i =>
        //    {
        //        // Temporary solution, TODO: need to check(update) all relevant tables([SfaSubmissions][SfaRecords] "IsDeleted") when a record has been deleted.
        //        RecentSubmissions.Add(new DashboardRecentSubmissions()
        //        {
        //            Id = i.Id,
        //            Display = (i.FormFk == null ? "[The Form has been deleted]" : i.FormFk.Name) + " - " + (i.RecordFk == null ? "[The Record has been deleted]" : i.RecordFk.RecordName),
        //            // Display = (i.FormFk == null ? "" : i.FormFk.Name) + " " + (i.RecordFk == null ? "" : i.RecordFk.RecordName),
        //            Status = i.SubmissionStatus,
        //            Time = i.LastModificationTime ?? i.CreationTime
        //        });
        //    });
        //    return RecentSubmissions;
        //}

        public List<DashboardRecentSubmissions> GetRecentSubmissions(GetDashboardDataInput input)
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var Submissions = _submissionRepository.GetAll()
                                    .Include(e => e.RecordFk).Include(e => e.FormFk)
                                    .Where(i => (i.LastModificationTime ?? i.CreationTime) > input.TodaysStartDate && (i.LastModificationTime ?? i.CreationTime)
                                    < input.TodaysEndDate && i.TenantId == AbpSession.TenantId)
                                    .OrderByDescending(i => i.LastModificationTime ?? i.CreationTime).Take(12);

            return Submissions.Select(i => new DashboardRecentSubmissions()
            {
                Id = i.Id,
                Display = (i.FormFk == null ? "[The Form has been deleted]" : i.FormFk.Name) + " - " + (i.RecordFk == null ? "[The Record has been deleted]" : i.RecordFk.RecordName),
                Status = i.SubmissionStatus,
                Time = i.LastModificationTime ?? i.CreationTime
            }).ToList();
        }
    }
}