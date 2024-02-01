namespace Syntaq.Falcon.Url
{
    public interface IAppUrlService
    {
        string CreateEmailActivationUrlFormat(int? tenantId, string returnurl);

        string CreatePasswordResetUrlFormat(int? tenantId);

        string CreateEmailActivationUrlFormat(string tenancyName, string returnurl);

        string CreatePasswordResetUrlFormat(string tenancyName);
    }
}
