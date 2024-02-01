using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Documents.Dtos;
using Syntaq.Falcon.EntityFrameworkCore.Repositories;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Storage;
using Syntaq.Falcon.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Syntaq.Falcon.EntityVersionHistories;

namespace Syntaq.Falcon.Documents
{
	[EnableCors("AllowAll")]
	//[AbpAuthorize(AppPermissions.Pages_Templates)]
	public class TemplatesAppService : FalconAppServiceBase, ITemplatesAppService
	{
		private const int MaxDocumentTemplateSize = 10485760; //10MB
				
		private readonly ACLManager _ACLManager;
		private readonly FolderManager _folderManager;
		private readonly IAppFolders _appFolders;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IRepository<ACL, int> _aclRepository;
		private readonly IRepository<Folder, Guid> _folderRepository;
		private readonly IRepository<Template, Guid> _templateRepository;
		private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
		private readonly ICustomDocumentTemplatesRepository _customtemplateRepository;
		private readonly ITempFileCacheManager _tempFileCacheManager;
		private readonly IRepository<EntityVersionHistory, Guid> _entityVersionHistoryRepository;
		public TemplatesAppService(
			ACLManager aclManager, 
			IRepository<ACL, int> aclRepository, 
			FolderManager folderManager, 
			IAppFolders appFolders, 
			IUnitOfWorkManager unitOfWorkManager, 
			IRepository<Folder, Guid> folderRepository, 
			IRepository<Template, Guid> templateRepository, 
			IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, 
			ITempFileCacheManager tempFileCacheManager, 
			ICustomDocumentTemplatesRepository customtemplateRepository, 
			IRepository<EntityVersionHistory, Guid> entityVersionHistoryRepository) 
		{
			_ACLManager = aclManager;
			_folderManager = folderManager;
			_appFolders = appFolders;
			_unitOfWorkManager = unitOfWorkManager;
			_customtemplateRepository = customtemplateRepository;
			_folderRepository = folderRepository;
			_aclRepository = aclRepository;
			_userOrganizationUnitRepository = userOrganizationUnitRepository;
			_templateRepository = templateRepository;
			_tempFileCacheManager = tempFileCacheManager;
			_entityVersionHistoryRepository = entityVersionHistoryRepository;
		}

		[AbpAuthorize(AppPermissions.Pages_Templates_Create, AppPermissions.Pages_ACLs)]
		public async Task<PagedResultDto<TemplateFoldersDto>> GetAll(GetAllTemplatesInput input)
		{

			Guid userRootFolderId = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, "T");
			Guid parentFolderId = (string.IsNullOrEmpty(input.Id) || input.Id == "00000000-0000-0000-0000-000000000000") ? userRootFolderId : new Guid(input.Id);
			input.Id = input.Id == "00000000-0000-0000-0000-000000000000" || input.Id == null ? userRootFolderId.ToString() : input.Id;

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = Guid.Parse(input.Id), UserId = AbpSession.UserId, TenantId = AbpSession.TenantId });

			if (ACLResult.IsAuthed)
			{
				input.Filter = input.Filter?.Trim();

				List<TemplateFoldersDto> dtoResult = new List<TemplateFoldersDto>();

                var templates = _templateRepository.GetAll()
                                        .Where(template =>
                                            template.CreatorUserId == (long)AbpSession.UserId &&
                                            template.FolderId == parentFolderId).ToList()
                                        .GroupBy(template => template.OriginalId)
                                        .SelectMany(group => group
                                            .OrderByDescending(template => template.CreationTime)
                                            .Take(1))
                                        .OrderByDescending(template => template.CreationTime)
                                        .ToList();

                // Old COde
                //var templates = _templateRepository
                //				.GetAll()
                //				.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Comments.Contains(input.Filter))
                //				.Where(e => e.FolderId == parentFolderId && e.Version == e.CurrentVersion)
                //				//.ToList()
                //				//.GroupBy(c => c.OriginalId)								 
                //				.Select(d => new Template
                //				{
                //					Id = d.Id,
                //					Version = d.Version,
                //					//Comments = grp.First().Comments,
                //					Comments = d.Comments,
                //					CurrentVersion = d.CurrentVersion,
                //					Name = d.Name,
                //					OriginalId = d.OriginalId,
                //					TenantId = d.TenantId,
                //					CreationTime = d.CreationTime
                //				})
                //				.ToList();

                var folders = _folderRepository.GetAll()
					.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e =>   e.Name.Contains(input.Filter))
					.Where(e => e.ParentId == parentFolderId && e.Type == "T")
					.ToList();

				if (parentFolderId == userRootFolderId)
				{

					// ACLS for shared document template
					var acls = _aclRepository.GetAll().Where(a => a.Type == "DocumentTemplate" && a.UserId == AbpSession.UserId && a.CreatorUserId != AbpSession.UserId).Select(a => a.EntityID).ToList();

					var sharedTemplates = (from template in _templateRepository
										   .GetAll()
										   .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Comments.Contains(input.Filter))
										   //.ToList()											
										   //join acl in _aclRepository.GetAll() on template.OriginalId equals acl.EntityID																					   
										   //where acl.CreatorUserId != AbpSession.UserId && acl.UserId == AbpSession.UserId                                           
										   where template.Version == template.CurrentVersion 
										   && acls.Contains(template.Id)
										   //where template.Name == string.IsNullOrEmpty(input.Filter) ? template.Name : input.Filter
										   //group template by template.OriginalId into grp											
										   select new Template
											{
												Id = template.Id,
												Version = template.Version ,
												Comments = template.Comments,
												CurrentVersion = template.CurrentVersion,
												Name = template.Name,
												OriginalId = template.OriginalId,
												TenantId = template.TenantId,
												CreationTime = template.CreationTime 
											}).ToList();



					var orgs = _userOrganizationUnitRepository.GetAll().Where(o => o.OrganizationUnitId == AbpSession.UserId ).Select(o => o.Id).ToList();
					acls = _aclRepository.GetAll().Where(a => a.Type == "DocumentTemplate" && orgs.Contains((long)a.OrganizationUnitId)).Select(a => a.EntityID).ToList();

					sharedTemplates = sharedTemplates.Concat(
											from template in _templateRepository
												.GetAll()
												.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Comments.Contains(input.Filter))
											
												//.ToList()
											//join acl in _aclRepository.GetAll() on template.OriginalId equals acl.EntityID
											//join ut in _userOrganizationUnitRepository.GetAll() on acl.OrganizationUnitId equals ut.OrganizationUnitId
											//where ut.UserId == AbpSession.UserId
                                            
											where template.Version == template.CurrentVersion
											&& acls.Contains(template.Id)
											//group template by template.OriginalId into grp
											
											select new Template
											{
												Id = template.Id,
												Version = template.Version,
												Comments = template.Comments,
												CurrentVersion = template.CurrentVersion,
												Name = template.Name,
												OriginalId = template.OriginalId,
												TenantId = template.TenantId,
												CreationTime = template.CreationTime
											}).ToList();

					//sharedTemplates = sharedtemplates.Distinct();
					templates = templates.Concat(sharedTemplates).Distinct().ToList();

					var sharedFolders = (from folder in _folderRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e =>   e.Name.Contains(input.Filter))
										join acl in _aclRepository.GetAll() on folder.Id equals acl.EntityID
										where acl.CreatorUserId != AbpSession.UserId && acl.UserId == AbpSession.UserId && folder.Type == "T"
										select folder).ToList();

					sharedFolders = (sharedFolders.Concat(from folder in _folderRepository.GetAll().WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e =>   e.Name.Contains(input.Filter))
										join acl in _aclRepository.GetAll() on folder.Id equals acl.EntityID
										join ut in _userOrganizationUnitRepository.GetAll() on acl.OrganizationUnitId equals ut.OrganizationUnitId
										where ut.UserId == AbpSession.UserId && folder.Type == "T"
										select new Folder
                                        {
                                            OrganizationUnitId = ut.OrganizationUnitId,
                                            ACLRole = acl.Role,
                                            Id = folder.Id,
                                            Name = folder.Name,
                                            Description = folder.Description,
                                        })).ToList();

					//sharedfolders = sharedfolders.Distinct();
					folders = folders.Concat(sharedFolders).Distinct().ToList();
				}

				var formcnt = templates.Count();
				var foldercnt = folders.Where(i => i.Name != "Your Templates").Count();
			
				folders.Where(i => i.Name != "Your Templates").Skip(input.SkipCount).Take(input.MaxResultCount).ToList().ForEach(i =>
				{
					var Result = new TemplateFoldersDto()
					{
						Id = i.Id,
						Name = i.Name,
						Description = i.Description,
						LastModified = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime,
						Type = "Folder",
						UserACLPermission = i.ParentId == parentFolderId ? ACLResult.Permission : _ACLManager.FetchRole(new ACLCheckDto() { EntityId = i.Id, UserId = AbpSession.UserId, OrgId = i.OrganizationUnitId != null ? i.OrganizationUnitId : null })
					};
					dtoResult.Add(Result);
				});

				var templatesToSkip = input.SkipCount == 0 ? input.SkipCount : (input.SkipCount - dtoResult.Count() - foldercnt) + dtoResult.Count();
				var templatesToTake = input.MaxResultCount - dtoResult.Count;

				List<TemplateDto> TemplatesList = new List<TemplateDto>();
				templates.OrderByDescending(i => i.CreationTime).Skip(templatesToSkip).Take(templatesToTake).ToList().ForEach(i => {
				
					var Result = new TemplateFoldersDto()
					{
						Id = i.Id,
						Name = i.Name,
						DocumentName = i.DocumentName,
						Version = i.Version,
						CurrentVersion = i.CurrentVersion,
						Comments = i.Comments,
						OriginalId = i.OriginalId,
						LastModified = i.CreationTime,
						Type = "Template",
						UserACLPermission = i.FolderId == parentFolderId ? ACLResult.Permission : _ACLManager.FetchRole(new ACLCheckDto() { EntityId = i.Id, UserId = AbpSession.UserId })
					};
					dtoResult.Add(Result);
				});

				IQueryable<TemplateFoldersDto> iQtemplateFolders;
				iQtemplateFolders = dtoResult.AsQueryable();
				iQtemplateFolders = iQtemplateFolders.OrderBy("type asc").ThenBy(input.Sorting ?? "type asc");//.PageBy(input);
				dtoResult = iQtemplateFolders.ToList();

				int totalCount = formcnt + foldercnt;

				return new PagedResultDto<TemplateFoldersDto>(
					totalCount,
					dtoResult
				);
			}
			else
			{
				return null;
			}


        }

		[Authorize(Policy = "ViewByOriginalId")]
		public async Task<List<GetTemplateForView>> GetVersionHistory(Guid OriginalId)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = OriginalId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
                Logger.Info("TemplatesAppService.GetVersionHistory.1");

                var query = (from o in _templateRepository.GetAll()
								.Where(i => i.OriginalId == OriginalId)
								.OrderByDescending(i => i.Version)
								select new GetTemplateForView() { Template =
									 new TemplateDto()
									 {
										 Comments = o.Comments,
										 CurrentVersion = o.CurrentVersion,
										 DocumentName = o.DocumentName,
										 Id = o.Id,
										 Name = o.Name,
										 OriginalId = o.OriginalId,
										 Version = o.Version,
										CreationTime = o.CreationTime,
										CreatorUserId = o.CreatorUserId
									 }
                                }
							 );

                Logger.Info("TemplatesAppService.GetVersionHistory.2");

                return new List<GetTemplateForView>(
					await query.ToListAsync()
				);

			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		[AbpAuthorize(AppPermissions.Pages_Templates_Edit)]
		public async Task<GetTemplateForEditOutput> GetTemplateForEdit(EntityDto<Guid> input)
        {

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
                Logger.Info("TemplatesAppService.GetTemplateForEdit.1");

                var check = await _templateRepository.GetAll().Where(i => i.OriginalId == input.Id).AnyAsync(i => i.CurrentVersion == i.Version);
				if (check)
				{
                    Logger.Info("TemplatesAppService.GetTemplateForEdit.2");

                    var temp = await _templateRepository.GetAll()
						.Where(i => i.OriginalId == input.Id && i.CurrentVersion == i.Version)
						.Select( i => new CreateOrEditTemplateDto() { 
							Comments = i.Comments,
							CurrentVersion = i.CurrentVersion,
							DocumentName = i.DocumentName,
							FolderId = i.FolderId,
							Id = i.Id,
							LockToTenant = i.LockToTenant,
							Name = i.Name,
							OriginalId = i.OriginalId,
							TenantId = i.TenantId,
							Version = i.Version							    
						 })
						.OrderByDescending(i => i.Version).FirstOrDefaultAsync();

                    Logger.Info("TemplatesAppService.GetTemplateForEdit.3");

                    return new GetTemplateForEditOutput { Template = temp };

				}

                Logger.Info("TemplatesAppService.GetTemplateForEdit.4");

                var template = await _templateRepository.GetAll()
					.Where(i => i.OriginalId == input.Id)
                    .Select(i => new CreateOrEditTemplateDto()
                    {
                        Comments = i.Comments,
                        CurrentVersion = i.CurrentVersion,
                        DocumentName = i.DocumentName,
                        FolderId = i.FolderId,
                        Id = i.Id,
                        LockToTenant = i.LockToTenant,
                        Name = i.Name,
                        OriginalId = i.OriginalId,
                        TenantId = i.TenantId,
                        Version = i.Version
                    })
                    .OrderByDescending(i => i.Version)
					.FirstOrDefaultAsync();
                
				Logger.Info("TemplatesAppService.GetTemplateForEdit.5");

                return new GetTemplateForEditOutput { Template = template };
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

        [AbpAuthorize(AppPermissions.Pages_Templates_Edit)]
        public async Task<GetTemplateForView> GetTemplateForView(EntityDto<Guid> input)
        {

            ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
            if (ACLResult.IsAuthed)
            {

                var check = await _templateRepository.GetAll().Where(i => i.OriginalId == input.Id).AnyAsync(i => i.CurrentVersion == i.Version);
                if (check)
                {
                    var temp = await _templateRepository.GetAll().Where(i => i.OriginalId == input.Id && i.CurrentVersion == i.Version).OrderByDescending(i => i.Version).FirstOrDefaultAsync();
                    return new GetTemplateForView { Template = ObjectMapper.Map<TemplateDto>(temp) };
                }
                var template = await _templateRepository.GetAll().Where(i => i.OriginalId == input.Id).OrderByDescending(i => i.Version).FirstOrDefaultAsync();
                return new GetTemplateForView { Template = ObjectMapper.Map<TemplateDto>(template) };
            }
            else
            {
                throw new UserFriendlyException("Not Authorised");
            }

        }

        public async Task<GetTemplateForEditOutput> GetTemplateForDownload(Guid OriginalId, string version)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = OriginalId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });

			// TODO Review
			var authorised = true; // ACLResult.IsAuthed;

#if STQ_PRODUCTION
			authorised = true;
#endif

			if (authorised)
			{
				GetTemplateForEditOutput output = null;
				using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
				{
					Template template = null;
					int.TryParse(version, out int VersionInt);
					if (VersionInt != 0 && VersionInt.GetType() == typeof(int))
					{
						template = await _templateRepository.GetAll().Where(i => i.OriginalId == OriginalId && i.Version == VersionInt).FirstOrDefaultAsync();
					}
					else if (!string.IsNullOrEmpty(version) && version.GetType() == typeof(string))
					{
						var temp = await _templateRepository.GetAll().Where(i => i.OriginalId == OriginalId).FirstOrDefaultAsync();
						template = await _templateRepository.GetAll().Where(i => i.OriginalId == OriginalId && i.Version == temp.CurrentVersion).FirstOrDefaultAsync();
					}
					else
					{
						try
						{
							var temp = await _templateRepository.GetAll().Where(i => i.OriginalId == OriginalId).FirstOrDefaultAsync();
							template = await _templateRepository.GetAll().Where(i => i.OriginalId == OriginalId && i.Version == temp.CurrentVersion).FirstOrDefaultAsync();
						}
						catch (Exception ex)
						{
							throw new UserFriendlyException("Document download failed because: " + ex);
						}
					}

					if (template.LockToTenant && template.TenantId != AbpSession.TenantId)
					{
						output = new GetTemplateForEditOutput
						{
							Template = new CreateOrEditTemplateDto
							{
								LockToTenant = template.LockToTenant,
								TenantId = template.TenantId,
								Document = null,
								DocumentName = string.Empty
							}
						};
					}
					else
					{
						output = new GetTemplateForEditOutput
						{
							Template = new CreateOrEditTemplateDto
							{
								LockToTenant = template.LockToTenant,
								TenantId = template.TenantId,
								Document = template.Document,
								DocumentName = template.DocumentName.ToLower().EndsWith(".docx") || template.DocumentName.ToLower().EndsWith(".doc") ? template.DocumentName : template.DocumentName + ".docx"
							}
						};
					}

				}
				return output;
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		[AbpAuthorize(AppPermissions.Pages_Templates_Create, AppPermissions.Pages_Templates_Edit)]
		public async Task CreateOrEdit(CreateOrEditTemplateDto input)
		{
			ACL aCL = new ACL(){ UserId = AbpSession.UserId };
			if (input.Version == 0)
			{
				aCL.Role = "O";
				await Create(aCL, input);
			}
			else
			{
				var template = _templateRepository.FirstOrDefaultAsync(i => i.OriginalId == input.OriginalId);
				if (template.Result == null)
				{
					aCL.Role = "O";
					await Create(aCL, input);
				}
				else
				{

					ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = (Guid) input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
					if (ACLResult.IsAuthed)
					{
						await Update(input);
					}
					else
					{
						throw new UserFriendlyException("Not Authorised");
					}

				}
			}
		}

		 private async Task Create(ACL ACL, CreateOrEditTemplateDto input)
		 {
			if(input.Id != input.OriginalId)
			{
				var templates = _templateRepository.GetAll().Where(i => i.OriginalId == input.OriginalId).OrderByDescending(i => i.Version);
				input.Version = templates.Max(i => i.Version) + 1;
				input.CurrentVersion = templates.First().CurrentVersion;
			}
			else
			{
				input.Version = 1;
				input.CurrentVersion = 1;
			}

			var template = ObjectMapper.Map<Template>(input);

			template.Document = GetDocumentTemplate(Id: input.OriginalId);

            // Continue. Unknown internal ASPOSE error on upload from Legal Consolidated // try and continue if error
            try{template.Document = AsposeUtility.BytesToWord(template.Document);}
            catch{}
			            
			if (AbpSession.TenantId != null)
			{
				template.TenantId = AbpSession.TenantId;
				ACL.TenantId = AbpSession.TenantId;
			}
		
			await _templateRepository.InsertAsync(template);
			ACL.EntityID = template.Id;
			ACL.Type = "DocumentTemplate";
			await _ACLManager.AddACL(ACL);

			//create a new template version and live version ==1	
			if (input.Version == 1 && input.CurrentVersion == 1)
			{
				var rma = new EntityVersionHistory()
				{
					Data = "{}",
					Name = input.Name,
					Description = "Create a New Template",
					Version = input.Version,
					PreviousVersion = input.CurrentVersion, //live version	
					EntityId = (Guid)input.Id,
					UserId = AbpSession.UserId,
					TenantId = AbpSession.TenantId,
					VersionName = "Version " + input.Version.ToString(),
					Type = "Template",
				};
				await _entityVersionHistoryRepository.InsertAsync(rma);

				ACL aCL = new ACL()
				{
					UserId = AbpSession.UserId,
					TenantId = AbpSession.TenantId,
					EntityID = rma.Id,
					Type = "EntityVersionHistory",
					Role = "O",
			};
				await _ACLManager.AddACL(aCL);
			}
		}

		 private async Task Update(CreateOrEditTemplateDto input)
		 {
			Template template = _templateRepository.GetAll().Where(i => i.OriginalId == input.OriginalId && i.Version == input.Version).First();

			if (AbpSession.TenantId != null)
			{
				template.TenantId = AbpSession.TenantId;
			}
            // If input has Document which come from Word Addin

            // If Updating
            // If null doc the use existing one

            byte[] doc = input.Document;
            if(doc == null ) doc =GetDocumentTemplate(Id: input.OriginalId);
            if(doc == null) doc = template.Document;

			template.Document = doc;
            template.LockToTenant = input.LockToTenant;
            template.DocumentName = input.DocumentName;
            template.Name = input.Name;
            template.Comments = input.Comments;

            await _templateRepository.UpdateAsync(template);

            //Keep all versions of the template has same name and document name??
            _templateRepository.GetAll().Where(i => i.OriginalId == input.OriginalId).ToList().ForEach(i => {
                i.DocumentName = input.DocumentName;
                i.Name = input.Name;
                i.Comments = input.Comments;
                i.LockToTenant = input.LockToTenant;
                _templateRepository.Update(i);
            });

			if (input.Version == input.CurrentVersion)
			{
				var rma = new EntityVersionHistory()
				{
					Data = "{}",
					Name = input.Name,
					Description = "Update the live Version",
					Version = input.Version,
					PreviousVersion = input.CurrentVersion, //live version	
					EntityId = (Guid)input.Id,
					UserId = AbpSession.UserId,
					TenantId = AbpSession.TenantId,
					VersionName = "Version " + input.Version.ToString(),
					Type = "Template",
				};
				await _entityVersionHistoryRepository.InsertAsync(rma);

				ACL aCL = new ACL()
				{
					UserId = AbpSession.UserId,
					TenantId = AbpSession.TenantId,
					EntityID = rma.Id,
					Type = "EntityVersionHistory",
					Role = "O",
				};
				await _ACLManager.AddACL(aCL);
			}


		}

		// TODO Move to tempalte manager       
		private byte[] GetDocumentTemplate(Guid Id)
		{
			byte[] DocumentBytes = _tempFileCacheManager.GetFile(Id.ToString());

            if (DocumentBytes != null)
            {
			    if (DocumentBytes.Length > MaxDocumentTemplateSize)
			    {
				    throw new UserFriendlyException(L("DocumentTemplate_Warn_SizeLimit", AppConsts.MaxDocumentTemplateBytesUserFriendlyValue));
			    }
            }

            return DocumentBytes;
		}

		private static byte[] ReadFile(string filePath)
		{
			byte[] buffer;
			FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			try
			{
				int length = (int)fileStream.Length;  // get file length
				buffer = new byte[length];            // create buffer
				int count;                            // actual number of bytes read
				int sum = 0;                          // total number of bytes read

				// read until Read method returns 0 (end of the stream has been reached)
				while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
					sum += count;  // sum is a buffer offset for next reading
			}
			finally
			{
				fileStream.Close();
			}
			return buffer;
		}


		[AbpAuthorize(AppPermissions.Pages_Templates_Delete)]
		public async Task<MessageOutput> DeleteIndividual(EntityDto<Guid> input)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Delete", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				using (var unitOfWork = _unitOfWorkManager.Begin())
				{
					await _templateRepository.DeleteAsync(input.Id);
					await _ACLManager.RemoveACL(new ACL() { EntityID = input.Id, UserId = AbpSession.UserId });
					unitOfWork.Complete();
				}
				return new MessageOutput()
				{
					Message = "Template Removed",
					Success = true
				};
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		[AbpAuthorize(AppPermissions.Pages_Templates_Delete)]
		public async Task<MessageOutput> DeleteAll(EntityDto<Guid> input)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Delete", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				using (var unitOfWork = _unitOfWorkManager.Begin())
				{
					var AllTemplatesByOriginalId = _templateRepository.GetAll().Where(i => i.OriginalId == input.Id);
					await AllTemplatesByOriginalId.ForEachAsync(async i =>
					{
						await _templateRepository.DeleteAsync(i.Id);

						//var ACL = _aclRepository.FirstOrDefault(n => n.EntityID == i.Id && n.UserId == AbpSession.UserId);
						//if (ACL != null)
						//	await _aclRepository.DeleteAsync(ACL);

					});
					unitOfWork.Complete();
				}
				return new MessageOutput()
				{
					Message = "Templates Removed",
					Success = true
				};

			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}
 

		}


		public async Task SetCurrent(TemplateVersionDto templateVersionDto)            
		{


			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = templateVersionDto.OriginalId, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{

				var templates = _templateRepository.GetAll().Where(i => i.OriginalId == templateVersionDto.OriginalId);


				//var templateOld = await _templateRepository.FirstOrDefaultAsync(i => i.OriginalId == templateVersionDto.OriginalId);	
				var templateNew = await _templateRepository.FirstOrDefaultAsync(i => i.OriginalId == templateVersionDto.OriginalId && i.Version == templateVersionDto.Version);


				foreach (Template template in templates)
				{
					template.CurrentVersion = templateVersionDto.Version;
					await _templateRepository.UpdateAsync(template);
				}
				
				var rma = new EntityVersionHistory()
				{
					Data = "{}",
					Name = templateNew.Name,
					Description = templateVersionDto.VersionDes,
					Version = templateVersionDto.Version,
					PreviousVersion = templateVersionDto.CurrentVersion, //previous live version	
					EntityId = templateNew.Id,
					UserId = AbpSession.UserId,
					TenantId = AbpSession.TenantId,
					VersionName = "Version " + templateVersionDto.Version.ToString(),
					Type = "Template",
				};
				await _entityVersionHistoryRepository.InsertAsync(rma);

				ACL aCL = new ACL()
				{
					UserId = AbpSession.UserId,
					TenantId = AbpSession.TenantId,
					EntityID = rma.Id,
					Type = "EntityVersionHistory",
					Role = "O",
				};
				await _ACLManager.AddACL(aCL);

			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

 
            
		}

		public async Task<bool> Move(MoveFolderDto moveFolderDto)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = moveFolderDto.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				moveFolderDto.UserId = AbpSession.UserId;
				return await _folderManager.Move(moveFolderDto);
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		// No ACL and Authorise check because this will be accessed by roles like author, annon.
		public async Task<GetTemplateForEditOutput> GetTemplateForUserAcceptanceViewers(EntityDto<Guid> input)
        {
			_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);
			var originalId = _templateRepository.FirstOrDefault(i => i.Id == input.Id).OriginalId;
			// always return the live version of the template
			var temp = await _templateRepository.GetAll().Where(i => i.OriginalId == originalId && i.CurrentVersion == i.Version).OrderByDescending(i => i.Version).FirstOrDefaultAsync();
            return new GetTemplateForEditOutput { Template = ObjectMapper.Map<CreateOrEditTemplateDto>(temp) };
        }
    }
}