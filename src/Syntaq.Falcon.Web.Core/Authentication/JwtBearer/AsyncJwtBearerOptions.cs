using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Syntaq.Falcon.Web.Authentication.JwtBearer
{
    public class AsyncJwtBearerOptions : JwtBearerOptions
    {
        public readonly List<IAsyncSecurityTokenValidator> AsyncSecurityTokenValidators;
        
        private readonly FalconAsyncJwtSecurityTokenHandler _defaultAsyncHandler = new FalconAsyncJwtSecurityTokenHandler();

        public AsyncJwtBearerOptions()
        {
            AsyncSecurityTokenValidators = new List<IAsyncSecurityTokenValidator>() {_defaultAsyncHandler};
        }
    }

}
