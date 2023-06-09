﻿using System;
using System.Collections.Generic;

namespace PetMerchandise.core.db.entity;

public partial class PurchaseOrder
{
    public ulong Index { get; set; }

    public string ProductUuid { get; set; } = null!;

    public int? Year { get; set; }

    public int? Month { get; set; }

    public int? Day { get; set; }

    public int? Qty { get; set; }

    public int PurchaseYear { get; set; }

    public int PurchaseMonth { get; set; }

    public int PurchaseDay { get; set; }

    /// <summary>
    /// 轉入狀態
    /// </summary>
    public int TransInStatus { get; set; }
}
