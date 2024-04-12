using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ArticleRewriteWorker.Models;

public partial class ReutersContext : DbContext
{
    public ReutersContext()
    {
    }

    public ReutersContext(DbContextOptions<ReutersContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ReutersCategory> ReutersCategories { get; set; }

    public virtual DbSet<ReutersItem> ReutersItems { get; set; }

    public virtual DbSet<ReutersItemCategory> ReutersItemCategories { get; set; }

    public virtual DbSet<ReutersItemsDetail> ReutersItemsDetails { get; set; }

    public virtual DbSet<ReutersItemsDetailsError> ReutersItemsDetailsErrors { get; set; }

    public virtual DbSet<RsItemsToConvert> RsItemsToConverts { get; set; }

    public virtual DbSet<RsSite> RsSites { get; set; }

    public virtual DbSet<SiteCategory> SiteCategories { get; set; }

    public virtual DbSet<SiteCategoryRelation> SiteCategoryRelations { get; set; }

    public virtual DbSet<SiteStory> SiteStories { get; set; }

    public virtual DbSet<TEnergyCategory> TEnergyCategories { get; set; }

    public virtual DbSet<TestXml> TestXmls { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=easton.mlnk.co,51433;Database=Reuters;User Id=binh;Password=Rtrs202304#tn;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReutersCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("reuters_Categories");

            entity.Property(e => e.CategoryId).ValueGeneratedOnAdd();
            entity.Property(e => e.Code)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ReutersItem>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("reuters_Items");

            entity.Property(e => e.Channel)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("channel");
            entity.Property(e => e.DateCreated)
                .HasPrecision(0)
                .HasColumnName("dateCreated");
            entity.Property(e => e.DateRupdated)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("dateRUpdated");
            entity.Property(e => e.Guid)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("guid");
            entity.Property(e => e.Headline)
                .HasMaxLength(200)
                .HasColumnName("headline");
            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Rid)
                .ValueGeneratedOnAdd()
                .HasColumnName("rid");
            entity.Property(e => e.Slug)
                .HasMaxLength(200)
                .HasColumnName("slug");
            entity.Property(e => e.Xml)
                .HasColumnType("xml")
                .HasColumnName("xml");
        });

        modelBuilder.Entity<ReutersItemCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("reuters_ItemCategories");
        });

        modelBuilder.Entity<ReutersItemsDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("reuters_ItemsDetails");

            entity.Property(e => e.ArticleId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Content).HasColumnType("xml");
            entity.Property(e => e.DateRupdated)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("dateRUpdated");
            entity.Property(e => e.Headline).HasMaxLength(200);
            entity.Property(e => e.ReuterItemId).ValueGeneratedOnAdd();
            entity.Property(e => e.Slugline).HasMaxLength(200);
            entity.Property(e => e.TransmitId)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Xml)
                .HasColumnType("xml")
                .HasColumnName("XML");
        });

        modelBuilder.Entity<ReutersItemsDetailsError>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("reuters_ItemsDetailsErrors");

            entity.Property(e => e.DateCreated)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Rid)
                .ValueGeneratedOnAdd()
                .HasColumnName("rid");
            entity.Property(e => e.TransmitId)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RsItemsToConvert>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("rs_ItemsToConvert");

            entity.Property(e => e.DateSelected).HasPrecision(0);
            entity.Property(e => e.DateSpined).HasPrecision(0);
            entity.Property(e => e.Note)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.SpinerService)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RsSite>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("rs_Sites");

            entity.Property(e => e.SiteId).ValueGeneratedOnAdd();
            entity.Property(e => e.SiteName).HasMaxLength(200);
        });

        modelBuilder.Entity<SiteCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("site_Categories");

            entity.Property(e => e.IsActiveOnSite).HasDefaultValue(false);
            entity.Property(e => e.SiteCategoryId).ValueGeneratedOnAdd();
            entity.Property(e => e.SiteCategoryName).HasMaxLength(200);
            entity.Property(e => e.SiteCategoryUrl).HasMaxLength(200);
        });

        modelBuilder.Entity<SiteCategoryRelation>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("site_CategoryRelations");
        });

        modelBuilder.Entity<SiteStory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("site_Stories");

            entity.Property(e => e.Author).HasMaxLength(300);
            entity.Property(e => e.ContentHtml).HasColumnName("ContentHTML");
            entity.Property(e => e.DatePubUpdated)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.DatePublised)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Headline).HasMaxLength(300);
            entity.Property(e => e.StoryId).ValueGeneratedOnAdd();
            entity.Property(e => e.UrlId).HasMaxLength(300);
        });

        modelBuilder.Entity<TEnergyCategory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("_t_EnergyCategories");

            entity.Property(e => e.Code).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Energy).HasMaxLength(255);
        });

        modelBuilder.Entity<TestXml>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("_test_XML");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Note)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("note");
            entity.Property(e => e.Xml)
                .HasColumnType("xml")
                .HasColumnName("xml");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
