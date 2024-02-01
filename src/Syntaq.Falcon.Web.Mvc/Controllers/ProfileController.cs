using System;
using System.IO;
using System.Threading.Tasks;
using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetZeroCore.Net;
using Abp.Auditing;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Authorization.Users.Profile;
using Syntaq.Falcon.Authorization.Users.Profile.Dto;
using Syntaq.Falcon.Friendships;
using Syntaq.Falcon.Storage;

namespace Syntaq.Falcon.Web.Controllers
{
    [AbpMvcAuthorize]
    [DisableAuditing]
    public class ProfileController : ProfileControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IFriendshipManager _friendshipManager;


        // STQ MODIFIED
        private readonly IProfileAppService _profileAppService;

        public ProfileController(
                UserManager userManager,
                IBinaryObjectManager binaryObjectManager,
                ITempFileCacheManager tempFileCacheManager,
                IFriendshipManager friendshipManager,
                IProfileAppService profileAppService
            ) : base(tempFileCacheManager, profileAppService)
        {
            _userManager = userManager;
            _binaryObjectManager = binaryObjectManager;
            _friendshipManager = friendshipManager;
            _profileAppService = profileAppService;
        }

        public async Task<FileResult> GetProfilePicture()
        {
            var output = await _profileAppService.GetProfilePicture();

            if (output.ProfilePicture.IsNullOrEmpty())
            {
                return GetDefaultProfilePictureInternal();
            }

            return File(Convert.FromBase64String(output.ProfilePicture), MimeTypeNames.ImageJpeg);
        }
        
        public virtual async Task<FileResult> GetFriendProfilePicture(long userId, int? tenantId)
        {
            var output = await _profileAppService.GetFriendProfilePicture(new GetFriendProfilePictureInput()
            {
                TenantId = tenantId,
                UserId = userId
            });

            if (output.ProfilePicture.IsNullOrEmpty())
            {
                return GetDefaultProfilePictureInternal();
            }

            return File(Convert.FromBase64String(output.ProfilePicture), MimeTypeNames.ImageJpeg);
        }
        public async Task<FileResult> GetProfilePictureById(Guid id)
        {
            var file = await _binaryObjectManager.GetOrNullAsync(id);
            if (file == null)
            {
                return GetDefaultProfilePictureInternal();
            }

            return File(file.Bytes, MimeTypeNames.ImageJpeg);
        }

        // STQ MODIFIED
        public async Task<FileResult> GetLogoPicture(long userid)
        {
            var user = await _userManager.GetUserByIdAsync(userid); // (AbpSession.GetUserId());
            if (user.ProfilePictureId == null)
            {
                return GetDefaultLogoPicture();
            }

            return await GetProfilePictureById(user.ProfilePictureId.Value);
        }

        private FileResult GetDefaultLogoPicture()
        {
            return File(
                @"Common\Images\default-logo-picture.png",
                MimeTypeNames.ImagePng
            );
        }

    }
}
