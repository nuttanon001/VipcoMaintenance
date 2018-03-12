﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using VipcoMaintenance.Models.Maintenances;

namespace VipcoMaintenance.Migrations
{
    [DbContext(typeof(MaintenanceContext))]
    partial class MaintenanceContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-preview1-28290")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.Branch", b =>
                {
                    b.Property<int>("BranchId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.HasKey("BranchId");

                    b.ToTable("Branch");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BranchId");

                    b.Property<string>("Brand")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<string>("EmpResponsible");

                    b.Property<string>("ItemCode")
                        .HasMaxLength(50);

                    b.Property<string>("ItemImage");

                    b.Property<int?>("ItemStatus");

                    b.Property<int?>("ItemTypeId");

                    b.Property<string>("Model")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.Property<string>("Property")
                        .HasMaxLength(200);

                    b.Property<string>("Property2")
                        .HasMaxLength(200);

                    b.Property<string>("Property3")
                        .HasMaxLength(200);

                    b.HasKey("ItemId");

                    b.HasIndex("BranchId");

                    b.HasIndex("ItemTypeId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.ItemMaintenance", b =>
                {
                    b.Property<int>("ItemMaintenanceId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ActualEndDate");

                    b.Property<DateTime?>("ActualStartDate");

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("ItemMaintenanceCode")
                        .IsRequired();

                    b.Property<string>("MaintenanceEmp");

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<DateTime>("PlanEndDate");

                    b.Property<DateTime>("PlanStartDate");

                    b.Property<string>("Remark")
                        .HasMaxLength(200);

                    b.Property<int?>("RequireMaintenanceId");

                    b.Property<int?>("StatusMaintenance");

                    b.Property<int?>("TypeMaintenanceId");

                    b.HasKey("ItemMaintenanceId");

                    b.HasIndex("RequireMaintenanceId")
                        .IsUnique()
                        .HasFilter("[RequireMaintenanceId] IS NOT NULL");

                    b.HasIndex("TypeMaintenanceId");

                    b.ToTable("ItemMaintenance");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.ItemType", b =>
                {
                    b.Property<int>("ItemTypeId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(150);

                    b.Property<string>("Remark")
                        .HasMaxLength(200);

                    b.Property<int?>("WorkGroupId");

                    b.HasKey("ItemTypeId");

                    b.HasIndex("WorkGroupId");

                    b.ToTable("ItemType");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.MovementStockSp", b =>
                {
                    b.Property<int>("MovementStockSpId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<DateTime>("MovementDate");

                    b.Property<int>("MovementStatus");

                    b.Property<double>("Quantity");

                    b.HasKey("MovementStockSpId");

                    b.ToTable("MovementStockSp");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.Permission", b =>
                {
                    b.Property<int>("PermissionId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<int>("LevelPermission");

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<int>("UserId");

                    b.HasKey("PermissionId");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.ReceiveStockSp", b =>
                {
                    b.Property<int>("ReceiveStockSpId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<int?>("MovementStockSpId");

                    b.Property<string>("PurchaseOrder");

                    b.Property<string>("ReceiveEmp");

                    b.Property<string>("Remark")
                        .HasMaxLength(200);

                    b.Property<int?>("SparePartId");

                    b.HasKey("ReceiveStockSpId");

                    b.HasIndex("MovementStockSpId")
                        .IsUnique()
                        .HasFilter("[MovementStockSpId] IS NOT NULL");

                    b.HasIndex("SparePartId");

                    b.ToTable("ReceiveStockSp");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.RequireMaintenance", b =>
                {
                    b.Property<int>("RequireMaintenanceId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BranchId");

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("GroupMIS");

                    b.Property<int?>("ItemId");

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Remark")
                        .HasMaxLength(200);

                    b.Property<DateTime>("RequireDate");

                    b.Property<string>("RequireEmp");

                    b.Property<string>("RequireNo");

                    b.Property<int?>("RequireStatus");

                    b.HasKey("RequireMaintenanceId");

                    b.HasIndex("BranchId");

                    b.HasIndex("ItemId");

                    b.ToTable("RequireMaintenance");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.RequisitionStockSp", b =>
                {
                    b.Property<int>("RequisitionStockSpId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<int?>("ItemMaintenanceId");

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<int?>("MovementStockSpId");

                    b.Property<string>("Remark");

                    b.Property<string>("RequisitionEmp");

                    b.Property<int?>("SparePartId");

                    b.HasKey("RequisitionStockSpId");

                    b.HasIndex("ItemMaintenanceId");

                    b.HasIndex("MovementStockSpId")
                        .IsUnique()
                        .HasFilter("[MovementStockSpId] IS NOT NULL");

                    b.HasIndex("SparePartId");

                    b.ToTable("RequisitionStockSp");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.SparePart", b =>
                {
                    b.Property<int?>("SparePartId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<double?>("MaxStock");

                    b.Property<double?>("MinStock");

                    b.Property<string>("Model")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.Property<string>("Property")
                        .HasMaxLength(200);

                    b.Property<string>("Remark")
                        .HasMaxLength(200);

                    b.Property<string>("Size")
                        .HasMaxLength(200);

                    b.Property<string>("SparePartImage");

                    b.Property<int?>("WorkGroupId");

                    b.HasKey("SparePartId");

                    b.HasIndex("WorkGroupId");

                    b.ToTable("SparePart");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.TypeMaintenance", b =>
                {
                    b.Property<int>("TypeMaintenanceId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(50);

                    b.HasKey("TypeMaintenanceId");

                    b.ToTable("TypeMaintenance");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.WorkGroup", b =>
                {
                    b.Property<int>("WorkGroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("CreateDate");

                    b.Property<string>("Creator")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ModifyDate");

                    b.Property<string>("Modifyer")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(150);

                    b.Property<string>("Remark")
                        .HasMaxLength(200);

                    b.HasKey("WorkGroupId");

                    b.ToTable("WorkGroup");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.Item", b =>
                {
                    b.HasOne("VipcoMaintenance.Models.Maintenances.Branch", "Branch")
                        .WithMany("Items")
                        .HasForeignKey("BranchId");

                    b.HasOne("VipcoMaintenance.Models.Maintenances.ItemType", "ItemType")
                        .WithMany("Items")
                        .HasForeignKey("ItemTypeId");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.ItemMaintenance", b =>
                {
                    b.HasOne("VipcoMaintenance.Models.Maintenances.RequireMaintenance", "RequireMaintenance")
                        .WithOne("ItemMaintenance")
                        .HasForeignKey("VipcoMaintenance.Models.Maintenances.ItemMaintenance", "RequireMaintenanceId");

                    b.HasOne("VipcoMaintenance.Models.Maintenances.TypeMaintenance", "TypeMaintenance")
                        .WithMany("ItemMaintenances")
                        .HasForeignKey("TypeMaintenanceId");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.ItemType", b =>
                {
                    b.HasOne("VipcoMaintenance.Models.Maintenances.WorkGroup", "WorkGroup")
                        .WithMany("ItemTypes")
                        .HasForeignKey("WorkGroupId");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.ReceiveStockSp", b =>
                {
                    b.HasOne("VipcoMaintenance.Models.Maintenances.MovementStockSp", "MovementStockSp")
                        .WithOne("ReceiveStockSp")
                        .HasForeignKey("VipcoMaintenance.Models.Maintenances.ReceiveStockSp", "MovementStockSpId");

                    b.HasOne("VipcoMaintenance.Models.Maintenances.SparePart", "SparePart")
                        .WithMany()
                        .HasForeignKey("SparePartId");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.RequireMaintenance", b =>
                {
                    b.HasOne("VipcoMaintenance.Models.Maintenances.Branch", "Branch")
                        .WithMany("RequireMaintenances")
                        .HasForeignKey("BranchId");

                    b.HasOne("VipcoMaintenance.Models.Maintenances.Item", "Item")
                        .WithMany("RequireMaintenances")
                        .HasForeignKey("ItemId");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.RequisitionStockSp", b =>
                {
                    b.HasOne("VipcoMaintenance.Models.Maintenances.ItemMaintenance", "ItemMaintenance")
                        .WithMany("RequisitionStockSps")
                        .HasForeignKey("ItemMaintenanceId");

                    b.HasOne("VipcoMaintenance.Models.Maintenances.MovementStockSp", "MovementStockSp")
                        .WithOne("RequisitionStockSp")
                        .HasForeignKey("VipcoMaintenance.Models.Maintenances.RequisitionStockSp", "MovementStockSpId");

                    b.HasOne("VipcoMaintenance.Models.Maintenances.SparePart", "SparePart")
                        .WithMany("RequisitionStockSps")
                        .HasForeignKey("SparePartId");
                });

            modelBuilder.Entity("VipcoMaintenance.Models.Maintenances.SparePart", b =>
                {
                    b.HasOne("VipcoMaintenance.Models.Maintenances.WorkGroup", "WorkGroup")
                        .WithMany("SpareParts")
                        .HasForeignKey("WorkGroupId");
                });
#pragma warning restore 612, 618
        }
    }
}
