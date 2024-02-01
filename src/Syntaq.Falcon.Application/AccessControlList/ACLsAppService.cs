using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.UI;
using Newtonsoft.Json;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Documents.Dtos;
using Syntaq.Falcon.EntityFrameworkCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.AccessControlList
{
	[AbpAuthorize(AppPermissions.Pages_ACLs)]
	public class ACLsAppService : FalconAppServiceBase, IACLsAppService
	{
		private readonly IRepository<ACL> _aclRepository;
		private readonly IRepository<User, long> _userRepository;
		private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
		private readonly ACLManager _ACLManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICustomRecordsRepository _customrecordRepository;
        private readonly IRepository<Template, Guid> _templateRepository;

        public ACLsAppService(
            IRepository<ACL> aclRepository, 
            IRepository<User, long> userRepository, 
            IRepository<OrganizationUnit, long> organizationUnitRepository, 
            ACLManager aclManager,
            IUnitOfWorkManager unitOfWorkManager,
            ICustomRecordsRepository customrecordRepository,
            IRepository<Template, Guid> templateRepository
        )
		{
			_aclRepository = aclRepository;
			_userRepository = userRepository;
			_organizationUnitRepository = organizationUnitRepository;
			_ACLManager = aclManager;
            _unitOfWorkManager = unitOfWorkManager;
            _customrecordRepository = customrecordRepository;
            _templateRepository = templateRepository;

        }

		//is this method used?????
		public bool CheckCanShareACL(Guid EntityId)
		{
			ACLCheckDto aCLCheckDto = new ACLCheckDto()
			{
				Action = "Share",
				EntityId = EntityId,
				UserId = AbpSession.UserId
			};
			bool IsAuthed =  _ACLManager.CheckAccess(aCLCheckDto).IsAuthed;
			return IsAuthed;
		}

		[AbpAuthorize(AppPermissions.Pages_Administration)]
		public async Task AcceptSharedByTenant(ACLInput input)
		{
			// Acl is shared (created by a different tenant)
			_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
			if (_aclRepository.GetAll().Any(a => a.EntityID == input.EntityId && a.TargetTenantId == AbpSession.TenantId && a.Type == input.Type))
			{
				var acl = _aclRepository.GetAll().FirstOrDefault(a => a.EntityID == input.EntityId && a.TargetTenantId == AbpSession.TenantId && a.Type == input.Type);
				acl.Accepted = true;
				await _aclRepository.UpdateAsync(acl);
			}
			else
			{
				throw new UserFriendlyException(L("PermissionDenied"));
			}
		}

		[AbpAuthorize(AppPermissions.Pages_Administration)]
		public async Task RemoveSharedByTenant(ACLInput input)
		{
			// Acl is shared (created by a different tenant)
			_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
			if (_aclRepository.GetAll().Any(a => a.EntityID == input.EntityId && a.TargetTenantId == AbpSession.TenantId && a.Type == input.Type))
			{
				var acl = _aclRepository.GetAll().FirstOrDefault(a => a.EntityID == input.EntityId && a.TargetTenantId == AbpSession.TenantId && a.Type == input.Type);
				await _aclRepository.DeleteAsync(acl);
			}
			else
			{
				throw new UserFriendlyException(L("PermissionDenied"));
			}
		}

		public async Task GrantACL_Old(dynamic grantACLDto)
		{

			string Type = grantACLDto.Type.ToString();
			string tenantName = Convert.ToString(grantACLDto.TenantName);

			if (_ACLManager.CheckAccess(new ACLCheckDto()
			{
				Action = "Edit",
				EntityId = grantACLDto.EntityId,
				UserId = AbpSession.UserId,
				TenantId = AbpSession.TenantId
			}).IsAuthed)
			{

            if ( ! string.IsNullOrEmpty(tenantName) )
            {
                try
                {
						foreach(string tname in tenantName.Split(','))
						{

							var tenant = TenantManager.GetTenantId(tname.Trim());
							if (tenant != null)
							{
								long targetTenantId = (long)tenant.Result;
								ACL ACL = new ACL()
								{
									EntityID = grantACLDto.EntityId,
									UserId = null,
									OrganizationUnitId = null,
									Role = "S",
									TargetTenantId = targetTenantId,
									TenantId = AbpSession.TenantId,
									Type = Type
								};

								await _ACLManager.AddACL(ACL);

							}

						}

				}
                catch
                {
					throw new UserFriendlyException("Tenant Not Found");	
                }

				}
                else
                {
					List<GrantACLDto> ACLList = JsonConvert.DeserializeObject<List<GrantACLDto>>(Convert.ToString(grantACLDto.Assignees));
					ACLList.ForEach(async i =>
					{
						try
						{
							ACL ACL = new ACL()
							{
								EntityID = grantACLDto.EntityId,
								UserId = i.Type == "User" ? i.Id : null,
								OrganizationUnitId = i.Type == "Team" ? i.Id : null,
								Role = grantACLDto.Role,
								TenantId = AbpSession.TenantId,
								Type = Type
							};

							await _ACLManager.AddACL(ACL);
						}
						catch (Exception)
						{
							throw new UserFriendlyException("Couldn't Grant ACL: " + i.Type == "User" ? "User" : "Team" + " not found");
						}
					});
                }
			}
			else
			{
				throw new UserFriendlyException("Couldn't Grant ACL: Denied");
			}
 
		}


        public async Task GrantACL(dynamic grantACLDto)
        {
            string Type = grantACLDto.Type.ToString();
            string tenantName = Convert.ToString(grantACLDto.TenantName);

            //STQ Modified
            List<Guid> recordids = new List<Guid>();
            IEnumerable<Records.Record> records;
            List<Records.Record> RecordList = new List<Records.Record>();
            try
            {
                if (Type == "Folder")
                {
                    Guid userRootFolderId = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, "R");
                    Guid? parentFolderId = (string.IsNullOrEmpty(grantACLDto.EntityId.Value) || grantACLDto.EntityId.Value == "00000000-0000-0000-0000-000000000000") ? userRootFolderId : new Guid(grantACLDto.EntityId.Value);
                    var RecordLists = _customrecordRepository.GetAllWithRecordsRaw(parentFolderId, "", recordids, false);
                    RecordList = RecordLists.ToList();
                }
            }
            catch (Exception ex) { }
            if (_ACLManager.CheckAccess(new ACLCheckDto()
            {
                Action = "Edit",
                EntityId = grantACLDto.EntityId,
                UserId = AbpSession.UserId,
                TenantId = AbpSession.TenantId
            }).IsAuthed)
            {
                if (!string.IsNullOrEmpty(tenantName))
                {
                    try
                    {
                        foreach (string tname in tenantName.Split(','))
                        {

                            var tenant = TenantManager.GetTenantId(tname.Trim());
                            if (tenant != null)
                            {
                                long targetTenantId = (long)tenant.Result;
                                ACL ACL = new ACL()
                                {
                                    EntityID = grantACLDto.EntityId,
                                    UserId = null,
                                    OrganizationUnitId = null,
                                    Role = "S",
                                    TargetTenantId = targetTenantId,
                                    TenantId = AbpSession.TenantId,
                                    Type = Type
                                };
                                await _ACLManager.AddACL(ACL);
                            }
                        }
                    }
                    catch
                    {
                        throw new UserFriendlyException("Tenant Not Found");
                    }
                }
                else
                {
                    List<GrantACLDto> ACLList = JsonConvert.DeserializeObject<List<GrantACLDto>>(Convert.ToString(grantACLDto.Assignees));
					Guid EntityIDs = grantACLDto.EntityId;
                    var check = _templateRepository.GetAll().FirstOrDefault(i => i.Id == EntityIDs);
					

                    ACLList.ForEach(async i =>
                    {
                        try
                        {
                            ACL ACL = new ACL()
                            {
                                EntityID = grantACLDto.EntityId,
                                UserId = i.Type == "User" ? i.Id : null,
                                OrganizationUnitId = i.Type == "Team" ? i.Id : null,
                                Role = grantACLDto.Role,
                                TenantId = AbpSession.TenantId,
                                Type = Type
                            };
                            await _ACLManager.AddACL(ACL);

							//Add an entry on the basis of Original ID as well to resolve the Shared Edit Issue
                            if(check != null)
							{
                                ACL ACL2 = new ACL()
                                {
                                    EntityID = check.OriginalId,
                                    UserId = i.Type == "User" ? i.Id : null,
                                    OrganizationUnitId = i.Type == "Team" ? i.Id : null,
                                    Role = grantACLDto.Role,
                                    TenantId = AbpSession.TenantId,
                                    Type = Type
                                };
                                await _ACLManager.AddACL(ACL2);
                            }
                        }
                        catch (Exception)
                        {
                            throw new UserFriendlyException("Couldn't Grant ACL: " + i.Type == "User" ? "User" : "Team" + " not found");
                        }
                    });

                    //STQ MOdified
                    if (RecordList.Count > 0)
                    {
                        foreach (var record in RecordList)
                        {
                            ACLList.ForEach(async i =>
                            {
                                try
                                {
                                    ACL ACL = new ACL()
                                    {
                                        EntityID = record.Id,
                                        UserId = i.Type == "User" ? i.Id : null,
                                        OrganizationUnitId = i.Type == "Team" ? i.Id : null,
                                        Role = grantACLDto.Role,
                                        TenantId = AbpSession.TenantId,
                                        Type = "Record"
                                    };

                                    await _ACLManager.AddACL(ACL);
                                }
                                catch (Exception)
                                {
                                    throw new UserFriendlyException("Couldn't Grant ACL: " + i.Type == "User" ? "User" : "Team" + " not found");
                                }
                            });
                        }
                    }
                }
            }
            else
            {
                throw new UserFriendlyException("Couldn't Grant ACL: Denied");
            }
        }
        public async Task UpdateACL(ACLInput input)
		{
  
			if (_ACLManager.CheckAccess(new ACLCheckDto()
			{
				Action = "Edit",
				EntityId = input.EntityId,
				UserId = AbpSession.UserId
			}).IsAuthed)
			{
				ACL ACL = _aclRepository.Single(i => i.Id == input.ACLId);
				ACL.Role = input.Role;
				await _aclRepository.UpdateAsync(ACL);
			}
			else
			{
				throw new UserFriendlyException("Couldn't Grant ACL: Denied");
			}

		}

		public async Task RevokeACL(ACLInput input)
		{

			if (_ACLManager.CheckAccess(new ACLCheckDto()
			{
				Action = "Edit",
				EntityId = input.EntityId,
				UserId = AbpSession.UserId
			}).IsAuthed)
			{
				ACL ACL = _aclRepository.Single(i => i.Id == input.ACLId);
				await _aclRepository.DeleteAsync(ACL);
			}
			else
			{
				throw new Exception("Couldn't Grant ACL: Denied");
			}


		}

        //[AbpAuthorize(AppPermissions.Pages_ACLs_Edit)]>	Syntaq.Falcon.Application.dll!Syntaq.Falcon.AccessControlList.ACLsAppService.GetACLForEdit(Syntaq.Falcon.AccessControlList.Dtos.ACLCheckDto aCLCheckDto) Line 230	C#

        //[Interceptors.Attributes.SfaAuthorize]
        //[AbpAuthorize(AbpSession.UserId)]
        public ListResultDto<GetACLForEditOutput> GetACLForEdit(ACLCheckDto aCLCheckDto)
        {

			_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

            List<GetACLForEditOutput> output = new List<GetACLForEditOutput>();

            if (_ACLManager.CheckAccess(aCLCheckDto).IsAuthed)
			{



				List<ACL> acls = new List<ACL>();

                if (aCLCheckDto.Type == ACLCheckType.UserOrTeam)
                {
					acls = _aclRepository.GetAllIncluding(i => i.User, i => i.OrganizationUnit)
					.Where(i =>
						i.EntityID == aCLCheckDto.EntityId &&
						//i.CreatorUserId != i.UserId &&
						i.TargetTenantId == null
					).ToList();
				}

				if (aCLCheckDto.Type == ACLCheckType.Tenant)
				{
					acls = _aclRepository.GetAllIncluding(i => i.User, i => i.OrganizationUnit)
						.Where(i =>
							i.EntityID == aCLCheckDto.EntityId &&
							//i.CreatorUserId != i.UserId &&
							i.TargetTenantId != null 
						).ToList();
				}

				var t = AbpSession.TenantId;

				acls.Where(a => a.TenantId == AbpSession.TenantId).ToList().ForEach(i =>
				{
					GetACLForEditOutput getACLForEditOutput = new GetACLForEditOutput()
					{
						ACLId = i.Id,
						Accepted = i.Accepted,
						UserId = i.User?.Id,
						UserName = i.User?.FullName,
						UserEmail = i.User?.EmailAddress,
						OrganizationUnitId = i.OrganizationUnit?.Id,
						OrganizationUnitDisplayName = i.OrganizationUnit?.DisplayName,
						Role = i.Role,
						TargetTenantId = i.TargetTenantId,
						TargetTenantName = i.TargetTenantId != null ?  TenantManager.GetTenantName( (int)i.TargetTenantId).Result : string.Empty
					};
					if (getACLForEditOutput.UserId != null || getACLForEditOutput.OrganizationUnitId != null || getACLForEditOutput.TargetTenantId != null)
					{
						output.Add(getACLForEditOutput);
					}
				});
				return new ListResultDto<GetACLForEditOutput>(ObjectMapper.Map<List<GetACLForEditOutput>>(output));

			}
			else
			{
				throw new Exception("Couldn't Grant ACL: Denied");
			}

        }

    }
}