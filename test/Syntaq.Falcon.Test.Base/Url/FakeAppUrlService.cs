using Syntaq.Falcon.Url;

namespace Syntaq.Falcon.Test.Base.Url
{
    public class FakeAppUrlService : IAppUrlService
    {
        public string CreateEmailActivationUrlFormat(int? tenantId)
        {
            return "http://test.com/";
        }

        public string CreatePasswordResetUrlFormat(int? tenantId)
        {
            return "http://test.com/";
        }

        public string CreateEmailActivationUrlFormat(string tenancyName)
        {
            return "http://test.com/";
        }

        public string CreatePasswordResetUrlFormat(string tenancyName)
        {
            return "http://test.com/";
        }

        /// <summary>
        ///  STQ Mofidified
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="returnurl"></param>
        /// <returns></returns>
        public string CreateEmailActivationUrlFormat(int? tenantId, string returnurl)
        {
            return "http://test.com/";
        }

        public string CreateEmailActivationUrlFormat(string tenancyName, string returnurl)
        {
            return "http://test.com/";
        }
    }
}
