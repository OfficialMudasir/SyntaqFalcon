using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
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
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Folders
{
	[EnableCors("AllowAll")]
	[AbpAuthorize(AppPermissions.Pages_Folders)]
	public class FoldersAppService : FalconAppServiceBase, IFoldersAppService
	{
		private readonly ACLManager _ACLManager;
		private readonly EntityManager _entityManager;
		private readonly FolderManager _folderManager;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IRepository<Form, Guid> _formRepository;
		private readonly IRepository<Folder, Guid> _folderRepository;
		private readonly IRepository<Record, Guid> _recordRepository;
        private readonly IRepository<Submissions.Submission, Guid> _submissionRepository;

        public FoldersAppService(
            ACLManager aCLManager
            , EntityManager entityManager
            , FolderManager folderManager
            , IUnitOfWorkManager unitOfWorkManager
            , IRepository<Form, Guid> formRepository
            , IRepository<Folder, Guid> folderRepository
            , IRepository<Record, Guid> recordRepository
            , IRepository<Submissions.Submission, Guid> submissionRepository
            ) 
		{
			_ACLManager = aCLManager;
			_entityManager = entityManager;
			_folderManager = folderManager;
			_unitOfWorkManager = unitOfWorkManager;
			_formRepository = formRepository;
			_folderRepository = folderRepository;
			_recordRepository = recordRepository;
            _submissionRepository = submissionRepository;

        }

		public async Task<List<FolderDto>> GetBreadcrumbs(string Id, string Type)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = new Guid(Id), UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				Guid RootFolder = new Guid("00000000-0000-0000-0000-000000000000");
				try
				{
					RootFolder = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, Type);
				}
				catch (Exception) { }

				Folder TopFolder = Id == "00000000-0000-0000-0000-000000000000" || Id == null ? _folderRepository.GetAll().Where(i => i.Id == RootFolder).FirstOrDefault() : _folderRepository.GetAll().Where(i => i.Id == Guid.Parse(Id)).FirstOrDefault();

				var CurrentFoldersTree = new List<FolderDto>();

				while (TopFolder.ParentId.ToString() != "00000000-0000-0000-0000-000000000000" && TopFolder.ParentId != null)
				{
					CurrentFoldersTree.Add(new FolderDto()
					{
						Id = TopFolder.Id,
						Name = TopFolder.Name,
						Description = TopFolder.Description
					});
					TopFolder = _folderRepository.GetAll().Where(i => i.Id == TopFolder.ParentId).First();
				}

				TopFolder = _folderRepository.GetAll().Where(i => i.Id == RootFolder).First();

				CurrentFoldersTree.Add(new FolderDto()
				{
					Id = TopFolder.Id,
					Name = TopFolder.Name,
					Description = TopFolder.Description
				});

				CurrentFoldersTree.Reverse();

				return CurrentFoldersTree;
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		public async Task<PagedResultDto<FolderLookupTableDto>> GetAllFoldersForLookupTable(GetAllForLookupTableInput input)
		{

			List<ACL> acls = _ACLManager.FetchAllUserACLs(new GetAllACLsInput() { UserId = (long)AbpSession.UserId });
			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "View", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				input.Filter = input.Filter?.Trim();

				Guid RootFolder = _ACLManager.FetchUserRootFolder((long)AbpSession.UserId, input.Type);
				IQueryable<Folder> query = null;
				if (input.Id == RootFolder || input.Id.ToString() == "00000000-0000-0000-0000-000000000000")
				{
					query = _folderRepository.GetAll().Where(i => i.ParentId == RootFolder && i.Type == input.Type).WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.Name.ToString().Contains(input.Filter));
					//Cater for non-owner folders
				}
				else
				{
					//Fetch Nested Folders
					//Cater for non-owner folders
				}

				var folderList = await query
					.PageBy(input)
					.ToListAsync();

				var lookupTableDtoList = new List<FolderLookupTableDto>();
				foreach (var folder in folderList)
				{
					lookupTableDtoList.Add(new FolderLookupTableDto
					{
						Id = folder.Id.ToString(),
						DisplayName = folder.Name.ToString()
					});
				}

				return new PagedResultDto<FolderLookupTableDto>(
					query.Count(),
					lookupTableDtoList
				);

			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}


		}

		public async Task<GetFolderForEditOutput> GetFolderForEdit(EntityDto<Guid> input)
		{


			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				var folder = await _folderRepository.FirstOrDefaultAsync(input.Id);
				var output = new GetFolderForEditOutput { Folder = ObjectMapper.Map<CreateOrEditFolderDto>(folder) };
				return output;
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}

		public async Task CreateOrEdit(CreateOrEditFolderDto input)
		{
			
			using (var unitOfWork = _unitOfWorkManager.Begin())
			{
				 
				Folder Folder = ObjectMapper.Map<Folder>(input);
				ACL aCL = new ACL() { UserId = AbpSession.UserId };

				if (Folder.Id == null || !_folderRepository.GetAll().Any(i => i.Id == Folder.Id))
				{
					aCL.Role = "O";
					await Create(aCL, Folder);
				}
				else
				{
					ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Edit", EntityId = (Guid)input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
					if (ACLResult.IsAuthed)
					{
						if (AbpSession.TenantId != null)
						{
							Folder.TenantId = AbpSession.TenantId;
						}						
						await Update(Folder);
						unitOfWork.Complete();
					}
					else
					{
						throw new UserFriendlyException("Not Authorised");
					}
				}

				unitOfWork.Complete();
			}

 
		}
 
		private async Task Create(ACL ACL, Folder Folder)
		{
			if (AbpSession.TenantId != null)
			{
				Folder.TenantId = AbpSession.TenantId;
				ACL.TenantId = AbpSession.TenantId;
			}
			using (var unitOfWork = _unitOfWorkManager.Begin())
			{
				await _folderRepository.InsertAsync(Folder);
				ACL.EntityID = Folder.Id;
				ACL.Type = "Folder";
				await _ACLManager.AddACL(ACL);
				unitOfWork.Complete();
			}
		}

		private async Task Update(Folder Folder)
		{
			if (AbpSession.TenantId != null)
			{
				Folder.TenantId = AbpSession.TenantId;
			}
			await _folderRepository.UpdateAsync(Folder);
		}

		public async Task<MessageOutput> Delete(EntityDto<Guid> input)
		{

			ACLResultDto ACLResult = _ACLManager.CheckAccess(new ACLCheckDto() { Action = "Delete", EntityId = input.Id, UserId = AbpSession.UserId, AccessToken = string.Empty, TenantId = AbpSession.TenantId });
			if (ACLResult.IsAuthed)
			{
				using (var unitOfWork = _unitOfWorkManager.Begin())
				{
					var folder = await _folderRepository.FirstOrDefaultAsync(input.Id);
					switch (folder.Type)
					{
						case "R":
							//using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
							//{
								if (_recordRepository.GetAll().Where(i => i.FolderId == folder.Id).Any())
								{
									//await _recordRepository.GetAll().Where(i => i.FolderId == folder.Id).ForEachAsync(i =>
									//{
									//	if (_entityManager.IsSoftDelete(i.Id, null, EntityManager.SoftDeleteEntities.Record))
									//	{
									//		_recordRepository.Delete(_recordRepository.Get(i.Id));
									//	}
									//});
									//_unitOfWorkManager.Current.SaveChanges();
									throw new UserFriendlyException("This Folder contains Records you cannot delete it.");
								}
							//}
							break;
						case "F":
							//using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
							//{
								if (_formRepository.GetAll().Where(i => i.FolderId == folder.Id).Any())
								{
									//await _formRepository.GetAll().Where(i => i.FolderId == folder.Id).ForEachAsync(i =>
									//{
									//	if (_entityManager.IsSoftDelete(i.Id, null, EntityManager.SoftDeleteEntities.Form))
									//	{
									//		_formRepository.Delete(_formRepository.Get(i.Id));

									//		_submissionRepository.GetAll().Where(n => n.FormId == i.Id).ForEachAsync(s => {
									//			s.FormId = null;
									//		});


									//	}
									//});
									//_unitOfWorkManager.Current.SaveChanges();

									throw new UserFriendlyException("This Folder contains Forms you cannot delete it.");
								}
							//}
							break;
					}


					_folderRepository.Delete(folder.Id);
					_unitOfWorkManager.Current.SaveChanges();

					//await _ACLManager.RemoveACL(new ACL() { EntityID = input.Id, UserId = AbpSession.UserId });
					//_unitOfWorkManager.Current.SaveChanges();
					unitOfWork.Complete();
				}
				return new MessageOutput()
				{
					Message = "Folder Removed",
					Success = true
				};
			}
			else
			{
				throw new UserFriendlyException("Not Authorised");
			}

		}
	}
}