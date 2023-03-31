using System;
using System.Collections.Generic;

namespace PetMerchandise.core.db.entity;

public partial class Inventory
{
    public string Uuid { get; set; } = null!;

    public int ExpY { get; set; }

    public int ExpM { get; set; }

    public int ExpD { get; set; }

    public int? Qty { get; set; }

    public ulong Seq { get; set; }
}
