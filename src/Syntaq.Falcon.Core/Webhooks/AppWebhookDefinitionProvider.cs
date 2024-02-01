using Abp.Localization;
using Abp.Webhooks;

namespace Syntaq.Falcon.WebHooks
{
    public class AppWebhookDefinitionProvider : WebhookDefinitionProvider
    {
        public override void SetWebhooks(IWebhookDefinitionContext context)
        {
            context.Manager.Add(new WebhookDefinition(
                name: AppWebHookNames.TestWebhook
            ));

            //Add your webhook definitions here 
            //STQ MODIFIED
            context.Manager.Add(new WebhookDefinition(
                name: AppWebHookNames.DocumentsGenerated,
                displayName: L("DocumentsGenerated")
            ));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, FalconConsts.LocalizationSourceName);
        }
    }
}
