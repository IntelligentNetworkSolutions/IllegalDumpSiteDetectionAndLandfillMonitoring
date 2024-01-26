﻿// <auto-generated />
using System;
using DAL.ApplicationStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240125114443_GeomAsGeometryIgnore")]
    partial class GeomAsGeometryIgnore
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "postgis");
            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Entities.ApplicationSettings", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("text")
                        .HasColumnName("key");

                    b.Property<int>("DataType")
                        .HasColumnType("integer")
                        .HasColumnName("data_type");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Module")
                        .HasColumnType("text")
                        .HasColumnName("module");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("Key")
                        .HasName("pk_application_settings");

                    b.ToTable("application_settings");
                });

            modelBuilder.Entity("Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer")
                        .HasColumnName("access_failed_count");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("email");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("email_confirmed");

                    b.Property<string>("FirstName")
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<string>("LastName")
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("lockout_enabled");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("lockout_end");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_email");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_user_name");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text")
                        .HasColumnName("password_hash");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text")
                        .HasColumnName("phone_number");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean")
                        .HasColumnName("phone_number_confirmed");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text")
                        .HasColumnName("security_stamp");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean")
                        .HasColumnName("two_factor_enabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_users");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("asp_net_users", (string)null);
                });

            modelBuilder.Entity("Entities.AuditLog", b =>
                {
                    b.Property<long>("AuditLogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("audit_log_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("AuditLogId"));

                    b.Property<string>("AuditAction")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("audit_action");

                    b.Property<string>("AuditData")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("audit_data");

                    b.Property<DateTime>("AuditDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("audit_date");

                    b.Property<string>("AuditInternalUser")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("audit_internal_user");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("entity_type");

                    b.Property<string>("TablePk")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("table_pk");

                    b.HasKey("AuditLogId")
                        .HasName("pk_audit_log");

                    b.ToTable("audit_log");
                });

            modelBuilder.Entity("Entities.DatasetEntities.Dataset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by_id");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool>("IsPublished")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_published");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<Guid?>("ParentDatasetId")
                        .HasColumnType("uuid")
                        .HasColumnName("parent_dataset_id");

                    b.Property<string>("UpdatedById")
                        .HasColumnType("text")
                        .HasColumnName("updated_by_id");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_datasets");

                    b.HasIndex("CreatedById")
                        .IsUnique();

                    b.HasIndex("ParentDatasetId")
                        .IsUnique();

                    b.HasIndex("UpdatedById")
                        .IsUnique();

                    b.ToTable("datasets");
                });

            modelBuilder.Entity("Entities.DatasetEntities.DatasetClass", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<int>("ClassId")
                        .HasColumnType("integer")
                        .HasColumnName("class_id");

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("class_name");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by_id");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

                    b.Property<Guid>("DatasetId")
                        .HasColumnType("uuid")
                        .HasColumnName("dataset_id");

                    b.HasKey("Id")
                        .HasName("pk_dataset_classes");

                    b.HasIndex("CreatedById")
                        .IsUnique();

                    b.HasIndex("DatasetId")
                        .IsUnique();

                    b.ToTable("dataset_classes");
                });

            modelBuilder.Entity("Entities.DatasetEntities.DatasetImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by_id");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

                    b.Property<Guid?>("DatasetId")
                        .HasColumnType("uuid")
                        .HasColumnName("dataset_id");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("file_name");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("image_path");

                    b.Property<bool>("IsEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_enabled");

                    b.Property<string>("UpdatedById")
                        .HasColumnType("text")
                        .HasColumnName("updated_by_id");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_dataset_images");

                    b.HasIndex("CreatedById")
                        .IsUnique();

                    b.HasIndex("DatasetId")
                        .IsUnique();

                    b.HasIndex("UpdatedById")
                        .IsUnique();

                    b.ToTable("dataset_images");
                });

            modelBuilder.Entity("Entities.DatasetEntities.ImageAnnotation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("AnnotationsGeoJson")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("annotations_geo_json");

                    b.Property<string>("CreatedById")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("created_by_id");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'");

                    b.Property<Guid?>("DatasetImageId")
                        .HasColumnType("uuid")
                        .HasColumnName("dataset_image_id");

                    b.Property<Geometry>("Geom")
                        .IsRequired()
                        .HasColumnType("geometry")
                        .HasColumnName("geom");

                    b.Property<bool>("IsEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(false)
                        .HasColumnName("is_enabled");

                    b.Property<string>("UpdatedById")
                        .HasColumnType("text")
                        .HasColumnName("updated_by_id");

                    b.Property<DateTime?>("UpdatedOn")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_on");

                    b.HasKey("Id")
                        .HasName("pk_image_annotations");

                    b.HasIndex("CreatedById")
                        .IsUnique();

                    b.HasIndex("DatasetImageId")
                        .IsUnique();

                    b.HasIndex("UpdatedById")
                        .IsUnique();

                    b.ToTable("image_annotations");
                });

            modelBuilder.Entity("Entities.IntranetPortalUsersToken", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ID"));

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("application_user_id");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("token");

                    b.Property<bool>("isTokenUsed")
                        .HasColumnType("boolean")
                        .HasColumnName("is_token_used");

                    b.HasKey("ID")
                        .HasName("pk_intranet_portal_users_tokens");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("intranet_portal_users_tokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text")
                        .HasColumnName("concurrency_stamp");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("name");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)")
                        .HasColumnName("normalized_name");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_roles");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("asp_net_roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("role_id");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_role_claims");

                    b.HasIndex("RoleId");

                    b.ToTable("asp_net_role_claims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text")
                        .HasColumnName("claim_type");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text")
                        .HasColumnName("claim_value");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_asp_net_user_claims");

                    b.HasIndex("UserId");

                    b.ToTable("asp_net_user_claims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text")
                        .HasColumnName("provider_key");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text")
                        .HasColumnName("provider_display_name");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("LoginProvider", "ProviderKey")
                        .HasName("pk_asp_net_user_logins");

                    b.HasIndex("UserId");

                    b.ToTable("asp_net_user_logins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("RoleId")
                        .HasColumnType("text")
                        .HasColumnName("role_id");

                    b.HasKey("UserId", "RoleId")
                        .HasName("pk_asp_net_user_roles");

                    b.HasIndex("RoleId");

                    b.ToTable("asp_net_user_roles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text")
                        .HasColumnName("login_provider");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Value")
                        .HasColumnType("text")
                        .HasColumnName("value");

                    b.HasKey("UserId", "LoginProvider", "Name")
                        .HasName("pk_asp_net_user_tokens");

                    b.ToTable("asp_net_user_tokens", (string)null);
                });

            modelBuilder.Entity("Entities.DatasetEntities.Dataset", b =>
                {
                    b.HasOne("Entities.ApplicationUser", "CreatedBy")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.Dataset", "CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_datasets_asp_net_users_created_by_id");

                    b.HasOne("Entities.DatasetEntities.Dataset", "ParentDataset")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.Dataset", "ParentDatasetId")
                        .HasConstraintName("fk_datasets_datasets_parent_dataset_id");

                    b.HasOne("Entities.ApplicationUser", "UpdatedBy")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.Dataset", "UpdatedById")
                        .HasConstraintName("fk_datasets_asp_net_users_updated_by_id");

                    b.Navigation("CreatedBy");

                    b.Navigation("ParentDataset");

                    b.Navigation("UpdatedBy");
                });

            modelBuilder.Entity("Entities.DatasetEntities.DatasetClass", b =>
                {
                    b.HasOne("Entities.ApplicationUser", "CreatedBy")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.DatasetClass", "CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_dataset_classes_asp_net_users_created_by_id");

                    b.HasOne("Entities.DatasetEntities.Dataset", "Dataset")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.DatasetClass", "DatasetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_dataset_classes_datasets_dataset_id");

                    b.Navigation("CreatedBy");

                    b.Navigation("Dataset");
                });

            modelBuilder.Entity("Entities.DatasetEntities.DatasetImage", b =>
                {
                    b.HasOne("Entities.ApplicationUser", "CreatedBy")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.DatasetImage", "CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_dataset_images_asp_net_users_created_by_id");

                    b.HasOne("Entities.DatasetEntities.Dataset", "Dataset")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.DatasetImage", "DatasetId")
                        .HasConstraintName("fk_dataset_images_datasets_dataset_id");

                    b.HasOne("Entities.ApplicationUser", "UpdatedBy")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.DatasetImage", "UpdatedById")
                        .HasConstraintName("fk_dataset_images_asp_net_users_updated_by_id");

                    b.Navigation("CreatedBy");

                    b.Navigation("Dataset");

                    b.Navigation("UpdatedBy");
                });

            modelBuilder.Entity("Entities.DatasetEntities.ImageAnnotation", b =>
                {
                    b.HasOne("Entities.ApplicationUser", "CreatedBy")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.ImageAnnotation", "CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_image_annotations_asp_net_users_created_by_id");

                    b.HasOne("Entities.DatasetEntities.DatasetImage", "DatasetImage")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.ImageAnnotation", "DatasetImageId")
                        .HasConstraintName("fk_image_annotations_dataset_images_dataset_image_id");

                    b.HasOne("Entities.ApplicationUser", "UpdatedBy")
                        .WithOne()
                        .HasForeignKey("Entities.DatasetEntities.ImageAnnotation", "UpdatedById")
                        .HasConstraintName("fk_image_annotations_asp_net_users_updated_by_id");

                    b.Navigation("CreatedBy");

                    b.Navigation("DatasetImage");

                    b.Navigation("UpdatedBy");
                });

            modelBuilder.Entity("Entities.IntranetPortalUsersToken", b =>
                {
                    b.HasOne("Entities.ApplicationUser", "ApplicationUsers")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_intranet_portal_users_tokens_asp_net_users_application_user~");

                    b.Navigation("ApplicationUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_role_claims_asp_net_roles_role_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_claims_asp_net_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_logins_asp_net_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_roles_role_id");

                    b.HasOne("Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_users_user_id");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_asp_net_user_tokens_asp_net_users_user_id");
                });
#pragma warning restore 612, 618
        }
    }
}
