﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF;

#nullable disable

namespace IntegrationEventLogEF.Migrations
{
    [DbContext(typeof(IntegrationEventLogContext))]
    partial class IntegrationEventLogContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.BuildingBlocks.IntegrationEventLogEF.IntegrationEventLogEntry", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasColumnName("event_id")
                        .HasComment("事件Id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("content")
                        .HasComment("事件内容");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_time")
                        .HasComment("记录时间");

                    b.Property<string>("EventTypeName")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("event_type_name")
                        .HasComment("事件类型名");

                    b.Property<int>("State")
                        .HasColumnType("int")
                        .HasColumnName("state")
                        .HasComment("事件状态");

                    b.Property<int>("TimesSent")
                        .HasColumnType("int")
                        .HasColumnName("times_sent")
                        .HasComment("发送次数");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("transaction_id")
                        .HasComment("事务Id");

                    b.HasKey("EventId");

                    b.ToTable("integrationevent_log", (string)null);

                    b.HasComment("事件日志表");
                });
#pragma warning restore 612, 618
        }
    }
}
