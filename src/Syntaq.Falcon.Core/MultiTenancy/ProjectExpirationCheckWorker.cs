using System;
using System.Linq;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Timing;
using Syntaq.Falcon.Projects;
using Abp.Domain.Uow;
using static Syntaq.Falcon.Projects.ProjectConsts;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Submissions;
using System.Collections.Generic;
using Abp.Threading;
using System.Diagnostics;
using Syntaq.Falcon.Configuration;
using Abp.Auditing;
using Abp.UI;
using static Syntaq.Falcon.RecordPolicyActions.RecordPolicyActionConsts;
using Syntaq.Falcon.RecordPolicies;
using Syntaq.Falcon.RecordPolicyActions;
using Syntaq.Falcon.Authorization.Users;


namespace Syntaq.Falcon.MultiTenancy
{
    [Audited]
    public class ProjectExpirationCheckWorker : BackgroundJob<int>, ITransientDependency
    {
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<ACL> _aclRepository;
        private readonly IRepository<Record, Guid> _recordRepository;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
        private readonly IRepository<RecordMatterAudit, Guid> _recordMatterAuditRepository;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;
        private readonly IRepository<Submission, Guid> _submissionRepository;
        private readonly IRepository<RecordPolicyAction, Guid> _recordPolicyActionRepository;
        private readonly IRepository<RecordPolicy, Guid> _recordPolicyRepository;
        private readonly IRepository<RecordMatterItemHistory, Guid> _recordMatterItemHistoryRepository;
        private readonly UserEmailer _userEmailer;

        public ProjectExpirationCheckWorker(
            IRepository<RecordMatterItemHistory, Guid> recordMatterItemHistoryRepository,
            IRepository<RecordPolicy, Guid> recordPolicyRepository,
            IRepository<Tenant> tenantRepository,
            IUnitOfWorkManager unitOfWorkManager,

            IRepository<ACL> aclRepository,
            IRepository<Record, Guid> recordRepository,
            IRepository<RecordMatter, Guid> recordMatterRepository,
            IRepository<RecordMatterAudit, Guid> recordMatterAuditRepository,
            IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
            IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository,
            IRepository<Submission, Guid> submissionRepository,
            IRepository<RecordPolicyAction, Guid> recordPolicyActionRepository,
            UserEmailer userEmailer,

        IRepository<Project, Guid> projectRepository)
        {
            _recordMatterItemHistoryRepository = recordMatterItemHistoryRepository;
            _tenantRepository = tenantRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _recordPolicyRepository = recordPolicyRepository;

            _recordRepository = recordRepository;
            _recordMatterRepository = recordMatterRepository;
            _recordMatterAuditRepository = recordMatterAuditRepository;
            _recordMatterItemRepository = recordMatterItemRepository;
            _recordMatterContributorRepository = recordMatterContributorRepository;

            _projectRepository = projectRepository;
            _submissionRepository = submissionRepository;
            _aclRepository = aclRepository;
            _recordPolicyActionRepository = recordPolicyActionRepository;
            _userEmailer = userEmailer;

        }

       
        [UnitOfWork]
        public override void Execute(int number)
        {
            //step1: fetch all tenant
            //step2: find all policies in this tenant, and if there are projects meet the rules
           
            var utcNow = Clock.Now.ToUniversalTime();

            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete, AbpDataFilters.MayHaveTenant))
            {

                var tenantList = _tenantRepository.GetAll().ToList();
                foreach (var tenant in tenantList)
                {
                    var softdeleteprojects = new List<string>();
                    var harddeleteprojects = new List<string>();
                    var achiveprojects = new List<string>();

                    try
                    {
                        var recordpolicy = _recordPolicyRepository.FirstOrDefault(rp => rp.AppliedTenantId == tenant.Id);

                        if (recordpolicy == null)
                        {
                            recordpolicy = _recordPolicyRepository.FirstOrDefault(rp => rp.AppliedTenantId == -1);
                        }

                        if (recordpolicy != null)
                        {
                            var recorddeletepolicies = _recordPolicyActionRepository.GetAll().Where(rdp => rdp.RecordPolicyId == recordpolicy.Id).ToList();
                            //??forach if recordpolicy is not null, recorddeletepoliucy is null??? what happened

                            // assign the variable to check whether all recorddeletepolices are active or not
                            bool isPoliciesActive = false;
                            foreach (var recorddeletepolicy in recorddeletepolicies)
                            {
                                if (recorddeletepolicy.Active == true)
                                {
                                    switch (recorddeletepolicy.Type)
                                    {
                                        case RecordPolicyActionType.SoftDelete:
                                            softdeleteprojects = softdeleteRecord(tenant.Id, recorddeletepolicy, softdeleteprojects);
                                            break;
                                        case RecordPolicyActionType.HardDelete:
                                            harddeleteprojects = hardDeleteRecord(tenant.Id, recorddeletepolicy, harddeleteprojects);
                                            break;
                                        case RecordPolicyActionType.Archive:
                                            achiveprojects = archiveRecord(tenant.Id, recorddeletepolicy, achiveprojects);
                                            break;
                                    }

                                    // If any active 
                                    if (softdeleteprojects.Count > 0 || harddeleteprojects.Count > 0 || achiveprojects.Count > 0)
                                        isPoliciesActive = !isPoliciesActive;
                                }
                            }
                            //AsyncHelper.RunSync(() => _userEmailer.SendRunRecordPolicySuccessEmail(tenant.Id, softdeleteprojects, harddeleteprojects, achiveprojects));

                            // despite the fact that all policies are marked as inactive all tenant admins are receiving an email
                            if (isPoliciesActive)
                                _userEmailer.SendRunRecordPolicySuccessEmail(tenant.Id, softdeleteprojects, harddeleteprojects, achiveprojects);
                        }

                    }
                    catch (Exception e)
                    {
                        string failedTenancyNames = tenant.TenancyName;
                        Logger.Error("Archiving, SoftDelete, HardDelete tasks Excemption: " + e.Message, e);
                        // AsyncHelper.RunSync(() => _userEmailer.SendFailedRecordPolicy(tenant.Id, failedTenancyNames,utcNow, softdeleteprojects, harddeleteprojects, achiveprojects, e.Message));
                        //even though the test failed, we still want to excute the policy functions.
                         //_userEmailer.SendFailedRecordPolicy(tenant.Id, failedTenancyNames, utcNow, softdeleteprojects, harddeleteprojects, achiveprojects, e.Message);
                    }

                }

            }
        }

        //can only opreate on the projects in it's own tenant, but not shared tenant projects
        private List<string> softdeleteRecord(int tenantId, RecordPolicyAction recorddeletepolicy, List<string> softdeleteprojects)
        {
            var projects = _projectRepository.GetAll().Where(p => p.TenantId == tenantId && p.Type == ProjectType.User).ToList();
            foreach (var project in projects)
            { 
                var softdeleteSpan = Clock.Now.Subtract(TimeSpan.FromDays(Convert.ToDouble(recorddeletepolicy.ExpireDays)));

                switch (recorddeletepolicy.RecordStatus)
                {
                    case RecordStatusType.New:

                        if ((project.CreationTime < softdeleteSpan && project.Status == ProjectStatus.New && project.LastModificationTime == null&& !project.Archived &&!project.IsDeleted) || (project.LastModificationTime != null && project.LastModificationTime < softdeleteSpan && project.Status == ProjectStatus.New && !project.Archived && !project.IsDeleted))
                        {
                            Logger.Info("Start Soft delete: " + project.Name);
                            _projectRepository.Delete(pt => pt.Id == project.Id);
                            softdeleteprojects.Add(project.Name);
                        }
                        break;
                    case RecordStatusType.InProgress:
                        if (project.LastModificationTime != null && project.LastModificationTime < softdeleteSpan && project.Status == ProjectStatus.InProgress && !project.Archived && !project.IsDeleted)
                        {
                            Logger.Info("Start Soft delete: " + project.Name);
                            _projectRepository.Delete(pt => pt.Id == project.Id);
                            softdeleteprojects.Add(project.Name);
                        }
                        break;
                    case RecordStatusType.Completed:
                        if (project.LastModificationTime != null && project.LastModificationTime < softdeleteSpan && project.Status == ProjectStatus.Completed && !project.Archived && !project.IsDeleted)
                        {
                            Logger.Info("Start Soft delete: " + project.Name);
                            _projectRepository.Delete(pt => pt.Id == project.Id);
                            softdeleteprojects.Add(project.Name);
                        }
                        break;
                    case RecordStatusType.Archived:
                        if (project.LastModificationTime != null && project.LastModificationTime < softdeleteSpan && project.Archived && !project.IsDeleted)
                        {
                            Logger.Info("Start Soft delete: " + project.Name);
                            _projectRepository.Delete(pt => pt.Id == project.Id);
                            softdeleteprojects.Add(project.Name);
                        }
                        break;
                    case RecordStatusType.IsDeleted:
                        break;
                }
            }
            CurrentUnitOfWork.SaveChanges();
            return softdeleteprojects;
        }

        private List<string> hardDeleteRecord(int tenantId, RecordPolicyAction recorddeletepolicy, List<string> harddeleteprojects)
        {
            List<Guid> ACLList = new List<Guid>();
            var projects = _projectRepository.GetAll().Where(p => p.TenantId == tenantId && p.Type == ProjectType.User).ToList();
            foreach (var inactiveProject in projects)
            {
                var HardDeleteSpan = Clock.Now.Subtract(TimeSpan.FromDays(Convert.ToDouble(recorddeletepolicy.ExpireDays)));
                if (inactiveProject.DeletionTime.HasValue && inactiveProject.IsDeleted == true && inactiveProject.DeletionTime < HardDeleteSpan)
                {

                    ACLList.Add(inactiveProject.Id);

                    var record = _recordRepository.GetAll()
                        .Where(e => e.Id == inactiveProject.RecordId).FirstOrDefault();

                    ACLList.Add(record.Id);

                    //delete recortd matters
                    var recordMatters = _recordMatterRepository.GetAll().Where(e => e.RecordId == record.Id).ToList();
                    foreach (var recordMatter in recordMatters)
                    {
                        ACLList.Add(recordMatter.Id);

                        var rmas = _recordMatterAuditRepository.GetAll().Where(e => e.RecordMatterId == recordMatter.Id).ToList();
                        foreach (var rma in rmas)
                        {
                            _recordMatterAuditRepository.Delete(rma);
                           
                        }

                        var rmacs = _recordMatterContributorRepository.GetAll().Where(e => e.RecordMatterId == recordMatter.Id).ToList();
                        foreach (var rma in rmacs)
                        {
                            _recordMatterContributorRepository.HardDelete(rma);
                        }


                        //var rmis = _recordMatterItemRepository.GetAll().Where(e => e.RecordMatterId == recordMatter.Id);
                        var subs = _submissionRepository.GetAll().Where(e => e.RecordMatterId == recordMatter.Id).ToList();
                        foreach (var sub in subs)
                        {
                            _submissionRepository.HardDelete(sub);
                        }

                        var rmis = _recordMatterItemRepository.GetAll().Where(e => e.RecordMatterId == recordMatter.Id).ToList();
                        foreach (var rmi in rmis)
                        {
                            var rmhs = _recordMatterItemHistoryRepository.GetAll().Where(e => e.RecordMatterItemId == rmi.Id).ToList();
                            foreach (var rmh in rmhs)
                            {
                                _recordMatterItemHistoryRepository.Delete(rmh);
                            }

                            _recordMatterItemRepository.HardDelete(rmi);
                        }

                        _recordMatterRepository.HardDelete(recordMatter);
                    }

                    _recordRepository.HardDelete(record);

                    harddeleteprojects.Add(inactiveProject.Name);
                    _projectRepository.HardDelete(inactiveProject);
                   

                    //delete acls
                    var acls = _aclRepository.GetAll()
                        .Where(i => ACLList.Contains(i.EntityID)).ToList();

                    foreach (var acl in acls)
                    {
                        _aclRepository.Delete(acl);

                    }

                    //is inactive can be used now??
                    ACLList.ForEach(i => Logger.Info("Hard delete: " + inactiveProject.Name + "related EntityId: " + i));

                }

            }
            CurrentUnitOfWork.SaveChanges();

            return harddeleteprojects;
        }
        private List<string> archiveRecord(int tenantId, RecordPolicyAction recorddeletepolicy, List<string> archiveprojects)
        {
            var projects = _projectRepository.GetAll().Where(p => p.TenantId == tenantId && p.Type == ProjectType.User && p.Archived==false && p.IsDeleted==false).ToList();
            foreach (var project in projects)
            {
                var archiveSpan = Clock.Now.Subtract(TimeSpan.FromDays(Convert.ToDouble(recorddeletepolicy.ExpireDays)));

                switch (recorddeletepolicy.RecordStatus)
                {
                    case RecordStatusType.New:
                        if ((project.CreationTime < archiveSpan && project.Status == ProjectStatus.New && project.LastModificationTime == null && !project.Archived && !project.IsDeleted) || (project.LastModificationTime != null && project.LastModificationTime < archiveSpan && project.Status == ProjectStatus.New))
                        {
                            Logger.Info("Start archive: " + project.Name);
                            project.Archived = true;
                            _projectRepository.Update(project);
                            archiveprojects.Add(project.Name);
                        }
                        break;
                    case RecordStatusType.InProgress:
                        if (project.LastModificationTime != null && project.LastModificationTime < archiveSpan && project.Status == ProjectStatus.InProgress && !project.Archived && !project.IsDeleted)
                        {
                            Logger.Info("Start archive: " + project.Name);
                            project.Archived = true;
                            _projectRepository.Update(project);
                            archiveprojects.Add(project.Name);
                        }
                        break;
                    case RecordStatusType.Completed:
                        if (project.LastModificationTime != null && project.LastModificationTime < archiveSpan && project.Status == ProjectStatus.Completed && !project.Archived && !project.IsDeleted)
                        {
                            Logger.Info("Start archive: " + project.Name);
                            project.Archived = true;
                            _projectRepository.Update(project);
                            archiveprojects.Add(project.Name);
                        }
                        break;
                    case RecordStatusType.Archived:
                        break;
                    case RecordStatusType.IsDeleted:
                        break;
                }
            }
            CurrentUnitOfWork.SaveChanges();

            return archiveprojects;
        }

    }

}
