using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace mof.DataModels.Models
{
    public partial class MOFContext : DbContext
    {
        public MOFContext()
        {
        }

        public MOFContext(DbContextOptions<MOFContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agreement> Agreement { get; set; }
        public virtual DbSet<AgreementAct> AgreementAct { get; set; }
        public virtual DbSet<AgreementPaymentPlan> AgreementPaymentPlan { get; set; }
        public virtual DbSet<AgreementTrans> AgreementTrans { get; set; }
        public virtual DbSet<Amount> Amount { get; set; }
        public virtual DbSet<ApiClaims> ApiClaims { get; set; }
        public virtual DbSet<ApiLog> ApiLog { get; set; }
        public virtual DbSet<ApiProperties> ApiProperties { get; set; }
        public virtual DbSet<ApiResources> ApiResources { get; set; }
        public virtual DbSet<ApiScopeClaims> ApiScopeClaims { get; set; }
        public virtual DbSet<ApiScopes> ApiScopes { get; set; }
        public virtual DbSet<ApiSecrets> ApiSecrets { get; set; }
        public virtual DbSet<ApiSession> ApiSession { get; set; }
        public virtual DbSet<AttachFile> AttachFile { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<BsProvince> BsProvince { get; set; }
        public virtual DbSet<BsProvinceSection> BsProvinceSection { get; set; }
        public virtual DbSet<CeLov> CeLov { get; set; }
        public virtual DbSet<CeLovextend> CeLovextend { get; set; }
        public virtual DbSet<CeLovgroup> CeLovgroup { get; set; }
        public virtual DbSet<ClientClaims> ClientClaims { get; set; }
        public virtual DbSet<ClientCorsOrigins> ClientCorsOrigins { get; set; }
        public virtual DbSet<ClientGrantTypes> ClientGrantTypes { get; set; }
        public virtual DbSet<ClientIdPrestrictions> ClientIdPrestrictions { get; set; }
        public virtual DbSet<ClientPostLogoutRedirectUris> ClientPostLogoutRedirectUris { get; set; }
        public virtual DbSet<ClientProperties> ClientProperties { get; set; }
        public virtual DbSet<ClientRedirectUris> ClientRedirectUris { get; set; }
        public virtual DbSet<ClientScopes> ClientScopes { get; set; }
        public virtual DbSet<ClientSecrets> ClientSecrets { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<CurrencyRate> CurrencyRate { get; set; }
        public virtual DbSet<CurrencyYear> CurrencyYear { get; set; }
        public virtual DbSet<DataLog> DataLog { get; set; }
        public virtual DbSet<DataProtectionKeys> DataProtectionKeys { get; set; }
        public virtual DbSet<DebtPayAmt> DebtPayAmt { get; set; }
        public virtual DbSet<DeviceCodes> DeviceCodes { get; set; }
        public virtual DbSet<Dscrnote> Dscrnote { get; set; }
        public virtual DbSet<FileData> FileData { get; set; }
        public virtual DbSet<IdentityClaims> IdentityClaims { get; set; }
        public virtual DbSet<IdentityProperties> IdentityProperties { get; set; }
        public virtual DbSet<IdentityResources> IdentityResources { get; set; }
        public virtual DbSet<IipmAgency> IipmAgency { get; set; }
        public virtual DbSet<IipmApproval> IipmApproval { get; set; }
        public virtual DbSet<IipmMinistry> IipmMinistry { get; set; }
        public virtual DbSet<IipmPlanYearBudget> IipmPlanYearBudget { get; set; }
        public virtual DbSet<IipmProject> IipmProject { get; set; }
        public virtual DbSet<IipmProjectPlan> IipmProjectPlan { get; set; }
        public virtual DbSet<IipmSector> IipmSector { get; set; }
        public virtual DbSet<Iipmproj> Iipmproj { get; set; }
        public virtual DbSet<LawOfdebt> LawOfdebt { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<MaplanType> MaplanType { get; set; }
        public virtual DbSet<MasterAgreement> MasterAgreement { get; set; }
        public virtual DbSet<MasterAgreementMapping> MasterAgreementMapping { get; set; }
        public virtual DbSet<Ministry> Ministry { get; set; }
        public virtual DbSet<MonthRep> MonthRep { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Orglod> Orglod { get; set; }
        public virtual DbSet<OrgtoMany> OrgtoMany { get; set; }
        public virtual DbSet<Parameter> Parameter { get; set; }
        public virtual DbSet<PaymentPlan> PaymentPlan { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<PermissionGroup> PermissionGroup { get; set; }
        public virtual DbSet<PersistedGrants> PersistedGrants { get; set; }
        public virtual DbSet<Plan> Plan { get; set; }
        public virtual DbSet<PlanAct> PlanAct { get; set; }
        public virtual DbSet<PlanActAmount> PlanActAmount { get; set; }
        public virtual DbSet<PlanAgreement> PlanAgreement { get; set; }
        public virtual DbSet<PlanAttach> PlanAttach { get; set; }
        public virtual DbSet<PlanExist> PlanExist { get; set; }
        public virtual DbSet<PlanExistAgreement> PlanExistAgreement { get; set; }
        public virtual DbSet<PlanExtend> PlanExtend { get; set; }
        public virtual DbSet<PlanFinance> PlanFinance { get; set; }
        public virtual DbSet<PlanLoan> PlanLoan { get; set; }
        public virtual DbSet<PlanProject> PlanProject { get; set; }
        public virtual DbSet<PlanProjectFile> PlanProjectFile { get; set; }
        public virtual DbSet<PlanProjectResolution> PlanProjectResolution { get; set; }
        public virtual DbSet<ProjAct> ProjAct { get; set; }
        public virtual DbSet<ProjActAmount> ProjActAmount { get; set; }
        public virtual DbSet<ProjAmt> ProjAmt { get; set; }
        public virtual DbSet<ProjMaterial> ProjMaterial { get; set; }
        public virtual DbSet<ProjToMany> ProjToMany { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectExtend> ProjectExtend { get; set; }
        public virtual DbSet<ProjectLocation> ProjectLocation { get; set; }
        public virtual DbSet<ProjectResolution> ProjectResolution { get; set; }
        public virtual DbSet<ProposalPlan> ProposalPlan { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<Request> Request { get; set; }
        public virtual DbSet<RoleClaims> RoleClaims { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<ShareHolder> ShareHolder { get; set; }
        public virtual DbSet<SurrogateKey> SurrogateKey { get; set; }
        public virtual DbSet<UserClaims> UserClaims { get; set; }
        public virtual DbSet<UserLogins> UserLogins { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<UserTokens> UserTokens { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<YContracts> YContracts { get; set; }
        public virtual DbSet<YProjectTa> YProjectTa { get; set; }
        public virtual DbSet<ZLawTemplate> ZLawTemplate { get; set; }
        public virtual DbSet<ZLawTemplate2> ZLawTemplate2 { get; set; }
        public virtual DbSet<ZOrganizationToImport> ZOrganizationToImport { get; set; }
        public virtual DbSet<ZOrganizationToImport2> ZOrganizationToImport2 { get; set; }

        // Unable to generate entity type for table 'tb.y_contracts_to_import'. Please see the warning messages.
        // Unable to generate entity type for table 'tb.z_agreement_to_import_02'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.z_import_agreement'. Please see the warning messages.
        // Unable to generate entity type for table 'tb.Organization_1107'. Please see the warning messages.
        // Unable to generate entity type for table 'tb.z_organization_to_import_1107'. Please see the warning messages.
        // Unable to generate entity type for table 'tb.ORGToMany_1107'. Please see the warning messages.
        // Unable to generate entity type for table 'tb.Organization_0730'. Please see the warning messages.
        // Unable to generate entity type for table 'tb.z_agreement_to_import'. Please see the warning messages.
        // Unable to generate entity type for table 'tb.z_cf_to_import'. Please see the warning messages.
        // Unable to generate entity type for table 'tb.z_plan_to_import'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=common.mgtsvr.com;Initial Catalog=pdmo;MultipleActiveResultSets=true;User ID=sa;Password=DBShare@1234");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Agreement>(entity =>
            {
                entity.ToTable("Agreement", "tb");

                entity.Property(e => e.AgreementId).HasColumnName("AgreementID");

                entity.Property(e => e.AcctAssRefName).HasMaxLength(255);

                entity.Property(e => e.AgreementNameTh)
                    .HasColumnName("AgreementNameTH")
                    .HasMaxLength(255);

                entity.Property(e => e.Counterparty)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DebtSubType).HasMaxLength(255);

                entity.Property(e => e.DebtType).HasMaxLength(255);

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.GftrrefCode)
                    .IsRequired()
                    .HasColumnName("GFTRRefCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.IncomingDueAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.IncomingDueDate).HasColumnType("datetime");

                entity.Property(e => e.InterestFormula).HasMaxLength(255);

                entity.Property(e => e.InterestRate).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.LoanAge).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LoanAmount).HasColumnType("decimal(24, 6)");

                entity.Property(e => e.LoanAmountThb)
                    .HasColumnName("LoanAmountTHB")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LoanCurrency)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MasterAgreement).HasMaxLength(255);

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.OutStandingDebt).HasColumnType("decimal(24, 6)");

                entity.Property(e => e.OutStandingDebtThb)
                    .HasColumnName("OutStandingDebtTHB")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Ptyp).HasMaxLength(255);

                entity.Property(e => e.ReferenceCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SignDate).HasColumnType("datetime");

                entity.Property(e => e.SourceType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TbillFlag)
                    .HasColumnName("TBillFlag")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Ttyp).HasMaxLength(255);

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Agreement)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Agreement_Organization");
            });

            modelBuilder.Entity<AgreementAct>(entity =>
            {
                entity.ToTable("AgreementAct", "tb");

                entity.HasIndex(e => e.AgreementId)
                    .HasName("IX_AgreementAct");

                entity.HasIndex(e => e.PlanActId)
                    .HasName("IX_AgreementAct_1");

                entity.Property(e => e.AgreementActId).HasColumnName("AgreementActID");

                entity.Property(e => e.AgreementId).HasColumnName("AgreementID");

                entity.Property(e => e.PlanActId).HasColumnName("PlanActID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Agreement)
                    .WithMany(p => p.AgreementAct)
                    .HasForeignKey(d => d.AgreementId)
                    .HasConstraintName("FK_AgreementAct_Agreement");

                entity.HasOne(d => d.PlanAct)
                    .WithMany(p => p.AgreementAct)
                    .HasForeignKey(d => d.PlanActId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_AgreementAct_PlanAct");
            });

            modelBuilder.Entity<AgreementPaymentPlan>(entity =>
            {
                entity.ToTable("AgreementPaymentPlan", "tb");

                entity.HasIndex(e => e.AgreementId)
                    .HasName("IX_AgreementPaymentPlan");

                entity.Property(e => e.AgreementPaymentPlanId).HasColumnName("AgreementPaymentPlanID");

                entity.Property(e => e.AgreementId).HasColumnName("AgreementID");

                entity.Property(e => e.PaymentPlanId).HasColumnName("PaymentPlanID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Agreement)
                    .WithMany(p => p.AgreementPaymentPlan)
                    .HasForeignKey(d => d.AgreementId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_AgreementPaymentPlan_Agreement");

                entity.HasOne(d => d.PaymentPlan)
                    .WithMany(p => p.AgreementPaymentPlan)
                    .HasForeignKey(d => d.PaymentPlanId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_AgreementPaymentPlan_PaymentPlan");
            });

            modelBuilder.Entity<AgreementTrans>(entity =>
            {
                entity.HasKey(e => e.AgreemantTransId);

                entity.ToTable("AgreementTrans", "tb");

                entity.HasIndex(e => e.AgreementId)
                    .HasName("IX_AgreementTrans");

                entity.Property(e => e.AgreemantTransId).HasColumnName("AgreemantTransID");

                entity.Property(e => e.AgreementId).HasColumnName("AgreementID");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.BaseAmount).HasColumnType("decimal(18, 6)");

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.GftrrefCode)
                    .HasColumnName("GFTRRefCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostinDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(255);

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Agreement)
                    .WithMany(p => p.AgreementTrans)
                    .HasForeignKey(d => d.AgreementId)
                    .HasConstraintName("FK_AgreementTrans_Agreement");

                entity.HasOne(d => d.FlowTypeNavigation)
                    .WithMany(p => p.AgreementTrans)
                    .HasForeignKey(d => d.FlowType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AgreementTrans_FType");
            });

            modelBuilder.Entity<Amount>(entity =>
            {
                entity.ToTable("Amount", "tb");

                entity.Property(e => e.AmountId).HasColumnName("AmountID");

                entity.Property(e => e.Amount1)
                    .HasColumnName("Amount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.AmountGroup)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PeriodType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SourceType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.AmountTypeNavigation)
                    .WithMany(p => p.Amount)
                    .HasForeignKey(d => d.AmountType)
                    .HasConstraintName("FK_Amount_AmountType");
            });

            modelBuilder.Entity<ApiClaims>(entity =>
            {
                entity.HasIndex(e => e.ApiResourceId);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.ApiResource)
                    .WithMany(p => p.ApiClaims)
                    .HasForeignKey(d => d.ApiResourceId);
            });

            modelBuilder.Entity<ApiLog>(entity =>
            {
                entity.ToTable("ApiLog", "tb");

                entity.Property(e => e.Action).HasMaxLength(100);

                entity.Property(e => e.ApiEndpoint).HasMaxLength(255);

                entity.Property(e => e.Request).HasColumnType("ntext");

                entity.Property(e => e.RequestContent).HasColumnType("ntext");

                entity.Property(e => e.RequestDt)
                    .HasColumnName("RequestDT")
                    .HasColumnType("datetime");

                entity.Property(e => e.Response).HasColumnType("ntext");

                entity.Property(e => e.ResponseDt)
                    .HasColumnName("ResponseDT")
                    .HasColumnType("datetime");

                entity.Property(e => e.ResponseStatus)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.ApiLog)
                    .HasForeignKey(d => d.SessionId)
                    .HasConstraintName("FK_ApiLog_ApiSession");
            });

            modelBuilder.Entity<ApiProperties>(entity =>
            {
                entity.HasIndex(e => e.ApiResourceId);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.ApiResource)
                    .WithMany(p => p.ApiProperties)
                    .HasForeignKey(d => d.ApiResourceId);
            });

            modelBuilder.Entity<ApiResources>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<ApiScopeClaims>(entity =>
            {
                entity.HasIndex(e => e.ApiScopeId);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.ApiScope)
                    .WithMany(p => p.ApiScopeClaims)
                    .HasForeignKey(d => d.ApiScopeId);
            });

            modelBuilder.Entity<ApiScopes>(entity =>
            {
                entity.HasIndex(e => e.ApiResourceId);

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.ApiResource)
                    .WithMany(p => p.ApiScopes)
                    .HasForeignKey(d => d.ApiResourceId);
            });

            modelBuilder.Entity<ApiSecrets>(entity =>
            {
                entity.HasIndex(e => e.ApiResourceId);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.ApiResource)
                    .WithMany(p => p.ApiSecrets)
                    .HasForeignKey(d => d.ApiResourceId);
            });

            modelBuilder.Entity<ApiSession>(entity =>
            {
                entity.ToTable("ApiSession", "tb");

                entity.Property(e => e.Action).HasMaxLength(50);

                entity.Property(e => e.FinishDt)
                    .HasColumnName("FinishDT")
                    .HasColumnType("datetime");

                entity.Property(e => e.SessionDt)
                    .HasColumnName("SessionDT")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<AttachFile>(entity =>
            {
                entity.ToTable("AttachFile", "tb");

                entity.Property(e => e.AttachFileId).HasColumnName("AttachFileID");

                entity.Property(e => e.CreateDt)
                    .HasColumnName("CreateDT")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.FieldName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FileDetail).HasColumnType("ntext");

                entity.Property(e => e.FileExtension)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.RecordId).HasColumnName("RecordID");

                entity.Property(e => e.TableName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branch", "tb");

                entity.Property(e => e.BranchId).HasColumnName("BranchID");

                entity.Property(e => e.BranchName)
                    .IsRequired()
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<BsProvince>(entity =>
            {
                entity.HasKey(e => e.ProvinceId)
                    .HasName("PK_province_id");

                entity.ToTable("bs_province", "tb");

                entity.Property(e => e.ProvinceId).HasColumnName("province_id");

                entity.Property(e => e.ProvinceCode).HasColumnName("province_code");

                entity.Property(e => e.ProvinceName)
                    .HasColumnName("province_name")
                    .HasMaxLength(255);

                entity.Property(e => e.SectionCode).HasColumnName("section_code");
            });

            modelBuilder.Entity<BsProvinceSection>(entity =>
            {
                entity.HasKey(e => e.SectionId)
                    .HasName("PK_section_id");

                entity.ToTable("bs_province_section", "tb");

                entity.Property(e => e.SectionId).HasColumnName("section_id");

                entity.Property(e => e.SectionName)
                    .HasColumnName("section_name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<CeLov>(entity =>
            {
                entity.HasKey(e => e.Lovkey);

                entity.ToTable("CE_LOV", "tb");

                entity.HasIndex(e => new { e.LovgroupCode, e.Lovcode })
                    .HasName("IX_CE_LOV")
                    .IsUnique();

                entity.Property(e => e.Lovkey).HasColumnName("LOVKey");

                entity.Property(e => e.Lovcode)
                    .IsRequired()
                    .HasColumnName("LOVCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LovgroupCode)
                    .IsRequired()
                    .HasColumnName("LOVGroupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Lovvalue)
                    .IsRequired()
                    .HasColumnName("LOVValue")
                    .HasMaxLength(255);

                entity.Property(e => e.OrderNo)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ParentGroup)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ParentLov)
                    .HasColumnName("ParentLOV")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.TimeStamp)
                    .IsRequired()
                    .IsRowVersion();

                entity.HasOne(d => d.LovgroupCodeNavigation)
                    .WithMany(p => p.CeLov)
                    .HasForeignKey(d => d.LovgroupCode)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CE_LOV_CE_LOVGroup");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasPrincipalKey(p => new { p.LovgroupCode, p.Lovcode })
                    .HasForeignKey(d => new { d.ParentGroup, d.ParentLov })
                    .HasConstraintName("FK_CE_LOV_CE_LOV");
            });

            modelBuilder.Entity<CeLovextend>(entity =>
            {
                entity.HasKey(e => e.CeLovextendKey);

                entity.ToTable("CE_LOVExtend", "tb");

                entity.HasIndex(e => new { e.ExtendType, e.Lovkey })
                    .HasName("IX_CE_LOVExtend");

                entity.Property(e => e.CeLovextendKey).HasColumnName("CE_LOVExtendKey");

                entity.Property(e => e.ExtendType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ExtendValue).HasColumnType("ntext");

                entity.Property(e => e.Lovkey).HasColumnName("LOVKey");

                entity.Property(e => e.TimeStamp).HasMaxLength(10);

                entity.HasOne(d => d.LovkeyNavigation)
                    .WithMany(p => p.CeLovextend)
                    .HasForeignKey(d => d.Lovkey)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CE_LOVExtend_LOVKey");
            });

            modelBuilder.Entity<CeLovgroup>(entity =>
            {
                entity.HasKey(e => e.LovgroupCode);

                entity.ToTable("CE_LOVGroup", "tb");

                entity.Property(e => e.LovgroupCode)
                    .HasColumnName("LOVGroupCode")
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.LovgroupName)
                    .IsRequired()
                    .HasColumnName("LOVGroupName")
                    .HasMaxLength(255);

                entity.Property(e => e.TimeStamp)
                    .IsRequired()
                    .IsRowVersion();
            });

            modelBuilder.Entity<ClientClaims>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientClaims)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientCorsOrigins>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.Origin)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientCorsOrigins)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientGrantTypes>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.GrantType)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientGrantTypes)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientIdPrestrictions>(entity =>
            {
                entity.ToTable("ClientIdPRestrictions");

                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientIdPrestrictions)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientPostLogoutRedirectUris>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.PostLogoutRedirectUri)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientPostLogoutRedirectUris)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientProperties>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientProperties)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientRedirectUris>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.RedirectUri)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientRedirectUris)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientScopes>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.Scope)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientScopes)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<ClientSecrets>(entity =>
            {
                entity.HasIndex(e => e.ClientId);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(4000);

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientSecrets)
                    .HasForeignKey(d => d.ClientId);
            });

            modelBuilder.Entity<Clients>(entity =>
            {
                entity.HasIndex(e => e.ClientId)
                    .IsUnique();

                entity.Property(e => e.BackChannelLogoutUri).HasMaxLength(2000);

                entity.Property(e => e.ClientClaimsPrefix).HasMaxLength(200);

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ClientName).HasMaxLength(200);

                entity.Property(e => e.ClientUri).HasMaxLength(2000);

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.FrontChannelLogoutUri).HasMaxLength(2000);

                entity.Property(e => e.LogoUri).HasMaxLength(2000);

                entity.Property(e => e.PairWiseSubjectSalt).HasMaxLength(200);

                entity.Property(e => e.ProtocolType)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.UserCodeType).HasMaxLength(100);
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.CurrencyCode);

                entity.ToTable("Currency", "tb");

                entity.Property(e => e.CurrencyCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CurrencyName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<CurrencyRate>(entity =>
            {
                entity.HasKey(e => e.RateId);

                entity.ToTable("CurrencyRate", "tb");

                entity.HasIndex(e => new { e.CurrencyYear, e.CurrencyCode })
                    .HasName("IX_CurrencyRate");

                entity.Property(e => e.RateId).HasColumnName("RateID");

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyRate1)
                    .HasColumnName("CurrencyRate")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.CurrencyCodeNavigation)
                    .WithMany(p => p.CurrencyRate)
                    .HasForeignKey(d => d.CurrencyCode)
                    .HasConstraintName("FK_CurrencyRate_Currency");

                entity.HasOne(d => d.CurrencyYearNavigation)
                    .WithMany(p => p.CurrencyRate)
                    .HasForeignKey(d => d.CurrencyYear)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CurrencyRate_CurrencyYear");
            });

            modelBuilder.Entity<CurrencyYear>(entity =>
            {
                entity.HasKey(e => e.Year);

                entity.ToTable("CurrencyYear", "tb");

                entity.Property(e => e.Year).ValueGeneratedNever();

                entity.Property(e => e.RateDate).HasColumnType("datetime");

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<DataLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("DataLog", "tb");

                entity.HasIndex(e => new { e.TableName, e.TableKey })
                    .HasName("IX_DataLog");

                entity.Property(e => e.LogId).HasColumnName("LogID");

                entity.Property(e => e.LogDt)
                    .HasColumnName("LogDT")
                    .HasColumnType("datetime");

                entity.Property(e => e.LogType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Remark).HasMaxLength(255);

                entity.Property(e => e.TableName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.LogStatusNavigation)
                    .WithMany(p => p.DataLog)
                    .HasForeignKey(d => d.LogStatus)
                    .HasConstraintName("FK_DataLog_LogStatus");
            });

            modelBuilder.Entity<DebtPayAmt>(entity =>
            {
                entity.ToTable("DebtPayAmt", "tb");

                entity.Property(e => e.DebtPayAmtId).HasColumnName("DebtPayAmtID");

                entity.Property(e => e.InterestReference).HasMaxLength(1000);

                entity.Property(e => e.PaymentPlanId).HasColumnName("PaymentPlanID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.InterestSaveAmountNavigation)
                    .WithMany(p => p.DebtPayAmtInterestSaveAmountNavigation)
                    .HasForeignKey(d => d.InterestSaveAmount)
                    .HasConstraintName("FK_DebtPayAmt_InterestSave");

                entity.HasOne(d => d.PaymentPlan)
                    .WithMany(p => p.DebtPayAmt)
                    .HasForeignKey(d => d.PaymentPlanId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_DebtPayAmt_PaymentPlan");

                entity.HasOne(d => d.PlanAmountNavigation)
                    .WithMany(p => p.DebtPayAmtPlanAmountNavigation)
                    .HasForeignKey(d => d.PlanAmount)
                    .HasConstraintName("FK_DebtPayAmt_PlanAmount");
            });

            modelBuilder.Entity<DeviceCodes>(entity =>
            {
                entity.HasKey(e => e.UserCode);

                entity.HasIndex(e => e.DeviceCode)
                    .IsUnique();

                entity.Property(e => e.UserCode)
                    .HasMaxLength(200)
                    .ValueGeneratedNever();

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.DeviceCode)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.SubjectId).HasMaxLength(200);
            });

            modelBuilder.Entity<Dscrnote>(entity =>
            {
                entity.ToTable("DSCRNote", "tb");

                entity.Property(e => e.DscrnoteId).HasColumnName("DSCRNoteID");

                entity.Property(e => e.Dscr)
                    .HasColumnName("DSCR")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.ProgressUpdate).HasColumnType("ntext");

                entity.Property(e => e.Reason).HasColumnType("ntext");

                entity.Property(e => e.Solution).HasColumnType("ntext");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.Dscrnote)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_DSCRNote_Plan");
            });

            modelBuilder.Entity<FileData>(entity =>
            {
                entity.ToTable("FileData", "tb");

                entity.Property(e => e.FileDataId).HasColumnName("FileDataID");

                entity.Property(e => e.FileData1)
                    .IsRequired()
                    .HasColumnName("FileData");
            });

            modelBuilder.Entity<IdentityClaims>(entity =>
            {
                entity.HasIndex(e => e.IdentityResourceId);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasOne(d => d.IdentityResource)
                    .WithMany(p => p.IdentityClaims)
                    .HasForeignKey(d => d.IdentityResourceId);
            });

            modelBuilder.Entity<IdentityProperties>(entity =>
            {
                entity.HasIndex(e => e.IdentityResourceId);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.HasOne(d => d.IdentityResource)
                    .WithMany(p => p.IdentityProperties)
                    .HasForeignKey(d => d.IdentityResourceId);
            });

            modelBuilder.Entity<IdentityResources>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.DisplayName).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<IipmAgency>(entity =>
            {
                entity.ToTable("IIPM_Agency", "tb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate)
                    .HasColumnName("endDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.MinistryCode)
                    .HasColumnName("ministryCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MinistryId).HasColumnName("ministryId");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.SId)
                    .HasColumnName("sId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnName("startDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<IipmApproval>(entity =>
            {
                entity.HasKey(e => e.ApprovalId)
                    .HasName("PK_IIPM_Approvals");

                entity.ToTable("IIPM_Approval", "tb");

                entity.Property(e => e.ApprovalId).ValueGeneratedNever();

                entity.Property(e => e.Detail)
                    .HasColumnName("detail")
                    .HasMaxLength(255);

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Pdmocode)
                    .HasColumnName("PDMOCode")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<IipmMinistry>(entity =>
            {
                entity.ToTable("IIPM_Ministry", "tb");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate)
                    .HasColumnName("endDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.SId)
                    .HasColumnName("sId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate)
                    .HasColumnName("startDate")
                    .HasColumnType("datetime");
            });

            modelBuilder.Entity<IipmPlanYearBudget>(entity =>
            {
                entity.HasKey(e => e.Pybid);

                entity.ToTable("IIPM_PlanYearBudget", "tb");

                entity.Property(e => e.Pybid)
                    .HasColumnName("PYBId")
                    .ValueGeneratedNever();

                entity.Property(e => e.Budget)
                    .HasColumnName("budget")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ProjectPlanId).HasColumnName("projectPlanId");

                entity.Property(e => e.SourceOfffundId).HasColumnName("sourceOfffundId");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<IipmProject>(entity =>
            {
                entity.HasKey(e => e.ProjId);

                entity.ToTable("IIPM_Project", "tb");

                entity.Property(e => e.ProjId)
                    .HasColumnName("ProjID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AgencyId).HasColumnName("agencyId");

                entity.Property(e => e.ApprovedAt)
                    .HasColumnName("approvedAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.Background)
                    .HasColumnName("background")
                    .HasColumnType("ntext");

                entity.Property(e => e.Budget)
                    .HasColumnName("budget")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Code)
                    .HasColumnName("code")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createdAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.CreditChannelId).HasColumnName("creditChannelId");

                entity.Property(e => e.DirectorMail)
                    .HasColumnName("directorMail")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.DirectorTel)
                    .HasColumnName("directorTel")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.EndedAt)
                    .HasColumnName("endedAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.FlagTypeId)
                    .HasColumnName("flagTypeId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Goal)
                    .HasColumnName("goal")
                    .HasColumnType("ntext");

                entity.Property(e => e.HasEld).HasColumnName("hasEld");

                entity.Property(e => e.HasPvy).HasColumnName("hasPvy");

                entity.Property(e => e.IdRef)
                    .HasColumnName("idRef")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ImportContent)
                    .HasColumnName("importContent")
                    .HasColumnType("decimal(18, 4)");

                entity.Property(e => e.IsGovBurden).HasColumnName("isGovBurden");

                entity.Property(e => e.IsOnGoing).HasColumnName("isOnGoing");

                entity.Property(e => e.IsPlanLocked).HasColumnName("isPlanLocked");

                entity.Property(e => e.KindTypeId)
                    .HasColumnName("kindTypeId")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.OperationAt)
                    .HasColumnName("operationAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.OperationTypeCode)
                    .HasColumnName("operationTypeCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PdmoprojId).HasColumnName("PDMOProjId");

                entity.Property(e => e.ProjectArea)
                    .HasColumnName("projectArea")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectLocked).HasColumnName("projectLocked");

                entity.Property(e => e.ProjectLogFrameLocked).HasColumnName("projectLogFrameLocked");

                entity.Property(e => e.ProjectScope)
                    .HasColumnName("projectScope")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProvinceCode)
                    .HasColumnName("provinceCode")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SectorId).HasColumnName("sectorId");

                entity.Property(e => e.StartedAt)
                    .HasColumnName("startedAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updatedAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");
            });

            modelBuilder.Entity<IipmProjectPlan>(entity =>
            {
                entity.HasKey(e => e.ProjPlanId);

                entity.ToTable("IIPM_ProjectPlan", "tb");

                entity.Property(e => e.ProjPlanId).ValueGeneratedNever();

                entity.Property(e => e.CoordinatorMail)
                    .HasColumnName("coordinatorMail")
                    .HasMaxLength(255);

                entity.Property(e => e.CoordinatorName)
                    .HasColumnName("coordinatorName")
                    .HasMaxLength(255);

                entity.Property(e => e.CoordinatorPosition)
                    .HasColumnName("coordinatorPosition")
                    .HasMaxLength(255);

                entity.Property(e => e.CoordinatorTel)
                    .HasColumnName("coordinatorTel")
                    .HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("createdAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.IsEnable).HasColumnName("isEnable");

                entity.Property(e => e.PlanConfigId).HasColumnName("planConfigId");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updatedAt")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");
            });

            modelBuilder.Entity<IipmSector>(entity =>
            {
                entity.HasKey(e => e.SectId);

                entity.ToTable("IIPM_Sector", "tb");

                entity.Property(e => e.SectId).ValueGeneratedNever();

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<Iipmproj>(entity =>
            {
                entity.ToTable("iipmproj");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Org).HasMaxLength(255);

                entity.Property(e => e.ProjectName).HasMaxLength(255);
            });

            modelBuilder.Entity<LawOfdebt>(entity =>
            {
                entity.ToTable("LawOFDebt", "tb");

                entity.Property(e => e.LawOfdebtId).HasColumnName("LawOFDebtID");

                entity.Property(e => e.Detail).IsRequired();

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.Level).HasMaxLength(128);
            });

            modelBuilder.Entity<MaplanType>(entity =>
            {
                entity.HasKey(e => e.PlanTypeCode);

                entity.ToTable("MAPlanType", "tb");

                entity.Property(e => e.PlanTypeCode)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.PlanTypeName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<MasterAgreement>(entity =>
            {
                entity.HasKey(e => new { e.OrganizationType, e.PlanType, e.ObjectiveCode });

                entity.ToTable("MasterAgreement", "tb");

                entity.Property(e => e.OrganizationType)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PlanType)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectiveCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MasterAgreementNo)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MasterAgreementMapping>(entity =>
            {
                entity.HasKey(e => new { e.Orgtype, e.Pjtype });

                entity.ToTable("MasterAgreementMapping", "tb");

                entity.Property(e => e.Orgtype)
                    .HasColumnName("ORGType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Pjtype)
                    .HasColumnName("PJType")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ForeignLoan)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LocalLoan)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Ministry>(entity =>
            {
                entity.ToTable("Ministry", "tb");

                entity.HasIndex(e => e.MinistryCode)
                    .HasName("IX_Ministry")
                    .IsUnique();

                entity.Property(e => e.MinistryId).HasColumnName("MinistryID");

                entity.Property(e => e.MinistryCode).HasMaxLength(50);

                entity.Property(e => e.MinistryEnname)
                    .HasColumnName("MinistryENName")
                    .HasMaxLength(255);

                entity.Property(e => e.MinistryThname)
                    .HasColumnName("MinistryTHName")
                    .HasMaxLength(255);

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<MonthRep>(entity =>
            {
                entity.ToTable("MonthRep", "tb");

                entity.Property(e => e.MonthRepId)
                    .HasColumnName("MonthRepID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ExistDebtId).HasColumnName("ExistDebtID");

                entity.Property(e => e.NewDebtId).HasColumnName("NewDebtID");

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization", "tb");

                entity.HasIndex(e => e.OrganizationCode)
                    .HasName("IX_Organization")
                    .IsUnique();

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.EstablishmentLaw).HasColumnType("ntext");

                entity.Property(e => e.Fdapropotion)
                    .HasColumnName("FDAPropotion")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FinanceDebtSection).HasColumnType("ntext");

                entity.Property(e => e.LoanPowerSection).HasColumnType("ntext");

                entity.Property(e => e.Orgaffiliate).HasColumnName("ORGAffiliate");

                entity.Property(e => e.OrganizationCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OrganizationEnname)
                    .HasColumnName("OrganizationENName")
                    .HasMaxLength(255);

                entity.Property(e => e.OrganizationThname)
                    .IsRequired()
                    .HasColumnName("OrganizationTHName")
                    .HasMaxLength(255);

                entity.Property(e => e.Orgstatus).HasColumnName("ORGStatus");

                entity.Property(e => e.Orgtype).HasColumnName("ORGType");

                entity.Property(e => e.Pdapropotion)
                    .HasColumnName("PDAPropotion")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Pdmotype).HasColumnName("PDMOType");

                entity.Property(e => e.PublicDebtSection).HasColumnType("ntext");

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.RequestData).HasColumnType("ntext");

                entity.Property(e => e.Tel)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.DebtCalculationNavigation)
                    .WithMany(p => p.OrganizationDebtCalculationNavigation)
                    .HasForeignKey(d => d.DebtCalculation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organization_DebtCalulation");

                entity.HasOne(d => d.FieldNavigation)
                    .WithMany(p => p.OrganizationFieldNavigation)
                    .HasForeignKey(d => d.Field)
                    .HasConstraintName("FK_Organization_Field");

                entity.HasOne(d => d.OrgaffiliateNavigation)
                    .WithMany(p => p.InverseOrgaffiliateNavigation)
                    .HasForeignKey(d => d.Orgaffiliate)
                    .HasConstraintName("FK_Organization_Affiliate");

                entity.HasOne(d => d.OrgstatusNavigation)
                    .WithMany(p => p.OrganizationOrgstatusNavigation)
                    .HasForeignKey(d => d.Orgstatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organization_CE_LOV");

                entity.HasOne(d => d.OrgtypeNavigation)
                    .WithMany(p => p.OrganizationOrgtypeNavigation)
                    .HasForeignKey(d => d.Orgtype)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Organization_ORGType");

                entity.HasOne(d => d.PdmotypeNavigation)
                    .WithMany(p => p.OrganizationPdmotypeNavigation)
                    .HasForeignKey(d => d.Pdmotype)
                    .HasConstraintName("FK_Organization_PDMOType");

                entity.HasOne(d => d.ProvinceNavigation)
                    .WithMany(p => p.Organization)
                    .HasForeignKey(d => d.Province)
                    .HasConstraintName("FK_Organization_Province");

                entity.HasOne(d => d.RequestStatusNavigation)
                    .WithMany(p => p.Organization)
                    .HasForeignKey(d => d.RequestStatus)
                    .HasConstraintName("FK_Organization_RequestStatus");

                entity.HasOne(d => d.SubFieldNavigation)
                    .WithMany(p => p.OrganizationSubFieldNavigation)
                    .HasForeignKey(d => d.SubField)
                    .HasConstraintName("FK_Organization_SubField");
            });

            modelBuilder.Entity<Orglod>(entity =>
            {
                entity.ToTable("ORGLOD", "tb");

                entity.HasIndex(e => new { e.OrganizationId, e.LawOfdebtId })
                    .HasName("IX_ORGLOD")
                    .IsUnique();

                entity.Property(e => e.Orglodid).HasColumnName("ORGLODID");

                entity.Property(e => e.LawOfdebtId).HasColumnName("LawOFDebtID");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.LawOfdebt)
                    .WithMany(p => p.Orglod)
                    .HasForeignKey(d => d.LawOfdebtId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ORGLOD_LawOFDebt");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Orglod)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ORGLOD_Organization");
            });

            modelBuilder.Entity<OrgtoMany>(entity =>
            {
                entity.ToTable("ORGToMany", "tb");

                entity.HasIndex(e => e.OrganizationId)
                    .HasName("IX_ORGToMany");

                entity.Property(e => e.OrgtoManyId).HasColumnName("ORGToManyID");

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ManyId).HasColumnName("ManyID");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.OrgtoMany)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ORGToMany_Organization");
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.HasKey(e => e.Year);

                entity.ToTable("Parameter", "tb");

                entity.Property(e => e.Year).ValueGeneratedNever();

                entity.Property(e => e.Budget).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.DebtSettlement).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.EstIncome).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ExportIncome).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Gdp)
                    .HasColumnName("GDP")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Interest).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.DataLogNavigation)
                    .WithMany(p => p.Parameter)
                    .HasForeignKey(d => d.DataLog)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GDP_DataLog");
            });

            modelBuilder.Entity<PaymentPlan>(entity =>
            {
                entity.ToTable("PaymentPlan", "tb");

                entity.HasIndex(e => e.PlanExistId);

                entity.Property(e => e.PaymentPlanId).HasColumnName("PaymentPlanID");

                entity.Property(e => e.ManageType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlanExistId).HasColumnName("PlanExistID");

                entity.HasOne(d => d.DebtPaymentPlanTypeNavigation)
                    .WithMany(p => p.PaymentPlanDebtPaymentPlanTypeNavigation)
                    .HasForeignKey(d => d.DebtPaymentPlanType)
                    .HasConstraintName("FK_PaymentPlan_DebtPaymentPlanType");

                entity.HasOne(d => d.PaymentSourceNavigation)
                    .WithMany(p => p.PaymentPlanPaymentSourceNavigation)
                    .HasForeignKey(d => d.PaymentSource)
                    .HasConstraintName("FK_PaymentPlan_PaymentSource");

                entity.HasOne(d => d.PlanExist)
                    .WithMany(p => p.PaymentPlan)
                    .HasForeignKey(d => d.PlanExistId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PaymentPlan_PlanExist");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.PermissionCode);

                entity.Property(e => e.PermissionCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.PermissionGroup)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PermissionName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.PermissionGroupNavigation)
                    .WithMany(p => p.Permission)
                    .HasForeignKey(d => d.PermissionGroup)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Permission_PermissionGroup");
            });

            modelBuilder.Entity<PermissionGroup>(entity =>
            {
                entity.HasKey(e => e.PermissionGroupCode);

                entity.Property(e => e.PermissionGroupCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.PermissionGroupDetail).HasMaxLength(1000);

                entity.Property(e => e.PermissionGroupName)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<PersistedGrants>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.HasIndex(e => new { e.SubjectId, e.ClientId, e.Type });

                entity.Property(e => e.Key)
                    .HasMaxLength(200)
                    .ValueGeneratedNever();

                entity.Property(e => e.ClientId)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Data).IsRequired();

                entity.Property(e => e.SubjectId).HasMaxLength(200);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Plan>(entity =>
            {
                entity.ToTable("Plan", "tb");

                entity.HasIndex(e => e.PlanCode)
                    .HasName("IX_Plan")
                    .IsUnique();

                entity.HasIndex(e => new { e.StartYear, e.PlanType });

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.PlanCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.DataLogNavigation)
                    .WithMany(p => p.PlanDataLogNavigation)
                    .HasForeignKey(d => d.DataLog)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Plan_DataLog");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Plan)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Plan_Organization");

                entity.HasOne(d => d.PlanReleaseNavigation)
                    .WithMany(p => p.PlanPlanReleaseNavigation)
                    .HasForeignKey(d => d.PlanRelease)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Plan_PlanRelease");

                entity.HasOne(d => d.PlanTypeNavigation)
                    .WithMany(p => p.PlanPlanTypeNavigation)
                    .HasForeignKey(d => d.PlanType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Plan_PlanType");

                entity.HasOne(d => d.ProposalStatusNavigation)
                    .WithMany(p => p.PlanProposalStatusNavigation)
                    .HasForeignKey(d => d.ProposalStatus)
                    .HasConstraintName("FK_Plan_ProposalStatus");
            });

            modelBuilder.Entity<PlanAct>(entity =>
            {
                entity.ToTable("PlanAct", "tb");

                entity.Property(e => e.PlanActId).HasColumnName("PlanActID");

                entity.Property(e => e.ActivityName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.MasterAgreement).HasMaxLength(50);

                entity.Property(e => e.PlanProjId).HasColumnName("PlanProjID");

                entity.Property(e => e.ProjActId).HasColumnName("ProjActID");

                entity.Property(e => e.ReferencePlanActId).HasColumnName("ReferencePlanActID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.PlanProj)
                    .WithMany(p => p.PlanAct)
                    .HasForeignKey(d => d.PlanProjId)
                    .HasConstraintName("FK_PlanAct_PlanProj");

                entity.HasOne(d => d.ReferencePlanAct)
                    .WithMany(p => p.InverseReferencePlanAct)
                    .HasForeignKey(d => d.ReferencePlanActId)
                    .HasConstraintName("FK_PlanAct_ReferencePlanAct");
            });

            modelBuilder.Entity<PlanActAmount>(entity =>
            {
                entity.ToTable("PlanActAmount", "tb");

                entity.HasIndex(e => e.PlanActId)
                    .HasName("IX_PlanActAmount");

                entity.Property(e => e.PlanActAmountId).HasColumnName("PlanActAmountID");

                entity.Property(e => e.AmountId).HasColumnName("AmountID");

                entity.Property(e => e.PlanActId).HasColumnName("PlanActID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Amount)
                    .WithMany(p => p.PlanActAmount)
                    .HasForeignKey(d => d.AmountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlanActAmount_Amount");

                entity.HasOne(d => d.PlanAct)
                    .WithMany(p => p.PlanActAmount)
                    .HasForeignKey(d => d.PlanActId)
                    .HasConstraintName("FK_PlanActAmount_PlanAct");
            });

            modelBuilder.Entity<PlanAgreement>(entity =>
            {
                entity.ToTable("PlanAgreement", "tb");

                entity.Property(e => e.PlanAgreementId).HasColumnName("PlanAgreementID");

                entity.Property(e => e.AgreementId).HasColumnName("AgreementID");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Agreement)
                    .WithMany(p => p.PlanAgreement)
                    .HasForeignKey(d => d.AgreementId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PlanAgreement_Agreement");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.PlanAgreement)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PlanAgreement_Plan");
            });

            modelBuilder.Entity<PlanAttach>(entity =>
            {
                entity.ToTable("PlanAttach", "tb");

                entity.Property(e => e.PlanAttachId).HasColumnName("PlanAttachID");

                entity.Property(e => e.AttachFileId).HasColumnName("AttachFileID");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.AttachFile)
                    .WithMany(p => p.PlanAttach)
                    .HasForeignKey(d => d.AttachFileId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PlanAttach_Attach");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.PlanAttach)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PlanAttach_Plan");
            });

            modelBuilder.Entity<PlanExist>(entity =>
            {
                entity.ToTable("PlanExist", "tb");

                entity.Property(e => e.PlanExistId).HasColumnName("PlanExistID");

                entity.Property(e => e.IsNotRequiredApproval).HasColumnName("isNotRequiredApproval");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.PlanExist)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PlanExist_Plan");
            });

            modelBuilder.Entity<PlanExistAgreement>(entity =>
            {
                entity.ToTable("PlanExistAgreement", "tb");

                entity.Property(e => e.PlanExistAgreementId).HasColumnName("PlanExistAgreementID");

                entity.Property(e => e.ActualDueAmount).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ActualDueDate).HasColumnType("datetime");

                entity.Property(e => e.AgreementId).HasColumnName("AgreementID");

                entity.Property(e => e.MasterAgreement)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlanExistId).HasColumnName("PlanExistID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Agreement)
                    .WithMany(p => p.PlanExistAgreement)
                    .HasForeignKey(d => d.AgreementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlanExistAgreement_Agreement");

                entity.HasOne(d => d.ObjectiveNavigation)
                    .WithMany(p => p.PlanExistAgreementObjectiveNavigation)
                    .HasForeignKey(d => d.Objective)
                    .HasConstraintName("FK_PlanExistAgreement_Objective");

                entity.HasOne(d => d.PlanExist)
                    .WithMany(p => p.PlanExistAgreement)
                    .HasForeignKey(d => d.PlanExistId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PlanExistAgreement_PlanExist");

                entity.HasOne(d => d.PlanTypeNavigation)
                    .WithMany(p => p.PlanExistAgreementPlanTypeNavigation)
                    .HasForeignKey(d => d.PlanType)
                    .HasConstraintName("FK_PlanExistAgreement_PlanType");

                entity.HasOne(d => d.TransactionTypeNavigation)
                    .WithMany(p => p.PlanExistAgreementTransactionTypeNavigation)
                    .HasForeignKey(d => d.TransactionType)
                    .HasConstraintName("FK_PlanExistAgreement_CE_LOV");
            });

            modelBuilder.Entity<PlanExtend>(entity =>
            {
                entity.ToTable("PlanExtend", "tb");

                entity.Property(e => e.PlanExtendId).HasColumnName("PlanExtendID");

                entity.Property(e => e.DataGroup)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.PlanExtend)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_PlanExtend_Plan");
            });

            modelBuilder.Entity<PlanFinance>(entity =>
            {
                entity.ToTable("PlanFinance", "tb");

                entity.Property(e => e.PlanFinanceId).HasColumnName("PlanFinanceID");

                entity.Property(e => e.AmountId).HasColumnName("AmountID");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Amount)
                    .WithMany(p => p.PlanFinance)
                    .HasForeignKey(d => d.AmountId)
                    .HasConstraintName("FK_PlanFinance_Amount");

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.PlanFinance)
                    .HasForeignKey(d => d.PlanId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_PlanFinance_Plan");
            });

            modelBuilder.Entity<PlanLoan>(entity =>
            {
                entity.ToTable("PlanLoan", "tb");

                entity.HasIndex(e => e.PlanProjectId)
                    .HasName("IX_PlanLoan");

                entity.Property(e => e.PlanLoanId).HasColumnName("PlanLoanID");

                entity.Property(e => e.Apr).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Aug).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Dec).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Feb).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Jan).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Jul).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Jun).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LoanAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LoanCurrency)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LoanType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mar).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.May).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Nov).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Oct).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PeriodType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlanProjectId).HasColumnName("PlanProjectID");

                entity.Property(e => e.Sep).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Thbamount)
                    .HasColumnName("THBAmount")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.PlanProject)
                    .WithMany(p => p.PlanLoan)
                    .HasForeignKey(d => d.PlanProjectId)
                    .HasConstraintName("FK_PlanLoan_PlanProject");

                entity.HasOne(d => d.SourceLoan)
                    .WithMany(p => p.PlanLoan)
                    .HasForeignKey(d => d.SourceLoanId)
                    .HasConstraintName("FK_PlanLoan_CE_LOV");
            });

            modelBuilder.Entity<PlanProject>(entity =>
            {
                entity.ToTable("PlanProject", "tb");

                entity.Property(e => e.PlanProjectId).HasColumnName("PlanProjectID");

                entity.Property(e => e.CoordinatorEmail).HasMaxLength(100);

                entity.Property(e => e.CoordinatorName).HasMaxLength(255);

                entity.Property(e => e.CoordinatorPosition).HasMaxLength(255);

                entity.Property(e => e.CoordinatorTel).HasMaxLength(100);

                entity.Property(e => e.IsNotRequiredApproval).HasColumnName("isNotRequiredApproval");

                entity.Property(e => e.MasterAgreement)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.PlanProject)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("FK_PlanProject_Plan");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.PlanProject)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlanProject_Project");

                entity.HasOne(d => d.ProjectTypeNavigation)
                    .WithMany(p => p.PlanProject)
                    .HasForeignKey(d => d.ProjectType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlanProject_ProjecType");
            });

            modelBuilder.Entity<PlanProjectFile>(entity =>
            {
                entity.ToTable("PlanProjectFile", "tb");

                entity.Property(e => e.Detail).HasMaxLength(500);

                entity.HasOne(d => d.File)
                    .WithMany(p => p.PlanProjectFile)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK_PlanProjectFile_AttachFile");

                entity.HasOne(d => d.PlanProject)
                    .WithMany(p => p.PlanProjectFile)
                    .HasForeignKey(d => d.PlanProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlanProjectFile_PlanProject");
            });

            modelBuilder.Entity<PlanProjectResolution>(entity =>
            {
                entity.ToTable("PlanProjectResolution", "tb");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Detail).HasMaxLength(500);

                entity.HasOne(d => d.File)
                    .WithMany(p => p.PlanProjectResolution)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK_PlanProjectResolution_AttachFile");

                entity.HasOne(d => d.PlanProject)
                    .WithMany(p => p.PlanProjectResolution)
                    .HasForeignKey(d => d.PlanProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PlanProjectResolution_PlanProject");
            });

            modelBuilder.Entity<ProjAct>(entity =>
            {
                entity.ToTable("ProjAct", "tb");

                entity.HasIndex(e => e.ProjectId);

                entity.Property(e => e.ProjActId).HasColumnName("ProjActID");

                entity.Property(e => e.ActivityName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjAct)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjAct_Project");
            });

            modelBuilder.Entity<ProjActAmount>(entity =>
            {
                entity.ToTable("ProjActAmount", "tb");

                entity.Property(e => e.ProjActAmountId).HasColumnName("ProjActAmountID");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PeriodType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ProjectActId).HasColumnName("ProjectActID");

                entity.Property(e => e.SourceType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.AmountTypeNavigation)
                    .WithMany(p => p.ProjActAmountAmountTypeNavigation)
                    .HasForeignKey(d => d.AmountType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjActAmount_Amount");

                entity.HasOne(d => d.ProjectAct)
                    .WithMany(p => p.ProjActAmount)
                    .HasForeignKey(d => d.ProjectActId)
                    .HasConstraintName("FK_ProjActAmount_ProjAct");

                entity.HasOne(d => d.SourceLoanNavigation)
                    .WithMany(p => p.ProjActAmountSourceLoanNavigation)
                    .HasForeignKey(d => d.SourceLoan)
                    .HasConstraintName("FK_ProjActAmount_Source_Loan");
            });

            modelBuilder.Entity<ProjAmt>(entity =>
            {
                entity.ToTable("ProjAmt", "tb");

                entity.Property(e => e.ProjAmtId).HasColumnName("ProjAmtID");

                entity.Property(e => e.AmountId).HasColumnName("AmountID");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Amount)
                    .WithMany(p => p.ProjAmt)
                    .HasForeignKey(d => d.AmountId)
                    .HasConstraintName("FK_ProjAmt_Amount");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjAmt)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ProjAmt_Project");
            });

            modelBuilder.Entity<ProjMaterial>(entity =>
            {
                entity.ToTable("ProjMaterial", "tb");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("IX_ProjMaterial");

                entity.Property(e => e.ProjMaterialId).HasColumnName("ProjMaterialID");

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CurrencyRate).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.LimitAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.SourceType)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjMaterial)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjMaterial_Project");
            });

            modelBuilder.Entity<ProjToMany>(entity =>
            {
                entity.HasKey(e => e.ProjManyId);

                entity.ToTable("ProjToMany", "tb");

                entity.HasIndex(e => e.ProjectId)
                    .HasName("IX_ProjToMany");

                entity.Property(e => e.ProjManyId).HasColumnName("ProjManyID");

                entity.Property(e => e.GroupCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ManyId).HasColumnName("ManyID");

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjToMany)
                    .HasForeignKey(d => d.ProjectId)
                    .HasConstraintName("FK_ProjToMany_Project");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project", "tb");

                entity.HasIndex(e => e.ProjectCode)
                    .HasName("IX_Project")
                    .IsUnique();

                entity.Property(e => e.ProjectId).HasColumnName("ProjectID");

                entity.Property(e => e.CapitalSource).HasMaxLength(255);

                entity.Property(e => e.DirectorMail).HasMaxLength(256);

                entity.Property(e => e.DirectorName).HasMaxLength(300);

                entity.Property(e => e.DirectorPosition).HasMaxLength(300);

                entity.Property(e => e.DirectorTel).HasMaxLength(128);

                entity.Property(e => e.Eirr)
                    .HasColumnName("EIRR")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Firr)
                    .HasColumnName("FIRR")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LimitAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MapDrawing).HasColumnType("ntext");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.Pdmoagreement).HasColumnName("PDMOAgreement");

                entity.Property(e => e.ProjectBackground)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectBranch).HasMaxLength(255);

                entity.Property(e => e.ProjectCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ProjectEnname)
                    .HasColumnName("ProjectENName")
                    .HasMaxLength(255);

                entity.Property(e => e.ProjectObjective)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectScope)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectTarget)
                    .IsRequired()
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectThname)
                    .IsRequired()
                    .HasColumnName("ProjectTHName")
                    .HasMaxLength(255);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.CreditChannel)
                    .WithMany(p => p.ProjectCreditChannel)
                    .HasForeignKey(d => d.CreditChannelId)
                    .HasConstraintName("FK_Project_CreditChannel");

                entity.HasOne(d => d.DataLogNavigation)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.DataLog)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_DataLog");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Project)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_Organization");

                entity.HasOne(d => d.ProjectTypeNavigation)
                    .WithMany(p => p.ProjectProjectTypeNavigation)
                    .HasForeignKey(d => d.ProjectType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Project_ProjectType");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.ProjectProvince)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Project_Province");

                entity.HasOne(d => d.Sector)
                    .WithMany(p => p.ProjectSector)
                    .HasForeignKey(d => d.SectorId)
                    .HasConstraintName("FK_Project_Sector");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ProjectStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK_Project_Status");
            });

            modelBuilder.Entity<ProjectExtend>(entity =>
            {
                entity.ToTable("ProjectExtend", "tb");

                entity.Property(e => e.ExtendData).HasMaxLength(1000);

                entity.Property(e => e.GroupCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectExtend)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectExtend_Project");
            });

            modelBuilder.Entity<ProjectLocation>(entity =>
            {
                entity.ToTable("ProjectLocation", "tb");

                entity.Property(e => e.Location).HasMaxLength(500);

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectLocation)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectLocation_Project");
            });

            modelBuilder.Entity<ProjectResolution>(entity =>
            {
                entity.ToTable("ProjectResolution", "tb");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Detail).HasMaxLength(500);

                entity.HasOne(d => d.File)
                    .WithMany(p => p.ProjectResolution)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK_ProjectResolution_AttachFile");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.ProjectResolution)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProjectResolution_Project");
            });

            modelBuilder.Entity<ProposalPlan>(entity =>
            {
                entity.ToTable("ProposalPlan", "tb");

                entity.HasIndex(e => e.PlanId);

                entity.HasIndex(e => e.ProposalId);

                entity.Property(e => e.ProposalPlanId).HasColumnName("ProposalPlanID");

                entity.Property(e => e.PlanId).HasColumnName("PlanID");

                entity.Property(e => e.ProposalId).HasColumnName("ProposalID");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Plan)
                    .WithMany(p => p.ProposalPlanPlan)
                    .HasForeignKey(d => d.PlanId)
                    .HasConstraintName("FK_ProposalPlan_Plan");

                entity.HasOne(d => d.Proposal)
                    .WithMany(p => p.ProposalPlanProposal)
                    .HasForeignKey(d => d.ProposalId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_ProposalPlan_Proposal");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.ToTable("Province", "tb");

                entity.Property(e => e.ProvinceId)
                    .HasColumnName("ProvinceID")
                    .ValueGeneratedNever();

                entity.Property(e => e.ProvinceName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TimeStamp).IsRowVersion();
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("Request", "tb");

                entity.HasIndex(e => new { e.RequestType, e.IssueId })
                    .HasName("IX_Request");

                entity.Property(e => e.RequestId).HasColumnName("RequestID");

                entity.Property(e => e.IssueId).HasColumnName("IssueID");

                entity.Property(e => e.RequestData).HasColumnType("ntext");

                entity.Property(e => e.RequestDt)
                    .HasColumnName("RequestDT")
                    .HasColumnType("datetime");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("UserID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.RequestStatusNavigation)
                    .WithMany(p => p.RequestRequestStatusNavigation)
                    .HasForeignKey(d => d.RequestStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Request_RequestStatus");

                entity.HasOne(d => d.RequestTypeNavigation)
                    .WithMany(p => p.RequestRequestTypeNavigation)
                    .HasForeignKey(d => d.RequestType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Request_RequestType");
            });

            modelBuilder.Entity<RoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ApplicationCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<ShareHolder>(entity =>
            {
                entity.ToTable("ShareHolder", "tb");

                entity.Property(e => e.ShareHolderId).HasColumnName("ShareHolderID");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.OrganizationName).HasMaxLength(255);

                entity.Property(e => e.OrgshareHolder).HasColumnName("ORGShareHolder");

                entity.Property(e => e.Proportion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TimeStamp).IsRowVersion();

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ShareHolderOrganization)
                    .HasForeignKey(d => d.OrganizationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ShareHolder_Organization");

                entity.HasOne(d => d.OrgshareHolderNavigation)
                    .WithMany(p => p.ShareHolderOrgshareHolderNavigation)
                    .HasForeignKey(d => d.OrgshareHolder)
                    .HasConstraintName("FK_ShareHolder_ORGShareHolder");
            });

            modelBuilder.Entity<SurrogateKey>(entity =>
            {
                entity.HasKey(e => new { e.GroupCode, e.Prefix })
                    .HasName("PK_SurrogateKey_1");

                entity.ToTable("SurrogateKey", "tb");

                entity.Property(e => e.GroupCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Prefix)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<UserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Department).HasMaxLength(100);

                entity.Property(e => e.EfirstName)
                    .HasColumnName("EFirstName")
                    .HasMaxLength(100);

                entity.Property(e => e.ElastName)
                    .HasColumnName("ELastName")
                    .HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.LastAccess).HasColumnType("datetime");

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.Pin)
                    .HasColumnName("PIN")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PincreatedDt)
                    .HasColumnName("PINCreatedDT")
                    .HasColumnType("datetime");

                entity.Property(e => e.Position).HasMaxLength(100);

                entity.Property(e => e.Tel1)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Tel2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TfirstName)
                    .HasColumnName("TFirstName")
                    .HasMaxLength(100);

                entity.Property(e => e.TlastName)
                    .HasColumnName("TLastName")
                    .HasMaxLength(100);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<YContracts>(entity =>
            {
                entity.HasKey(e => e.ContractId)
                    .HasName("PK_contract_id");

                entity.ToTable("y_contracts", "tb");

                entity.Property(e => e.ContractId).HasColumnName("contract_id");

                entity.Property(e => e.ContractActualAmount).HasColumnName("contract_actual_amount");

                entity.Property(e => e.ContractDurationYear).HasColumnName("contract_duration_year");

                entity.Property(e => e.ContractEndDate)
                    .HasColumnName("contract_end_date")
                    .HasColumnType("date");

                entity.Property(e => e.ContractInterestMeta)
                    .HasColumnName("contract_interest_meta")
                    .HasMaxLength(255);

                entity.Property(e => e.ContractInterestRate).HasColumnName("contract_interest_rate");

                entity.Property(e => e.ContractInterestType)
                    .HasColumnName("contract_interest_type")
                    .HasMaxLength(255);

                entity.Property(e => e.ContractLoanPaidback).HasColumnName("contract_loan_paidback");

                entity.Property(e => e.ContractLoanPending).HasColumnName("contract_loan_pending");

                entity.Property(e => e.ContractNumber)
                    .HasColumnName("contract_number")
                    .HasMaxLength(255);

                entity.Property(e => e.ContractOrgName)
                    .HasColumnName("contract_org_name")
                    .HasMaxLength(255);

                entity.Property(e => e.ContractOrgType)
                    .HasColumnName("contract_org_type")
                    .HasMaxLength(255);

                entity.Property(e => e.ContractPaperAmount).HasColumnName("contract_paper_amount");

                entity.Property(e => e.ContractProjectType).HasColumnName("contract_project_type");

                entity.Property(e => e.ContractProvince).HasColumnName("contract_province");

                entity.Property(e => e.ContractPurpose).HasColumnName("contract_purpose");

                entity.Property(e => e.ContractQuarter).HasColumnName("contract_quarter");

                entity.Property(e => e.ContractRemarks)
                    .HasColumnName("contract_remarks")
                    .HasColumnType("ntext");

                entity.Property(e => e.ContractSource)
                    .HasColumnName("contract_source")
                    .HasMaxLength(255);

                entity.Property(e => e.ContractStartDate)
                    .HasColumnName("contract_start_date")
                    .HasColumnType("date");

                entity.Property(e => e.ContractYear).HasColumnName("contract_year");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");
            });

            modelBuilder.Entity<YProjectTa>(entity =>
            {
                entity.HasKey(e => e.ProjectId);

                entity.ToTable("y_project_ta", "tb");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.OrganizationId).HasColumnName("OrganizationID");

                entity.Property(e => e.ProjectConcordance1)
                    .HasColumnName("project_concordance_1")
                    .HasMaxLength(5);

                entity.Property(e => e.ProjectConcordance1Detail)
                    .HasColumnName("project_concordance_1_detail")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectConcordance2)
                    .HasColumnName("project_concordance_2")
                    .HasMaxLength(5);

                entity.Property(e => e.ProjectConcordance2Detail)
                    .HasColumnName("project_concordance_2_detail")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectConcordance3)
                    .HasColumnName("project_concordance_3")
                    .HasMaxLength(5);

                entity.Property(e => e.ProjectConcordance3Detail)
                    .HasColumnName("project_concordance_3_detail")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectConcordance4)
                    .HasColumnName("project_concordance_4")
                    .HasMaxLength(5);

                entity.Property(e => e.ProjectConcordance4Detail)
                    .HasColumnName("project_concordance_4_detail")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectConcordance5)
                    .HasColumnName("project_concordance_5")
                    .HasMaxLength(5);

                entity.Property(e => e.ProjectConcordance5Detail)
                    .HasColumnName("project_concordance_5_detail")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectConcordance6)
                    .HasColumnName("project_concordance_6")
                    .HasMaxLength(5);

                entity.Property(e => e.ProjectConcordance6Detail)
                    .HasColumnName("project_concordance_6_detail")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectContractDepartment)
                    .HasColumnName("project_contract_department")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectContractEmail)
                    .HasColumnName("project_contract_email")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectContractFax)
                    .HasColumnName("project_contract_fax")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectContractFirstName)
                    .HasColumnName("project_contract_first_name")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectContractLastName)
                    .HasColumnName("project_contract_last_name")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectContractOrg)
                    .HasColumnName("project_contract_org")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectContractPosition)
                    .HasColumnName("project_contract_position")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectContractTel)
                    .HasColumnName("project_contract_tel")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectDesc)
                    .HasColumnName("project_desc")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectEndDate)
                    .HasColumnName("project_end_date")
                    .HasMaxLength(50);

                entity.Property(e => e.ProjectExpected)
                    .HasColumnName("project_expected")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectHelpReceived)
                    .HasColumnName("project_help_received")
                    .HasMaxLength(5);

                entity.Property(e => e.ProjectHelpReceivedDetail)
                    .HasColumnName("project_help_received_detail")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectNameEn)
                    .HasColumnName("project_name_en")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectNameTh)
                    .HasColumnName("project_name_th")
                    .HasMaxLength(200);

                entity.Property(e => e.ProjectObjective)
                    .HasColumnName("project_objective")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectRemark)
                    .HasColumnName("project_remark")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectScope)
                    .HasColumnName("project_scope")
                    .HasColumnType("ntext");

                entity.Property(e => e.ProjectStartDate)
                    .HasColumnName("project_start_date")
                    .HasMaxLength(50);

                entity.Property(e => e.ProjectYear).HasColumnName("project_year");
            });

            modelBuilder.Entity<ZLawTemplate>(entity =>
            {
                entity.HasKey(e => e.TemplateName);

                entity.ToTable("z_law_template", "tb");

                entity.Property(e => e.TemplateName)
                    .HasColumnName("template_name")
                    .HasMaxLength(255)
                    .ValueGeneratedNever();

                entity.Property(e => e.TemplateLovkey)
                    .HasColumnName("template_lovkey")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<ZLawTemplate2>(entity =>
            {
                entity.HasKey(e => e.TemplateName);

                entity.ToTable("z_law_template_2", "tb");

                entity.Property(e => e.TemplateName)
                    .HasColumnName("template_name")
                    .HasMaxLength(255)
                    .ValueGeneratedNever();

                entity.Property(e => e.TemplateLovkey).HasColumnName("template_lovkey");
            });

            modelBuilder.Entity<ZOrganizationToImport>(entity =>
            {
                entity.HasKey(e => e.OrganizationCode);

                entity.ToTable("z_organization_to_import", "tb");

                entity.Property(e => e.OrganizationCode)
                    .HasMaxLength(255)
                    .ValueGeneratedNever();

                entity.Property(e => e.ChangeRequest).HasMaxLength(255);

                entity.Property(e => e.Dcal)
                    .HasColumnName("dcal")
                    .HasMaxLength(255);

                entity.Property(e => e.DebtCalculation).HasMaxLength(255);

                entity.Property(e => e.EstablishmentLaw).HasColumnType("ntext");

                entity.Property(e => e.Fda)
                    .HasColumnName("fda")
                    .HasMaxLength(255);

                entity.Property(e => e.Fdapropotion)
                    .HasColumnName("FDAPropotion")
                    .HasMaxLength(255);

                entity.Property(e => e.Field).HasMaxLength(255);

                entity.Property(e => e.FinanceDebtSection).HasColumnType("ntext");

                entity.Property(e => e.Financedebtacts)
                    .HasColumnName("financedebtacts")
                    .HasMaxLength(255);

                entity.Property(e => e.HasLoanPower).HasMaxLength(255);

                entity.Property(e => e.IsCanceled).HasMaxLength(255);

                entity.Property(e => e.LoanPowerSection).HasColumnType("ntext");

                entity.Property(e => e.Orgaffiliate)
                    .HasColumnName("ORGAffiliate")
                    .HasMaxLength(255);

                entity.Property(e => e.OrganizationEnname)
                    .HasColumnName("OrganizationENName")
                    .HasMaxLength(255);

                entity.Property(e => e.OrganizationId)
                    .HasColumnName("OrganizationID")
                    .HasMaxLength(255);

                entity.Property(e => e.OrganizationThname)
                    .IsRequired()
                    .HasColumnName("OrganizationTHName")
                    .HasMaxLength(255);

                entity.Property(e => e.Orgstatus)
                    .HasColumnName("ORGStatus")
                    .HasMaxLength(255);

                entity.Property(e => e.Orgtype)
                    .HasColumnName("ORGType")
                    .HasMaxLength(255);

                entity.Property(e => e.Orgtypetitle)
                    .HasColumnName("orgtypetitle")
                    .HasMaxLength(255);

                entity.Property(e => e.Pda)
                    .HasColumnName("pda")
                    .HasMaxLength(255);

                entity.Property(e => e.Pdapropotion)
                    .HasColumnName("PDAPropotion")
                    .HasMaxLength(255);

                entity.Property(e => e.PublicDebtSection).HasColumnType("ntext");

                entity.Property(e => e.Publicdebtacts)
                    .HasColumnName("publicdebtacts")
                    .HasMaxLength(255);

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.RequestData).HasColumnType("ntext");

                entity.Property(e => e.RequestStatus).HasMaxLength(255);

                entity.Property(e => e.Shareholders)
                    .HasColumnName("shareholders")
                    .HasColumnType("ntext");

                entity.Property(e => e.SubField).HasMaxLength(255);

                entity.Property(e => e.Template)
                    .HasColumnName("template")
                    .HasMaxLength(255);

                entity.Property(e => e.TimeStamp).HasMaxLength(255);
            });

            modelBuilder.Entity<ZOrganizationToImport2>(entity =>
            {
                entity.HasKey(e => e.OrganizationCode);

                entity.ToTable("z_organization_to_import_2", "tb");

                entity.Property(e => e.OrganizationCode)
                    .HasMaxLength(255)
                    .ValueGeneratedNever();

                entity.Property(e => e.ChangeRequest).HasMaxLength(255);

                entity.Property(e => e.Dcal)
                    .HasColumnName("dcal")
                    .HasMaxLength(255);

                entity.Property(e => e.DebtCalculation).HasMaxLength(255);

                entity.Property(e => e.EstablishmentLaw).HasColumnType("ntext");

                entity.Property(e => e.Fda)
                    .HasColumnName("fda")
                    .HasMaxLength(255);

                entity.Property(e => e.Fdapropotion)
                    .HasColumnName("FDAPropotion")
                    .HasMaxLength(255);

                entity.Property(e => e.Field).HasMaxLength(255);

                entity.Property(e => e.FinanceDebtSection).HasColumnType("ntext");

                entity.Property(e => e.Financedebtacts)
                    .HasColumnName("financedebtacts")
                    .HasMaxLength(255);

                entity.Property(e => e.HasLoanPower).HasMaxLength(255);

                entity.Property(e => e.ImportStatus)
                    .HasColumnName("import_status")
                    .HasMaxLength(255);

                entity.Property(e => e.IsCanceled).HasMaxLength(255);

                entity.Property(e => e.LoanPowerSection).HasColumnType("ntext");

                entity.Property(e => e.Meta1)
                    .HasColumnName("meta_1")
                    .HasMaxLength(255);

                entity.Property(e => e.Meta2)
                    .HasColumnName("meta_2")
                    .HasMaxLength(255);

                entity.Property(e => e.Meta3)
                    .HasColumnName("meta_3")
                    .HasMaxLength(255);

                entity.Property(e => e.Meta4)
                    .HasColumnName("meta_4")
                    .HasMaxLength(255);

                entity.Property(e => e.Meta5)
                    .HasColumnName("meta_5")
                    .HasMaxLength(255);

                entity.Property(e => e.Orgaffiliate)
                    .HasColumnName("ORGAffiliate")
                    .HasMaxLength(255);

                entity.Property(e => e.OrganizationEnname)
                    .HasColumnName("OrganizationENName")
                    .HasMaxLength(255);

                entity.Property(e => e.OrganizationId)
                    .HasColumnName("OrganizationID")
                    .HasMaxLength(255);

                entity.Property(e => e.OrganizationThname)
                    .IsRequired()
                    .HasColumnName("OrganizationTHName")
                    .HasMaxLength(255);

                entity.Property(e => e.Orgstatus)
                    .HasColumnName("ORGStatus")
                    .HasMaxLength(255);

                entity.Property(e => e.Orgtype)
                    .HasColumnName("ORGType")
                    .HasMaxLength(255);

                entity.Property(e => e.Orgtypetitle)
                    .HasColumnName("orgtypetitle")
                    .HasMaxLength(255);

                entity.Property(e => e.Pda)
                    .HasColumnName("pda")
                    .HasMaxLength(255);

                entity.Property(e => e.Pdapropotion)
                    .HasColumnName("PDAPropotion")
                    .HasMaxLength(255);

                entity.Property(e => e.PublicDebtSection).HasColumnType("ntext");

                entity.Property(e => e.Publicdebtacts)
                    .HasColumnName("publicdebtacts")
                    .HasMaxLength(255);

                entity.Property(e => e.Remark).HasColumnType("ntext");

                entity.Property(e => e.RequestData).HasColumnType("ntext");

                entity.Property(e => e.RequestStatus).HasMaxLength(255);

                entity.Property(e => e.Shareholders)
                    .HasColumnName("shareholders")
                    .HasColumnType("ntext");

                entity.Property(e => e.SubField).HasMaxLength(255);

                entity.Property(e => e.Template)
                    .HasColumnName("template")
                    .HasMaxLength(255);

                entity.Property(e => e.TimeStamp).HasMaxLength(255);
            });
        }
    }
}
