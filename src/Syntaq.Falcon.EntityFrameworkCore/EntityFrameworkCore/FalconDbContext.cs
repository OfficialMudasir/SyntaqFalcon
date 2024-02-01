//STQ MODIFIED
using Syntaq.Falcon.ASIC;
using Syntaq.Falcon.RecordPolicyActions;
using Syntaq.Falcon.RecordPolicies;
using Syntaq.Falcon.Tags;
using Syntaq.Falcon.EntityVersionHistories;
using Syntaq.Falcon.Projects;
using Syntaq.Falcon.VoucherEntitites;
using Syntaq.Falcon.VoucherUsages;
using Syntaq.Falcon.Vouchers;
using Syntaq.Falcon.Submissions;
using Syntaq.Falcon.MergeTexts;
using Syntaq.Falcon.Records;
using Syntaq.Falcon.Forms;
using Syntaq.Falcon.Folders;
using Syntaq.Falcon.Documents;
using Syntaq.Falcon.Apps;
using Syntaq.Falcon.AccessControlList;
using Abp.IdentityServer4vNext;

using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syntaq.Falcon.Authorization.Delegation;
using Syntaq.Falcon.Authorization.Roles;
using Syntaq.Falcon.Authorization.Users;
using Syntaq.Falcon.Chat;
using Syntaq.Falcon.Editions;
using Syntaq.Falcon.Friendships;
using Syntaq.Falcon.MultiTenancy;
using Syntaq.Falcon.MultiTenancy.Accounting;
using Syntaq.Falcon.MultiTenancy.Payments;
using Syntaq.Falcon.Storage;
using Syntaq.Falcon.Users;

namespace Syntaq.Falcon.EntityFrameworkCore
{
    public class FalconDbContext : AbpZeroDbContext<Tenant, Role, User, FalconDbContext>, IAbpPersistedGrantDbContext
    {
        public virtual DbSet<ProjectDeployment> ProjectDeployments { get; set; }

        public virtual DbSet<ProjectTenant> ProjectTenants { get; set; }

        public virtual DbSet<ProjectRelease> ProjectReleases { get; set; }

        public virtual DbSet<ProjectEnvironment> ProjectEnvironments { get; set; }

        public virtual DbSet<UserAcceptance> UserAcceptances { get; set; }

        public virtual DbSet<UserAcceptanceType> UserAcceptanceTypes { get; set; }

        public virtual DbSet<Asic> Asic { get; set; }

        public virtual DbSet<RecordPolicyAction> RecordPolicyActions { get; set; }

        public virtual DbSet<RecordPolicy> RecordPolicies { get; set; }

        public virtual DbSet<EntityVersionHistory> EntityVersionHistories { get; set; }

        public virtual DbSet<TagValue> TagValues { get; set; }

        public virtual DbSet<TagEntityType> TagEntityTypes { get; set; }

        public virtual DbSet<TagEntity> TagEntities { get; set; }

        public virtual DbSet<Tag> Tags { get; set; }

        public virtual DbSet<RecordMatterItemHistory> RecordMatterItemHistories { get; set; }

        public virtual DbSet<RecordMatterAudit> RecordMatterAudits { get; set; }

        public virtual DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }

        public virtual DbSet<FormFeedback> FormFeedbacks { get; set; }

        public virtual DbSet<RecordMatterContributor> RecordMatterContributors { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<VoucherEntity> VoucherEntities { get; set; }

        public virtual DbSet<VoucherUsage> VoucherUsages { get; set; }

        public virtual DbSet<Voucher> Vouchers { get; set; }

        public virtual DbSet<Submission> Submissions { get; set; }

        public virtual DbSet<MergeText> MergeTexts { get; set; }

        public virtual DbSet<MergeTextItem> MergeTextItems { get; set; }

        public virtual DbSet<MergeTextItemValue> MergeTextItemValues { get; set; }

        //public virtual DbSet<Report> Reports { get; set; }

        public virtual DbSet<FormRule> FormRules { get; set; }

        //public virtual DbSet<File> Files { get; set; }

        public virtual DbSet<Folder> Folders { get; set; }

        public virtual DbSet<AppJob> AppJobs { get; set; }

        public virtual DbSet<Apps.App> Apps { get; set; }

        public virtual DbSet<Template> Templates { get; set; }

        public virtual DbSet<ACL> ACLs { get; set; }

        //public virtual DbSet<TeamUserRole> TeamUserRoles { get; set; }

        public virtual DbSet<RecordMatterItem> RecordMatterItems { get; set; }

        public virtual DbSet<RecordMatter> RecordMatters { get; set; }

        public virtual DbSet<Record> Records { get; set; }

        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormType> FormTypes { get; set; }

        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public virtual DbSet<Invoice> Invoices { get; set; }

        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual DbSet<SubscriptionPaymentExtensionData> SubscriptionPaymentExtensionDatas { get; set; }

        public virtual DbSet<UserDelegation> UserDelegations { get; set; }

        public virtual DbSet<RecentPassword> RecentPasswords { get; set; }

        public FalconDbContext(DbContextOptions<FalconDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjectTenant>(p =>
            {
                p.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<ProjectDeployment>(p =>
                       {
                           p.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<ProjectTenant>(p =>
                       {
                           p.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<ProjectRelease>(p =>
                       {
                           p.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<ProjectEnvironment>(p =>
                       {
                           p.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<UserAcceptance>(u =>
                       {
                           u.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<UserAcceptanceType>(u =>
                       {
                           u.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Asic>(a =>
                       {
                           a.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<RecordPolicyAction>(r =>
                       {
                           r.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<RecordPolicy>(r =>
                       {
                           r.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<EntityVersionHistory>(x =>
            {
                x.HasIndex(e => new { e.TenantId });
            });
            modelBuilder.Entity<TagEntity>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<TagValue>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<TagEntityType>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<TagEntity>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Tag>(t =>
                       {
                           t.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<RecordMatterItemHistory>(r =>
                       {
                           r.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<RecordMatterAudit>(r =>
                       {
                           r.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<UserPasswordHistory>(u =>
                       {
                           u.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<FormFeedback>(f =>
                       {
                           f.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<RecordMatterContributor>(r =>
                       {
                           r.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Project>(p =>
                       {
                           p.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<VoucherEntity>(v =>
                       {
                           v.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<VoucherUsage>(v =>
                       {
                           v.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Voucher>(v =>
                       {
                           v.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Submission>(s =>
                       {
                           s.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<MergeText>(m =>
                       {
                           m.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<MergeTextItem>(m =>
                       {
                           m.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<MergeTextItemValue>(m =>
                       {
                           m.HasIndex(e => new { e.TenantId });
                       });
            //modelBuilder.Entity<Report>(r =>
            //           {
            //               r.HasIndex(e => new { e.TenantId });
            //           });
            modelBuilder.Entity<FormRule>(f =>
                       {
                           f.HasIndex(e => new { e.TenantId });
                       });
            //modelBuilder.Entity<File>(f =>
            //           {
            //               f.HasIndex(e => new { e.TenantId });
            //           });
            modelBuilder.Entity<Folder>(F =>
                       {
                           F.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Template>(T =>
                      modelBuilder.Entity<AppJob>(A =>
                       {
                           A.HasIndex(e => new { e.TenantId });
                       }));

            modelBuilder.Entity<Apps.App>(A =>
                       {
                           A.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<Template>(T =>
                       {
                           T.HasIndex(e => new { e.TenantId });
                       });
            modelBuilder.Entity<ACL>(A =>
                       {
                           A.HasIndex(e => new { e.TenantId });
                       });
            //modelBuilder.Entity<TeamUserRole>(T =>
            //           {
            //               T.HasIndex(e => new { e.TenantId });
            //           });

            modelBuilder.Entity<RecordMatterItem>(R =>
            {
                R.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<RecordMatter>(R =>
            {
                R.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<Record>(R =>
            {
                R.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<Form>(F =>
            {
                F.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<BinaryObject>(b =>
            {
                b.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { PaymentId = e.ExternalPaymentId, e.Gateway });
            });

            modelBuilder.Entity<SubscriptionPaymentExtensionData>(b =>
            {
                b.HasQueryFilter(m => !m.IsDeleted)
                    .HasIndex(e => new { e.SubscriptionPaymentId, e.Key, e.IsDeleted })
                    .IsUnique();
            });

            modelBuilder.Entity<UserDelegation>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.SourceUserId });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId });
            });

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}