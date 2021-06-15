using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using BifrostApi.Models.Converters;
using BifrostApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BifrostApi.Models
{
    public partial class bifrostContext : DbContext
    {
        public bifrostContext()
        {
        }

        public bifrostContext(DbContextOptions<bifrostContext> options)
            : base(options)
        {
        }

        public virtual DbSet<GroupPermission> GroupPermissions { get; set; }
        public virtual DbSet<Machine> Machines { get; set; }
        public virtual DbSet<MachineToken> MachineTokens { get; set; }
        public virtual DbSet<PermissionProperty> PermissionProperties { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new SqlConnectionStringBuilder
                {
                    DataSource = $"{Environment.GetEnvironmentVariable("DB_URL")},{Environment.GetEnvironmentVariable("Db_PORT")}",
                    InitialCatalog = Environment.GetEnvironmentVariable("DB_DefaultDatabase"),
                    UserID = Environment.GetEnvironmentVariable("DB_UserId"),
                    Password = Environment.GetEnvironmentVariable("DB_Password"),
                };

                string connectionString = builder.ConnectionString;

                optionsBuilder.UseNpgsql(connectionString);
            }
        }

        public IQueryable<MachineHierarchyDTO> GetHierarchyForUser(Guid initialParent)
            => FromExpression(() => GetHierarchyForUser(initialParent));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp")
                .HasAnnotation("Relational:Collation", "en_US.utf8");

            modelBuilder.Entity<GroupPermission>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("group_permissions");

                entity.HasIndex(e => new { e.GroupUid, e.PermissionPropertyUid }, "group_permissions_role_permission_property_key")
                    .IsUnique();

                entity.Property(e => e.GroupUid).HasColumnName("group");

                entity.Property(e => e.PermissionPropertyUid).HasColumnName("permission_property");

                entity.HasOne(d => d.GroupNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.GroupUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("group_permissions_role_fkey");

                entity.HasOne(d => d.PermissionPropertyNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.PermissionPropertyUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("group_permissions_permission_property_fkey");
            });

            modelBuilder.Entity<Machine>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("machines_pkey");

                entity.ToTable("machines");

                entity.HasIndex(e => e.Ip, "machines_ip_key")
                    .IsUnique();

                entity.Property(e => e.Uid)
                    .HasColumnName("uid")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasColumnName("deleted")
                    .HasDefaultValueSql("(0)::bit(1)")
                    .HasConversion(ConverterProvider.GetBoolToBitArrayConverter());

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("ip");

                entity.Property(e => e.LastOnline).HasColumnName("last_online");

                entity.Property(e => e.Name)
                    .IsRequired() 
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.UserUid).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Machines)
                    .HasForeignKey(d => d.UserUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("machines_user_id_fkey");
            });

            modelBuilder.Entity<MachineToken>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("machine_tokens_pkey");

                entity.ToTable("machine_tokens");

                entity.HasIndex(e => e.Token, "machine_tokens_token_key")
                    .IsUnique();

                entity.Property(e => e.Uid)
                    .HasColumnName("uid")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasColumnName("active")
                    .HasDefaultValueSql("(0)::bit(1)")
                    .HasConversion(ConverterProvider.GetBoolToBitArrayConverter());

                entity.Property(e => e.CreateDate).HasColumnName("create_date");

                entity.Property(e => e.MachineUid).HasColumnName("machine_id");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("token");

                entity.HasOne(d => d.Machine)
                    .WithMany(p => p.MachineTokens)
                    .HasForeignKey(d => d.MachineUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("machine_tokens_machine_id_fkey");
            });

            modelBuilder.Entity<PermissionProperty>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("permission_properties_pkey");

                entity.ToTable("permission_properties");

                entity.Property(e => e.Id)
                    .HasColumnName("uid")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasColumnName("deleted")
                    .HasDefaultValueSql("(0)::bit(1)")
                    .HasConversion(ConverterProvider.GetBoolToBitArrayConverter());

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");
            });

            //modelBuilder.Entity<PasswordKey>(entity =>
            //{
            //    entity.HasKey(e => e.Uid)
            //    .HasName("users_pkey");
            //    entity.Property(e => e.Uid)
            //        .HasColumnName("uid")
            //        .HasDefaultValueSql("uuid_generate_v4()");
            //    entity.HasOne(e => e.UserNavigation).WithOne(o => o.PasswordKey).HasForeignKey<User>(e => e.Uid);
            //    entity.ToTable("users");
            //});

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("users_pkey");

                entity.ToTable("users");

                entity.HasIndex(e => e.UserName, "users_username_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("uid")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasColumnName("deleted")
                    .HasDefaultValueSql("(0)::bit(1)")
                    .HasConversion(ConverterProvider.GetBoolToBitArrayConverter());

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                //entity.HasOne(e => e.PasswordKey).WithOne(o => o.UserNavigation).HasForeignKey<User>(e => e.Uid);

                entity.Property(e => e.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("passwordsalt");

                entity.Property(e => e.UserGroupUid).HasColumnName("user_group_id");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("username");

                entity.HasOne(d => d.UserGroup)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserGroupUid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("users_user_group_id_fkey");
            });

            //modelBuilder.Entity<User>().Ignore(e => e.Password);
            //modelBuilder.Entity<User>().Ignore(e => e.Passwordsalt);

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("user_group_pkey");

                entity.ToTable("user_group");

                entity.HasIndex(e => e.Name, "user_group_name_key")
                    .IsUnique();

                entity.Property(e => e.Uid)
                    .HasColumnName("uid")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Deleted)
                    .IsRequired()
                    .HasColumnType("bit(1)")
                    .HasColumnName("deleted")
                    .HasDefaultValueSql("(0)::bit(1)")
                    .HasConversion(ConverterProvider.GetBoolToBitArrayConverter());

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.ParentUid).HasColumnName("parent");

                entity.HasOne(d => d.ParentNavigation)
                    .WithMany(p => p.InverseParentNavigation)
                    .HasForeignKey(d => d.ParentUid)
                    .HasConstraintName("user_group_parent_fkey");

                modelBuilder.HasDbFunction(typeof(bifrostContext).GetMethod(nameof(GetHierarchyForUser), new[] { typeof(Guid) }))
                .HasName("GetHierarchy2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
