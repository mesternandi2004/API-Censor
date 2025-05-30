﻿// <auto-generated />
using CENZURAZO_CQW1QQ_MESTER.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CENZURAZO_CQW1QQ_MESTER.Migrations
{
    [DbContext(typeof(CensorDbContext))]
    partial class CensorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CENZURAZO_CQW1QQ_MESTER.Models.AlternativeWord", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Alternative")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReplacementDataID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("ReplacementDataID");

                    b.ToTable("AlternativeWords");
                });

            modelBuilder.Entity("CENZURAZO_CQW1QQ_MESTER.Models.ReplacementData", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<string>("Word")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.ToTable("ReplacementDatas");
                });

            modelBuilder.Entity("CENZURAZO_CQW1QQ_MESTER.Models.AlternativeWord", b =>
                {
                    b.HasOne("CENZURAZO_CQW1QQ_MESTER.Models.ReplacementData", "ReplacementData")
                        .WithMany("Alternatives")
                        .HasForeignKey("ReplacementDataID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReplacementData");
                });

            modelBuilder.Entity("CENZURAZO_CQW1QQ_MESTER.Models.ReplacementData", b =>
                {
                    b.Navigation("Alternatives");
                });
#pragma warning restore 612, 618
        }
    }
}
