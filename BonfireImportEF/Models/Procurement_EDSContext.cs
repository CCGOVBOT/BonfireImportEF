using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace BonfireImportEF.Models
{
    public partial class Procurement_EDSContext : DbContext
    {
        public Procurement_EDSContext()
        {
        }

        public Procurement_EDSContext(DbContextOptions<Procurement_EDSContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BidListCommodityGroup> BidListCommodityGroups { get; set; }
        public virtual DbSet<BidListContract> BidListContracts { get; set; }
        public virtual DbSet<BidListContractsTest> BidListContractsTests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //optionsBuilder.UseSqlServer("server=BOTSQLTEST2.ccounty.com,1617;user=svc-edsppusr;password=$^C_3dSpPuSr2o20;database=Procurement_EDS_PP;");
                var builder = new ConfigurationBuilder().AddJsonFile($"appSettings.json", true, true);
                var config = builder.Build();
                optionsBuilder.UseSqlServer(config["DBConnection"].ToString());

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("svc-edsppusr")
                .HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<BidListCommodityGroup>(entity =>
            {
                entity.HasKey(e => e.CommodityGroupId);

                entity.ToTable("BidList_CommodityGroups", "dbo");

                entity.Property(e => e.CommodityGroupId).HasColumnName("CommodityGroupID");

                entity.Property(e => e.Code).HasMaxLength(10);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.OperatorId)
                    .HasMaxLength(10)
                    .HasColumnName("OperatorID");
            });

            modelBuilder.Entity<BidListContract>(entity =>
            {
                entity.HasKey(e => e.ContractId);

                entity.ToTable("BidList_Contracts", "dbo");

                entity.Property(e => e.ContractId).HasColumnName("ContractID");

                entity.Property(e => e.Agency)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.AssignedTo).HasMaxLength(256);

                entity.Property(e => e.BidExpiration).HasColumnType("smalldatetime");

                entity.Property(e => e.BidOpening).HasColumnType("smalldatetime");

                entity.Property(e => e.Cfor)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("CFor");

                entity.Property(e => e.Cfrom)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CFrom");

                entity.Property(e => e.CommodityGroupId).HasColumnName("CommodityGroupID");

                entity.Property(e => e.ContractNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasMaxLength(256);

                entity.Property(e => e.DateAwarded).HasColumnType("date");

                entity.Property(e => e.DateCreated).HasPrecision(0);

                entity.Property(e => e.DatePosted).HasColumnType("smalldatetime");

                entity.Property(e => e.DateStamp).HasPrecision(0);

                entity.Property(e => e.Detail)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FileType).HasMaxLength(50);

                entity.Property(e => e.LinkTo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OperatorId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("OperatorID");

                entity.Property(e => e.StatusLastUser).HasMaxLength(100);

                entity.Property(e => e.StatusValue)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Subdivision)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.VendorAwarded)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId).HasColumnName("VendorID");

                entity.HasOne(d => d.CommodityGroup)
                    .WithMany(p => p.BidListContracts)
                    .HasForeignKey(d => d.CommodityGroupId)
                    .HasConstraintName("FK_BidList_Contracts_BidList_CommodityGroups");
            });

            modelBuilder.Entity<BidListContractsTest>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("BidList_ContractsTest", "dbo");

                entity.Property(e => e.Agency)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.AssignedTo).HasMaxLength(256);

                entity.Property(e => e.BidExpiration).HasColumnType("smalldatetime");

                entity.Property(e => e.BidOpening).HasColumnType("smalldatetime");

                entity.Property(e => e.Cfor)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .HasColumnName("CFor");

                entity.Property(e => e.Cfrom)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("CFrom");

                entity.Property(e => e.CommodityGroupId).HasColumnName("CommodityGroupID");

                entity.Property(e => e.ContractId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ContractID");

                entity.Property(e => e.ContractNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasMaxLength(256);

                entity.Property(e => e.DateAwarded).HasColumnType("date");

                entity.Property(e => e.DateCreated).HasPrecision(0);

                entity.Property(e => e.DatePosted).HasColumnType("smalldatetime");

                entity.Property(e => e.DateStamp).HasPrecision(0);

                entity.Property(e => e.Detail)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FileType).HasMaxLength(50);

                entity.Property(e => e.LinkTo)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.OperatorId)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("OperatorID");

                entity.Property(e => e.StatusLastUser).HasMaxLength(100);

                entity.Property(e => e.StatusValue)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Subdivision)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.Type)
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.VendorAwarded)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.VendorId).HasColumnName("VendorID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
