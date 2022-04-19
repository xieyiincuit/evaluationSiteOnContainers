﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Infrastructure;

#nullable disable

namespace Evaluation.API.Infrastructure.EvaluationMigrations
{
    [DbContext(typeof(EvaluationContext))]
    partial class EvaluationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationArticle", b =>
                {
                    b.Property<int>("ArticleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("article_id")
                        .HasComment("主键")
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ArticleImage")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("article_image")
                        .HasComment("内容Top呈现图");

                    b.Property<int?>("CategoryTypeId")
                        .HasColumnType("int")
                        .HasColumnName("category_type_id")
                        .HasComment("测评类别主键");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("content")
                        .HasComment("测评文章内容");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_time")
                        .HasComment("测评内容创建时间");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("description")
                        .HasComment("文章测评简介");

                    b.Property<string>("DescriptionImage")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("description_image")
                        .HasComment("展示略缩图");

                    b.Property<int>("GameId")
                        .HasColumnType("int")
                        .HasColumnName("game_id")
                        .HasComment("测评内容关联游戏id");

                    b.Property<string>("GameName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("game_name")
                        .HasComment("测评内容关联游戏名");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_deleted")
                        .HasComment("逻辑删除");

                    b.Property<int>("JoinCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("join_count")
                        .HasComment("文章浏览量");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("user_name")
                        .HasComment("测评内容作者姓名");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("article_status")
                        .HasComment("文章发布状态");

                    b.Property<int>("SupportCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("support_count")
                        .HasComment("文章点赞数量");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("title")
                        .HasComment("测评文章标题");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("update_time")
                        .HasComment("测评内容更新时间");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("varchar(450)")
                        .HasColumnName("user_id")
                        .HasComment("测评内容作者id");

                    b.HasKey("ArticleId");

                    b.HasIndex("CategoryTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("evaluation_article", (string)null);

                    b.HasComment("游戏测评文章信息表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("category_id")
                        .HasComment("测评类别主键");

                    b.Property<string>("CategoryType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("varchar(10)")
                        .HasColumnName("category_type")
                        .HasComment("测评类别名");

                    b.Property<bool?>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false)
                        .HasColumnName("is_deleted")
                        .HasComment("逻辑删除");

                    b.HasKey("CategoryId");

                    b.ToTable("evaluation_category", (string)null);

                    b.HasComment("测评文章分类表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationComment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("comment_id")
                        .HasComment("评论主键");

                    b.Property<int>("ArticleId")
                        .HasColumnType("int")
                        .HasColumnName("article_id")
                        .HasComment("评论对应的测评id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("varchar(500)")
                        .HasColumnName("content")
                        .HasComment("评论内容");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("create_time")
                        .HasComment("评论时间");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_deleted")
                        .HasComment("逻辑删除");

                    b.Property<bool?>("IsReply")
                        .HasColumnType("tinyint(1)")
                        .HasColumnName("is_reply")
                        .HasComment("该评论是否为回复");

                    b.Property<int?>("ReplyCommentId")
                        .HasColumnType("int")
                        .HasColumnName("reply_comment_id")
                        .HasComment("回复的评论id");

                    b.Property<string>("ReplyUserId")
                        .HasMaxLength(450)
                        .HasColumnType("varchar(450)")
                        .HasColumnName("reply_userid")
                        .HasComment("回复的玩家Id");

                    b.Property<int?>("RootCommentId")
                        .HasColumnType("int")
                        .HasColumnName("root_comment_id")
                        .HasComment("回复评论属于哪个主评论");

                    b.Property<int>("SupportCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("support_count")
                        .HasComment("评论点赞数量");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(450)
                        .HasColumnType("varchar(450)")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("CommentId");

                    b.HasIndex("ArticleId");

                    b.HasIndex("RootCommentId");

                    b.HasIndex("UserId");

                    b.ToTable("evaluation_comment", (string)null);

                    b.HasComment("测评文章评论表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationLikeRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int>("ArticleId")
                        .HasColumnType("int")
                        .HasColumnName("article_id")
                        .HasComment("文章id");

                    b.Property<DateTime>("CreateTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)")
                        .HasDefaultValue(new DateTime(2022, 4, 19, 21, 8, 25, 351, DateTimeKind.Local).AddTicks(290))
                        .HasColumnName("create_time")
                        .HasComment("点赞时间");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("Id");

                    b.HasIndex("ArticleId");

                    b.HasIndex("UserId");

                    b.ToTable("evaluation_like", (string)null);

                    b.HasComment("测评文章点赞记录表");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationArticle", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationCategory", "CategoryType")
                        .WithMany()
                        .HasForeignKey("CategoryTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("foreignKey_type_article");

                    b.Navigation("CategoryType");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationComment", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationArticle", "EvaluationArticle")
                        .WithMany("EvaluationComments")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("foreignKey_comment_article");

                    b.Navigation("EvaluationArticle");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationLikeRecord", b =>
                {
                    b.HasOne("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationArticle", "EvaluationArticle")
                        .WithMany("EvaluationLikeRecords")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("foreignKey_likeRecord_article");

                    b.Navigation("EvaluationArticle");
                });

            modelBuilder.Entity("Zhouxieyi.evaluationSiteOnContainers.Services.Evaluation.API.Models.EvaluationArticle", b =>
                {
                    b.Navigation("EvaluationComments");

                    b.Navigation("EvaluationLikeRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
