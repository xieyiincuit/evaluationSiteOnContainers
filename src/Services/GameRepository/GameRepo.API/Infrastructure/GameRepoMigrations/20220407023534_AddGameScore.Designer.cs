﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Infrastructure;

#nullable disable

namespace GameRepo.API.Infrastructure.GameRepoMigrations
{
    [DbContext(typeof(GameRepoContext))]
    [Migration("20220407023534_AddGameScore")]
    partial class AddGameScore
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("type_id")
                        .HasComment("主键");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("category_name")
                        .HasComment("游戏类型名");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_deleted")
                        .HasComment("逻辑删除");

                    b.HasKey("Id");

                    b.ToTable("game_type");

                    b.HasComment("游戏类型分类表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameCompany", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("company_id")
                        .HasComment("主键");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("company_name")
                        .HasComment("游戏发行公司名");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_deleted")
                        .HasComment("逻辑删除");

                    b.HasKey("Id");

                    b.ToTable("game_company");

                    b.HasComment("发行公司");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("game_id")
                        .HasComment("主键");

                    b.Property<double?>("AverageScore")
                        .HasColumnType("double")
                        .HasColumnName("average_score")
                        .HasComment("游戏评分");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("description")
                        .HasComment("游戏描述");

                    b.Property<string>("DetailsPicture")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("details_picture")
                        .HasComment("游戏展示图大图");

                    b.Property<int?>("GameCategoryId")
                        .HasColumnType("int")
                        .HasColumnName("category_id")
                        .HasComment("游戏类别外键");

                    b.Property<int?>("GameCompanyId")
                        .HasColumnType("int")
                        .HasColumnName("company_id")
                        .HasComment("游戏公司外键");

                    b.Property<int?>("GamePlaySuggestionId")
                        .HasColumnType("int")
                        .HasColumnName("game_playsuggestion_id")
                        .HasComment("游戏游玩建议外键");

                    b.Property<long?>("HotPoints")
                        .HasColumnType("bigint")
                        .HasColumnName("hot_points")
                        .HasComment("游戏热度");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name")
                        .HasComment("游戏名");

                    b.Property<string>("RoughPicture")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("rough_picture")
                        .HasComment("游戏展示图小图");

                    b.Property<DateTime?>("SellTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("sell_time")
                        .HasComment("发售时间");

                    b.Property<string>("SupportPlatform")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("support_platform")
                        .HasComment("游玩平台");

                    b.HasKey("Id");

                    b.HasIndex("GameCategoryId");

                    b.HasIndex("GameCompanyId");

                    b.ToTable("game_info");

                    b.HasComment("游戏信息表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameInfoTag", b =>
                {
                    b.Property<int>("GameId")
                        .HasColumnType("int")
                        .HasColumnName("game_id")
                        .HasComment("游戏id");

                    b.Property<int>("TagId")
                        .HasColumnType("int")
                        .HasColumnName("tag_id")
                        .HasComment("标签id");

                    b.HasKey("GameId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("gameinfo_tag");

                    b.HasComment("游戏与标签的多对多链接表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameItemSDK", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("sdk_id")
                        .HasComment("SDK主键");

                    b.Property<int>("GameItemId")
                        .HasColumnType("int")
                        .HasColumnName("item_id")
                        .HasComment("游戏商品外键");

                    b.Property<bool?>("HasSend")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("has_send")
                        .HasComment("是否已卖出");

                    b.Property<DateTime>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("datetime(6)")
                        .HasColumnName("row_version")
                        .HasComment("行版本");

                    b.Property<string>("SDKString")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("varchar(450)")
                        .HasColumnName("sdk_string")
                        .HasComment("游戏SDK码");

                    b.Property<DateTime?>("SendTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("sent_time")
                        .HasComment("发送时间");

                    b.HasKey("Id");

                    b.HasIndex("GameItemId");

                    b.ToTable("gameitem_sdk");

                    b.HasComment("sdk存放表, 负责给玩家发送未发送的sdk");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameOwner", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.Property<int>("GameId")
                        .HasColumnType("int")
                        .HasColumnName("game_id")
                        .HasComment("游戏信息id");

                    b.Property<double>("GameScore")
                        .HasColumnType("double")
                        .HasColumnName("game_score")
                        .HasComment("游戏评分");

                    b.HasKey("UserId", "GameId");

                    b.ToTable("game_owner");

                    b.HasComment("玩家游戏拥有表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GamePlaySuggestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("suggestion_id")
                        .HasComment("主键");

                    b.Property<string>("CPUName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("cpu_name")
                        .HasComment("CPU型号建议");

                    b.Property<double>("DiskSize")
                        .HasColumnType("double")
                        .HasColumnName("disk_size")
                        .HasComment("磁盘大小建议");

                    b.Property<int>("GameId")
                        .HasColumnType("int")
                        .HasColumnName("game_id")
                        .HasComment("游戏外键id");

                    b.Property<string>("GraphicsCard")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("graphics_card")
                        .HasComment("显卡型号建议");

                    b.Property<double>("MemorySize")
                        .HasColumnType("double")
                        .HasColumnName("memory_size")
                        .HasComment("内存大小建议");

                    b.Property<string>("OperationSystem")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("operation_system")
                        .HasComment("操作系统建议");

                    b.HasKey("Id");

                    b.HasIndex("GameId")
                        .IsUnique();

                    b.ToTable("game_playsuggestion");

                    b.HasComment("游玩游戏配置建议表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameSDKForPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("sdk_player_id")
                        .HasComment("主键");

                    b.Property<DateTime?>("CheckTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("check_time")
                        .HasComment("校验的时间");

                    b.Property<bool?>("HasChecked")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("has_checked")
                        .HasComment("是否已经被校验");

                    b.Property<long>("SDKItemId")
                        .HasColumnType("bigint")
                        .HasColumnName("sdk_id")
                        .HasComment("游戏发放的sdk外键");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("user_id")
                        .HasComment("购买用户的Id");

                    b.HasKey("Id");

                    b.HasIndex("SDKItemId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("gamesdk_player");

                    b.HasComment("保存用户购买的SDK信息");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameShopItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("item_id")
                        .HasComment("主键");

                    b.Property<int>("AvailableStock")
                        .HasColumnType("int")
                        .HasColumnName("available_stock")
                        .HasComment("库存量");

                    b.Property<float>("Discount")
                        .HasColumnType("float")
                        .HasColumnName("discount")
                        .HasComment("折扣");

                    b.Property<int>("GameInfoId")
                        .HasColumnType("int")
                        .HasColumnName("game_id")
                        .HasComment("游戏信息外键");

                    b.Property<double>("HotSellPoint")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("double")
                        .HasDefaultValue(10.0)
                        .HasColumnName("hotsell_point")
                        .HasComment("销售热度");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)")
                        .HasColumnName("price")
                        .HasComment("售价");

                    b.Property<string>("SellPictrue")
                        .HasColumnType("longtext")
                        .HasColumnName("sell_pictrue")
                        .HasComment("商品图片");

                    b.Property<bool?>("TemporaryStopSell")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("temporary_stopsell")
                        .HasComment("暂停销售");

                    b.HasKey("Id");

                    b.HasIndex("GameInfoId")
                        .IsUnique();

                    b.ToTable("gameshop_items");

                    b.HasComment("游戏售卖商品表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameTag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("tag_id")
                        .HasComment("主键");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_deleted")
                        .HasComment("逻辑删除");

                    b.Property<string>("TagName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("tag_name")
                        .HasComment("游戏标签名");

                    b.HasKey("Id");

                    b.ToTable("game_tag");

                    b.HasComment("游戏标签表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameInfo", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameCategory", "GameCategory")
                        .WithMany()
                        .HasForeignKey("GameCategoryId");

                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameCompany", "GameCompany")
                        .WithMany()
                        .HasForeignKey("GameCompanyId");

                    b.Navigation("GameCategory");

                    b.Navigation("GameCompany");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameInfoTag", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameInfo", "GameInfo")
                        .WithMany("GameInfoTags")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameTag", "GameTag")
                        .WithMany("GameInfoTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameInfo");

                    b.Navigation("GameTag");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameItemSDK", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameShopItem", "GameShopItem")
                        .WithMany("GameSDKList")
                        .HasForeignKey("GameItemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("GameShopItem");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GamePlaySuggestion", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameInfo", "GameInfo")
                        .WithOne("GamePlaySuggestion")
                        .HasForeignKey("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GamePlaySuggestion", "GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameInfo");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameSDKForPlayer", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameItemSDK", "GameItemSDK")
                        .WithOne("GameSDKForPlayer")
                        .HasForeignKey("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameSDKForPlayer", "SDKItemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("GameItemSDK");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameShopItem", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameInfo", "GameInfo")
                        .WithOne("GameShopItem")
                        .HasForeignKey("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameShopItem", "GameInfoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("GameInfo");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameInfo", b =>
                {
                    b.Navigation("GameInfoTags");

                    b.Navigation("GamePlaySuggestion")
                        .IsRequired();

                    b.Navigation("GameShopItem")
                        .IsRequired();
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameItemSDK", b =>
                {
                    b.Navigation("GameSDKForPlayer")
                        .IsRequired();
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameShopItem", b =>
                {
                    b.Navigation("GameSDKList");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.GameRepo.API.Models.GameTag", b =>
                {
                    b.Navigation("GameInfoTags");
                });
#pragma warning restore 612, 618
        }
    }
}
