//using Syntaq.Falcon.Authorization.Users;

//using System;
//using System.Linq;
//using System.Linq.Dynamic.Core;
//using Abp.Linq.Extensions;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Abp.Domain.Repositories;
//using Syntaq.Falcon.Authorization.Users.Dtos;
//using Syntaq.Falcon.Dto;
//using Abp.Application.Services.Dto;
//using Syntaq.Falcon.Authorization;
//using Abp.Extensions;
//using Abp.Authorization;
//using Microsoft.EntityFrameworkCore;

//namespace Syntaq.Falcon.Authorization.Users
//{
//    [AbpAuthorize(AppPermissions.Pages_UserPasswordHistories)]
//    public class UserPasswordHistoriesAppService : FalconAppServiceBase, IUserPasswordHistoriesAppService
//    {
//        private readonly IRepository<UserPasswordHistory, Guid> _userPasswordHistoryRepository;
//        private readonly IRepository<User, long> _lookup_userRepository;

//        public UserPasswordHistoriesAppService(IRepository<UserPasswordHistory, Guid> userPasswordHistoryRepository, IRepository<User, long> lookup_userRepository)
//        {
//            _userPasswordHistoryRepository = userPasswordHistoryRepository;
//            _lookup_userRepository = lookup_userRepository;

//        }

//        public async Task<PagedResultDto<GetUserPasswordHistoryForViewDto>> GetAll(GetAllUserPasswordHistoriesInput input)
//        {

//            var filteredUserPasswordHistories = _userPasswordHistoryRepository.GetAll()
//                        .Include(e => e.UserFk)
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.PasswordHash.Contains(input.Filter))
//                        .WhereIf(!string.IsNullOrWhiteSpace(input.UserNameFilter), e => e.UserFk != null && e.UserFk.Name == input.UserNameFilter);

//            var pagedAndFilteredUserPasswordHistories = filteredUserPasswordHistories
//                .OrderBy(input.Sorting ?? "id asc")
//                .PageBy(input);

//            var userPasswordHistories = from o in pagedAndFilteredUserPasswordHistories
//                                        join o1 in _lookup_userRepository.GetAll() on o.UserId equals o1.Id into j1
//                                        from s1 in j1.DefaultIfEmpty()

//                                        select new GetUserPasswordHistoryForViewDto()
//                                        {
//                                            UserPasswordHistory = new UserPasswordHistoryDto
//                                            {
//                                                Id = o.Id
//                                            },
//                                            UserName = s1 == null || s1.Name == null ? "" : s1.Name.ToString()
//                                        };

//            var totalCount = await filteredUserPasswordHistories.CountAsync();

//            return new PagedResultDto<GetUserPasswordHistoryForViewDto>(
//                totalCount,
//                await userPasswordHistories.ToListAsync()
//            );
//        }

//        [AbpAuthorize(AppPermissions.Pages_UserPasswordHistories_Edit)]
//        public async Task<GetUserPasswordHistoryForEditOutput> GetUserPasswordHistoryForEdit(EntityDto<Guid> input)
//        {
//            var userPasswordHistory = await _userPasswordHistoryRepository.FirstOrDefaultAsync(input.Id);

//            var output = new GetUserPasswordHistoryForEditOutput { UserPasswordHistory = ObjectMapper.Map<CreateOrEditUserPasswordHistoryDto>(userPasswordHistory) };

//            if (output.UserPasswordHistory.UserId != null)
//            {
//                var _lookupUser = await _lookup_userRepository.FirstOrDefaultAsync((long)output.UserPasswordHistory.UserId);
//                output.UserName = _lookupUser?.Name?.ToString();
//            }

//            return output;
//        }

//        public async Task CreateOrEdit(CreateOrEditUserPasswordHistoryDto input)
//        {
//            if (input.Id == null)
//            {
//                await Create(input);
//            }
//            else
//            {
//                await Update(input);
//            }
//        }

//        [AbpAuthorize(AppPermissions.Pages_UserPasswordHistories_Create)]
//        protected virtual async Task Create(CreateOrEditUserPasswordHistoryDto input)
//        {
//            var userPasswordHistory = ObjectMapper.Map<UserPasswordHistory>(input);

//            if (AbpSession.TenantId != null)
//            {
//                userPasswordHistory.TenantId = (int?)AbpSession.TenantId;
//            }

//            await _userPasswordHistoryRepository.InsertAsync(userPasswordHistory);
//        }

//        [AbpAuthorize(AppPermissions.Pages_UserPasswordHistories_Edit)]
//        protected virtual async Task Update(CreateOrEditUserPasswordHistoryDto input)
//        {
//            var userPasswordHistory = await _userPasswordHistoryRepository.FirstOrDefaultAsync((Guid)input.Id);
//            ObjectMapper.Map(input, userPasswordHistory);
//        }

//        [AbpAuthorize(AppPermissions.Pages_UserPasswordHistories_Delete)]
//        public async Task Delete(EntityDto<Guid> input)
//        {
//            await _userPasswordHistoryRepository.DeleteAsync(input.Id);
//        }

//        [AbpAuthorize(AppPermissions.Pages_UserPasswordHistories)]
//        public async Task<PagedResultDto<UserPasswordHistoryUserLookupTableDto>> GetAllUserForLookupTable(GetAllForLookupTableInput input)
//        {
//            var query = _lookup_userRepository.GetAll().WhereIf(
//                   !string.IsNullOrWhiteSpace(input.Filter),
//                  e => e.Name != null && e.Name.Contains(input.Filter)
//               );

//            var totalCount = await query.CountAsync();

//            var userList = await query
//                .PageBy(input)
//                .ToListAsync();

//            var lookupTableDtoList = new List<UserPasswordHistoryUserLookupTableDto>();
//            foreach (var user in userList)
//            {
//                lookupTableDtoList.Add(new UserPasswordHistoryUserLookupTableDto
//                {
//                    Id = user.Id,
//                    DisplayName = user.Name?.ToString()
//                });
//            }

//            return new PagedResultDto<UserPasswordHistoryUserLookupTableDto>(
//                totalCount,
//                lookupTableDtoList
//            );
//        }
//    }
//}