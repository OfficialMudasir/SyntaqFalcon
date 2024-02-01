using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Abp.AspNetZeroCore.Validation;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Extensions;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Security;

namespace Syntaq.Falcon.Web.Models.Account
{
    public class SetTenantViewModel 
    {
     
        public int? TenantId { get; set; }
        public String ReturnUrl { get; set; }
        // STQ Modified

    }
}