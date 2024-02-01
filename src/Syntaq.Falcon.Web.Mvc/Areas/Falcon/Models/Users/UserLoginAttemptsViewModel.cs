using System.Collections.Generic;
using Abp.Application.Services.Dto;

namespace Syntaq.Falcon.Web.Areas.Falcon.Models.Users
{
    public class UserLoginAttemptsViewModel
    {
        public List<ComboboxItemDto> LoginAttemptResults { get; set; }
    }
}