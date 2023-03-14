using System;
using System.Collections.Generic;

namespace PetMerchandise.core.db.entity;

public partial class SaleOrderDetail
{
    public string OrderNo { get; set; } = null!;

    public string ProductUuid { get; set; } = null!;

    public int ExpY { get; set; }

    public int ExpM { get; set; }

    public int ExpD { get; set; }

    public int? Qty { get; set; }

    public decimal? UnitPrice { get; set; }
}
