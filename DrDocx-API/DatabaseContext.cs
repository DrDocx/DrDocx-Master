﻿using Microsoft.EntityFrameworkCore;
using DrDocx.Models;

namespace DrDocx.API
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<TestGroup> TestGroups { get; set; }
        public DbSet<TestResultGroup> TestResultGroups { get; set; }
        public DbSet<TestGroupTest> TestGroupTests { get; set; }
        
        public DbSet<Field> Fields { get; set; }
        public DbSet<FieldGroup> FieldGroups { get; set; }
        public DbSet<FieldValue> FieldValues { get; set; }
        public DbSet<FieldValueGroup> FieldValueGroups { get; set; }
        
        public DbSet<ReportTemplate> ReportTemplates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={Paths.DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestGroupTest>()
                .HasKey(tgt => new { tgt.TestGroupId, tgt.TestId });
            modelBuilder.Entity<TestGroupTest>()
                .HasOne(tgt => tgt.TestGroup)
                .WithMany(tg => tg.TestGroupTests)
                .HasForeignKey(tgt => tgt.TestGroupId);
            modelBuilder.Entity<TestGroupTest>()
                .HasOne(tgt => tgt.Test)
                .WithMany(t => t.TestGroupTests)
                .HasForeignKey(tgt => tgt.TestId);
            modelBuilder.Entity<FieldValueGroup>()
                .HasOne(fvg => fvg.Patient);
            modelBuilder.Entity<FieldValueGroup>()
                .HasMany(fvg => fvg.FieldValues)
                .WithOne(fv => fv.FieldValueGroup)
                .IsRequired();
            modelBuilder.Entity<TestResultGroup>()
                .HasMany(trg => trg.Tests)
                .WithOne(tr => tr.TestResultGroup)
                .IsRequired();
        }
    }
}
