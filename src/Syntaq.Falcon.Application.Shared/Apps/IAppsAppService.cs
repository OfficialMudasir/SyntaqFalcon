using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Common.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Syntaq.Falcon.Apps
{
	public interface IAppsAppService : IApplicationService 
	{
		Task<PagedResultDto<GetAppForView>> GetAll(GetAllAppsInput input);

		Task<GetAppForEditOutput> GetAppForEdit(EntityDto<Guid> input);
					 
		Task CreateOrEdit(CreateOrEditAppDto input);

		Task CreateOrEditData(CreateOrEditAppDto input);

		Task<MessageOutput> Delete(EntityDto<Guid> input);

		Task<List<AppDto>> GetAppsList();

		Task<Guid> Run(dynamic input);
	}
}