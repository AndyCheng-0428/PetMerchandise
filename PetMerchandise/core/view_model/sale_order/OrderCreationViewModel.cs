using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows.Input;
using PetMerchandise.core.db.entity;
using PetMerchandise.core.view_model.misc;
using PetMerchandise.view.bean.order_handler;
using PetMerchandise.view.misc;
using PetMerchandise.view.page;

namespace PetMerchandise.core.view_model.sale_order;

/// <summary>
/// 訂單建立的ViewModel
/// 本類別中進行以下處理
/// 1.建立主要訂單 sale_order --> 此表新增資料
/// 2.根據主要訂單建立底下的商品清單 sale_order_detail --> 此表新增資料 (尚不更新庫存)
/// 並應進行下述檢查
/// 1.主要清單中的購買者Facebook ID是否存在黑名單中，若是，則不可購買，則錯誤
/// 2.商品清單中的商品 對應於庫存商品數量是否足夠，若足夠方得加入，若不足夠，則錯誤
///
/// 由於Database中的商品並不一定有EAN13可以輸入，故應將所有商品取出返回給予其作選取
/// </summary>
public class OrderCreationViewModel : BaseImportExportViewModel
{
    public OrderViewBean OrderViewBean { set; get; }

    public ICommand SaveOrderCommand { get; set; }
    public ICommand QueryChannelOrderCommand { get; set; }

    public ICommand SaleTypeChangedCommand { get; set; }


    /// <summary>
    /// 初始化所需使用之元素   
    /// </summary>
    protected override void InitialElement()
    {
        OrderViewBean = new();
        base.InitialElement();
    }


    /// <summary>
    /// 初始化與UI層可能會使用到之命令
    /// </summary>
    protected override void InitialCommand()
    {
        base.InitialCommand();
        SaveOrderCommand = new RelayCommand<object>(_ => { CreateSaleOrder(); });
        QueryChannelOrderCommand = new RelayCommand<object>(_ => { QueryChannelOrder(); });
        SaleTypeChangedCommand = new RelayCommand<object>(_ => { SaleTypeChanged(); });
    }

    private void SaleTypeChanged()
    {
        if (!string.Equals(OrderViewBean.channel, "0"))
        {
            OrderViewBean.orderNo = "";
            OrderViewBean.NotifyPropertyChanged();
            return;
        }

        // 自取自動帶入編號
        DateTime dateTime = DateTime.Today;
        SaleContext saleContext = _saleOrderRepository.DbContext as SaleContext;
        var result = from so in saleContext.SaleOrders
            where so.OrderY == dateTime.Year && so.OrderM == dateTime.Month && so.OrderD == dateTime.Day
            select so;
        int Count = result.Count();
        int LastIndex = Count + 1;
        OrderViewBean.orderNo = string.Format("{0:0000}{1:00}{2:00}{3:000000}", dateTime.Year, dateTime.Month,
            dateTime.Day, LastIndex);
        OrderViewBean.NotifyPropertyChanged();
    }


    /// <summary>
    /// 根據銷售渠道查詢已有訂單，若為空　則不查
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void QueryChannelOrder()
    {
        if (string.IsNullOrWhiteSpace(OrderViewBean.channel) || string.IsNullOrWhiteSpace(OrderViewBean.orderNo))
        {
            return;
        }

        SaleOrder saleOrder = _saleOrderRepository.Read(order =>
            order.Channel == OrderViewBean.channel && order.ChannelOrderNo == OrderViewBean.orderNo);
        if (null == saleOrder)
        {
            return;
        }

        // 清空商品清單內容
        OrderDetailBeanList.Clear();

        // 根據後臺資料取出主訂單內容並設值
        OrderViewBean.purchaserFbId = saleOrder.PurchaserFbId;
        OrderViewBean.orderEstablishDate = string.Format("{0:0000}/{1:00}/{2:00}", saleOrder.OrderY, saleOrder.OrderM,
            saleOrder.OrderD);
        OrderViewBean.purchaser = saleOrder.Purchaser;
        OrderViewBean.purchaserPhone = saleOrder.PurchaserPhone;
        OrderViewBean.NotifyPropertyChanged();

        var context = _saleOrderRepository.DbContext as SaleContext;

        var results = (from sod in context.SaleOrderDetails
            join p in context.Products on sod.ProductUuid equals p.Uuid
            where sod.OrderNo == saleOrder.OrderNo
            select new { sod, p }).ToList();
        foreach (var result in results)
        {
            OrderDetailViewBean detailViewBean = new();
            detailViewBean.ean13 = result.p.Ean13;
            detailViewBean.groupType1 = result.p.TypeGroup1;
            detailViewBean.groupType2 = result.p.TypeGroup2;

            detailViewBean.brand = result.p.Brand;
            detailViewBean.brandNameSource.Add(detailViewBean.brand);
            detailViewBean.productName = result.p.Name;
            detailViewBean.productNameSource.Add(detailViewBean.productName);
            detailViewBean.productSubName = result.p.SubName;
            detailViewBean.productSubNameSource.Add(detailViewBean.productSubName);

            detailViewBean.Uuid = result.p.Uuid;
            detailViewBean.uuidSource.Add(detailViewBean.Uuid);
            detailViewBean.qty = result.sod.Qty;
            detailViewBean.unitPrice = result.sod.UnitPrice;
            detailViewBean.expDate = DateTime.Parse(string.Format("{0:0000}/{1:00}/{2:00}", result.sod.ExpY,
                result.sod.ExpM, result.sod.ExpD));
            detailViewBean.package = result.p.Package;
            detailViewBean.packageSource.Add(detailViewBean.package);
            detailViewBean.resetNeed = false;
            OrderDetailBeanList.Add(detailViewBean);
            detailViewBean.NotifyPropertyChanged();
        }
    }

    /// <summary>
    /// 根據整體表單進行資料表的檢查
    /// 新增販售主訂單 以及詳細單
    /// 若該張訂單本身就存在，則須確認訂單狀態是否為-1 （準備中），若非準備中，則不可增加
    /// </summary>
    private void CreateSaleOrder()
    {
        if (string.IsNullOrWhiteSpace(OrderViewBean.purchaser))
        {
            OnShowConfirmation("錯誤", "購買者未填");
            return;
        }

        if (string.IsNullOrWhiteSpace(OrderViewBean.channel))
        {
            OnShowConfirmation("錯誤", "銷售渠道未填");
            return;
        }

        if (string.IsNullOrWhiteSpace(OrderViewBean.orderNo))
        {
            OnShowConfirmation("錯誤", "渠道訂單編號未填");
            return;
        }

        if (string.IsNullOrWhiteSpace(OrderViewBean.purchaserPhone))
        {
            OnShowConfirmation("錯誤", "購買者手機未填");
            return;
        }

        if (string.IsNullOrWhiteSpace(OrderViewBean.purchaserFbId))
        {
            OnShowConfirmation("錯誤", "FaceBook ID未填");
            return;
        }


        if (!Regex.IsMatch(OrderViewBean.purchaserPhone, RegexExpression.MOBILE_PHONE))
        {
            OnShowConfirmation("錯誤", "手機號碼輸入有誤");
            return;
        }

        if (!Regex.IsMatch(OrderViewBean.purchaserFbId, RegexExpression.FB_ID))
        {
            OnShowConfirmation("錯誤", "FaceBook ID輸入有誤");
            return;
        }


        BlackList? blackList = _blackListRepository.Read(bl => bl.FbId == OrderViewBean.purchaserFbId);
        if (null != blackList)
        {
            OnShowConfirmation("注意", "此人在黑名單中");
            return;
        }

        SaleOrder? saleOrder = _saleOrderRepository.Read(
            so => so.Channel == OrderViewBean.channel && so.ChannelOrderNo == OrderViewBean.orderNo
        );
        if (null != saleOrder)
        {
            if (saleOrder.OrderStatus != -1)
            {
                OnShowConfirmation("錯誤", "此訂單並非準備中，故不可修正");
                return;
            }

            saleOrder.OrderY = Int32.Parse(OrderViewBean.orderEstablishDate.Substring(0, 4));
            saleOrder.OrderM = Int32.Parse(OrderViewBean.orderEstablishDate.Substring(5, 2));
            saleOrder.OrderD = Int32.Parse(OrderViewBean.orderEstablishDate.Substring(8, 2));
            saleOrder.PurchaserFbId = OrderViewBean.purchaserFbId;
            saleOrder.Purchaser = OrderViewBean.purchaser;
            saleOrder.PurchaserPhone = OrderViewBean.purchaserPhone;
            saleOrder.DeliveryFee = OrderViewBean.deliveryFee;
            saleOrder.DeliveryFeeType = int.Parse(OrderViewBean.deliveryFeeType);
            _saleOrderRepository.Update(saleOrder,
                new Expression<Func<SaleOrder, object>>[]
                {
                    so => so.OrderY, so => so.OrderM, so => so.OrderD,
                    so => so.Purchaser, so => so.PurchaserPhone, so => so.PurchaserFbId,
                    so => so.DeliveryFee, so => so.DeliveryFeeType
                });
        }
        else
        {
            saleOrder = new();
            saleOrder.OrderStatus = -1;
            saleOrder.Channel = OrderViewBean.channel;
            saleOrder.OrderY = Int32.Parse(OrderViewBean.orderEstablishDate.Substring(0, 4));
            saleOrder.OrderM = Int32.Parse(OrderViewBean.orderEstablishDate.Substring(5, 2));
            saleOrder.OrderD = Int32.Parse(OrderViewBean.orderEstablishDate.Substring(8, 2));
            saleOrder.ChannelOrderNo = OrderViewBean.orderNo;
            saleOrder.PurchaserFbId = OrderViewBean.purchaserFbId;
            saleOrder.Purchaser = OrderViewBean.purchaser;
            saleOrder.PurchaserPhone = OrderViewBean.purchaserPhone;
            saleOrder.DeliveryFee = OrderViewBean.deliveryFee;
            saleOrder.DeliveryFeeType = int.Parse(OrderViewBean.deliveryFeeType);
            saleOrder.OrderNo = Guid.NewGuid().ToString().Replace("-", "");
            _saleOrderRepository.Create(saleOrder);
        }

        _saleOrderRepository.SaveChanges();

        // 只要改動 就要先刪除所有的商品細項，之後再增加
        List<SaleOrderDetail> sods = _saleOrderDetailRepository.Reads().Where(sod => sod.OrderNo == saleOrder.OrderNo)
            .ToList();
        foreach (var sod in sods)
        {
            _saleOrderDetailRepository.Delete(sod);
        }

        _saleOrderDetailRepository.SaveChanges();

        Dictionary<string, Inventory> inventories;
        var context = _inventoryRepository.DbContext as SaleContext;

        var inventorys = from orderDetail in OrderDetailBeanList
            join inventory in context.Inventories
                on new
                {
                    orderDetail.Uuid,
                    ExpY = string.Format("{0:yyyy}", orderDetail.expDate),
                    ExpM = string.Format("{0:MM}", orderDetail.expDate),
                    ExpD = string.Format("{0:dd}", orderDetail.expDate)
                }
                equals new
                {
                    inventory.Uuid,
                    ExpY = string.Format("{0:0000}", inventory.ExpY),
                    ExpM = string.Format("{0:00}", inventory.ExpM),
                    ExpD = string.Format("{0:00}", inventory.ExpD)
                }
            select new Inventory
            {
                Uuid = inventory.Uuid, Qty = inventory.Qty, ExpY = inventory.ExpY, ExpM = inventory.ExpM,
                ExpD = inventory.ExpD
            };
        inventories = inventorys.ToDictionary(inv => GenerateUniqueKey(inv.Uuid, inv.ExpY, inv.ExpM, inv.ExpD),
            inventory => inventory);


        foreach (var orderDetail in OrderDetailBeanList)
        {
            if (string.IsNullOrWhiteSpace(orderDetail.Uuid))
            {
                continue;
            }

            SaleOrderDetail saleOrderDetail = new SaleOrderDetail();
            saleOrderDetail.UnitPrice = orderDetail.unitPrice;
            saleOrderDetail.Qty = orderDetail.qty;
            saleOrderDetail.ProductUuid = orderDetail.Uuid;
            saleOrderDetail.OrderNo = saleOrder.OrderNo;
            saleOrderDetail.ExpY = orderDetail.expDate.Year;
            saleOrderDetail.ExpM = orderDetail.expDate.Month;
            saleOrderDetail.ExpD = orderDetail.expDate.Day;
            string uniqueKey = GenerateUniqueKey(saleOrderDetail.ProductUuid, saleOrderDetail.ExpY,
                saleOrderDetail.ExpM, saleOrderDetail.ExpD);
            if (!inventories.ContainsKey(uniqueKey))
            {
                OnShowConfirmation("錯誤",
                    string.Format("第{0}筆 商品{1} 效期:{2:yyyy/MM/dd} 不存在", OrderDetailBeanList.IndexOf(orderDetail) + 1,
                        orderDetail.productName, orderDetail.expDate));
                return;
            }

            if (inventories[uniqueKey].Qty < orderDetail.qty)
            {
                OnShowConfirmation("錯誤",
                    string.Format("第{0}筆 商品{1} 效期:{2:yyyy/MM/dd} 數量不足 該效期最大數量為{3}",
                        OrderDetailBeanList.IndexOf(orderDetail) + 1, orderDetail.productName, orderDetail.expDate,
                        inventories[uniqueKey].Qty));
                return;
            }

            _saleOrderDetailRepository.Create(saleOrderDetail);
        }

        _saleOrderDetailRepository.SaveChanges();
        OnShowConfirmation("成功", string.Format("渠道訂單編號:{0}建立成功!", saleOrder.ChannelOrderNo));
        ClearData();
    }

    /// <summary>
    /// 產生庫存對照查詢Key
    /// </summary>
    /// <param name="uuid"></param>商品的唯一辨識碼
    /// <param name="year"></param>商品效期 年
    /// <param name="month"></param>商品效期 月
    /// <param name="day"></param>商品效期 日
    /// <returns>uuid-yyyy-MM-dd</returns>
    private string GenerateUniqueKey(string uuid, int? year, int? month, int? day)
    {
        return string.Format("{0}-{1:0000}-{2:00}-{3:00}", uuid, year, month, day);
    }

    private void ClearData()
    {
        OrderViewBean.channel = "1";
        OrderViewBean.purchaser = "";
        OrderViewBean.orderEstablishDate = DateTime.Today.ToString("yyyy/MM/dd");
        OrderViewBean.orderNo = "";
        OrderViewBean.purchaserPhone = "";
        OrderViewBean.deliveryFee = 0;
        OrderViewBean.purchaserFbId = "";
        OrderViewBean.deliveryFeeType = "0";
        OrderViewBean.NotifyPropertyChanged();
        base.ClearAll();
    }
}