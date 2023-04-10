using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PetMerchandise.view.bean.promo_handler;

public class PromotionBean : BindingBean
{
    public string PromotionText { get; set; }
    public string ConsoleText { get; set; }
    
    public string imageSourcePath { get; set; }
    public string imageOutputPath { get; set; }
    public string groupType1 { get; set; }
    public string groupType2 { get; set; }

    public string brand { get; set; }
    public bool isGenki { get; set; }
    public bool isWanMeow { get; set; }
    public bool isByUuid { get; set; } //是否依據商品Uuid進行產生
    public bool isBySaleName { get; set; } = true; //是否依據商品販售名稱進行產生

    public ObservableHashSet<string> BrandSource { get; set; } = new();
    public ObservableHashSet<string> GroupType1Source { get; set; } = new();
    public ObservableHashSet<string> GroupType2Source { get; set; } = new();

    public void NotifyPropertyChanged()
    {
        OnPropertyChanged(nameof(PromotionText));
        OnPropertyChanged(nameof(ConsoleText));
    }
}