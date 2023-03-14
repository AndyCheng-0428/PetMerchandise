using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PetMerchandise.core.db.entity;

public partial class SaleContext : DbContext
{
    public SaleContext()
    {
    }

    public SaleContext(DbContextOptions<SaleContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlackList> BlackLists { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<SaleOrder> SaleOrders { get; set; }

    public virtual DbSet<SaleOrderDetail> SaleOrderDetails { get; set; }

    public virtual DbSet<TransferOrder> TransferOrders { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=beta_db;user=Andy;password=aa123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.21-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<BlackList>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("black_list");

            entity.Property(e => e.FbId)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("FB_ID");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("PHONE");
            entity.Property(e => e.Reason)
                .HasColumnType("text")
                .HasColumnName("REASON");
            entity.Property(e => e.RelativeOrder)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("RELATIVE_ORDER");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("STATUS");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("inventory");

            entity.Property(e => e.ExpD).HasColumnName("EXP_D");
            entity.Property(e => e.ExpM).HasColumnName("EXP_M");
            entity.Property(e => e.ExpY).HasColumnName("EXP_Y");
            entity.Property(e => e.Qty).HasColumnName("QTY");
            entity.Property(e => e.Seq).HasColumnName("SEQ");
            entity.Property(e => e.Uuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("UUID");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("PRIMARY");

            entity.ToTable("product");

            entity.Property(e => e.Uuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("UUID");
            entity.Property(e => e.AgeGroup)
                .HasMaxLength(5)
                .HasColumnName("AGE_GROUP");
            entity.Property(e => e.Animal)
                .HasMaxLength(5)
                .HasColumnName("ANIMAL");
            entity.Property(e => e.Brand)
                .HasMaxLength(32)
                .HasColumnName("BRAND");
            entity.Property(e => e.ChannelGenki).HasColumnName("CHANNEL_GENKI");
            entity.Property(e => e.ChannelWanmiao).HasColumnName("CHANNEL_WANMIAO");
            entity.Property(e => e.Cost)
                .HasPrecision(16, 6)
                .HasColumnName("COST");
            entity.Property(e => e.CostLowest)
                .HasPrecision(16, 6)
                .HasColumnName("COST_LOWEST");
            entity.Property(e => e.Ean13)
                .HasMaxLength(13)
                .IsFixedLength()
                .HasColumnName("EAN13");
            entity.Property(e => e.Manufacture)
                .HasMaxLength(64)
                .HasColumnName("MANUFACTURE");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasColumnName("NAME");
            entity.Property(e => e.Package)
                .HasMaxLength(32)
                .HasColumnName("PACKAGE");
            entity.Property(e => e.SaleName)
                .HasMaxLength(32)
                .HasColumnName("SALE_NAME");
            entity.Property(e => e.SalePrice)
                .HasPrecision(16, 6)
                .HasColumnName("SALE_PRICE");
            entity.Property(e => e.SubName)
                .HasMaxLength(32)
                .HasColumnName("SUB_NAME");
            entity.Property(e => e.TypeGroup1)
                .HasMaxLength(10)
                .HasColumnName("TYPE_GROUP_1");
            entity.Property(e => e.TypeGroup2)
                .HasMaxLength(10)
                .HasColumnName("TYPE_GROUP_2");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("purchase_order");

            entity.Property(e => e.Index).HasColumnName("INDEX");
            entity.Property(e => e.Day).HasColumnName("DAY");
            entity.Property(e => e.Month).HasColumnName("MONTH");
            entity.Property(e => e.ProductUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("PRODUCT_UUID");
            entity.Property(e => e.PurchaseDay).HasColumnName("PURCHASE_DAY");
            entity.Property(e => e.PurchaseMonth).HasColumnName("PURCHASE_MONTH");
            entity.Property(e => e.PurchaseYear).HasColumnName("PURCHASE_YEAR");
            entity.Property(e => e.Qty).HasColumnName("QTY");
            entity.Property(e => e.TransInStatus).HasColumnName("TRANS_IN_STATUS");
            entity.Property(e => e.Year).HasColumnName("YEAR");
        });

        modelBuilder.Entity<SaleOrder>(entity =>
        {
            entity.HasKey(e => e.OrderNo).HasName("PRIMARY");

            entity.ToTable("sale_order");

            entity.HasIndex(e => new { e.ChannelOrderNo, e.Channel }, "key_name").IsUnique();

            entity.Property(e => e.OrderNo)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("ORDER_NO");
            entity.Property(e => e.Channel)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("CHANNEL");
            entity.Property(e => e.ChannelOrderNo)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("CHANNEL_ORDER_NO");
            entity.Property(e => e.DeliveryFee).HasColumnName("DELIVERY_FEE");
            entity.Property(e => e.DeliveryFeeType).HasColumnName("DELIVERY_FEE_TYPE");
            entity.Property(e => e.OrderD).HasColumnName("ORDER_D");
            entity.Property(e => e.OrderM).HasColumnName("ORDER_M");
            entity.Property(e => e.OrderStatus).HasColumnName("ORDER_STATUS");
            entity.Property(e => e.OrderY).HasColumnName("ORDER_Y");
            entity.Property(e => e.Purchaser)
                .HasMaxLength(10)
                .HasColumnName("PURCHASER");
            entity.Property(e => e.PurchaserFbId)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("PURCHASER_FB_ID");
            entity.Property(e => e.PurchaserPhone)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("PURCHASER_PHONE");
        });

        modelBuilder.Entity<SaleOrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.ExpD, e.OrderNo, e.ProductUuid, e.ExpY, e.ExpM })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0, 0 });

            entity.ToTable("sale_order_detail");

            entity.HasIndex(e => e.OrderNo, "index_name");

            entity.Property(e => e.ExpD).HasColumnName("EXP_D");
            entity.Property(e => e.OrderNo)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("ORDER_NO");
            entity.Property(e => e.ProductUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("PRODUCT_UUID");
            entity.Property(e => e.ExpY).HasColumnName("EXP_Y");
            entity.Property(e => e.ExpM).HasColumnName("EXP_M");
            entity.Property(e => e.Qty).HasColumnName("QTY");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 6)
                .HasColumnName("UNIT_PRICE");
        });

        modelBuilder.Entity<TransferOrder>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("transfer_order");

            entity.Property(e => e.Day).HasColumnName("DAY");
            entity.Property(e => e.Index).HasColumnName("INDEX");
            entity.Property(e => e.Month).HasColumnName("MONTH");
            entity.Property(e => e.ProductUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("PRODUCT_UUID");
            entity.Property(e => e.PurchaseDay).HasColumnName("PURCHASE_DAY");
            entity.Property(e => e.PurchaseMonth).HasColumnName("PURCHASE_MONTH");
            entity.Property(e => e.PurchaseYear).HasColumnName("PURCHASE_YEAR");
            entity.Property(e => e.Qty).HasColumnName("QTY");
            entity.Property(e => e.Year).HasColumnName("YEAR");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
