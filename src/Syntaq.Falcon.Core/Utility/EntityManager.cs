using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Submissions;
using System;
using System.Linq;

namespace Syntaq.Falcon.Utility
{
	public class EntityManager : FalconDomainServiceBase
	{
		public enum SoftDeleteEntities { ACL, Form, Record, RecordMatter, RecordMatterItem, Submission };

		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IRepository<ACL> _aclRepository;
		private readonly IRepository<Form, Guid> _formRepository;
		private readonly IRepository<Record, Guid> _recordRepository;
		private readonly IRepository<RecordMatter, Guid> _recordMatterRepository;
		private readonly IRepository<RecordMatterItem, Guid> _recordMatterItemRepository;
		private readonly IRepository<Submission, Guid> _submissionRepository;

		public EntityManager(IUnitOfWorkManager unitOfWorkManager, IRepository<ACL> aclRepository, IRepository<Form, Guid> formRepository, IRepository<Record, Guid> recordRepository, IRepository<RecordMatter, Guid> recordMatterRepository, IRepository<RecordMatterItem, Guid> recordMatterItemRepository, IRepository<Submission, Guid> submissionRepository)
		{
			_unitOfWorkManager = unitOfWorkManager;
			_aclRepository = aclRepository;
			_formRepository = formRepository;
			_recordRepository = recordRepository;
			_recordMatterRepository = recordMatterRepository;
			_recordMatterItemRepository = recordMatterItemRepository;
			_submissionRepository = submissionRepository;
		}

		public bool IsSoftDelete(Guid? GuidId, int? IntId, SoftDeleteEntities type)
		{
			if (GuidId == null & IntId == null)
			{
				return false;
			}

			using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
			{
				bool DeletedExists = false;
				switch (type)
				{
					case (SoftDeleteEntities.ACL):
						var result1 = _aclRepository.GetAll().Any(i => i.Id == IntId);
						DeletedExists = result1;
						break;
					case (SoftDeleteEntities.Form):
						var result2 = _formRepository.GetAll().Any(i => i.Id == GuidId && i.IsDeleted);
						DeletedExists = result2;
						break;
					case (SoftDeleteEntities.Record):
						var result3 = _recordRepository.GetAll().Any(i => i.Id == GuidId && i.IsDeleted);
						DeletedExists = result3;
						break;
					case (SoftDeleteEntities.RecordMatter):
						var result4 = _recordMatterRepository.GetAll().Any(i => i.Id == GuidId && i.IsDeleted);
						DeletedExists = result4;
						break;
					case (SoftDeleteEntities.RecordMatterItem):
						var result5 = _recordMatterItemRepository.GetAll().Any(i => i.Id == GuidId && i.IsDeleted);
						DeletedExists = result5;
						break;
					case (SoftDeleteEntities.Submission):
						var result6 = _submissionRepository.GetAll().Any(i => i.Id == GuidId && i.IsDeleted);
						DeletedExists = result6;
						break;
				}
				return DeletedExists;
			}
		}
	}
}
