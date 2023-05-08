﻿// <auto-generated />
using System;
using ESE.Cart.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ESE.Cart.API.Migrations
{
    [DbContext(typeof(CartDbContext))]
    partial class CartDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ESE.Cart.API.Models.CartClient", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Discount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("VoucherUsed")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .HasName("IDX_Client");

                    b.ToTable("CartClients");
                });

            modelBuilder.Entity("ESE.Cart.API.Models.CartItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CartId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Image")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("ESE.Cart.API.Models.CartClient", b =>
                {
                    b.OwnsOne("ESE.Cart.API.Models.Voucher", "Voucher", b1 =>
                        {
                            b1.Property<Guid>("CartClientId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Code")
                                .HasColumnName("VoucherCode")
                                .HasColumnType("varchar(50)");

                            b1.Property<decimal?>("DiscountValue")
                                .HasColumnName("DiscountValue")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<decimal?>("Percent")
                                .HasColumnName("Percent")
                                .HasColumnType("decimal(18,2)");

                            b1.Property<int>("TypeDiscount")
                                .HasColumnName("TypeDiscount")
                                .HasColumnType("int");

                            b1.HasKey("CartClientId");

                            b1.ToTable("CartClients");

                            b1.WithOwner()
                                .HasForeignKey("CartClientId");
                        });
                });

            modelBuilder.Entity("ESE.Cart.API.Models.CartItem", b =>
                {
                    b.HasOne("ESE.Cart.API.Models.CartClient", "CartClient")
                        .WithMany("Items")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
