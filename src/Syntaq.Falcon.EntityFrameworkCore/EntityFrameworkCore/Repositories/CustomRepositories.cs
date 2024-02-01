using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.EntityFrameworkCore;
using Syntaq.Falcon.EntityFrameworkCore.Repositories;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.MergeTexts;
using Syntaq.Falcon.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;

namespace Syntaq.Falcon.EntityFrameworkCore.Repositories
{

	public interface ICustomRecordsRepository : IRepository<Record, Guid>
	{
		IEnumerable<Record> GetAllWithRecords();
		IEnumerable<Record> GetAllWithRecordsSharedUser(long? UserId);
		IEnumerable<Record> GetAllWithRecordsSharedOrg(long? UserId);


		IEnumerable<Record> GetAllWithRecordsRaw(Guid? folderId, string filter, List<Guid> recordids, bool isArchived);
		IEnumerable<Record> GetAllWithRecordsSharedUserRaw(long? UserId);
		IEnumerable<Record> GetAllWithRecordsSharedOrgRaw(long? UserId);

	}

	public class CustomRecordsRepository : FalconRepositoryBase<Record, Guid>, ICustomRecordsRepository
	{
		public CustomRecordsRepository(IDbContextProvider<FalconDbContext> dbContextProvider) : base(dbContextProvider)
		{
		}

        public IEnumerable<Record> GetAllWithRecords()
        {
            var result = GetAll()
            .Include(r => r.RecordMatters)
            .ThenInclude(rm => rm.RecordMatterItems)
            .Select(r => new Record
            {
                Id = r.Id,
                Comments = r.Comments,
                CreationTime = r.CreationTime,
                LastModificationTime = r.LastModificationTime,
                CreatorUserId = r.CreatorUserId,
                FolderId = r.FolderId,
                RecordName = r.RecordName,
                TenantId = r.TenantId,
                UserId = r.UserId,
                RecordMatters = r.RecordMatters.OrderBy(rm => rm.Order).Select(rm => new RecordMatter
                {
                    // Data = rm.Data, do not include this in a bulk query - too large
                    HasFiles = rm.HasFiles,
                    Id = rm.Id,
                    Comments = rm.Comments,
                    RecordMatterName = rm.RecordMatterName,
                    Status = rm.Status,
                    UserId = rm.UserId,
                    TenantId = rm.TenantId,
                    Order = rm.Order,
                    CreationTime = rm.CreationTime,
                    LastModificationTime = rm.LastModificationTime,
                    FormId = rm.FormId == null ? rm.RecordMatterItems.FirstOrDefault() == null ? rm.FormId : rm.RecordMatterItems.FirstOrDefault().FormId : rm.FormId,
                    RequireApproval = rm.RequireApproval,
                    RequireReview = rm.RequireReview,
                    RecordMatterItems = rm.RecordMatterItems.Select(rmi => new RecordMatterItem
                    {
                        Id = rmi.Id,
                        GroupId = rmi.GroupId,
                        RecordMatterId = rmi.RecordMatterId,
                        FormId = rmi.FormId,
                        DocumentName = rmi.DocumentName,
                        HasDocument = rmi.HasDocument,
                        CreationTime = rmi.CreationTime,
                        AllowedFormats = rmi.AllowedFormats,
                        LastModificationTime = rmi.LastModificationTime,
                        Status = rmi.Status,
                        AllowWordAssignees = rmi.AllowWordAssignees,
                        AllowPdfAssignees = rmi.AllowPdfAssignees,
                        AllowHtmlAssignees = rmi.AllowHtmlAssignees,
                        LockOnBuild = rmi.LockOnBuild,
                        SubmissionId = rmi.SubmissionId,
                        Order = rmi.Order

                    }).ToList()
                }).ToList()
            });

            return result;

        }

        public IEnumerable<Record> GetAllWithRecordsSharedUser(long? userId)
		{

            var Context = this.GetContext();

            IQueryable<Record> result = from r in Context.Records.Include(record => record.RecordMatters).ThenInclude(recordmatter => recordmatter.RecordMatterItems)
                join a in Context.ACLs on r.Id equals a.EntityID 
                //join ut in Context.UserOrganizationUnits on a.OrganizationUnitId equals ut.OrganizationUnitId
                where a.UserId == userId && a.Role != "O"
                select new Record()
                {
                    Id = r.Id,
                    Comments = r.Comments,
                    CreationTime = r.CreationTime,
                    CreatorUserId = r.CreatorUserId,
                    FolderId = r.FolderId,
                    RecordName = r.RecordName,
                    TenantId = r.TenantId,
                    UserId = r.UserId,
                    //OrganizationUnitId = ut.OrganizationUnitId,
                    RecordMatters = r.RecordMatters.Select(rm => new RecordMatter
                    {
                        HasFiles = rm.HasFiles,
                        Id = rm.Id,
                        RecordMatterName = rm.RecordMatterName,
                        FormId = rm.FormId == null ? rm.RecordMatterItems.FirstOrDefault() == null ? rm.FormId : rm.RecordMatterItems.FirstOrDefault().FormId : rm.FormId,
                        UserId = rm.UserId,
                        TenantId = rm.TenantId,
                        Order = rm.Order,
                        CreationTime = rm.CreationTime,
                        RecordMatterItems = rm.RecordMatterItems.Select(rmi => new RecordMatterItem
                        {
                            Id = rmi.Id,
                            RecordMatterId = rmi.RecordMatterId,
                            FormId = rmi.FormId,
                            DocumentName = rmi.DocumentName,
                            HasDocument = rmi.HasDocument,
                            CreationTime = rmi.CreationTime,
                            AllowedFormats = rmi.AllowedFormats,
                            Order = rmi.Order,
                            AllowWordAssignees = rmi.AllowWordAssignees,
                            AllowPdfAssignees = rmi.AllowPdfAssignees,
                            AllowHtmlAssignees = rmi.AllowHtmlAssignees

                        }).ToList()
                    }).ToList()
                };
            return result;
        }

		public IEnumerable<Record> GetAllWithRecordsSharedOrg(long? userId)
		{
			var Context = this.GetContext();

			IQueryable<Record> result = from r in Context.Records.Include(record => record.RecordMatters).ThenInclude(recordmatter => recordmatter.RecordMatterItems)
										join a in Context.ACLs on r.Id equals a.EntityID
										join ut in Context.UserOrganizationUnits on a.OrganizationUnitId equals ut.OrganizationUnitId
										where ut.UserId == userId
										select new Record()
										{
											Id = r.Id,
											Comments = r.Comments,
											CreationTime = r.CreationTime,
											CreatorUserId = r.CreatorUserId,
											FolderId = r.FolderId,
											RecordName = r.RecordName,
											TenantId = r.TenantId,
											UserId = r.UserId,
											OrganizationUnitId = ut.OrganizationUnitId,
											RecordMatters = r.RecordMatters.Select(rm => new RecordMatter
											{
												HasFiles = rm.HasFiles,
												Id = rm.Id,
												RecordMatterName = rm.RecordMatterName,
												FormId = rm.FormId == null ? rm.RecordMatterItems.FirstOrDefault() == null ? rm.FormId : rm.RecordMatterItems.FirstOrDefault().FormId : rm.FormId,
												UserId = rm.UserId,
												TenantId = rm.TenantId,
												Order = rm.Order,
												CreationTime = rm.CreationTime,
												RecordMatterItems = rm.RecordMatterItems.Select(rmi => new RecordMatterItem
												{
													Id = rmi.Id,
													RecordMatterId = rmi.RecordMatterId,
													FormId = rmi.FormId,
													DocumentName = rmi.DocumentName,
													HasDocument = rmi.HasDocument,
													CreationTime = rmi.CreationTime,
													AllowedFormats = rmi.AllowedFormats,
													Order = rmi.Order,
													AllowWordAssignees = rmi.AllowWordAssignees,
													AllowPdfAssignees = rmi.AllowPdfAssignees,
													AllowHtmlAssignees = rmi.AllowHtmlAssignees

												}).ToList()
											}).ToList()
										};
			return result;
		}

		public IEnumerable<Record> GetAllWithRecordsRaw(Guid? folderId, string filter, List<Guid> recordIds, bool isArchived)
		{
			var Context = this.GetContext();

			var sql = $"select * from SfaRecords r ";

			var result = Context.Records.FromSqlRaw(sql)
				.Include(r => r.RecordMatters)
				.ThenInclude(rm => rm.RecordMatterItems)
				.Where 
					(e => 
						(recordIds.Count > 0 ? recordIds.Contains(e.Id) || recordIds.Contains(e.Id) || recordIds.Contains(e.Id) : string.IsNullOrEmpty(filter) ? e.FolderId == folderId : false ) &&
						e.IsArchived == isArchived
                    ) 

				.Select(r => new Record
				{
					Id = r.Id,
					Comments = r.Comments,
					CreationTime = r.CreationTime,
					LastModificationTime = r.LastModificationTime,
					IsArchived = r.IsArchived,
					CreatorUserId = r.CreatorUserId,
					FolderId = r.FolderId,
					RecordName = r.RecordName,
					TenantId = r.TenantId,
					UserId = r.UserId,
					RecordMatters = r.RecordMatters.OrderBy(rm => rm.Order).Select(rm => new RecordMatter
					{
						// Data = rm.Data, do not include this in a bulk query - too large
						HasFiles = rm.HasFiles,
						Id = rm.Id,
						Comments = rm.Comments,
						RecordMatterName = rm.RecordMatterName,
						Status = rm.Status,
						UserId = rm.UserId,
						TenantId = rm.TenantId,
						Order = rm.Order,
						CreationTime = rm.CreationTime,
						LastModificationTime = rm.LastModificationTime,
						FormId = rm.FormId == null? rm.RecordMatterItems.FirstOrDefault() == null? rm.FormId : rm.RecordMatterItems.FirstOrDefault().FormId : rm.FormId,
						RequireApproval = rm.RequireApproval,
						RequireReview = rm.RequireReview,
						RecordMatterItems = rm.RecordMatterItems.Select(rmi => new RecordMatterItem
						{
							Id = rmi.Id,
							GroupId = rmi.GroupId,
							RecordMatterId = rmi.RecordMatterId,
							FormId = rmi.FormId,
							DocumentName = rmi.DocumentName,
							HasDocument = rmi.HasDocument,
							CreationTime = rmi.CreationTime,
							AllowedFormats = rmi.AllowedFormats,
							LastModificationTime = rmi.LastModificationTime,
							Status = rmi.Status,
							AllowWordAssignees = rmi.AllowWordAssignees,
							AllowPdfAssignees = rmi.AllowPdfAssignees,
							AllowHtmlAssignees = rmi.AllowHtmlAssignees,
							LockOnBuild = rmi.LockOnBuild,
							Order = rmi.Order

						})
					})
				});

			return result;

		}

		public IEnumerable<Record> GetAllWithRecordsSharedUserRaw(long? userId)
		{
			var Context = this.GetContext();

			var sql = $"select r.Id, r.Comments,r.CreationTime, r.CreatorUserId, r.FolderId, r.RecordName, r.TenantId,r.UserId,r.LastModificationTime from SfaRecords r inner join SfaACLs a on a.entityid = r.id where a.userid = {userId} and a.creatoruserid <> {userId}";

			var result = Context.Records.FromSqlRaw(sql)
				.Include(r => r.RecordMatters)
				.ThenInclude(rm => rm.RecordMatterItems)
				.Select(r => new Record
				{
					Id = r.Id,
					Comments = r.Comments,
					CreationTime = r.CreationTime,
					LastModificationTime = r.LastModificationTime,
					CreatorUserId = r.CreatorUserId,
					FolderId = r.FolderId,
					RecordName = r.RecordName,
					TenantId = r.TenantId,
					UserId = r.UserId,
					RecordMatters = r.RecordMatters.OrderBy(rm => rm.Order).Select(rm => new RecordMatter
					{
						// Data = rm.Data, do not include this in a bulk query - too large
						HasFiles = rm.HasFiles,
						Id = rm.Id,
						Comments = rm.Comments,
						RecordMatterName = rm.RecordMatterName,
						Status = rm.Status,
						UserId = rm.UserId,
						TenantId = rm.TenantId,
						Order = rm.Order,
						CreationTime = rm.CreationTime,
						LastModificationTime = rm.LastModificationTime,
						FormId = rm.FormId == null ? rm.RecordMatterItems.FirstOrDefault() == null ? rm.FormId : rm.RecordMatterItems.FirstOrDefault().FormId : rm.FormId,
						RequireApproval = rm.RequireApproval,
						RequireReview = rm.RequireReview,
						RecordMatterItems = rm.RecordMatterItems.Select(rmi => new RecordMatterItem
						{
							Id = rmi.Id,
							GroupId = rmi.GroupId,
							RecordMatterId = rmi.RecordMatterId,
							FormId = rmi.FormId,
							DocumentName = rmi.DocumentName,
							HasDocument = rmi.HasDocument,
							CreationTime = rmi.CreationTime,
							AllowedFormats = rmi.AllowedFormats,
							LastModificationTime = rmi.LastModificationTime,
							Status = rmi.Status,
							AllowWordAssignees = rmi.AllowWordAssignees,
							AllowPdfAssignees = rmi.AllowPdfAssignees,
							AllowHtmlAssignees = rmi.AllowHtmlAssignees,
							LockOnBuild = rmi.LockOnBuild,
							Order = rmi.Order

						})
					})
				});

			return result;
		}

		public IEnumerable<Record> GetAllWithRecordsSharedOrgRaw(long? userId)
		{
			var Context = this.GetContext();

			var result = Context.Records.FromSqlInterpolated($"select * from SfaRecords r inner join SfaACLs a on a.entityid = r.id inner join AbpUserOrganizationUnits ut on a.OrganizationUnitId = ut.OrganizationUnitId where ut.userid = {userId}")
				.Include(r => r.RecordMatters)
				.ThenInclude(rm => rm.RecordMatterItems)
				.Select(r => new Record
				{
					Id = r.Id,
					Comments = r.Comments,
					CreationTime = r.CreationTime,
					LastModificationTime = r.LastModificationTime,
					CreatorUserId = r.CreatorUserId,
					FolderId = r.FolderId,
					RecordName = r.RecordName,
					TenantId = r.TenantId,
					UserId = r.UserId,
					RecordMatters = r.RecordMatters.OrderBy(rm => rm.Order).Select(rm => new RecordMatter
					{
						// Data = rm.Data, do not include this in a bulk query - too large
						HasFiles = rm.HasFiles,
						Id = rm.Id,
						Comments = rm.Comments,
						RecordMatterName = rm.RecordMatterName,
						Status = rm.Status,
						UserId = rm.UserId,
						TenantId = rm.TenantId,
						Order = rm.Order,
						CreationTime = rm.CreationTime,
						LastModificationTime = rm.LastModificationTime,
						FormId = rm.FormId == null ? rm.RecordMatterItems.FirstOrDefault() == null ? rm.FormId : rm.RecordMatterItems.FirstOrDefault().FormId : rm.FormId,
						RequireApproval = rm.RequireApproval,
						RequireReview = rm.RequireReview,
						RecordMatterItems = rm.RecordMatterItems.Select(rmi => new RecordMatterItem
						{
							Id = rmi.Id,
							GroupId = rmi.GroupId,
							RecordMatterId = rmi.RecordMatterId,
							FormId = rmi.FormId,
							DocumentName = rmi.DocumentName,
							HasDocument = rmi.HasDocument,
							CreationTime = rmi.CreationTime,
							AllowedFormats = rmi.AllowedFormats,
							LastModificationTime = rmi.LastModificationTime,
							Status = rmi.Status,
							AllowWordAssignees = rmi.AllowWordAssignees,
							AllowPdfAssignees = rmi.AllowPdfAssignees,
							AllowHtmlAssignees = rmi.AllowHtmlAssignees,
							LockOnBuild = rmi.LockOnBuild,
							Order = rmi.Order

						})
					})
				});



			return result;
		}


	}

	public interface ICustomDocumentTemplatesRepository : IRepository<Template, Guid>
	{
		IEnumerable<Template> GetAllForIndex();
	}

	public class CustomDocumentTemplatesRepository : FalconRepositoryBase<Template, Guid>, ICustomDocumentTemplatesRepository
	{
		public CustomDocumentTemplatesRepository(IDbContextProvider<FalconDbContext> dbContextProvider) : base(dbContextProvider)
		{
		}

		// Get all without schemas, scripts and rules
		public IEnumerable<Template> GetAllForIndex()
		{
			var Context = this.GetContext();

			var result = from f in Context.Templates
						 select new Template()
						 {
							 Id = f.Id,
							 Comments = f.Comments,
							 CreationTime = f.CreationTime,
							 CreatorUserId = f.CreatorUserId,
							 CurrentVersion = f.CurrentVersion,
							 Document = null,
							 DocumentName = f.DocumentName,
							 FolderId = f.FolderId,
							 Name = f.Name,
							 OriginalId = f.OriginalId,
							 TenantId = f.TenantId,
							 Version = f.Version
						 };
			return result;
		}
	}

	public interface ICustomFormsRepository : IRepository<Form, Guid>
	{
		IEnumerable<Form> GetAllForIndex(long? userId);
		IEnumerable<Form> GetSharedForms(long? userId);
	}

	public interface ICustomProjectsRepository : IRepository<Projects.Project, Guid>
	{
		IEnumerable<Projects.Project> GetSharedProjects(long? userId);
	}

	public class CustomFormsRepository : FalconRepositoryBase<Form, Guid>, ICustomFormsRepository
	{
		public CustomFormsRepository(IDbContextProvider<FalconDbContext> dbContextProvider) : base(dbContextProvider)
		{
		}

		// Get all without schemas, scripts and rules
		public IEnumerable<Form> GetAllForIndex(long? userId)
		{
			var result = GetAll().Include(f => f.Folder).Select(f => new Form()
			{
				Id = f.Id,
				CreationTime = f.CreationTime,
				CreatorUserId = f.CreatorUserId,
				FolderId = f.FolderId,
				TenantId = f.TenantId,
				CurrentVersion = f.CurrentVersion,
				Description = f.Description,
				LastModificationTime = f.LastModificationTime,
				LastModifierUserId = f.LastModifierUserId,
				Name = f.Name,
				OriginalId = f.OriginalId,
				PaymentAmount = f.PaymentAmount,
				PaymentCurrency = f.PaymentCurrency,
				PaymentEnabled = f.PaymentEnabled,
				Version = f.Version,
				VersionName = f.VersionName
			});

			//    var result = (from f in Context.Forms
			//join a in Context.ACLs on f.OriginalId equals a.EntityID
			//where a.UserId == userId // && a.CreatorUserId == userId
			//where f.Version == f.CurrentVersion
			//select new Form()
			//{
			// ACLRole = a.Role,
			// Id = f.Id,
			// CreationTime = f.CreationTime,
			// CreatorUserId = f.CreatorUserId,
			// FolderId = f.FolderId,
			// TenantId = f.TenantId,
			// CurrentVersion = f.CurrentVersion,
			// Description = f.Description,
			// LastModificationTime = f.LastModificationTime,
			// LastModifierUserId = f.LastModifierUserId,
			// Name = f.Name,
			// OriginalId = f.OriginalId,
			// PaymentAmount = f.PaymentAmount,
			// PaymentCurrency = f.PaymentCurrency,
			// PaymentEnabled = f.PaymentEnabled,
			// Version = f.Version,
			// VersionName = f.VersionName,
			//}).ToList();

			return result;
		}

		// Get all without schemas, scripts and rules
		public IEnumerable<Form> GetSharedForms(long? userId)
		{
			var Context = this.GetContext();

			var result = (from f in Context.Forms
						  join a in Context.ACLs on f.OriginalId equals a.EntityID
						  where a.UserId == userId && a.CreatorUserId != userId
						  where f.Version == f.CurrentVersion
						  select new Form()
						  {
							  ACLRole = a.Role,
							  Id = f.Id,
							  CreationTime = f.CreationTime,
							  CreatorUserId = f.CreatorUserId,
							  FolderId = f.FolderId,
							  TenantId = f.TenantId,
							  CurrentVersion = f.CurrentVersion,
							  Description = f.Description,
							  LastModificationTime = f.LastModificationTime,
							  LastModifierUserId = f.LastModifierUserId,
							  Name = f.Name,
							  OriginalId = f.OriginalId,
							  PaymentAmount = f.PaymentAmount,
							  PaymentCurrency = f.PaymentCurrency,
							  PaymentEnabled = f.PaymentEnabled,
							  Version = f.Version,
							  VersionName = f.VersionName,
						  }).ToList();

			result = (result.Concat(from f in Context.Forms
									join a in Context.ACLs on f.OriginalId equals a.EntityID
									join ut in Context.UserOrganizationUnits on a.OrganizationUnitId equals ut.OrganizationUnitId
									where ut.UserId == userId
									where f.Version == f.CurrentVersion
									select new Form()
									{
										OrganizationUnitId = ut.OrganizationUnitId,
										ACLRole = a.Role,
										Id = f.Id,
										CreationTime = f.CreationTime,
										CreatorUserId = f.CreatorUserId,
										FolderId = f.FolderId,
										TenantId = f.TenantId,
										CurrentVersion = f.CurrentVersion,
										Description = f.Description,
										LastModificationTime = f.LastModificationTime,
										LastModifierUserId = f.LastModifierUserId,
										Name = f.Name,
										OriginalId = f.OriginalId,
										PaymentAmount = f.PaymentAmount,
										PaymentCurrency = f.PaymentCurrency,
										PaymentEnabled = f.PaymentEnabled,
										Version = f.Version,
										VersionName = f.VersionName,
									})).ToList();

			return result;
		}
	}

	public class CustomProjectsRepository : FalconRepositoryBase<Projects.Project, Guid>, ICustomProjectsRepository
	{
		public CustomProjectsRepository(IDbContextProvider<FalconDbContext> dbContextProvider) : base(dbContextProvider)
		{
		}

		// Get all without schemas, scripts and rules
		public IEnumerable<Projects.Project> GetSharedProjects(long? userId)
		{
			this.UnitOfWorkManager.Current.DisableFilter(Abp.Domain.Uow.AbpDataFilters.MayHaveTenant);

			var Context = this.GetContext();

			var result = (from p in Context.Projects
						  join a in Context.ACLs on p.Id equals a.EntityID
						  where a.UserId == userId
						  select new Projects.Project()
						  {
							  Id = p.Id,
							  CreationTime = p.CreationTime,
							  CreatorUserId = p.CreatorUserId,
							  TenantId = p.TenantId,
							  Description = p.Description,
							  LastModificationTime = p.LastModificationTime,
							  LastModifierUserId = p.LastModifierUserId,
							  Name = p.Name,
							  Archived = p.Archived,
							  DeleterUserId = p.DeleterUserId,
							  DeletionTime = p.DeletionTime,
							  Enabled = p.Enabled,
							  Filter = p.Filter,
							  IsDeleted = p.IsDeleted,
							  ProjectTemplateId = p.ProjectTemplateId,
							  RecordId = p.RecordId,
							  Status = p.Status,
							  Type = p.Type
						  }).ToList();

			result = (result.Concat(from p in Context.Projects
									join a in Context.ACLs on p.Id equals a.EntityID
									join ouo in Context.UserOrganizationUnits on a.OrganizationUnitId equals ouo.OrganizationUnitId
									where ouo.UserId == userId
									select new Projects.Project()
									{
										Id = p.Id,
										CreationTime = p.CreationTime,
										CreatorUserId = p.CreatorUserId,
										TenantId = p.TenantId,
										Description = p.Description,
										LastModificationTime = p.LastModificationTime,
										LastModifierUserId = p.LastModifierUserId,
										Name = p.Name,
										Archived = p.Archived,
										DeleterUserId = p.DeleterUserId,
										DeletionTime = p.DeletionTime,
										Enabled = p.Enabled,
										Filter = p.Filter,
										IsDeleted = p.IsDeleted,
										ProjectTemplateId = p.ProjectTemplateId,
										RecordId = p.RecordId,
										Status = p.Status,
										Type = p.Type
									})).ToList();


			result = (result.Concat(from p in Context.Projects
									join a in Context.ACLs on p.Id equals a.EntityID
									join our in Context.OrganizationUnitRoles on a.OrganizationUnitId equals our.OrganizationUnitId
									join ur in Context.UserRoles on our.RoleId equals ur.RoleId
									where ur.UserId == userId
									select new Projects.Project()
									{
										Id = p.Id,
										CreationTime = p.CreationTime,
										CreatorUserId = p.CreatorUserId,
										TenantId = p.TenantId,
										Description = p.Description,
										LastModificationTime = p.LastModificationTime,
										LastModifierUserId = p.LastModifierUserId,
										Name = p.Name,
										Archived = p.Archived,
										DeleterUserId = p.DeleterUserId,
										DeletionTime = p.DeletionTime,
										Enabled = p.Enabled,
										Filter = p.Filter,
										IsDeleted = p.IsDeleted,
										ProjectTemplateId = p.ProjectTemplateId,
										RecordId = p.RecordId,
										Status = p.Status,
										Type = p.Type
									})).ToList();


			return result;
		}

	}

	public interface ICustomMergeTextRepository : IRepository<MergeText, long>
	{
		IEnumerable<MergeText> GetAllIncluding();
	}

	public class CustomMergeTextRepository : FalconRepositoryBase<MergeText, long>, ICustomMergeTextRepository
	{
		public CustomMergeTextRepository(IDbContextProvider<FalconDbContext> dbContextProvider) : base(dbContextProvider)
		{
		}

		// Get all without schemas, scripts and rules
		public IEnumerable<MergeText> GetAllIncluding()
		{
			var Context = this.GetContext();

			IEnumerable<MergeText> result = Context.MergeTexts.Include(i => i.MergeTextItems).ThenInclude(n => n.MergeTextItemValues);
			return result;
		}
	}

}