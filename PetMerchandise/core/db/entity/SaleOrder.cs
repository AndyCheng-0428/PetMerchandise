using System;
using System.Collections.Generic;

namespace PetMerchandise.core.db.entity;

public partial class SaleOrder
{
    public string OrderNo { get; set; } = null!;

    public int? OrderY { get; set; }

    public int? OrderM { get; set; }

    public int? OrderD { get; set; }

    public int? OrderStatus { get; set; }

    public string? Channel { get; set; }

    public string? ChannelOrderNo { get; set; }

    public string? Purchaser { get; set; }

    public string? PurchaserPhone { get; set; }

    public string? PurchaserFbId { get; set; }

    public int? DeliveryFee { get; set; }

    public int? DeliveryFeeType { get; set; }
}
