using System;
using System.Collections.Generic;
using System.Text;
using BillingNextQuotation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BillingNextQuotation.Data
{
    public class ApplicationDbContext : IdentityDbContext<QuotationGenUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
          //  optionsBuilder.UseMySQL("Server=localhost;Database=quotation;User=ruchit;Password=Lolpass@123;");
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Models.Products>().ToTable("Products");
            builder.Entity<Models.Quotation>().ToTable("Quotation");
            builder.Entity<Models.QuotationDetails>().ToTable("QuotationDetails");
            builder.Entity<Models.QuotationNote>().ToTable("QuotationNote");
            builder.Entity<Models.SpecialCharges>().ToTable("SpecialCharges");
            builder.Entity<Models.Company>().ToTable("Company");
            builder.Entity<Models.QuotationSpecialCharges>().ToTable("QuotationSpecialCharges").HasKey(x => new { x.SpecialChargesId, x.QuotationId });
            builder.Entity<Models.QuotationSpecialCharges>()
            .HasOne(x => x.Quotation)
            .WithMany(m => m.QuotationSpecialCharges)
            .HasForeignKey(x => x.QuotationId);
            builder.Entity<Models.QuotationSpecialCharges>()
                .HasOne(x => x.SpecialCharges)
                .WithMany(e => e.QuotationSpecialCharges)
                .HasForeignKey(x => x.SpecialChargesId);


            builder.Entity<Models.Quotation>().Property(c => c.QuotationCreationDate).HasDefaultValueSql("NOW()");
            builder.Entity<Models.Products>().Property(c => c.ProductCreationDate).HasDefaultValueSql("NOW()");
            builder.Entity<Models.Company>().Property(c => c.CompanyCreationDate).HasDefaultValueSql("NOW()");
            builder.Entity<Models.Quotation>().HasIndex(c => c.QuotationNumber).IsUnique();
        }

        public DbSet<Models.Products> Products { get; set; }

        public DbSet<Models.Quotation> Quotations { get; set; }

        public DbSet<Models.QuotationDetails> QuotationDetails { get; set; }

        public DbSet<Models.QuotationNote> QuotationNotes { get; set; }

        public DbSet<Models.QuotationSpecialCharges> QuotationSpecialCharges { get; set; }

        public DbSet<Models.SpecialCharges> SpecialCharges { get; set; }

        public DbSet<Models.Company> Companies { get; set; }
    }
}
