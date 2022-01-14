﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Infrastructure;

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
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model.EvaluationArticle", b =>
                {
                    b.Property<int>("ArticleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("article_id")
                        .HasComment("主键");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ArticleId"), 1L, 1);

                    b.Property<string>("ArticleImage")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("article_image")
                        .HasComment("内容Top呈现图");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("author")
                        .HasComment("测评内容作者");

                    b.Property<int>("CategoryTypeId")
                        .HasColumnType("int")
                        .HasColumnName("category_type_id")
                        .HasComment("测评类别主键");

                    b.Property<int>("CommentsCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("comments_count")
                        .HasComment("文章评论数量");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("content")
                        .HasComment("测评文章内容");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("create_time")
                        .HasComment("测评内容创建时间");

                    b.Property<string>("DesciptionImage")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("desciption_image")
                        .HasComment("展示略缩图");

                    b.Property<string>("Description")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("description")
                        .HasComment("文章测评简介");

                    b.Property<int>("GameId")
                        .HasColumnType("int")
                        .HasColumnName("game_id")
                        .HasComment("测评内容关联游戏id");

                    b.Property<string>("GameName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("game_name")
                        .HasComment("测评内容关联游戏名");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("is_deleted")
                        .HasComment("逻辑删除");

                    b.Property<int>("JoinCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("join_count")
                        .HasComment("文章浏览量");

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
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("title")
                        .HasComment("测评文章标题");

                    b.Property<DateTime?>("UpdateTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("update_time")
                        .HasComment("测评内容更新时间");

                    b.HasKey("ArticleId");

                    b.HasIndex("CategoryTypeId");

                    b.ToTable("evaluation_article", (string)null);

                    b.HasComment("游戏测评文章信息表");
                });

            modelBuilder.Entity("Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model.EvaluationCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("category_id")
                        .HasComment("测评类别主键");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"), 1L, 1);

                    b.Property<string>("CategoryType")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)")
                        .HasColumnName("category_type")
                        .HasComment("测评类别名");

                    b.HasKey("CategoryId");

                    b.ToTable("evaluation_category", (string)null);

                    b.HasComment("测评文章分类表");
                });

            modelBuilder.Entity("Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model.EvaluationComment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("comment_id")
                        .HasComment("评论主键");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"), 1L, 1);

                    b.Property<int>("ArticleId")
                        .HasColumnType("int")
                        .HasColumnName("article_id")
                        .HasComment("评论对应的测评id");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("avatar")
                        .HasComment("用户头像");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("content")
                        .HasComment("评论内容");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("create_time")
                        .HasComment("评论时间");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit")
                        .HasColumnName("is_deleted")
                        .HasComment("逻辑删除");

                    b.Property<bool?>("IsReplay")
                        .HasColumnType("bit")
                        .HasColumnName("is_replay")
                        .HasComment("该评论是否为回复");

                    b.Property<string>("NickName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("nick_name")
                        .HasComment("用户名");

                    b.Property<int?>("ReplayId")
                        .HasColumnType("int")
                        .HasColumnName("replay_id")
                        .HasComment("回复的评论id");

                    b.Property<int>("SupportCount")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0)
                        .HasColumnName("support_count")
                        .HasComment("评论点赞数量");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("user_id")
                        .HasComment("用户id");

                    b.HasKey("CommentId");

                    b.HasIndex("ArticleId");

                    b.ToTable("evaluation_comment", (string)null);

                    b.HasComment("测评文章评论表");
                });

            modelBuilder.Entity("Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model.EvaluationArticle", b =>
                {
                    b.HasOne("Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model.EvaluationCategory", "CategoryType")
                        .WithMany()
                        .HasForeignKey("CategoryTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("foreignKey_type_article");

                    b.Navigation("CategoryType");
                });

            modelBuilder.Entity("Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model.EvaluationComment", b =>
                {
                    b.HasOne("Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model.EvaluationArticle", "EvaluationArticle")
                        .WithMany("EvaluationComments")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("foreignKey_comment_article");

                    b.Navigation("EvaluationArticle");
                });

            modelBuilder.Entity("Zhouxieyi.evalutionSiteOnContainers.Services.Evaluation.API.Model.EvaluationArticle", b =>
                {
                    b.Navigation("EvaluationComments");
                });
#pragma warning restore 612, 618
        }
    }
}
