using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PetMerchandise.view.order_handler;

public class OrderViewBean : INotifyPropertyChanged
{
    public string orderEstablishDate { set; get; } = DateTime.Today.ToString("yyyy/MM/dd"); //訂單建立日期

    public string channel { set; get; } //購買渠道 0:自取 1:賣貨便 2:萊賣貨

    public string orderNo { set; get; } //渠道訂單編號

    public string purchaser { set; get; } //購買者

    public string purchaserPhone { set; get; } //購買者電話

    public string purchaserFbId { set; get; } //購買者Facebook ID

    public int deliveryFee { set; get; } = 0; //運費

    public string deliveryFeeType { set; get; } //運費類型
    
    public void Clear()
    {
        orderEstablishDate = DateTime.Today.ToString("yyyy/MM/dd");
        orderNo = null;
        purchaser = null;
        purchaserPhone = null;
        purchaserFbId = null;
        NotifyPropertyChanged();
    }

    public void NotifyPropertyChanged()
    {
        OnPropertyChanged(nameof(orderEstablishDate));
        OnPropertyChanged(nameof(channel));
        OnPropertyChanged(nameof(orderNo));
        OnPropertyChanged(nameof(purchaser));
        OnPropertyChanged(nameof(purchaserPhone));
        OnPropertyChanged(nameof(purchaserFbId));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}