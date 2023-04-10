namespace PetMerchandise.core.db.temp_entity;

public class PromoTempEntity
{
    public string SaleName { get; set; }
    public int? SaleSum { get; set; }
    public decimal? SalePrice { get; set; }
    public int? expY { get; set; }
    public int? expM { get; set; }
    public int? expD { get; set; }
    public string typeGroup1 { get; set; }
    public string uuid { get; set; }

    public string Brand { get; set; }

    public string Animal { get; set; }

    public int MinRowNumber { get; set; }
}