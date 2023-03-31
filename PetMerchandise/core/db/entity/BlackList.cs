using System;
using System.Collections.Generic;

namespace PetMerchandise.core.db.entity;

public partial class BlackList
{
    public string FbId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? RelativeOrder { get; set; }
}
