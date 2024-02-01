using Syntaq.Falcon.ASIC.Dtos;
using Syntaq.Falcon.ASIC;
using Syntaq.Falcon.RecordPolicyActions.Dtos;
using Syntaq.Falcon.RecordPolicyActions;
using Syntaq.Falcon.RecordPolicies.Dtos;
using Syntaq.Falcon.RecordPolicies;
using Syntaq.Falcon.Tags.Dtos;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.EntityVersionHistories.Dtos;
using Syntaq.Falcon.EntityVersionHistories;
using Syntaq.Falcon.Authorization.Users.Dtos;
using Syntaq.Falcon.Projects.Dtos;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.VoucherEntitites.Dtos;
using Syntaq.Falcon.VoucherEntitites;
using Syntaq.Falcon.VoucherUsages.Dtos;
using Syntaq.Falcon.VoucherUsages;
using Syntaq.Falcon.Vouchers.Dtos;
using Syntaq.Falcon.Vouchers;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using Abp.Webhooks;
using AutoMapper;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Auditing.Dto;
using Syntaq.Falcon.Authorization.Accounts.Dto;
using Syntaq.Falcon.Authorization.Delegation;
using Syntaq.Falcon.Authorization.Permissions.Dto;
using Syntaq.Falcon.Authorization.Roles.Dto;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Authorization.Users.Delegation.Dto;
using Syntaq.Falcon.Authorization.Users.Dto;
using Syntaq.Falcon.Authorization.Users.Importing.Dto;
using Syntaq.Falcon.Authorization.Users.Profile.Dto;
using Syntaq.Falcon.Chat;
using Syntaq.Falcon.Chat.Dto;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Documents.Dtos;
using Syntaq.Falcon.Editions;
using Syntaq.Falcon.Editions.Dto;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Friendships;
using Syntaq.Falcon.Friendships.Cache;
using Syntaq.Falcon.Friendships.Dto;
using Syntaq.Falcon.Localization.Dto;
using Syntaq.Falcon.MergeTexts;
using Syntaq.Falcon.MergeTexts.Dtos;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.MultiTenancy.Dto;
using Syntaq.Falcon.MultiTenancy.HostDashboard.Dto;
using Syntaq.Falcon.MultiTenancy.Payments;
using Syntaq.Falcon.MultiTenancy.Payments.Dto;
using Syntaq.Falcon.Notifications.Dto;
using Syntaq.Falcon.Organizations.Dto;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Sessions.Dto;
using Syntaq.Falcon.WebHooks.Dto;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.Submissions.Dtos;
using Syntaq.Falcon.Teams;
using Syntaq.Falcon.Teams.Dtos;
using System.Collections.Generic;
using System.Linq;
using Syntaq.Falcon.Records.Dtos;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Forms.Dtos;
using Syntaq.Falcon.Apps.Dtos;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Folders.Dtos;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.MergeTexts.Dtos;
using Syntaq.Falcon.Documents.Dtos;
using Syntaq.Falcon.AccessControlList.Dtos;
using Syntaq.Falcon.AccessControlList;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.MergeTexts;
using Syntaq.Falcon.Users.Dtos;
using Syntaq.Falcon.Users;

//using FormRule = Syntaq.Falcon.Forms.FormRule;

namespace Syntaq.Falcon
{
    internal static class CustomDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditProjectDeploymentDto, ProjectDeployment>().ReverseMap();
            configuration.CreateMap<ProjectDeploymentDto, ProjectDeployment>().ReverseMap();
            configuration.CreateMap<CreateOrEditProjectTenantDto, ProjectTenant>().ReverseMap();
            configuration.CreateMap<ProjectTenantDto, ProjectTenant>().ReverseMap();
            configuration.CreateMap<CreateOrEditProjectReleaseDto, ProjectRelease>().ReverseMap();
            configuration.CreateMap<ProjectReleaseDto, ProjectRelease>().ReverseMap();
            configuration.CreateMap<CreateOrEditProjectEnvironmentDto, ProjectEnvironment>().ReverseMap();
            configuration.CreateMap<ProjectEnvironmentDto, ProjectEnvironment>().ReverseMap();

            configuration.CreateMap<CreateOrEditAsicDto, Asic>().ReverseMap();
            configuration.CreateMap<AsicDto, Asic>().ReverseMap();
            configuration.CreateMap<CreateOrEditRecordPolicyActionDto, RecordPolicyAction>().ReverseMap();
            configuration.CreateMap<RecordPolicyActionDto, RecordPolicyAction>().ReverseMap();
            configuration.CreateMap<CreateOrEditRecordPolicyDto, RecordPolicy>().ReverseMap();
            configuration.CreateMap<RecordPolicyDto, RecordPolicy>().ReverseMap();
            configuration.CreateMap<CreateOrEditTagValueDto, TagValue>().ReverseMap();
            configuration.CreateMap<TagValueDto, TagValue>().ReverseMap();
            configuration.CreateMap<CreateOrEditTagEntityTypeDto, TagEntityType>().ReverseMap();
            configuration.CreateMap<TagEntityTypeDto, TagEntityType>().ReverseMap();
            configuration.CreateMap<CreateOrEditTagEntityDto, TagEntity>().ReverseMap();
            configuration.CreateMap<TagEntityDto, TagEntity>().ReverseMap();
            configuration.CreateMap<CreateOrEditTagDto, Tag>().ReverseMap();
            configuration.CreateMap<TagDto, Tag>().ReverseMap();
            configuration.CreateMap<CreateOrEditEntityVersionHistoryDto, EntityVersionHistory>().ReverseMap();
            configuration.CreateMap<EntityVersionHistoryDto, EntityVersionHistory>().ReverseMap();
            configuration.CreateMap<CreateOrEditRecordMatterItemHistoryDto, RecordMatterItemHistory>().ReverseMap();
            configuration.CreateMap<RecordMatterItemHistoryDto, RecordMatterItemHistory>().ReverseMap();
            configuration.CreateMap<CreateOrEditRecordMatterAuditDto, RecordMatterAudit>().ReverseMap();
            configuration.CreateMap<RecordMatterAuditDto, RecordMatterAudit>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserPasswordHistoryDto, UserPasswordHistory>().ReverseMap();
            configuration.CreateMap<UserPasswordHistoryDto, UserPasswordHistory>().ReverseMap();
            configuration.CreateMap<CreateOrEditFormFeedbackDto, FormFeedback>().ReverseMap();
            configuration.CreateMap<FormFeedbackDto, FormFeedback>().ReverseMap();
            configuration.CreateMap<CreateOrEditRecordMatterContributorDto, RecordMatterContributor>().ReverseMap();
            configuration.CreateMap<RecordMatterContributorDto, RecordMatterContributor>().ReverseMap();
            configuration.CreateMap<CreateOrEditProjectDto, Project>().ReverseMap();
            configuration.CreateMap<ProjectDto, Project>().ReverseMap();

            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            //Chat
            configuration.CreateMap<ChatMessage, ChatMessageDto>();
            configuration.CreateMap<ChatMessage, ChatMessageExportDto>();

            //Feature
            configuration.CreateMap<FlatFeatureSelectDto, Feature>().ReverseMap();
            configuration.CreateMap<Feature, FlatFeatureDto>();

            //Role
            configuration.CreateMap<RoleEditDto, Authorization.Roles.Role>().ReverseMap();
            configuration.CreateMap<Authorization.Roles.Role, RoleListDto>();
            configuration.CreateMap<UserRole, UserListRoleDto>();

            //Edition
            configuration.CreateMap<EditionEditDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<EditionCreateDto, SubscribableEdition>();
            configuration.CreateMap<EditionSelectDto, SubscribableEdition>().ReverseMap();
            configuration.CreateMap<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<Edition, EditionInfoDto>().Include<SubscribableEdition, EditionInfoDto>();

            configuration.CreateMap<SubscribableEdition, EditionListDto>();
            configuration.CreateMap<Edition, EditionEditDto>();
            configuration.CreateMap<Edition, SubscribableEdition>();
            configuration.CreateMap<Edition, EditionSelectDto>();

            //Payment
            configuration.CreateMap<SubscriptionPaymentDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPaymentListDto, SubscriptionPayment>().ReverseMap();
            configuration.CreateMap<SubscriptionPayment, SubscriptionPaymentInfoDto>();

            //Permission
            configuration.CreateMap<Permission, FlatPermissionDto>();
            configuration.CreateMap<Permission, FlatPermissionWithLevelDto>();

            //Language
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageListDto>();
            configuration.CreateMap<NotificationDefinition, NotificationSubscriptionWithDisplayNameDto>();
            configuration.CreateMap<ApplicationLanguage, ApplicationLanguageEditDto>()
                .ForMember(ldto => ldto.IsEnabled, options => options.MapFrom(l => !l.IsDisabled));

            //Tenant
            configuration.CreateMap<Tenant, RecentTenant>();
            configuration.CreateMap<Tenant, TenantLoginInfoDto>();
            configuration.CreateMap<Tenant, TenantListDto>();
            configuration.CreateMap<TenantEditDto, Tenant>().ReverseMap();
            configuration.CreateMap<CurrentTenantInfoDto, Tenant>().ReverseMap();

            //User
            configuration.CreateMap<User, UserEditDto>()
                .ForMember(dto => dto.Password, options => options.Ignore())
                .ReverseMap()
                .ForMember(user => user.Password, options => options.Ignore());
            configuration.CreateMap<User, UserLoginInfoDto>();
            configuration.CreateMap<User, UserListDto>();
            configuration.CreateMap<User, ChatUserDto>();
            configuration.CreateMap<User, OrganizationUnitUserListDto>();
            configuration.CreateMap<Authorization.Roles.Role, OrganizationUnitRoleListDto>();
            configuration.CreateMap<CurrentUserProfileEditDto, User>().ReverseMap();
            configuration.CreateMap<UserLoginAttemptDto, UserLoginAttempt>().ReverseMap();
            configuration.CreateMap<ImportUserDto, User>();

            //AuditLog
            configuration.CreateMap<AuditLog, AuditLogListDto>();
            configuration.CreateMap<EntityChange, EntityChangeListDto>();
            configuration.CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

            //Friendship
            configuration.CreateMap<Friendship, FriendDto>();
            configuration.CreateMap<FriendCacheItem, FriendDto>();

            //OrganizationUnit
            configuration.CreateMap<OrganizationUnit, OrganizationUnitDto>();

            //Webhooks
            configuration.CreateMap<WebhookSubscription, GetAllSubscriptionsOutput>();
            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOutput>()
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.WebhookName,
                    options => options.MapFrom(l => l.WebhookEvent.WebhookName))
                .ForMember(webhookSendAttemptListDto => webhookSendAttemptListDto.Data,
                    options => options.MapFrom(l => l.WebhookEvent.Data));

            configuration.CreateMap<WebhookSendAttempt, GetAllSendAttemptsOfWebhookEventOutput>();

            //User Delegations
            configuration.CreateMap<CreateUserDelegationDto, UserDelegation>();

            /* ADD YOUR OWN CUSTOM AUTOMAPPER MAPPINGS HERE */

            configuration.CreateMap<VoucherEntity, CreateOrEditVoucherEntityDto>();
            configuration.CreateMap<Voucher, CreateOrEditVoucherDto>();

            configuration.CreateMap<CreateOrEditVoucherEntityDto, VoucherEntity>();
            configuration.CreateMap<VoucherEntity, VoucherEntityDto>();
            configuration.CreateMap<CreateOrEditVoucherUsageDto, VoucherUsage>();
            configuration.CreateMap<VoucherUsage, VoucherUsageDto>();
            configuration.CreateMap<CreateOrEditVoucherDto, Voucher>();
            configuration.CreateMap<Voucher, VoucherDto>();
            configuration.CreateMap<CreateOrEditSubmissionDto, Submission>();
            configuration.CreateMap<Submission, SubmissionDto>();
            configuration.CreateMap<CreateOrEditMergeTextDto, MergeText>();
            configuration.CreateMap<MergeText, MergeTextDto>();
            configuration.CreateMap<CreateOrEditMergeTextItemDto, MergeTextItem>();
            configuration.CreateMap<MergeTextItem, MergeTextItemDto>();
            configuration.CreateMap<CreateOrEditMergeTextItemValueDto, MergeTextItemValue>();
            configuration.CreateMap<MergeTextItemValue, MergeTextItemValueDto>();

            configuration.CreateMap<CreateOrEditFormRuleDto, FormRule>();
            configuration.CreateMap<FormRuleDto, FormRule>();

            configuration.CreateMap<Syntaq.Falcon.Documents.Template, CreateOrEditTemplateDto>();
            configuration.CreateMap<Apps.App, AppDto>();
            configuration.CreateMap<CreateOrEditFolderDto, Folder>();
            configuration.CreateMap<Folder, FolderDto>();
            configuration.CreateMap<CreateOrEditTemplateDto, Template>();
            configuration.CreateMap<Template, TemplateDto>();
            configuration.CreateMap<CreateOrEditACLDto, ACL>();
            configuration.CreateMap<ACL, ACLDto>();

            configuration.CreateMap<CreateOrEditFormDto, Form>().ForMember(x => x.CreatorUserId, opt => opt.Ignore());
            //Inputs
            configuration.CreateMap<CheckboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<SingleLineStringInputType, FeatureInputTypeDto>();
            configuration.CreateMap<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<IInputType, FeatureInputTypeDto>()
                .Include<CheckboxInputType, FeatureInputTypeDto>()
                .Include<SingleLineStringInputType, FeatureInputTypeDto>()
                .Include<ComboboxInputType, FeatureInputTypeDto>();
            configuration.CreateMap<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<ILocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>()
                .Include<StaticLocalizableComboboxItemSource, LocalizableComboboxItemSourceDto>();
            configuration.CreateMap<LocalizableComboboxItem, LocalizableComboboxItemDto>();
            configuration.CreateMap<ILocalizableComboboxItem, LocalizableComboboxItemDto>()
                .Include<LocalizableComboboxItem, LocalizableComboboxItemDto>();

            configuration.CreateMap<FormRule, FormRuleDto>();

            //APPS            
            configuration.CreateMap<Apps.App, CreateOrEditAppDto>();
            configuration.CreateMap<CreateOrEditAppDto, Apps.App>();

            configuration.CreateMap<AppJob, AppJobDto>();
            configuration.CreateMap<AppJob, CreateOrEditAppJobDto>();
            configuration.CreateMap<CreateOrEditAppJobDto, AppJob>();

            //Forms            
            configuration.CreateMap<CreateOrEditFormDto, Form>();
            configuration.CreateMap<Form, FormDto>();

            //Folders
            configuration.CreateMap<Folder, CreateOrEditFolderDto>();

            //Teams
            //configuration.CreateMap<Team, TeamDto>();
            configuration.CreateMap<OrganizationUnit, TeamDto>();
            configuration.CreateMap<CreateOrEditTeamDto, Team>();

            //Records
            configuration.CreateMap<Record, CreateOrEditRecordDto>();
            configuration.CreateMap<GetRecordForEditOutput, Record>();

            configuration.CreateMap<Record, RecordDto>();
            configuration.CreateMap<RecordDto, Record>();


            //Record Matters
            configuration.CreateMap<CreateOrEditRecordMatterDto, RecordMatter>();
            configuration.CreateMap<RecordMatter, CreateOrEditRecordMatterDto>();

            configuration.CreateMap<RecordMatter, RecordMatterDto>();
            configuration.CreateMap<RecordMatterDto, RecordMatter>();

            //Record Matter Items
            configuration.CreateMap<CreateOrEditRecordMatterItemDto, RecordMatterItem>();
            configuration.CreateMap<RecordMatterItem, CreateOrEditRecordMatterItemDto>();
            configuration.CreateMap<RecordMatterItem, RecordMatterItemDto>();
            configuration.CreateMap<RecordMatterItemDto, RecordMatterItem>();

            configuration.CreateMap<CreateOrEditUserAcceptanceTypeDto, UserAcceptanceType>().ReverseMap();
            configuration.CreateMap<UserAcceptanceTypeDto, UserAcceptanceType>().ReverseMap();

        }
    }
}