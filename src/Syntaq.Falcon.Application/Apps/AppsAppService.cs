using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.Timing;
using Abp.Timing.Timezone;
using Abp.UI;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Common.Dto;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Documents.Models;
using Syntaq.Falcon.Filters;
using Syntaq.Falcon.Submissions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace Syntaq.Falcon.Apps
{
	[EnableCors("AllowAll")]
	//[AbpAuthorize(AppPermissions.Pages_Apps)]
	public class AppsAppService : FalconAppServiceBase, IAppsAppService
	{
		private readonly ACLManager _ACLManager;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IRepository<App, Guid> _appRepository;
		private readonly IDocumentsAppService _documentAppService;
		private readonly IRepository<AppJob, Guid> _appJobRepository;
		private readonly IRepository<ACL, int> _aclRepository;

		private readonly IRepository<Submission, Guid> _lookup_submissionRepository;

		private readonly ITimeZoneConverter _timeZoneConverter;

		public ILogger Logger { get; set; }

		public AppsAppService(
			ITimeZoneConverter timeZoneConverter,
			ACLManager aclManager, 
			IRepository<ACL, int> aclRepository, 
			IUnitOfWorkManager unitOfWorkManager, 
			IRepository<App, Guid> appRepository, 
			IDocumentsAppService documentAppService, 
			IRepository<AppJob, Guid> appJobRepository,
			IRepository<Submission, Guid> submissionRepository
			) 
		{
			_ACLManager = aclManager;
			_appRepository = appRepository;
			_appJobRepository = appJobRepository;
			_unitOfWorkManager = unitOfWorkManager;
			_documentAppService = documentAppService;
			_aclRepository = aclRepository;
			_timeZoneConverter = timeZoneConverter;
			_lookup_submissionRepository = submissionRepository;

			Logger = NullLogger.Instance;

		}

        [AbpAuthorize(AppPermissions.Pages_Apps)]
        public async Task<PagedResultDto<GetAppForView>> GetAll(GetAllAppsInput input)
		{
			List<AppDto> AppsList = new List<AppDto>();
			List<GetAppForView> AppsForViewList = new List<GetAppForView>();
			IQueryable<GetAppForView> QueryableAppsList;

			var FilteredApps = (_appRepository.GetAll()
						.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Description.Contains(input.Filter) || e.Data.Contains(input.Filter))
						.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim())
						.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter),  e => e.Description.ToLower() == input.DescriptionFilter.ToLower().Trim())
						.Where(i => i.CreatorUserId == AbpSession.UserId)).ToList();

			List<ACL> UserACRs = _ACLManager.FetchAllUserACLs(new GetAllACLsInput() { UserId = (long)AbpSession.UserId, EntityFilter = "App" });

			var SharedApps = from record in _appRepository.GetAll().ToList()
							join acl in UserACRs on record.Id equals acl.EntityID
							where acl.CreatorUserId != AbpSession.UserId
							select record;

			SharedApps = SharedApps.Distinct().ToList();

			FilteredApps = FilteredApps.Concat(SharedApps).ToList();

			ACLCheckDto aCLCheckDto = new ACLCheckDto()
			{
				UserId = AbpSession.UserId
			};

			FilteredApps.ToList().ForEach(i => 
			{
				aCLCheckDto.EntityId = i.Id;
				var Result = new AppDto()
				{
					Id = i.Id,
					Name = i.Name,
					Description = i.Description,
					LastModified = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime,
					UserACLPermission = _ACLManager.FetchRole(aCLCheckDto)
				};
				AppsList.Add(Result);
			});

			AppsList.ForEach(i =>
			{
				var Result = new GetAppForView()
				{
					App = i
				};
				AppsForViewList.Add(Result);
			});

			QueryableAppsList = AppsForViewList.AsQueryable();

			AppsForViewList = QueryableAppsList
				.OrderBy(input.Sorting ?? "app.id asc")
				.PageBy(input)
				.ToList();

			return new PagedResultDto<GetAppForView>(
				AppsList.Count(),
				AppsForViewList
			);
		}

        [AbpAuthorize(AppPermissions.Pages_Apps)]
        public async Task <List<AppDto>> GetAppsList()
		{
			List<AppDto> AppsList = new List<AppDto>();

			var FilteredApps = (_appRepository.GetAll()
						.Where(i => i.CreatorUserId == AbpSession.UserId)).ToList();

			List<ACL> UserACRs = _ACLManager.FetchAllUserACLs(new GetAllACLsInput() { UserId = (long)AbpSession.UserId, EntityFilter = "App" });

			var SharedApps = from record in _appRepository.GetAll().ToList()
							 join acl in UserACRs on record.Id equals acl.EntityID
							 where acl.CreatorUserId != AbpSession.UserId
							 select record;

			SharedApps = SharedApps.Distinct().ToList();

			FilteredApps = FilteredApps.Concat(SharedApps).ToList();

			ACLCheckDto aCLCheckDto = new ACLCheckDto()
			{
				UserId = AbpSession.UserId
			};

			FilteredApps.ToList().ForEach(i =>
			{
				aCLCheckDto.EntityId = i.Id;
				var Result = new AppDto()
				{
					Id = i.Id,
					Name = i.Name,
					Description = i.Description,
					LastModified = i.LastModificationTime == null ? i.CreationTime : (DateTime)i.LastModificationTime,
					UserACLPermission = _ACLManager.FetchRole(aCLCheckDto)
				};
				AppsList.Add(Result);
			});
			return AppsList;

		}

		[RequiresFeature("App.AppBuilder")]
		[AbpAuthorize(AppPermissions.Pages_Apps_Edit)]
		public async Task<GetAppForEditOutput> GetAppForEdit(EntityDto<Guid> input)
		{
			var app = await _appRepository.FirstOrDefaultAsync(i => i.Id == input.Id && i.CreatorUserId == AbpSession.UserId);

            var output = new GetAppForEditOutput {App = ObjectMapper.Map<CreateOrEditAppDto>(app)};			
			return output;
		}

#if STQ_PRODUCTION
		public void AutoZume(dynamic input)
		{
			string AppID = input.AppId;
			App app = new App();
			IQueryable<AppJob> jobs;

			using (var unitOfWork = _unitOfWorkManager.Begin())
			{
				app = _appRepository.Get(new Guid(AppID));
				jobs = _appJobRepository.GetAll().Where(i => i.AppId == new Guid(AppID));
				unitOfWork.Complete();
			}

			dynamic output = new ExpandoObject();
			output.data = input;
			output.Job = jobs.ToList();

			_documentAppService.Automate(output);
		}
#endif

		/// <summary>
		/// Runs an App, recieves a JSON app definition
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[AbpAllowAnonymous]
		// [DateTimeZoneNormailser] DOES not WORK input is reserialisedeach time
		public async Task<Guid> Run( dynamic input)
		{
            _unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant);

			var str = Convert.ToString(input);
			AppClass appClass = JsonConvert.DeserializeObject<AppClass>(str,
					new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });

			App app = new App();
            IQueryable<AppJob> jobs;

            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                app = _appRepository.Get(new Guid(appClass.Id));
                jobs = _appJobRepository.GetAll().Where(i => i.AppId == new Guid(appClass.Id));
                unitOfWork.Complete();
            }
 
            dynamic output = new ExpandoObject();
            output.appId = app.Id;
            output.AnonAuthToken = appClass.AnonAuthToken;
            output.data = string.IsNullOrEmpty(appClass.data.ToString()) ? app.Data : Convert.ToString(appClass.data);

            Guid submissionid = Guid.NewGuid();
            output.SubmissionId = submissionid;
            output.Job = jobs.ToList();

            if (Utility.DynamicUtility.IsPropertyExist(input, "FormId"))
            {
                output.FormId = Convert.ToString(input.FormId);
            }

            _documentAppService.Automate(output);
            return submissionid;
		}


		private void NormaliseDates(JObject input)
		{
			foreach (JToken token in input.Descendants())
			{
				WalkNode(token, null, prop =>
				{
					DateTime temp;
					if (DateTime.TryParse( Convert.ToString( prop.Value), out temp))
					{
						DateTime dt = DateTime.Parse(prop.Value.ToString());
						var newvalue = dt.ToString("yyyy-MM-ddTHH:mm:ss");
						prop.Value = newvalue;
					}

					//if (prop.Value.Type == JTokenType.Date)
					//{
					//	DateTime dt = DateTime.Parse(prop.Value.ToString());
					//	var newvalue = dt.ToString("yyyy-MM-ddTHH:mm:ss");
					//	prop.Value = newvalue;
					//}
				});
			}
		}

		private void WalkNode(JToken node, Action<JObject> objectAction = null, Action<JProperty> propertyAction = null)
		{
			if (node.Type == JTokenType.Object)
			{
				if (objectAction != null) objectAction((JObject)node);

				foreach (JProperty child in node.Children<JProperty>())
				{
					if (propertyAction != null) propertyAction(child);
					WalkNode(child.Value, objectAction, propertyAction);
				}
			}
			else if (node.Type == JTokenType.Array)
			{
				foreach (JToken child in node.Children())
				{
					WalkNode(child, objectAction, propertyAction);
				}
			}
		}

        /// <summary>
        /// Creates new app or updates an existing app
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_Apps_Edit)]
        public async Task CreateOrEdit(CreateOrEditAppDto input)
		{
			if (input.Id == null)
			{
				await Create(input);
			}
			else
			{

				var app = await _appRepository.FirstOrDefaultAsync(i => i.Id == (Guid)input.Id && i.CreatorUserId == AbpSession.UserId);
				if (app != null)
				{
					await Update(input);
				}
				else
				{
					throw new UserFriendlyException("Update Permission Denied");
				}

			}
		}

        [AbpAuthorize(AppPermissions.Pages_Apps_Edit)]
        public async Task CreateOrEditData(CreateOrEditAppDto input)
		{
			if (input.Id != null && input.Id != new Guid("00000000-0000-0000-0000-000000000000"))
			{
				var app = await _appRepository.FirstOrDefaultAsync( i => i.Id == (Guid)input.Id && i.CreatorUserId == AbpSession.UserId);
				app.Data = input.Data;
				await _appRepository.UpdateAsync(app);
			}
		}

		/// <summary>
		/// Creates new app and app ACL
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[RequiresFeature("App.AppBuilder")]
		[AbpAuthorize(AppPermissions.Pages_Apps_Create)]
		private async Task Create(CreateOrEditAppDto input)
		{
			var app = ObjectMapper.Map<App>(input);
			ACL ACL = new ACL() { UserId = AbpSession.UserId, Role = "O" };

			if (AbpSession.TenantId != null)
			{
				app.TenantId = AbpSession.TenantId;
				ACL.TenantId = AbpSession.TenantId;
			}
			using (var unitOfWork = _unitOfWorkManager.Begin())
			{
				await _appRepository.InsertAsync(app);
				ACL.EntityID = app.Id;
				ACL.Type = "App";
				await _ACLManager.AddACL(ACL);
				unitOfWork.Complete();
			}
		}

		[AbpAuthorize(AppPermissions.Pages_Apps_Edit)]
		private async Task Update(CreateOrEditAppDto input)
		{
			var app = await _appRepository.FirstOrDefaultAsync(i => i.Id == (Guid)input.Id && i.CreatorUserId == AbpSession.UserId);
			ObjectMapper.Map(input, app);
		}

		[AbpAuthorize(AppPermissions.Pages_Apps_Delete)]
		public async Task<MessageOutput> Delete(EntityDto<Guid> input)
		{

			if (_lookup_submissionRepository.GetAll().Any(i => i.AppId == input.Id))
			{
				throw new UserFriendlyException("Cannot delete App with Existing Submissions");
			}
			else
			{
				var app = await _appRepository.FirstOrDefaultAsync(i => i.Id == (Guid)input.Id && i.CreatorUserId == AbpSession.UserId);
				if (app != null)
				{				

					IQueryable<AppJob> appjobs = _appJobRepository.GetAll().Where(i => i.AppId == input.Id);
					appjobs.ToList().ForEach(async i =>
					{
						await _appJobRepository.DeleteAsync(i);
					});

					//IQueryable<ACL> ACLs = _aclRepository.GetAll().Where(i => i.EntityID == input.Id);
					//ACLs.ToList().ForEach(async i =>
					//{
					//	await _aclRepository.DeleteAsync(i);
					//});

					await _appRepository.DeleteAsync(input.Id);

					return new MessageOutput()
					{
						Message = "App Deleted",
						Success = true
					};
				}
				else
				{
					throw new UserFriendlyException("Delete Permission Denied");
				}
			}
		}
	}

}