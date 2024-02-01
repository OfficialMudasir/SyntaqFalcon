using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Syntaq.Falcon.MergeTexts.Dtos;
using Syntaq.Falcon.Dto;

namespace Syntaq.Falcon.MergeTexts
{
    public interface IMergeTextItemValuesAppService : IApplicationService 
    {
        Task<PagedResultDto<GetMergeTextItemValueForViewDto>> GetAll(GetAllMergeTextItemValuesInput input);

#if STQ_PRODUCTION
		Task<GetMergeTextItemValueForEditOutput> GetMergeTextItemValueForEdit(EntityDto<long> input);
		Task CreateOrEdit(CreateOrEditMergeTextItemValueDto input);
		Task Delete(EntityDto<long> input);
#endif
		
    }
}