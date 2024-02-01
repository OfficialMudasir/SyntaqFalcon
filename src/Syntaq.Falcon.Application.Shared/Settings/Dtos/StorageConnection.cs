namespace Syntaq.Falcon.Web
{
    public class StorageConnection
    {
        public string ConnectionString { get; set; }
        public string BlobStorageContainer { get; set; }
    }

    public class AssemblyFunctionConnection
    {
        public string ConnectionString { get; set; }
    }

    public class NZBNConnection
    {
        public string Url { get; set; }
        public string AccessToken { get; set; }
    }

    public class FileValidationService
    {
        public string Url { get; set; }
        public string OcpApimSubscriptionKey { get; set; }
        public string ApiGovtNzInitiatedBy { get; set; }
    }

    public class AppConfig
    {
        public string WebSiteRootAddress { get; set; }
        public string RedirectAllowedExternalWebSites { get; set; }
        public string SwaggerEndPoint { get; set; }
    }

    public class GetEdgeConfig
    {
        public string XAuthEdge { get; set; }
        public string GetEdgeAPI { get; set; }
    }

    public class StGeorgeConfig
    {
        public string APIEndPoint{ get; set; } // "https://www.ipg.stgeorge.com.au/WebServiceAPI/service/transaction"
        public string AccessToken { get; set; }
        public string ClientID { get; set; }
    }

    public class JSONWebToken
    {
        public int Expiry { get; set; }
    }
}
