using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Records;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Folders
{
    public class FolderManager : FalconDomainServiceBase, IFolderManager
    {
        public IAbpSession _abpSession { get; set; }

        private readonly ACLManager _ACLManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Form, Guid> _formRepository;
        private readonly IRepository<Folder, Guid> _folderRepository;
        private readonly IRepository<Record, Guid> _recordRepository;
        private readonly IRepository<Template, Guid> _templateRepository;

        public FolderManager(ACLManager aCLManager, IUnitOfWorkManager unitOfWorkManager, IRepository<Form, Guid> formRepository, IRepository<Folder, Guid> folderRepository, IRepository<Record, Guid> recordRepository, IRepository<Template, Guid> templateRepository)
        {
            _ACLManager = aCLManager;
            _formRepository = formRepository;
            _folderRepository = folderRepository;
            _recordRepository = recordRepository;
            _templateRepository = templateRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual async Task CreateOrEditFolder(ACL ACL, Folder Folder)
        {
            if (Folder.Id == null || !_folderRepository.GetAll().Any(i => i.Id == Folder.Id))
            {
                ACL.Role = "O";
                await Create(ACL, Folder);
            }
            else
            {
                await Update(Folder);
            }
        }

        public virtual async Task<FolderDto> CreateAndOrFetchFolder(ACL acl, Folder folder, long userid)
        {
            FolderDto result;
            var UserRootFolder = _ACLManager.FetchUserRootFolder(userid, folder.Type);
            Guid ParentFolder = (string.IsNullOrEmpty(folder.Id.ToString()) || folder.Id.ToString() == "00000000-0000-0000-0000-000000000000") ? UserRootFolder : new Guid(folder.Id.ToString());
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                if (folder.Name != "Your Records" && folder.Name != "Your Forms") //Currently method only called from pipeline where folder context is records - if called from template context, add "Your Templates" option
                {

                    using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                    {

                        var f = _folderRepository.GetAll().FirstOrDefault(i => i.Id == folder.Id);
                        if (f.IsDeleted)
                        {
                            f.IsDeleted = false;
                            f.DeletionTime = null;
                            f.DeleterUserId = null;
                        }

                        if (folder.Id.ToString() == "00000000-0000-0000-0000-000000000000" || f == null)
                        {
                            acl.Role = "O";
                            folder.ParentId = ParentFolder;
                            await Create(acl, folder);

                        }

                        CurrentUnitOfWork.SaveChanges();
                    }


                }
                else
                {
                    folder.Id = ParentFolder;
                }

                ACLCheckDto aCLCheckDto = new ACLCheckDto()
                {
                    Action = "Edit", //ACL Param
                    EntityId = folder.Id,
                    UserId = acl.UserId,
                    AccessToken = acl.AccessToken
                };

                if (!_ACLManager.CheckAccess(aCLCheckDto).IsAuthed)
                {
                    // Allow jobs to accesssfolders
                    // throw new Exception();
                }

                var f2 = _folderRepository.GetAll().FirstOrDefault(f => f.Id == folder.Id);
                result = ObjectMapper.Map<FolderDto>(f2);

                unitOfWork.Complete();

            }
            return result;
        }

        private async Task Create(ACL ACL, Folder Folder)
        {
            if (_abpSession.TenantId != null)
            {
                Folder.TenantId = _abpSession.TenantId;
                ACL.TenantId = _abpSession.TenantId;
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
            if (_abpSession.TenantId != null)
            {
                Folder.TenantId = _abpSession.TenantId;
            }
            await _folderRepository.UpdateAsync(Folder);
        }

        public virtual async Task<bool> Move(MoveFolderDto moveFolderDto)
        {
            if (moveFolderDto.Id.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                //string Type = moveFolderDto.DraggableType.Substring(0, 1);
                moveFolderDto.Id = _ACLManager.FetchUserRootFolder((long)moveFolderDto.UserId, moveFolderDto.FolderType);
            }

            ACLCheckDto aCLCheckDto = new ACLCheckDto()
            {
                Action = "Edit",
                EntityId = moveFolderDto.DraggableId,
                UserId = moveFolderDto.UserId
            };
            bool IsDraggableAuthed = _ACLManager.CheckAccess(aCLCheckDto).IsAuthed;

            aCLCheckDto = new ACLCheckDto()
            {
                Action = "Edit",
                EntityId = moveFolderDto.Id,
                UserId = moveFolderDto.UserId
            };
            bool IsFolderAuthed = _ACLManager.CheckAccess(aCLCheckDto).IsAuthed;

            if (IsDraggableAuthed == true && IsFolderAuthed == true)
            {
                switch (moveFolderDto.DraggableType)
                {
                    case "Template":
                        Template template = _templateRepository.Get(Guid.Parse(moveFolderDto.DraggableId.ToString()));
                        var templates = _templateRepository.GetAll().Where(i => i.OriginalId == template.OriginalId);

                        foreach (Template temp in templates)
                        {
                            temp.FolderId = moveFolderDto.Id;
                            await _templateRepository.UpdateAsync(temp);
                        }
                        break;
                    case "Record":
                        _recordRepository.Get(Guid.Parse(moveFolderDto.DraggableId.ToString())).FolderId = moveFolderDto.Id;
                        _recordRepository.Update(_recordRepository.Get(Guid.Parse(moveFolderDto.DraggableId.ToString())));
                        break;
                    case "Form":
                        _formRepository.Get(Guid.Parse(moveFolderDto.DraggableId.ToString())).FolderId = moveFolderDto.Id;
                        _formRepository.Update(_formRepository.Get(Guid.Parse(moveFolderDto.DraggableId.ToString())));
                        break;
                    case "Folder":
                        _folderRepository.Get(Guid.Parse(moveFolderDto.DraggableId.ToString())).ParentId = moveFolderDto.Id;
                        _folderRepository.Update(_folderRepository.Get(Guid.Parse(moveFolderDto.DraggableId.ToString())));
                        break;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}