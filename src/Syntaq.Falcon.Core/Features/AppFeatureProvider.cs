using Abp.Application.Features;
using Abp.Localization;
using Abp.Runtime.Validation;
using Abp.UI.Inputs;

namespace Syntaq.Falcon.Features
{
    public class AppFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            context.Create(
                AppFeatures.MaxUserCount,
                defaultValue: "0", //0 = unlimited
                displayName: L("MaximumUserCount"),
                description: L("MaximumUserCount_Description"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, int.MaxValue))
            )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            {
                ValueTextNormalizer = value => value == "0" ? L("Unlimited") : new FixedLocalizableString(value),
                IsVisibleOnPricingTable = true
            };


            var chatFeature = context.Create(
                AppFeatures.ChatFeature,
                defaultValue: "false",
                displayName: L("ChatFeature"),
                inputType: new CheckboxInputType()
            );

            chatFeature.CreateChildFeature(
                AppFeatures.TenantToTenantChatFeature,
                defaultValue: "false",
                displayName: L("TenantToTenantChatFeature"),
                inputType: new CheckboxInputType()
            );

            chatFeature.CreateChildFeature(
                AppFeatures.TenantToHostChatFeature,
                defaultValue: "false",
                displayName: L("TenantToHostChatFeature"),
                inputType: new CheckboxInputType()
            );

            //STQ MODIFIED
            var submissionLimit = context.Create(
                AppFeatures.SubmissionLimit,
                defaultValue: "true",
                displayName: L("SubmissionLimit"),
                inputType: new CheckboxInputType()
                );

            submissionLimit.CreateChildFeature(
                AppFeatures.SubmissionLimitAmount,
                defaultValue: "100",
                displayName: L("SubmissionLimitAmount"),
                inputType: new SingleLineStringInputType(new NumericValueValidator(0, 100000))
                )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
                {
                    ValueTextNormalizer = value => value == "100" ? L("100") : new FixedLocalizableString(value),
                    IsVisibleOnPricingTable = true
                };

            //var UserManagement = context.Create(
            //    AppFeatures.UserManagement,
            //    defaultValue: "false",
            //    displayName: L("UserManagement"),
            //    inputType: new CheckboxInputType()
            //    );

            //var CustomClientPortal = context.Create(
            //    AppFeatures.CustomClientPortal,
            //    defaultValue: "false",
            //    displayName: L("CustomClientPortal"),
            //    inputType: new CheckboxInputType()
            //    )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            //    {
            //        IsVisibleOnPricingTable = true,
            //        TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
            //    };

            var RoleManagement = context.Create(
                AppFeatures.RoleManagement,
                defaultValue: "false",
                displayName: L("RoleManagement"),
                inputType: new CheckboxInputType()
                )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
                {
                    IsVisibleOnPricingTable = true,
                    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
                };

            var TeamsManagement = context.Create(
                AppFeatures.TeamsManagement,
                defaultValue: "true",
                displayName: L("TeamsManagement"),
                inputType: new CheckboxInputType()
                )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
                {
                    IsVisibleOnPricingTable = true,
                    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
                };

            var AppBuilder = context.Create(
                AppFeatures.AppBuilder,
                defaultValue: "true",
                displayName: L("AppBuilder"),
                inputType: new CheckboxInputType()
                )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
                {
                    IsVisibleOnPricingTable = true,
                    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
                };

            var Forms = context.Create(
                AppFeatures.Forms,
                defaultValue: "true",
                displayName: L("Forms"),
                inputType: new CheckboxInputType()
                )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
                {
                    IsVisibleOnPricingTable = true,
                    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
                };

            var ASIC = context.Create(
               AppFeatures.ASIC,
               defaultValue: "true",
               displayName: L("ASIC"),
               inputType: new CheckboxInputType()
               )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
               {
                   IsVisibleOnPricingTable = true,
                   TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
               };

            var UserAcceptance = context.Create(
               AppFeatures.UserAcceptance,
               defaultValue: "false",
               displayName: L("UserAcceptance"),
               inputType: new CheckboxInputType()
               )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
               {
                   IsVisibleOnPricingTable = true,
                   TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
               };

            var Languages = context.Create(
                AppFeatures.Languages,
                defaultValue: "false",
                displayName: L("Multi-LanguageSupport"),
                inputType: new CheckboxInputType()
                )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
                {
                    IsVisibleOnPricingTable = true,
                    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
                };

            var VisualSettings = context.Create(
                AppFeatures.VisualSettings,
                defaultValue: "false",
                displayName: L("CustomClientPortal"),
                inputType: new CheckboxInputType()
                )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
                {
                    IsVisibleOnPricingTable = true,
                    TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
                };

            //var Forms = context.Create(
            //    AppFeatures.Forms,
            //    defaultValue: "true",
            //    displayName: L("Forms"),
            //    inputType: new CheckboxInputType()
            //    )[FeatureMetadata.CustomFeatureKey] = new FeatureMetadata
            //    {
            //        IsVisibleOnPricingTable = true,
            //        TextHtmlColor = value => value == "true" ? "#5cb85c" : "#d9534f"
            //    };
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, FalconConsts.LocalizationSourceName);
        }
    }
}
