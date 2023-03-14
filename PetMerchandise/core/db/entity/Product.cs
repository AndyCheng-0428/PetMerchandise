using System;
using System.Collections.Generic;

namespace PetMerchandise.core.db.entity;

public partial class Product
{
    public string Uuid { get; set; } = null!;

    public string? Name { get; set; }

    public string? SubName { get; set; }

    public string? Manufacture { get; set; }

    public string? Brand { get; set; }

    public string? Package { get; set; }

    public decimal? SalePrice { get; set; }

    public string? Animal { get; set; }

    public string? AgeGroup { get; set; }

    public string? TypeGroup1 { get; set; }

    public string? TypeGroup2 { get; set; }

    public string? Ean13 { get; set; }

    public decimal? Cost { get; set; }

    public string? SaleName { get; set; }

    public decimal? CostLowest { get; set; }

    public sbyte? ChannelGenki { get; set; }

    public sbyte? ChannelWanmiao { get; set; }
}
