using System;
using System.Collections.Generic;

namespace PetMerchandise.core.db.entity;

public partial class SaleOrderDetail
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public string OrderNo { get; set; } = null!;

    /// <summary>
    /// 商品唯一辨識碼
    /// </summary>
    public string ProductUuid { get; set; } = null!;

    /// <summary>
    /// 效期(年)
    /// </summary>
    public int ExpY { get; set; }

    /// <summary>
    /// 效期(月)
    /// </summary>
    public int ExpM { get; set; }

    /// <summary>
    /// 效期(日)
    /// </summary>
    public int ExpD { get; set; }

    /// <summary>
    /// 販售數量
    /// </summary>
    public int? Qty { get; set; }

    /// <summary>
    /// 商品單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    public virtual SaleOrder OrderNoNavigation { get; set; } = null!;
}
