using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Apps.Exporting;
using Syntaq.Falcon.Authorization;
using Syntaq.Falcon.Teams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Apps
{
	[EnableCors("AllowAll")]
	[AbpAuthorize(AppPermissions.Pages_AppJobs)]
	public class AppJobsAppService : FalconAppServiceBase, IAppJobsAppService
	{
		private readonly IRepository<AppJob, Guid> _appJobRepository;
		private readonly IAppJobsExcelExporter _appJobsExcelExporter;
		private readonly IRepository<App,Guid> _appRepository;
		private readonly TeamManager _teamManager;
		 
		public AppJobsAppService(IRepository<AppJob, Guid> appJobRepository, IAppJobsExcelExporter appJobsExcelExporter , IRepository<App, Guid> appRepository, TeamManager teamManager) 
		{
			_appJobRepository = appJobRepository;
			_appJobsExcelExporter = appJobsExcelExporter;
			_appRepository = appRepository;
			_teamManager = teamManager;
		}

		 //public async Task<PagedResultDto<GetAppJobForView>> GetAll(GetAllAppJobsInput input)
		 //{


			//var filteredAppJobs = _appJobRepository.GetAll()
			//			.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Name.Contains(input.Filter) || e.Data.Contains(input.Filter))
			//			.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter), e => e.Name.ToLower() == input.NameFilter.ToLower().Trim());

			//var query = (from o in filteredAppJobs
			//			 join o1 in _appRepository.GetAll() on o.AppId equals o1.Id into j1
			//			 from s1 in j1.DefaultIfEmpty()
			//			 select new GetAppJobForView() {
			//				 AppJob = ObjectMapper.Map<AppJobDto>(o), AppName = s1 == null ? "" : s1.Name.ToString()
			//			 })
			//			.WhereIf(!string.IsNullOrWhiteSpace(input.AppNameFilter), e => e.AppName.ToLower() == input.AppNameFilter.ToLower().Trim());

			//var totalCount = await query.CountAsync();

			//var appJobs = await query
			//	.OrderBy(input.Sorting ?? "appJob.id asc")
			//	.PageBy(input)
			//	.ToListAsync();

			//return new PagedResultDto<GetAppJobForView>(
			//	totalCount,
			//	appJobs
			//);
		 //}

		public async Task<PagedResultDto<GetAppJobForView>> GetJobsByAppId(EntityDto<Guid> input)
		{

			var app = await _appRepository.FirstOrDefaultAsync(i => i.Id == (Guid)input.Id && i.CreatorUserId == AbpSession.UserId);

			if (app != null)
			{
				var filteredAppJobs = _appJobRepository.GetAll().Where(i => i.AppId == input.Id);

				var appJobs = (from o in filteredAppJobs
							 join o1 in _appRepository.GetAll() on o.AppId equals o1.Id into j1
							 from s1 in j1.DefaultIfEmpty()
							 orderby ("appJob.Name asc")
							 select new GetAppJobForView()
							 {
								 AppJob = ObjectMapper.Map<AppJobDto>(o),
								 AppName = s1 == null ? "" : s1.Name.ToString()
							 }).ToList();

							//.WhereIf(!string.IsNullOrWhiteSpace(input.AppNameFilter), e => e.AppName.ToLower() == input.AppNameFilter.ToLower().Trim());

				var totalCount = appJobs.Count();
 
				return new PagedResultDto<GetAppJobForView>(
					totalCount,
					appJobs
				);
			}
			else
			{
				return new PagedResultDto<GetAppJobForView>(
					0,
					new List<GetAppJobForView> ()
				);
			}

		}

		//[AbpAuthorize(AppPermissions.Pages_AppJobs_Edit)]
		//public async Task<GetAppJobForEditOutput> GetAppJobForCreate(EntityDto<Guid> input)
		//{
		//    var output = new GetAppJobForEditOutput { Id = new Guid(), AppId = input.Id , AppJobName = "New Job" };
		//    return output;
		//}
		/// <summary>
		/// Retrieves App Job for edit, accepts Guid Id for input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		[AbpAuthorize(AppPermissions.Pages_AppJobs_Edit)]
		public async Task<GetAppJobForEditOutput> GetAppJobForEdit(EntityDto<Guid> input)
		{

			// TODO TEMP we are looking up by both Id and EntityID untill we modify Apps
			var appJob = await _appJobRepository.FirstOrDefaultAsync(input.Id);

			// Check to see if it exists via eintity ID TODO REVIEW
			if (appJob == null)
			{
				appJob = await _appJobRepository.GetAll().FirstOrDefaultAsync(i => i.EntityId == input.Id);
			}

			CreateOrEditAppJobDto data = new CreateOrEditAppJobDto();
			GetAppJobForEditOutput output = new GetAppJobForEditOutput();
			if (appJob != null)
			{
				data = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateOrEditAppJobDto>(appJob.Data);
				output = new GetAppJobForEditOutput { Id = input.Id, AppJobName = appJob.Name, AppName = "", AppJob = data };
			}
			else
			{
				output.AppJob = new CreateOrEditAppJobDto();
				output.EntityId = input.Id; // This is a new Job for an Enityz
			}

			// userIds are JSON strings used by the Job UI wizard to build the tag input
			// These are re serialised to the Record Users on create or update
			if (output.AppJob != null)
			{
				if (output.AppJob.Document != null)
				{

					output.AppJob.Document.ForEach(i =>
					{
						if (i.AllowWordAssignees != null)
						{
							string ACLJSON = JsonConvert.SerializeObject(i.AllowWordAssignees);
							i.AllowWordAssigneeJSON = ACLJSON;
						}

						if (i.AllowPdfAssignees != null)
						{
							string ACLJSON = JsonConvert.SerializeObject(i.AllowPdfAssignees);
							i.AllowPdfAssigneeJSON = ACLJSON;
						}

						if (i.AllowWordAssignees != null)
						{
							string ACLJSON = JsonConvert.SerializeObject(i.AllowHtmlAssignees);
							i.AllowHtmlAssigneeJSON = ACLJSON;
						}

					});
				}

				if (output.AppJob.RecordMatter != null)
				{
					output.AppJob.RecordMatter.ForEach(i => {
						//List<GrantACLDto> ACLList = JsonConvert.DeserializeObject<List<GrantACLDto>>(grantACLDto.Assignees.ToString());
						if (i.Assignees != null)
						{
							string ACLJSON = JsonConvert.SerializeObject(i.Assignees);
							i.AssigneeJSON = ACLJSON;
						}
						//TOREMOVE START
						else
						{
							var ACLJSON = "[";
							if (!string.IsNullOrEmpty(i.UserIds))
							{
								string[] iDs = i.UserIds.Split(',');
								iDs.ToList().ForEach(n =>
								{
									var user = UserManager.GetUserByIdAsync(Convert.ToInt64(n));     //Possible Tenant Context Issue here                        
									ACLJSON += "{\"Id\": " + n + ", \"value\" : \"" + user.Result.UserName + "\", \"Type\" : \"User\"}";

									if (n != iDs.Last())
									{
										ACLJSON += ",";
									}

									if (!string.IsNullOrEmpty(i.TeamIds) && n == iDs.Last())
									{
										ACLJSON += ",";
									}
								});
							}

							if (!string.IsNullOrEmpty(i.TeamIds))
							{
								string[] iDs = i.TeamIds.Split(',');
								iDs.ToList().ForEach(n =>
								{
									var team = _teamManager.GetTeamById(Convert.ToInt64(n));
									ACLJSON += "{\"Id\": " + n + ", \"value\" : \"" + team.DisplayName + "\", \"Type\" : \"Team\"}";
									if (n != iDs.Last())
									{
										ACLJSON += ",";
									}
								});
							}

							ACLJSON += "]";
							i.AssigneeJSON = ACLJSON;
						}				
					});
				}

				if (output.AppJob.AppId != null)
				{
					// Load For FORM / APP
					var app = await _appRepository.FirstOrDefaultAsync((Guid)appJob.AppId);
					if (app != null)
					{
						output.AppJobName = appJob.Name;
						output.AppName = app.Name.ToString();
						output.AppId = app.Id;
						output.Id = appJob.Id;
					}
				}

				output.AppJob.WorkFlow = output.AppJob.WorkFlow ?? new CreateOrEditAppJobWorkFlowDto();
				output.AppJob.WorkFlow.AfterAssembly = output.AppJob.WorkFlow.AfterAssembly ?? new List<CreateOrEditAppJobRestDto>();
				output.AppJob.WorkFlow.BeforeAssembly = output.AppJob.WorkFlow.BeforeAssembly ?? new List<CreateOrEditAppJobRestDto>();
				output.AppJob.WorkFlow.Email = output.AppJob.WorkFlow.Email ?? new List<CreateOrEditAppJobEmailDto>();

				if (output.AppJob.WorkFlow.Email != null)
				{
					//var EmailCount = output.AppJob.WorkFlow.Email.Count();
					output.AppJob.WorkFlow.Email.ForEach(i =>
					{
						if ((i.DocumentAttachmentIds != "") && (i.DocumentAttachmentIds != null))
						{
							string[] iDs = i.DocumentAttachmentIds.Split(',');

							var documentIds = "[";

							var isversion1 = int.TryParse(iDs[0], out int n);
							if (isversion1)
							{
								// v1 Format 
								// i.e. 1 numeric only
								var iDsCount = iDs.Count();
								var k = 1;
								output.AppJob.Document.ForEach(j =>
								{
									if (iDs.Any(l => int.Parse(l) == j.DocumentId))
									{
										documentIds += "{\"value\": " + j.DocumentId + ", \"text\" : \"" + j.DocumentName + "\"}";
										documentIds = k != iDsCount ? documentIds += "," : documentIds;
										k++;
									}
								});
							}
							else
							{
								// v2 format with support for file format types on attachments
								// i.e. 1|doc.pdf
								// 11-12-2019
								try
								{
									foreach (string doc in iDs)
									{
										string[] docitems = doc.Split('|');
										var docitemid = docitems[0];
										var docitemname = docitems[1];
										documentIds += "{\"value\": \"" + docitemid + "|" + docitemname + "\", \"text\" : \"" + docitemname + "\"} ,";
									}
								}
								catch
								{ // error loading document attachment format 
								}

								documentIds = documentIds.TrimEnd(',');
							}

							documentIds += "]";

							i.DocumentAttachmentIds = documentIds;

						}

						if ((i.EmailBodyDocumentIds != "") && (i.EmailBodyDocumentIds != null))
						{
							string[] iDs = i.EmailBodyDocumentIds.Split(',');
							var iDsCount = iDs.Count();
							var documentIds = "[";
							var k = 1;
							output.AppJob.Document.ForEach(j =>
							{
								if (iDs.Any(l => int.Parse(l) == j.DocumentId))
								{
									documentIds += "{\"value\": " + j.DocumentId + ", \"text\" : \"" + j.DocumentName + "\"}";
									documentIds = k != iDsCount ? documentIds += "," : documentIds;
									k++;
								}
							});
							documentIds += "]";

							i.EmailBodyDocumentIds = documentIds;
						}


					});
				}
			}

			return output;

		}

		[AbpAuthorize(AppPermissions.Pages_AppJobs_Create)]
		public async Task CreateOrEdit(CreateOrEditAppJobDto input)
		{

			var user = await UserManager.GetUserByIdAsync((long)AbpSession.UserId);

			input.User.ID = user.Id;
			input.User.Email = user.EmailAddress;

			if (input.RecordMatter != null)
			{
				input.RecordMatter.ForEach(i =>
				{
					i.Users = new List<CreateOrEditAppJobUserDto>();
					i.Teams = new List<CreateOrEditAppJobTeamDto>();
					i.Assignees.ForEach(j =>
					{
						if (j.Type == "User")
						{
							i.Users.Add(new CreateOrEditAppJobUserDto()
							{
								//ID = Convert.ToInt64(n == "" ? "1" : n),
								ID = Convert.ToInt64(j.Id),
								Permission = "E"
							});
						}
						else
						{
							i.Teams.Add(new CreateOrEditAppJobTeamDto()
							{
								//ID = Convert.ToInt64(n == "" ? "1" : n),
								ID = Convert.ToInt64(j.Id),
								Permission = "E"
							});
						}
					});
				});
			}

			// Parse the record User IDs
			//if (input.RecordMatter != null)
			//{
			//    input.RecordMatter.ForEach(i =>
			//    {
			//        i.Users = new List<CreateOrEditAppJobUserDto>();
			//        i.UserIds.Split(',').ToList().ForEach(n =>
			//        {
			//            if (!string.IsNullOrEmpty(n))
			//            {
			//                i.Users.Add(new CreateOrEditAppJobUserDto()
			//                {
			//                    //ID = Convert.ToInt64(n == "" ? "1" : n),
			//                    ID = Convert.ToInt64(n),
			//                    Permission = "E"
			//                });
			//            }
			//        });
			//    });
			//}

			// Parse the record Team IDs
			//if (input.RecordMatter != null)
			//{
			//    input.RecordMatter.ForEach(i =>
			//    {
			//        i.Teams = new List<CreateOrEditAppJobTeamDto>();
			//        i.TeamIds.Split(',').ToList().ForEach(n =>
			//        {
			//            if (!string.IsNullOrEmpty(n))
			//            {
			//                i.Teams.Add(new CreateOrEditAppJobTeamDto()
			//                {
			//                    //ID = Convert.ToInt64(n == "" ? "1" : n),
			//                    ID = Convert.ToInt64(n),
			//                    Permission = "E"
			//                });
			//            }
			//        });
			//    });
			//}

			if (AbpSession.TenantId != null)
			{
				input.TenantId = AbpSession.TenantId;
			}

			if (input.Id == null || input.Id == new Guid("00000000-0000-0000-0000-000000000000"))
			{
				await Create(input);
			}
			else
			{
				await Update(input);
			}

		}
		 [AbpAuthorize(AppPermissions.Pages_AppJobs_Create)]
		 private async Task Create(CreateOrEditAppJobDto input)
		 {

			try
			{
				string json = JsonConvert.SerializeObject(input);

				var appJob = ObjectMapper.Map<AppJob>(input);
				appJob.Data = json;
			
				//if (AbpSession.TenantId != null)
				//{
				   // appJob.TenantId = (int?) AbpSession.TenantId;
				//}
	   //         appJob.TenantId = null;

				await _appJobRepository.InsertAsync(appJob);
			}
			catch(Exception ex)
			{
				var c = ex;
			}


		 }

		 [AbpAuthorize(AppPermissions.Pages_AppJobs_Edit)]
		 private async Task Update(CreateOrEditAppJobDto input)
		 {
			var appJob = await _appJobRepository.FirstOrDefaultAsync((Guid)input.Id);

			// Check to see if it exists via eintity ID TODO REVIEW
			if (appJob == null)
			{
				appJob = await _appJobRepository.GetAll().FirstOrDefaultAsync(i => i.EntityId == input.Id);
			}

			string json = JsonConvert.SerializeObject(input);

			appJob.Data = json;
			appJob.Name = input.Name;
			await _appJobRepository.UpdateAsync(appJob);

			//ObjectMapper.Map(input, appJob);

		 }

		 [AbpAuthorize(AppPermissions.Pages_AppJobs_Delete)]
		 public async Task Delete(EntityDto<Guid> input)
		 {

			var appJob = await _appJobRepository.FirstOrDefaultAsync(input.Id);

			if (appJob != null)
			{
				var app = await _appRepository.FirstOrDefaultAsync(i => i.Id == (Guid)appJob.AppId && i.CreatorUserId == AbpSession.UserId);

				if (app != null)
				{
					await _appJobRepository.DeleteAsync(input.Id);
				}
				else
				{
					throw new UserFriendlyException("Delete Permission Denied");
				}
			}
			else
			{
				throw new UserFriendlyException("AppJob not Found");
			}
		 }

		 //public async Task<FileDto> GetAppJobsToExcel(GetAllAppJobsForExcelInput input)
   //      {
			
			//var filteredAppJobs = _appJobRepository.GetAll()
			//			.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false  || e.Name.Contains(input.Filter) || e.Data.Contains(input.Filter))
			//			.WhereIf(!string.IsNullOrWhiteSpace(input.NameFilter),  e => e.Name.ToLower() == input.NameFilter.ToLower().Trim());


			//var query = (from o in filteredAppJobs
   //                      join o1 in _appRepository.GetAll() on o.AppId equals o1.Id into j1
   //                      from s1 in j1.DefaultIfEmpty()
						 
   //                      select new GetAppJobForView() { AppJob = ObjectMapper.Map<AppJobDto>(o)
			//			 , AppName = s1 == null ? "" : s1.Name.ToString()
					
			//			 })
						 
			//			.WhereIf(!string.IsNullOrWhiteSpace(input.AppNameFilter), e => e.AppName.ToLower() == input.AppNameFilter.ToLower().Trim());


   //         var appJobListDtos = await query.ToListAsync();

   //         return _appJobsExcelExporter.ExportToFile(appJobListDtos);
   //      }

		 //[AbpAuthorize(AppPermissions.Pages_AppJobs)]
		 //public async Task<PagedResultDto<AppLookupTableDto>> GetAllAppForLookupTable(GetAllForLookupTableInput input)
		 //{
			// var query = _appRepository.GetAll().WhereIf(
			//		!string.IsNullOrWhiteSpace(input.Filter),
			//	   e=> e.Name.ToString().Contains(input.Filter)
			//	);

			//var totalCount = await query.CountAsync();

			//var appList = await query
			//	.PageBy(input)
			//	.ToListAsync();

			//var lookupTableDtoList = new List<AppLookupTableDto>();
			//foreach(var app in appList){
			//	lookupTableDtoList.Add(new AppLookupTableDto
			//	{
			//		Id = app.Id.ToString(),
			//		DisplayName = app.Name.ToString()
			//	});
			//}

			//return new PagedResultDto<AppLookupTableDto>(
			//	totalCount,
			//	lookupTableDtoList
			//);
		 //}
	}
}