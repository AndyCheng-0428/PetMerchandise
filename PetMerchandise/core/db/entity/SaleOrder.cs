using System;
using System.Collections.Generic;

namespace PetMerchandise.core.db.entity;

public partial class SaleOrder
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public string OrderNo { get; set; } = null!;

    /// <summary>
    /// 訂單年
    /// </summary>
    public int? OrderY { get; set; }

    /// <summary>
    /// 訂單月
    /// </summary>
    public int? OrderM { get; set; }

    /// <summary>
    /// 訂單日
    /// </summary>
    public int? OrderD { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public int OrderStatus { get; set; }

    /// <summary>
    /// 渠道
    /// </summary>
    public string? Channel { get; set; }

    /// <summary>
    /// 渠道訂單編號
    /// </summary>
    public string? ChannelOrderNo { get; set; }

    /// <summary>
    /// 購買者
    /// </summary>
    public string? Purchaser { get; set; }

    /// <summary>
    /// 購買者電話
    /// </summary>
    public string? PurchaserPhone { get; set; }

    /// <summary>
    /// 購買者FB號碼
    /// </summary>
    public string? PurchaserFbId { get; set; }

    public int DeliveryFee { get; set; }

    public int DeliveryFeeType { get; set; }

    public virtual ICollection<SaleOrderDetail> SaleOrderDetails { get; } = new List<SaleOrderDetail>();
}
