﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using UrlShortener.Data;

#nullable disable

namespace UrlShortener.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240713083056_AddQrCodeAndUrlMappingRelation")]
    partial class AddQrCodeAndUrlMappingRelation
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("UrlShortener.Models.QrCode", b =>
                {
                    b.Property<string>("ShortenedKey")
                        .HasColumnType("text");

                    b.Property<string>("Base64QrCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ShortenedKey");

                    b.ToTable("QrCodes");
                });

            modelBuilder.Entity("UrlShortener.Models.UrlMapping", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("ExpiresOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("OriginalUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ShortenedKey")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ShortenedKey")
                        .IsUnique();

                    b.ToTable("Urls");
                });

            modelBuilder.Entity("UrlShortener.Models.UrlMapping", b =>
                {
                    b.HasOne("UrlShortener.Models.QrCode", "QrCode")
                        .WithOne("UrlMapping")
                        .HasForeignKey("UrlShortener.Models.UrlMapping", "ShortenedKey");

                    b.Navigation("QrCode");
                });

            modelBuilder.Entity("UrlShortener.Models.QrCode", b =>
                {
                    b.Navigation("UrlMapping")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
