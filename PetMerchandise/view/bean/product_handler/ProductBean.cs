namespace PetMerchandise.view.bean.product_handler;

public class ProductBean : BindingBean
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

    public bool ChannelGenki { get; set; }

    public bool ChannelWanmiao { get; set; }

    public void NotifyPropertyChanged()
    {
        OnPropertyChanged(nameof(Uuid));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(SubName));
        OnPropertyChanged(nameof(Manufacture));
        OnPropertyChanged(nameof(Brand));
        OnPropertyChanged(nameof(Package));
        OnPropertyChanged(nameof(SalePrice));
        OnPropertyChanged(nameof(Animal));
        OnPropertyChanged(nameof(AgeGroup));
        OnPropertyChanged(nameof(TypeGroup1));
        OnPropertyChanged(nameof(TypeGroup2));
        OnPropertyChanged(nameof(Ean13));
        OnPropertyChanged(nameof(Cost));
        OnPropertyChanged(nameof(SaleName));
        OnPropertyChanged(nameof(CostLowest));
        OnPropertyChanged(nameof(ChannelGenki));
        OnPropertyChanged(nameof(ChannelWanmiao));
    }

    public void Clear()
    {
        Uuid = "";
        Name = "";
        SubName = "";
        Manufacture = "";
        Brand = "";
        Package = "";
        SalePrice = 0;
        Animal = "";
        AgeGroup = "";
        TypeGroup1 = "";
        TypeGroup2 = "";
        Ean13 = "";
        Cost = 0;
        SaleName = "";
        CostLowest = 0;
        ChannelGenki = false;
        ChannelWanmiao = false;
        NotifyPropertyChanged();
    }
}