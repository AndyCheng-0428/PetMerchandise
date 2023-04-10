using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PetMerchandise.view.bean.order_handler;

public class OrderDetailViewBean : BindingBean
{
    public string? Uuid { set; get; } //商品的UUID -> 不可輸入

    public int? Status { set; get; } =
        -2; //商品入庫狀態 (已入庫 0 / 未入庫 -1 / 未登記 -2) 未登記(尚未儲存進DB資料表) -> 未入庫(僅登記到登記項) -> 已入庫(登記到登記項後，同時更改庫存表增加庫存量)

    public string StatusText { set; get; } = "未登記"; //商品入庫狀態文字

    public Boolean Enable { set; get; } = true;

    public string ean13 { set; get; } //商品國際條碼

    public DateTime expDate { set; get; } = DateTime.Today; //效期

    public int? qty { set; get; } //數量

    public decimal? unitPrice { set; get; } //單價

    public string? groupType1 { set; get; } //類別一

    public string? groupType2 { set; get; } //類別二

    public string? brand { set; get; } //品牌

    public string? productName { set; get; } //商品名

    public string? productSubName { set; get; } //商品副品名

    public string? package { set; get; } //商品包裝

    public bool resetNeed { set; get; } = true; //是否需要重設

    public ObservableHashSet<string> brandNameSource { get; set; } = new(); //此欄位會隨所挑選類別一變更
    public ObservableHashSet<string> productNameSource { get; set; } = new(); //此欄位會隨所挑選類別一以及品牌變更
    public ObservableHashSet<string> productSubNameSource { get; set; } = new(); //此欄位會隨所挑選類別一以及品牌變更
    public ObservableHashSet<string> packageSource { get; set; } = new(); //此欄位會隨所挑選類別一以及品牌變更
    public ObservableHashSet<string> uuidSource { get; set; } = new(); //此欄位會隨所挑選類別一以及品牌變更

    public void NotifyPropertyChanged()
    {
        OnPropertyChanged(nameof(brandNameSource));
        OnPropertyChanged(nameof(productNameSource));
        OnPropertyChanged(nameof(productSubNameSource));
        OnPropertyChanged(nameof(packageSource));
        OnPropertyChanged(nameof(uuidSource));
        OnPropertyChanged(nameof(Uuid));
        OnPropertyChanged(nameof(groupType1));
        OnPropertyChanged(nameof(groupType2));
        OnPropertyChanged(nameof(brand));
        OnPropertyChanged(nameof(productName));
        OnPropertyChanged(nameof(productSubName));
        OnPropertyChanged(nameof(package));
    }
}