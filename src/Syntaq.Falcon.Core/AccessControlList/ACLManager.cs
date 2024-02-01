using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Organizations;
using Abp.Runtime.Session;

using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Transactions;
using Syntaq.Falcon.Projects;
using Abp.Authorization;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.AccessControlList.Dtos;

namespace Syntaq.Falcon.AccessControlList
{
    public class ACLManager : FalconDomainServiceBase
    {
        public IAbpSession _abpSession { get; set; }
        private readonly IRepository<ACL> _aclRepository;
        private readonly IRepository<Record, Guid> _recordRepository;
        private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
        private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
        private readonly IRepository<RecordMatterAudit, Guid> _recordMatterAuditRepository;
        private readonly IRepository<RecordMatterContributor, Guid> _recordMatterContributorRepository;
        private readonly IRepository<Folder, Guid> _folderRepository;
        private readonly IRepository<Template, Guid> _templateRepository;
        private readonly IRepository<Apps.App, Guid> _appRepository;
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<Project, Guid> _projectRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;

        private readonly UserManager _userManager;
        private readonly IPermissionManager _permissionManager;
        private readonly IAbpSession _AbpSession;

        public ACLManager(
            IRepository<ACL> aclRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<Record, Guid> recordRepository,
            IRepository<RecordMatter, Guid> recordMatterRepository,
            IRepository<RecordMatterAudit, Guid> recordMatterAuditRepository,
            IRepository<RecordMatterItem, Guid> recordMatterItemRepository,
            IRepository<RecordMatterContributor, Guid> recordMatterContributorRepository,
            IRepository<Folder, Guid> folderRepository,
            IRepository<Template, Guid> templateRepository,
            IRepository<Apps.App, Guid> appRepository,
            IRepository<Form, Guid> formRepository,
            IRepository<Project, Guid> projectRepository,
            IRepository<User, long> userRepository,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            UserManager userManager,
            IPermissionManager permissionManager,
            IAbpSession abpSession
            )
        {
            _aclRepository = aclRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _recordRepository = recordRepository;
            _recordMatterRepository = recordMatterRepository;
            _recordMatterAuditRepository = recordMatterAuditRepository;
            _recordMatterItemRepository = recordMatterItemRepository;
            _recordMatterContributorRepository = recordMatterContributorRepository;
            _folderRepository = folderRepository;
            _templateRepository = templateRepository;
            _appRepository = appRepository;
            _formRepository = formRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _userManager = userManager;
            _permissionManager = permissionManager;
            _AbpSession = abpSession;
        }

        /// <summary>
        /// Checks the specified users access to the specified entity record
        /// </summary>
        /// <param name="aCLCheckDto"></param>
        /// <returns></returns>
        public ACLResultDto CheckAccess(ACLCheckDto aCLCheckDto)
        {

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                bool result = false;
                string permission = null;

                string[] ViewArray = new string[] { "View", "Share" };
                string[] EditArray = new string[] { "View", "Edit", "Delete" };
                string[] OwnerArray = new string[] { "View", "Edit", "Delete", "Share" };

                List<ACL> output = new List<ACL>();
                output = GetACLList(aCLCheckDto);

                // If there are no ACL records check up the Parent Entity Relationship RecordMatterItem > RecordMatter > Record > Folder
                // 1. What is the Parent Entity ID
                EntityType entitytype = GetEntityType(aCLCheckDto.EntityId);
                if (output.Count == 0)
                {
                    Guid parentEntityId = GetParentEntityId(aCLCheckDto.EntityId, entitytype);
                    while (output.Count == 0 && parentEntityId != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        output = GetACLList(new ACLCheckDto()
                        {
                            Action = aCLCheckDto.Action,
                            UserId = aCLCheckDto.UserId,
                            EntityId = parentEntityId
                        });
                        entitytype = GetEntityType(parentEntityId);
                        parentEntityId = GetParentEntityId(parentEntityId, entitytype);
                    }
                }

                //if (entitytype == EntityType.Folder)
                //{
                //    result = true;
                //}

                IList<string> Ids = new List<string> { "V", "E", "O", "S" };
                output = output.OrderBy(i => Ids.IndexOf(i.Role)).ToList();
                output.ForEach(i =>
                {
                    switch (i.Role)
                    {
                        case "V":
                            result = Array.Exists(ViewArray, element => element == aCLCheckDto.Action);
                            if (!result) result = Array.Exists(ViewArray, element => element == "S");
                            if (result) { permission = "V"; }
                            break;
                        case "E":
                            result = Array.Exists(EditArray, element => element == aCLCheckDto.Action);
                            if (result) { permission = "E"; }
                            break;
                        case "O":
                        case "S":
                            result = Array.Exists(OwnerArray, element => element == aCLCheckDto.Action);
                            if (result) { permission = "O"; }
                            break;
                    }
                });

				// If access not found then check to see if the entity does not exist.
				// If entity does not exist then grant access as the creator by default
				result = !result ? entitytype == EntityType.NotFound ? true : result : result;

                if (!result)
                {
                    // Check the Access Token

                    entitytype = GetEntityType(aCLCheckDto.EntityId);

                    if (entitytype == EntityType.ProjectTemplate)
                    {
                        //var isinrole = await _permissionManager.
                        if (aCLCheckDto.UserId.HasValue)
                        {
                            if (_userManager.IsGrantedAsync((long)aCLCheckDto.UserId, AppPermissions.Pages_ProjectTemplates_Edit).Result)
                            {
                                result = true;
                                permission = "E";
                            }
                        }
                    }

                    if (entitytype == EntityType.Record)
                    {
                        var record = _recordRepository.GetAll().FirstOrDefault(r => r.Id == aCLCheckDto.EntityId);                       
                        if (record?.AccessToken == aCLCheckDto.AccessToken && !JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken)) result = true; permission = "E";

                        var recordmatters = _recordMatterRepository.GetAll().Where(rm => rm.RecordId == aCLCheckDto.EntityId).Select(r => r.Id);

                        var recordmattercontributor = _recordMatterContributorRepository.GetAll()
                            .FirstOrDefault(e => recordmatters.Contains((Guid)e.RecordMatterId) && e.AccessToken == aCLCheckDto.AccessToken);

						//if (recordmattercontributor != null)
						//{
						//	// Check if the LastModificationTime is within the last 20 days
						//	var lastAccessedWithin20Days = recordmattercontributor.LastModificationTime >= DateTime.Now.AddDays(-20);
						//	if (recordmattercontributor.LastModificationTime == null)
						//	{
						//		// Update RecordMatterContributor LastModificationTime with the current date and time
						//		recordmattercontributor.LastModificationTime = DateTime.Now;
						//		recordmattercontributor.UserId = aCLCheckDto.UserId;

						//		// Save changes to the database to persist the updated LastModificationTime
						//		_recordMatterContributorRepository.Update(recordmattercontributor);
						//	}
							
						//	if (lastAccessedWithin20Days)
						//	{
						//		// Update RecordMatterContributor LastModificationTime with the current date and time
						//		recordmattercontributor.LastModificationTime = DateTime.Now;
						//		recordmattercontributor.UserId = aCLCheckDto.UserId;

						//		// Save changes to the database to persist the updated LastModificationTime
						//		_recordMatterContributorRepository.Update(recordmattercontributor);
						//	}
						//	else if (recordmattercontributor?.AccessToken == aCLCheckDto.AccessToken && !JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken))
						//	{
						//		result = true;
						//		permission = "E";
						//	}
						//}

                        if (recordmattercontributor?.AccessToken == aCLCheckDto.AccessToken && !JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken))
                            result = true; permission = "E";

                    }
					if (entitytype == EntityType.RecordMatter)
                    {
                        var recordmatter = _recordMatterRepository.GetAll().FirstOrDefault(rm => rm.Id == aCLCheckDto.EntityId);
                        if (recordmatter?.AccessToken == aCLCheckDto.AccessToken && !JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken))
                            result = true; permission = "E";

                        var recordmattercontributor = _recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.RecordMatterId == aCLCheckDto.EntityId && e.AccessToken == aCLCheckDto.AccessToken);

                        //if (recordmattercontributor != null)
                        //{
                        //                      // The 20 day expiry should be used in the final test of valid access
                        //	//// Check if the LastModificationTime is within the last 20 days
                        //	//var lastAccessedWithin20Days = recordmattercontributor.LastModificationTime >= DateTime.Now.AddDays(-20);
                        //	//if (recordmattercontributor.LastModificationTime == null)
                        //	//{
                        //	//	// Update RecordMatterContributor LastModificationTime with the current date and time
                        //	//	recordmattercontributor.LastModificationTime = DateTime.Now;
                        //	//	recordmattercontributor.UserId = aCLCheckDto.UserId;

                        //	//	// Save changes to the database to persist the updated LastModificationTime
                        //	//	_recordMatterContributorRepository.Update(recordmattercontributor);
                        //	//}
                        //	//if (lastAccessedWithin20Days)
                        //	//{
                        //	//	// Update RecordMatterContributor LastModificationTime with the current date and time
                        //	//	recordmattercontributor.LastModificationTime = DateTime.Now;
                        //	//	recordmattercontributor.UserId = aCLCheckDto.UserId;

                        //	//	// Save changes to the database to persist the updated LastModificationTime
                        //	//	_recordMatterContributorRepository.Update(recordmattercontributor);
                        //	//}

                        //	else if (recordmattercontributor?.AccessToken == aCLCheckDto.AccessToken && !JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken))
                        //	{
                        //		result = true;
                        //		permission = "E";
                        //	}
                        //}

                        if (recordmattercontributor?.AccessToken == aCLCheckDto.AccessToken && !JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken))
                            result = true; permission = "E";

                    }

					if (entitytype == EntityType.RecordMatterItem)
                    {
                        var recordmatteritem = _recordMatterItemRepository.GetAll().FirstOrDefault(rmi => rmi.Id == aCLCheckDto.EntityId);
                        if (recordmatteritem != null)
                        {
                            var recordmatter = _recordMatterRepository.GetAll().FirstOrDefault(rm => rm.Id == recordmatteritem.RecordMatterId);
                            if (recordmatter?.AccessToken == aCLCheckDto.AccessToken && ! JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken))
                                result = true; permission = "E";
							var recordmattercontributor = _recordMatterContributorRepository.GetAll().FirstOrDefault(e => e.RecordMatterId == aCLCheckDto.EntityId && e.AccessToken == aCLCheckDto.AccessToken);

							//if (recordmattercontributor != null)
							//{
							//	// Check if the LastModificationTime is within the last 20 days
							//	var lastAccessedWithin20Days = recordmattercontributor.LastModificationTime >= DateTime.Now.AddDays(-20);
							//	if (recordmattercontributor.LastModificationTime == null)
							//	{
							//		// Update RecordMatterContributor LastModificationTime with the current date and time
							//		recordmattercontributor.LastModificationTime = DateTime.Now;
							//		recordmattercontributor.UserId = aCLCheckDto.UserId;

							//		// Save changes to the database to persist the updated LastModificationTime
							//		_recordMatterContributorRepository.Update(recordmattercontributor);
							//	}
							//	if (lastAccessedWithin20Days)
							//	{
							//		// Update RecordMatterContributor LastModificationTime with the current date and time
							//		recordmattercontributor.LastModificationTime = DateTime.Now;
							//		recordmattercontributor.UserId = aCLCheckDto.UserId;

							//		// Save changes to the database to persist the updated LastModificationTime
							//		_recordMatterContributorRepository.Update(recordmattercontributor);
							//	}
							//	else if (recordmattercontributor?.AccessToken == aCLCheckDto.AccessToken && !JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken))
							//	{
							//		result = true;
							//		permission = "E";
							//	}
							//}

                            if (_recordMatterContributorRepository.GetAll().Any(rmc => rmc.RecordMatterId == recordmatteritem.RecordMatterId && rmc.AccessToken == aCLCheckDto.AccessToken && !JwtSecurityTokenProvider.IsTokenExpired(aCLCheckDto.AccessToken) && rmc.Status == RecordMatterContributorConsts.RecordMatterContributorStatus.Awaiting))
                                result = true; permission = "E";
                        }
					}



                }
                // }

                unitOfWork.Complete();
                return new ACLResultDto() { IsAuthed = result, Permission = permission };

            }

        }

        public string FetchRole(ACLCheckDto aCLCheckDto)
        {

            IQueryable<ACL> UACL;
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);


                if (aCLCheckDto.OrgId != null)
                {
                    //UACL = _aclRepository.GetAll().Where(i => i.EntityID == aCLCheckDto.EntityId && i.OrganizationUnitId == aCLCheckDto.OrgId && i.TenantId == _abpSession.TenantId);
                    UACL = _aclRepository.GetAll().Where(i => i.EntityID == aCLCheckDto.EntityId && i.OrganizationUnitId == aCLCheckDto.OrgId);
                }
                else
                {
                    //UACL = _aclRepository.GetAll().Where(i => i.EntityID == aCLCheckDto.EntityId && i.UserId == aCLCheckDto.UserId && i.TenantId == _abpSession.TenantId);
                    UACL = _aclRepository.GetAll().Where(i => i.EntityID == aCLCheckDto.EntityId && i.UserId == aCLCheckDto.UserId);
                }
                if (UACL.Count() == 0)
                {
                    EntityType entitytype = GetEntityType(aCLCheckDto.EntityId);
                    Guid parentEntityId = GetParentEntityId(aCLCheckDto.EntityId, entitytype);

                    while (UACL.Count() == 0 && parentEntityId != new Guid("00000000-0000-0000-0000-000000000000"))
                    {
                        using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                        {
                            //UACL = _aclRepository.GetAll().Where(i => i.EntityID == parentEntityId && i.UserId == aCLCheckDto.UserId && i.TenantId == _abpSession.TenantId);
                            if (aCLCheckDto.OrgId != null)
                            {
                                UACL = _aclRepository.GetAll().Where(i => i.EntityID == parentEntityId && i.OrganizationUnitId == aCLCheckDto.OrgId && i.TenantId == _abpSession.TenantId);
                            }
                            else
                            {
                                UACL = _aclRepository.GetAll().Where(i => i.EntityID == parentEntityId && i.UserId == aCLCheckDto.UserId && i.TenantId == _abpSession.TenantId);
                            }
                            //CurrentUnitOfWork.SaveChanges();
                            //                  unitOfWork.Complete();
                        }
                        if (UACL.Count() == 0)
                        {
                            entitytype = GetEntityType(parentEntityId);
                            parentEntityId = GetParentEntityId(parentEntityId, entitytype);
                        }
                    }
                }
                else if (UACL.Count() > 1)
                {
                    var result = UACL.Any(i => i.Role == "O") ? UACL.FirstOrDefault(i => i.Role == "O") :
                                 UACL.Any(i => i.Role == "E") ? UACL.FirstOrDefault(i => i.Role == "E") :
                                 UACL.FirstOrDefault(i => i.Role == "V");
                    unitOfWork.Complete();
                    return result.Role;
                }
                unitOfWork.Complete();
            }


            return UACL.FirstOrDefault() == null ? "N" : UACL.First().Role;
        }

        private List<ACL> GetACLList(ACLCheckDto aCLCheckDto)
        {
            List<ACL> output = new List<ACL>();

            if (aCLCheckDto.UserId != null)
            {
                using (var unitOfWork = _unitOfWorkManager.Begin())
                {
                    _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                    IQueryable<ACL> UACL = _aclRepository.GetAll().Where(i => i.EntityID == aCLCheckDto.EntityId && i.UserId == aCLCheckDto.UserId);
                    UACL.ToList().ForEach(i => output.Add(i));

                    // Tenant level sharing
                    if (aCLCheckDto.TenantId != null)
                    {
                        IQueryable<ACL> TACL = _aclRepository.GetAll().Where(i => i.EntityID == aCLCheckDto.EntityId && i.TargetTenantId == aCLCheckDto.TenantId);
                        TACL.ToList().ForEach(i => output.Add(i));
                    }

                    var UserOrgs = _userOrganizationUnitRepository.GetAll().Where(i => i.UserId == aCLCheckDto.UserId);
                    UserOrgs.ToList().ForEach(i =>
                    {
                        IQueryable<ACL> OACL = _aclRepository.GetAll().Where(j => j.EntityID == aCLCheckDto.EntityId && j.OrganizationUnitId == i.OrganizationUnitId);
                        OACL.ToList().ForEach(k => output.Add(k));
                    });

                    unitOfWork.Complete();
                }
            }

            return output;
        }

        private enum EntityType { Folder, Record, RecordMatter, RecordMatterItem, DocumentTemplate, App, Form, RecordMatterAudit, Project, ProjectTemplate, NotFound };
        private EntityType GetEntityType(Guid Id)
        {

            EntityType result = EntityType.NotFound;

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);


                if (_recordMatterItemRepository.GetAll().Any(e => e.Id == Id)) result = EntityType.RecordMatterItem;
                //if (_recordMatterItemRepository.GetAll().Any(i => i.Id == Id)) return EntityType.RecordMatterItem;

                if (_recordMatterRepository.GetAll().Any(e => e.Id == Id)) result = EntityType.RecordMatter;
                //if (_recordMatterRepository.GetAll().Any(i => i.Id == Id)) return EntityType.RecordMatter;

                if (_recordRepository.GetAll().Any(e => e.Id == Id)) result = EntityType.Record;
                //if (_recordRepository.GetAll().Any(i => i.Id == Id)) return EntityType.Record;

                if (_folderRepository.GetAll().Any(e => e.Id == Id)) result = EntityType.Folder;
                //if (_folderRepository.GetAll().Any(i => i.Id == Id)) return EntityType.Folder;

                if (_templateRepository.GetAll().Any(e => e.Id == Id)) result = EntityType.DocumentTemplate;
                //if (_templateRepository.GetAll().Any(i => i.Id == Id)) return EntityType.DocumentTemplate;

                if (_appRepository.GetAll().Any(e => e.Id == Id)) result = EntityType.App;
                //if (_appRepository.GetAll().Any(i => i.Id == Id)) return EntityType.App;

                if (_formRepository.GetAll().Any(e => e.Id == Id)) result = EntityType.Form;

                if (_recordMatterAuditRepository.GetAll().Any(e => e.Id == Id)) result = EntityType.RecordMatterAudit;

                if (_projectRepository.GetAll().Any(e => e.Id == Id && e.Type == ProjectConsts.ProjectType.User)) result = EntityType.Project;

                if (_projectRepository.GetAll().Any(e => e.Id == Id && e.Type == ProjectConsts.ProjectType.Template)) result = EntityType.ProjectTemplate;

                //if (_formRepository.GetAll().Any(i => i.Id == Id)) return EntityType.Form;
                unitOfWork.Complete();

            }
            return result;
        }

        private Guid GetParentEntityId(Guid Id, EntityType entitytype)
        {

            Guid result = new Guid("00000000-0000-0000-0000-000000000000");
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                switch (entitytype)
                {
                    case EntityType.Folder: // parent entities are folders
                        var folder = _folderRepository.Get(Id);
                        result = folder == null ? new Guid("00000000-0000-0000-0000-000000000000") : folder.ParentId == null ? new Guid("00000000-0000-0000-0000-000000000000") : (Guid)folder.ParentId;
                        break;
                    case EntityType.Record: // parent entities are folders
                        var record = _recordRepository.Get(Id);
                        result = record == null ? new Guid("00000000-0000-0000-0000-000000000000") : record.FolderId == null ? new Guid("00000000-0000-0000-0000-000000000000") : (Guid)record.FolderId;
                        break;
                    case EntityType.DocumentTemplate: // parent entities are folders
                        var template = _templateRepository.Get(Id);
                        result = template == null ? new Guid("00000000-0000-0000-0000-000000000000") : template.FolderId;
                        break;
                    case EntityType.RecordMatter: // parent entity is Record
                        var recordmatter = _recordMatterRepository.Get(Id);
                        result = recordmatter == null ? new Guid("00000000-0000-0000-0000-000000000000") : recordmatter.RecordId;
                        break;
                    case EntityType.RecordMatterItem: // parent entity is RecordMatter
                        var recordmatteritem = _recordMatterItemRepository.Get(Id);
                        result = recordmatteritem == null ? new Guid("00000000-0000-0000-0000-000000000000") : recordmatteritem.RecordMatterId;
                        break;
                    case EntityType.App:
                        result = new Guid("00000000-0000-0000-0000-000000000000");
                        break;
                    case EntityType.Form:
                        var form = _formRepository.Get(Id);
                        result = form == null ? new Guid("00000000-0000-0000-0000-000000000000") : form.FolderId;
                        break;
                    case EntityType.RecordMatterAudit:
                        var rma = _recordMatterAuditRepository.Get(Id);

                        var rm = _recordMatterRepository.Get((Guid)rma.RecordMatterId);

                        result = rm == null ? new Guid("00000000-0000-0000-0000-000000000000") : (Guid)rm.RecordId;
                        break;
                }
                unitOfWork.Complete();
            }



            return result;
        }

        /// <summary>
        /// Fetches all of a specified users personal and organisational Access Control List records
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public List<ACL> FetchAllUserACLs(GetAllACLsInput input)
        {

            List<ACL> UserACLs = new List<ACL>();
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                var FoundACLs = _aclRepository.GetAll()
                   .Where(i => i.UserId == input.UserId)
                   .WhereIf(!string.IsNullOrEmpty(input.EntityFilter), i => i.Type == input.EntityFilter)
                .ToList();

                FoundACLs.ForEach(k => UserACLs.Add(k));

                var UserOrgs = _userOrganizationUnitRepository.GetAll().Where(i => i.UserId == input.UserId);
                UserOrgs.ToList().ForEach(i =>
                {
                    FoundACLs = _aclRepository.GetAll().Where(j => j.OrganizationUnitId == i.OrganizationUnitId && j.Type == input.EntityFilter).ToList();
                    FoundACLs.ForEach(k => UserACLs.Add(k));
                });

                //Filter ACL by Role
                if (!string.IsNullOrEmpty(input.RoleFilter))
                {
                    string[] Roles = input.RoleFilter.Split(',');
                    Roles.ToList().ForEach(i =>
                    {
                        UserACLs.ToList().ForEach(j =>
                        {
                            if (j.Role == i)
                            {
                                UserACLs.Remove(j);
                            }
                        });
                    });
                }

                unitOfWork.Complete();
            }
            return UserACLs;
        }

        public Guid FetchUserRootFolder(long UserId, string Type)
        {

            Guid Result = Guid.Parse("00000000-0000-0000-0000-000000000000");
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
                var data = (from folder in _folderRepository.GetAll()
                            join acl in _aclRepository.GetAll() on folder.Id equals acl.EntityID
                            where acl.UserId == UserId && folder.Type == Type && (folder.ParentId == new Guid("00000000-0000-0000-0000-000000000000") || folder.ParentId == null)
                            select folder).AsNoTracking();

                // IF Root folders are not createwd then add them
                // Todo make this a single enrty point
                if (!data.Any())
                {

                    Folder folder = new Folder()
                    {
                        ACLRole = "O",
                        Name = Type == "T" ? "Your Templates" : Type == "F" ? "Your Forms" : "Your Records",
                        Type = Type,
                        ParentId = new Guid("00000000-0000-0000-0000-000000000000")
                    };

                    var folderid = _folderRepository.InsertAndGetId(folder);

                    AddACL(new ACL()
                    {
                        EntityID = folderid,
                        Role = "O",
                        Type = "Folder",
                        UserId = UserId
                    });

                    _unitOfWorkManager.Current.SaveChanges();

                    Result = folder.Id;
                }
                else
                {

                    Result = data != null ? data.FirstOrDefault().Id : Result;
                }


                unitOfWork.Complete();
            }
            return Result;
        }

        /// <summary>
        /// Creates a New Access Control List Record
        /// </summary>
        /// <param name="acl"></param>
        /// <returns></returns>
        public async Task AddACL(ACL acl)
        {

            if (acl.OrganizationUnitId != null)
            {
                if (!_aclRepository.GetAll().Any(i => i.EntityID == acl.EntityID && i.OrganizationUnitId == acl.OrganizationUnitId))
                {
                    _aclRepository.Insert(acl);
                    //await _aclRepository.InsertAsync(acl);
                }
            }
            else
            {
                if (acl.TargetTenantId != null)
                {
                    if (!_aclRepository.GetAll().Any(i => i.EntityID == acl.EntityID && i.TargetTenantId == acl.TargetTenantId))
                    {
                        _aclRepository.Insert(acl);
                    }
                }
                else
                {
                    if (!_aclRepository.GetAll().Any(i => i.EntityID == acl.EntityID && i.UserId == acl.UserId))
                    {
                        _aclRepository.Insert(acl);
                    }
                }
            }
        }

        /// <summary>
        /// Removes an Access Control List Record.
        /// </summary>
        /// <param name="acl">An ACL object containing either the entity id or the ACL id.</param>
        /// <returns>Void</returns>
        public async Task RemoveACL(ACL acl)
        {


            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                ACL ACL;
                if (acl.EntityID != null)
                {
                    ACL = _aclRepository.FirstOrDefault(i => i.EntityID == acl.EntityID && i.UserId == _abpSession.UserId);
                    if (ACL != null)
                        await _aclRepository.DeleteAsync(ACL);
                }
                else if (acl.Id != 0)
                {
                    ACL = _aclRepository.FirstOrDefault(i => i.Id == acl.Id);
                    if (ACL != null)
                        await _aclRepository.DeleteAsync(ACL);
                }
                unitOfWork.Complete();
            }



        }

        /// <summary>
        /// Removes all Access Control List Records for a user or tenant.
        /// </summary>
        /// <param name="acl">An ACL object containing either the user id and/or the tenant id.</param>
        /// <returns>Void</returns>
        public void RemoveAllACL(ACL acl)
        {


            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

                if (acl.UserId != null && acl.TenantId != null)
                {
                    IQueryable<ACL> ACLs = _aclRepository.GetAll().Where(i => i.UserId == acl.UserId && i.TenantId == acl.TenantId);
                    ACLs.ToList().ForEach(async i =>
                    {
                        await _aclRepository.DeleteAsync(i);
                    });
                }
                else if (acl.TenantId != null)
                {
                    IQueryable<ACL> ACLs = _aclRepository.GetAll().Where(i => i.TenantId == acl.TenantId);
                    ACLs.ToList().ForEach(async i =>
                    {
                        await _aclRepository.DeleteAsync(i);
                    });
                }
                else if (acl.UserId != null)
                {
                    IQueryable<ACL> ACLs = _aclRepository.GetAll().Where(i => i.UserId == acl.UserId);
                    ACLs.ToList().ForEach(async i =>
                    {
                        await _aclRepository.DeleteAsync(i);
                    });
                }
                unitOfWork.Complete();
            }
        }

        /// <summary>
        /// Removes all Access Control List Records for an Entity.
        /// </summary>
        /// <param name="acl">An ACL object containing either the user id and/or the tenant id.</param>
        /// <returns>Void</returns>
        public void RemoveAllACLForEntity(Guid entityid)
        {

            IQueryable<ACL> ACLs = _aclRepository.GetAll().Where(i => i.EntityID == entityid);
            ACLs.ToList().ForEach(async i =>
            {
                await _aclRepository.DeleteAsync(i);
            });

        }

        public async Task RemoveAllACLForEntityAsync(Guid entityid, bool removeowner = true)
        {
            using (var unitOfWork = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                var ACLs = _aclRepository.GetAll().Where(i => i.EntityID == entityid && removeowner == true ? true : i.Role != "O").ToList();
                ACLs.ForEach(async i =>
                {
                    await _aclRepository.DeleteAsync(i);
                });

                await unitOfWork.CompleteAsync();
            }


        }

        public List<ACL> GetAllACLByEntityId(Guid entityid, string type = "", string role = "")
        {

            var ACLs = _aclRepository.GetAll().Where(i => i.EntityID == entityid)
                .WhereIf(!string.IsNullOrWhiteSpace(type), e => e.Type == type)
                .WhereIf(!string.IsNullOrWhiteSpace(role), e => e.Role == role)
                .ToList();

            List<ACL> output = new List<ACL>();

            ACLs.ForEach(acl =>
            {
                output.Add(new ACL
                {
                    TenantId = acl.TenantId,
                    EntityID = acl.EntityID,
                    Role = acl.Role,
                    UserId = acl.UserId,
                    User = acl.UserId != null ? _userRepository.GetAll().Where(u => u.Id == acl.UserId).First() : null,
                    OrganizationUnitId = acl.OrganizationUnitId,
                    OrganizationUnit = acl.OrganizationUnitId != null ? _organizationUnitRepository.GetAll().Where(t => t.Id == acl.OrganizationUnitId).First() : null,
                    Type = acl.Type,
                    AccessToken = acl.AccessToken

                });

            });

            return output;
        }

        public List<Guid> GetAllSharedEntities(long? tenantId, string type = "Form")
        {
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            var entitys = _aclRepository.GetAll().Where(i => i.Type == type && i.TargetTenantId == tenantId)
            .ToList();

            var entityIds = _aclRepository.GetAll().Where(i => i.Type == type && i.TargetTenantId == tenantId && i.TargetTenantId != null)
                .Select(a => a.EntityID)
                .ToList();

            return entityIds;
        }

    }
}
