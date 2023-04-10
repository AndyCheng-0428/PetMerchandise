namespace PetMerchandise.view.bean.order_handler;

public class OrderStatusBean
{
    public string? OrderDate { get; set; }
    
    public string OrderNo { get; set; }

    public string? OrderChannel { get; set; }

    public string? OrderChannelNo { get; set; }

    public int? OrderStatus { get; set; }

    public decimal? OrderSaleSum { get; set; }

    public decimal? OrderDeliveryFee { get; set; }

    public string? OrderDeliveryFeeType { get; set; }

    public string? OrderDeliveryFeeTypeName { get; set; }

    public string? PurchaserName { get; set; }

    public string? PurchaserPhoneNo { get; set; }

    public string? PurchaserFacebookId { get; set; }
}