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

    public virtual DbSet<CategoryAttr> CategoryAttrs { get; set; }

    public virtual DbSet<CommodityAttr> CommodityAttrs { get; set; }

    public virtual DbSet<CommodityPrc> CommodityPrcs { get; set; }

    public virtual DbSet<FunctionAttr> FunctionAttrs { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<MannerAttr> MannerAttrs { get; set; }

    public virtual DbSet<PosConfig> PosConfigs { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<PurchaseOrder> PurchaseOrders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SaleOrder> SaleOrders { get; set; }

    public virtual DbSet<SaleOrderDetail> SaleOrderDetails { get; set; }

    public virtual DbSet<TransferOrder> TransferOrders { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=db;user=Andy;password=aa123456", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.21-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<BlackList>(entity =>
        {
            entity.HasKey(e => e.FbId).HasName("PRIMARY");

            entity.ToTable("black_list");

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

        modelBuilder.Entity<CategoryAttr>(entity =>
        {
            entity.HasKey(e => e.CategoryUuid).HasName("PRIMARY");

            entity.ToTable("category_attr");

            entity.HasIndex(e => e.CategoryCde, "CATEGORY_CDE").IsUnique();

            entity.Property(e => e.CategoryUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("CATEGORY_UUID");
            entity.Property(e => e.CategoryCde)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("CATEGORY_CDE");
            entity.Property(e => e.CategoryCompanyId)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("CATEGORY_COMPANY_ID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(24)
                .HasColumnName("CATEGORY_NAME")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.CategoryPosId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("CATEGORY_POS_ID");
            entity.Property(e => e.CategoryShopId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("CATEGORY_SHOP_ID");
            entity.Property(e => e.CategoryStatus)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("CATEGORY_STATUS");
        });

        modelBuilder.Entity<CommodityAttr>(entity =>
        {
            entity.HasKey(e => e.CommodityUuid).HasName("PRIMARY");

            entity.ToTable("commodity_attr");

            entity.HasIndex(e => e.CommodityCde, "commodity_COMMODITY_CDE_Unique_key").IsUnique();

            entity.Property(e => e.CommodityUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("COMMODITY_UUID");
            entity.Property(e => e.CommodityCde)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("COMMODITY_CDE");
            entity.Property(e => e.CommodityCompanyId)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("COMMODITY_COMPANY_ID");
            entity.Property(e => e.CommodityCreatedDate)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("COMMODITY_CREATED_DATE");
            entity.Property(e => e.CommodityCtg)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("COMMODITY_CTG");
            entity.Property(e => e.CommodityEndDate)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("COMMODITY_END_DATE");
            entity.Property(e => e.CommodityIcon)
                .HasMaxLength(3)
                .IsFixedLength()
                .HasColumnName("COMMODITY_ICON");
            entity.Property(e => e.CommodityName)
                .HasMaxLength(24)
                .HasColumnName("COMMODITY_NAME")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.CommodityPosId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("COMMODITY_POS_ID");
            entity.Property(e => e.CommodityShopId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("COMMODITY_SHOP_ID");
            entity.Property(e => e.CommodityStartDate)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("COMMODITY_START_DATE");
            entity.Property(e => e.CommodityStatus)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("COMMODITY_STATUS");
        });

        modelBuilder.Entity<CommodityPrc>(entity =>
        {
            entity.HasKey(e => e.CommodityPrcUuid).HasName("PRIMARY");

            entity.ToTable("commodity_prc");

            entity.HasIndex(e => e.CommodityCde, "commodity_prc_COMMODITY_CDE_index");

            entity.Property(e => e.CommodityPrcUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("COMMODITY_PRC_UUID");
            entity.Property(e => e.CommodityCde)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("COMMODITY_CDE");
            entity.Property(e => e.Cost)
                .HasPrecision(14, 6)
                .HasColumnName("COST");
            entity.Property(e => e.CrtDate)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("CRT_DATE");
            entity.Property(e => e.EndDate)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("END_DATE");
            entity.Property(e => e.StartDate)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("START_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("STATUS");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(14, 6)
                .HasColumnName("UNIT_PRICE");
        });

        modelBuilder.Entity<FunctionAttr>(entity =>
        {
            entity.HasKey(e => e.FunctionUuid).HasName("PRIMARY");

            entity.ToTable("function_attr");

            entity.Property(e => e.FunctionUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("FUNCTION_UUID");
            entity.Property(e => e.FunctionAccessRole)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("FUNCTION_ACCESS_ROLE");
            entity.Property(e => e.FunctionCde)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("FUNCTION_CDE");
            entity.Property(e => e.FunctionCompanyId)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("FUNCTION_COMPANY_ID");
            entity.Property(e => e.FunctionCtg)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("FUNCTION_CTG");
            entity.Property(e => e.FunctionExternalIconUrl)
                .HasMaxLength(4000)
                .HasColumnName("FUNCTION_EXTERNAL_ICON_URL");
            entity.Property(e => e.FunctionInternalIconType)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("FUNCTION_INTERNAL_ICON_TYPE");
            entity.Property(e => e.FunctionName)
                .HasMaxLength(10)
                .HasColumnName("FUNCTION_NAME")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.FunctionPosId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("FUNCTION_POS_ID");
            entity.Property(e => e.FunctionShopId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("FUNCTION_SHOP_ID");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => new { e.Uuid, e.ExpY, e.ExpM, e.ExpD })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0 });

            entity.ToTable("inventory");

            entity.HasIndex(e => e.Seq, "SEQ").IsUnique();

            entity.HasIndex(e => e.Uuid, "inventory_UUID_index");

            entity.Property(e => e.Uuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("UUID");
            entity.Property(e => e.ExpY).HasColumnName("EXP_Y");
            entity.Property(e => e.ExpM).HasColumnName("EXP_M");
            entity.Property(e => e.ExpD).HasColumnName("EXP_D");
            entity.Property(e => e.Qty).HasColumnName("QTY");
            entity.Property(e => e.Seq)
                .ValueGeneratedOnAdd()
                .HasColumnName("SEQ");
        });

        modelBuilder.Entity<MannerAttr>(entity =>
        {
            entity.HasKey(e => e.MannerUuid).HasName("PRIMARY");

            entity.ToTable("manner_attr");

            entity.Property(e => e.MannerUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("MANNER_UUID");
            entity.Property(e => e.MannerCde)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("MANNER_CDE");
            entity.Property(e => e.MannerCompanyId)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("MANNER_COMPANY_ID");
            entity.Property(e => e.MannerName)
                .HasMaxLength(10)
                .HasColumnName("MANNER_NAME")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.MannerPosId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("MANNER_POS_ID");
            entity.Property(e => e.MannerSeq).HasColumnName("MANNER_SEQ");
            entity.Property(e => e.MannerShopId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("MANNER_SHOP_ID");
            entity.Property(e => e.MannerStatus)
                .HasMaxLength(1)
                .HasDefaultValueSql("'1'")
                .IsFixedLength()
                .HasColumnName("MANNER_STATUS");
        });

        modelBuilder.Entity<PosConfig>(entity =>
        {
            entity.HasKey(e => e.ConfigUuid).HasName("PRIMARY");

            entity.ToTable("pos_config");

            entity.HasIndex(e => new { e.ConfigCompanyId, e.ConfigShopId, e.ConfigPosId }, "CONFIG_COMPANY_ID").IsUnique();

            entity.Property(e => e.ConfigUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("CONFIG_UUID");
            entity.Property(e => e.ConfigAccessToken)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("CONFIG_ACCESS_TOKEN");
            entity.Property(e => e.ConfigCompanyId)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("CONFIG_COMPANY_ID");
            entity.Property(e => e.ConfigPosId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("CONFIG_POS_ID");
            entity.Property(e => e.ConfigShopId)
                .HasMaxLength(6)
                .IsFixedLength()
                .HasColumnName("CONFIG_SHOP_ID");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Uuid).HasName("PRIMARY");

            entity.ToTable("product");

            entity.HasIndex(e => e.Uuid, "product_UUID_uindex").IsUnique();

            entity.Property(e => e.Uuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasColumnName("UUID");
            entity.Property(e => e.AgeGroup)
                .HasMaxLength(5)
                .HasColumnName("AGE_GROUP")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Animal)
                .HasMaxLength(5)
                .HasColumnName("ANIMAL")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Brand)
                .HasMaxLength(32)
                .HasColumnName("BRAND")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
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
                .HasColumnName("MANUFACTURE")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .HasColumnName("NAME")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.Package)
                .HasMaxLength(32)
                .HasColumnName("PACKAGE")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.SaleName)
                .HasMaxLength(32)
                .HasColumnName("SALE_NAME")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.SalePrice)
                .HasPrecision(16, 6)
                .HasColumnName("SALE_PRICE");
            entity.Property(e => e.SubName)
                .HasMaxLength(32)
                .HasColumnName("SUB_NAME")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.TypeGroup1)
                .HasMaxLength(10)
                .HasColumnName("TYPE_GROUP_1")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.TypeGroup2)
                .HasMaxLength(10)
                .HasColumnName("TYPE_GROUP_2")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("purchase_order");

            entity.HasIndex(e => e.Index, "INDEX").IsUnique();

            entity.HasIndex(e => new { e.PurchaseMonth, e.PurchaseYear, e.ProductUuid, e.PurchaseDay, e.Day, e.Year, e.Month }, "key_name").IsUnique();

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
            entity.Property(e => e.TransInStatus)
                .HasDefaultValueSql("'-1'")
                .HasComment("轉入狀態")
                .HasColumnName("TRANS_IN_STATUS");
            entity.Property(e => e.Year).HasColumnName("YEAR");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("role");

            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .HasColumnName("role_name")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.RoleSeq).HasColumnName("role_seq");
            entity.Property(e => e.UserSeq).HasColumnName("user_seq");
        });

        modelBuilder.Entity<SaleOrder>(entity =>
        {
            entity.HasKey(e => e.OrderNo).HasName("PRIMARY");

            entity.ToTable("sale_order");

            entity.HasIndex(e => new { e.ChannelOrderNo, e.Channel }, "key_name").IsUnique();

            entity.Property(e => e.OrderNo)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasComment("訂單編號")
                .HasColumnName("ORDER_NO");
            entity.Property(e => e.Channel)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasComment("渠道")
                .HasColumnName("CHANNEL");
            entity.Property(e => e.ChannelOrderNo)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasComment("渠道訂單編號")
                .HasColumnName("CHANNEL_ORDER_NO");
            entity.Property(e => e.DeliveryFee).HasColumnName("DELIVERY_FEE");
            entity.Property(e => e.DeliveryFeeType).HasColumnName("DELIVERY_FEE_TYPE");
            entity.Property(e => e.OrderD)
                .HasComment("訂單日")
                .HasColumnName("ORDER_D");
            entity.Property(e => e.OrderM)
                .HasComment("訂單月")
                .HasColumnName("ORDER_M");
            entity.Property(e => e.OrderStatus)
                .HasDefaultValueSql("'-1'")
                .HasComment("訂單狀態")
                .HasColumnName("ORDER_STATUS");
            entity.Property(e => e.OrderY)
                .HasComment("訂單年")
                .HasColumnName("ORDER_Y");
            entity.Property(e => e.Purchaser)
                .HasMaxLength(10)
                .HasComment("購買者")
                .HasColumnName("PURCHASER")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.PurchaserFbId)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasComment("購買者FB號碼")
                .HasColumnName("PURCHASER_FB_ID");
            entity.Property(e => e.PurchaserPhone)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasComment("購買者電話")
                .HasColumnName("PURCHASER_PHONE");
        });

        modelBuilder.Entity<SaleOrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderNo, e.ProductUuid, e.ExpY, e.ExpM, e.ExpD })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0, 0, 0 });

            entity.ToTable("sale_order_detail");

            entity.Property(e => e.OrderNo)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasComment("訂單編號")
                .HasColumnName("ORDER_NO");
            entity.Property(e => e.ProductUuid)
                .HasMaxLength(32)
                .IsFixedLength()
                .HasComment("商品唯一辨識碼")
                .HasColumnName("PRODUCT_UUID");
            entity.Property(e => e.ExpY)
                .HasComment("效期(年)")
                .HasColumnName("EXP_Y");
            entity.Property(e => e.ExpM)
                .HasComment("效期(月)")
                .HasColumnName("EXP_M");
            entity.Property(e => e.ExpD)
                .HasComment("效期(日)")
                .HasColumnName("EXP_D");
            entity.Property(e => e.Qty)
                .HasComment("販售數量")
                .HasColumnName("QTY");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 6)
                .HasComment("商品單價")
                .HasColumnName("UNIT_PRICE");

            entity.HasOne(d => d.OrderNoNavigation).WithMany(p => p.SaleOrderDetails)
                .HasForeignKey(d => d.OrderNo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sale_order_detail_ibfk_1");
        });

        modelBuilder.Entity<TransferOrder>(entity =>
        {
            entity.HasKey(e => e.Index).HasName("PRIMARY");

            entity.ToTable("transfer_order");

            entity.HasIndex(e => e.Index, "INDEX").IsUnique();

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
            entity.Property(e => e.Year).HasColumnName("YEAR");
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("user_info");

            entity.HasIndex(e => e.UserAccount, "user_account_uk").IsUnique();

            entity.Property(e => e.UserAccount)
                .HasMaxLength(16)
                .IsFixedLength()
                .HasColumnName("user_account");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(120)
                .IsFixedLength()
                .HasColumnName("user_email");
            entity.Property(e => e.UserName)
                .HasMaxLength(10)
                .HasColumnName("user_name")
                .UseCollation("utf8_general_ci")
                .HasCharSet("utf8");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(120)
                .HasColumnName("user_password");
            entity.Property(e => e.UserRoleCde)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("user_role_cde");
            entity.Property(e => e.UserSeq).HasColumnName("user_seq");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
