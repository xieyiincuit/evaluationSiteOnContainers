﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Infrastructure;

#nullable disable

namespace BackManage.API.Infrastructure.BackManageMigrations
{
    [DbContext(typeof(BackManageContext))]
    [Migration("20220407060951_ChangeBodyLength")]
    partial class ChangeBodyLength
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Models.ApproveRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("approve_id")
                        .HasComment("主键");

                    b.Property<DateTime>("ApplyTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValue(new DateTime(2022, 4, 7, 14, 9, 50, 980, DateTimeKind.Local).AddTicks(8253))
                        .HasColumnName("apply_time")
                        .HasComment("申请时间");

                    b.Property<DateTime?>("ApproveTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("approve_time")
                        .HasComment("审批时间");

                    b.Property<string>("ApproveUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("approve_user")
                        .HasComment("审批人");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("approve_body")
                        .HasComment("测评审批信息正文");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("approve_status")
                        .HasComment("审批状态");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("user_id")
                        .HasComment("申请用户id");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("approve_record");

                    b.HasComment("测评资格申请表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Models.BannedRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("banned_id")
                        .HasComment("主键");

                    b.Property<string>("ApproveUser")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("approve_user")
                        .HasComment("审批人");

                    b.Property<DateTime?>("BannedTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("banned_time")
                        .HasComment("冻结时间");

                    b.Property<int>("ReportCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1)
                        .HasColumnName("report_count")
                        .HasComment("被举报次数");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("banned_status")
                        .HasComment("封禁状态");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("banned_user_id")
                        .HasComment("被举报用户id");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("banned_record");

                    b.HasComment("用户举报记录表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.BackManage.API.Models.BannedUserLink", b =>
                {
                    b.Property<string>("BannedUserId")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("banned_user_id")
                        .HasComment("被举报用户id");

                    b.Property<string>("CheckUserId")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("check_user_id")
                        .HasComment("发起举报用户id");

                    b.HasKey("BannedUserId", "CheckUserId");

                    b.ToTable("banned_user_link");

                    b.HasComment("用户举报链接表");
                });
#pragma warning restore 612, 618
        }
    }
}
