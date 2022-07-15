using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ApiGateway.Models;
using Shared;

namespace ApiGateway.Data
{
    public partial class BackOfficeFerromexContext : DbContext
    {
        public BackOfficeFerromexContext()
        {
        }

        public BackOfficeFerromexContext(DbContextOptions<BackOfficeFerromexContext> options)
            : base(options)
        {
        }

        public virtual DbSet<LogRole> LogRoles { get; set; } = null!;
        public virtual DbSet<LogUserActivity> LogUserActivities { get; set; } = null!;
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=10.1.1.49;Database=BackOfficeFerromex;User=PROSIS_DEV;Password=Pr0$1$D3v;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<LogRole>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AspNetRolesId).HasMaxLength(450);

                entity.Property(e => e.IdUser)
                    .HasMaxLength(450)
                    .HasColumnName("ID_User");

                entity.Property(e => e.NewNameRol)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.OldNameRol)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.TypeAction)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<LogUserActivity>(entity =>
            {
                entity.ToTable("LogUserActivity");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.AspNetRolesIdNew).HasMaxLength(450);

                entity.Property(e => e.AspNetRolesIdOld).HasMaxLength(450);

                entity.Property(e => e.IdModifiedUser)
                    .HasMaxLength(450)
                    .HasColumnName("ID_ModifiedUser");

                entity.Property(e => e.IdUpdatedUser)
                    .HasMaxLength(450)
                    .HasColumnName("ID_UpdatedUser");

                entity.Property(e => e.NewLastName).HasMaxLength(60);

                entity.Property(e => e.NewName).HasMaxLength(60);

                entity.Property(e => e.OldLastName).HasMaxLength(60);

                entity.Property(e => e.OldName).HasMaxLength(60);

                entity.Property(e => e.TypeAction)
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
