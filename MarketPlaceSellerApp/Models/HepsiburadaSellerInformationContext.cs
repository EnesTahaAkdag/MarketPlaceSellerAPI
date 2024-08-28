﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MarketPlaceSellerApp.Models;

public partial class HepsiburadaSellerInformationContext : DbContext
{
    public HepsiburadaSellerInformationContext(DbContextOptions<HepsiburadaSellerInformationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SellerInformation> SellerInformations { get; set; }

    public virtual DbSet<UserDatum> UserData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SellerInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Seller-Information");

            entity.ToTable("Seller_Information");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.AverageDeliveryTime)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Category).HasMaxLength(500);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fax)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Link)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Mersis)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NumberOfProducts).HasMaxLength(50);
            entity.Property(e => e.RatingScore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ResponseTime)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SellerName).HasMaxLength(500);
            entity.Property(e => e.StoreName).HasMaxLength(255);
            entity.Property(e => e.StoreScore).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Telephone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Vkn)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("VKN");
        });

        modelBuilder.Entity<UserDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_UserDat");

            entity.Property(e => e.Age).HasColumnType("date");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}