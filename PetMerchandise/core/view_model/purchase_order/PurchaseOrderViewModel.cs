using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PetMerchandise.core.db.entity;
using PetMerchandise.core.view_model.misc;
using PetMerchandise.view.order_handler;

namespace PetMerchandise.core.view_model.purchase_order;

public class PurchaseOrderViewModel : BaseImportExportViewModel
{
    public string purchaseDate { set; get; } = DateTime.Today.ToString("yyyy/MM/dd"); //訂購單建立日期
    public ICommand ChangeDateCommand { get; private set; }
    
    public ICommand LoginCommand { get; private set; }
    
    public ICommand TransInCommand { get; private set; }

    private void QueryImportDate()
    {
        OrderDetailBeanList.Clear();
        DateTime dateTime = DateTime.Parse(purchaseDate);
        var context = ProductRepository.DbContext as SaleContext;
        var result = from po in context.PurchaseOrders
            join p in context.Products on po.ProductUuid equals p.Uuid
            where po.PurchaseYear == dateTime.Year && po.PurchaseMonth == dateTime.Month &&
                  po.PurchaseDay == dateTime.Day
            select new OrderDetailViewBean
            {
                Uuid = po.ProductUuid,
                Status = po.TransInStatus,
                StatusText = po.TransInStatus == -2 ? "未登記" : po.TransInStatus == -1 ? "未入庫" : "已入庫",
                Enable = po.TransInStatus != 0,
                ean13 = p.Ean13,
                expDate = DateTime.Parse(string.Format("{0:0000}/{1:00}/{2:00}", po.Year, po.Month, po.Day)),
                qty = po.Qty.Value,
                groupType1 = p.TypeGroup1,
                groupType2 = p.TypeGroup2,
                brand = p.Brand,
                productName = p.Name,
                productSubName = p.SubName,
                package = p.Package
            };
        List<OrderDetailViewBean> orderDetails = result.ToList();

        foreach (var orderDetail in orderDetails)
        {
            orderDetail.brandNameSource.Add(orderDetail.brand);
            orderDetail.productNameSource.Add(orderDetail.productName);
            orderDetail.productSubNameSource.Add(orderDetail.productSubName);
            orderDetail.packageSource.Add(orderDetail.package);
            orderDetail.uuidSource.Add(orderDetail.Uuid);

            OrderDetailBeanList.Add(orderDetail);
        }
    }

    /// <summary>
    /// 將入庫商品先登記在採購單中，並檢查同日&&同商品&&同效期之商品 不可以重複
    /// </summary>
    private void Login()
    {
        // 檢查是否有 相同商品 / 相同效期 登記兩筆現象
        HashSet<string> uniqueSet = new();
        foreach (var orderDetailBean in OrderDetailBeanList)
        {
            string key = GenerateImportKey(orderDetailBean.Uuid, orderDetailBean.expDate.Year,
                orderDetailBean.expDate.Month, orderDetailBean.expDate.Day);
            if (uniqueSet.Contains(key))
            {
                OnShowConfirmation("錯誤",
                    string.Format("商品:{0}-{1}, 效期:{2:0000}/{3:00}/{4:00} 重複登記", orderDetailBean.productName,
                        orderDetailBean.productSubName, orderDetailBean.expDate.Year, orderDetailBean.expDate.Month,
                        orderDetailBean.expDate.Day));
                return;
            }

            uniqueSet.Add(key);
        }
        DateTime dateTime = DateTime.Parse(purchaseDate);
        MySqlParameter[] @parameters =
        {
            new("@PURCHASE_YEAR", dateTime.Year),
            new("@PURCHASE_MONTH", dateTime.Month),
            new("@PURCHASE_DAY", dateTime.Day),
        };
        _purchaseOrderRepository.DbContext.Database.ExecuteSqlRaw(
            "call CLEAR_LOG_IN_PURCHASE_ORDER(@PURCHASE_YEAR, @PURCHASE_MONTH, @PURCHASE_DAY)", parameters);
        foreach (var orderDetailBean in OrderDetailBeanList)
        {
            // 若已入庫則跳過
            if (orderDetailBean.Status == 0)
            {
                continue;
            }

            PurchaseOrder purchaseOrder = new PurchaseOrder();
            purchaseOrder.TransInStatus = -1;
            purchaseOrder.ProductUuid = orderDetailBean.Uuid;
            DateTime purchaseDateTime = DateTime.Parse(purchaseDate);
            purchaseOrder.PurchaseYear = purchaseDateTime.Year;
            purchaseOrder.PurchaseMonth = purchaseDateTime.Month;
            purchaseOrder.PurchaseDay = purchaseDateTime.Day;
            purchaseOrder.Qty = orderDetailBean.qty;
            purchaseOrder.Year = orderDetailBean.expDate.Year;
            purchaseOrder.Month = orderDetailBean.expDate.Month;
            purchaseOrder.Day = orderDetailBean.expDate.Day;
            _purchaseOrderRepository.Create(purchaseOrder);
        }

        _purchaseOrderRepository.SaveChanges();
        QueryImportDate();
    }

    private void TransIn()
    {
        DateTime dateTime = DateTime.Parse(purchaseDate);
        MySqlParameter[] @parameters =
        {
            new("@PURCHASE_YEAR", dateTime.Year),
            new("@PURCHASE_MONTH", dateTime.Month),
            new("@PURCHASE_DAY", dateTime.Day),
        };
        _purchaseOrderRepository.DbContext.Database.ExecuteSqlRaw("call PURCHASE_TRANS_IN(@PURCHASE_YEAR, @PURCHASE_MONTH, @PURCHASE_DAY)", @parameters);
        QueryImportDate();
    }

    private string GenerateImportKey(string uuid, int year, int month, int day)
    {
        return string.Format("{0}-{1:0000}/{2:00}/{3:00}", uuid, year, month, day);
    }

    protected override void InitialCommand()
    {
        base.InitialCommand();
        ChangeDateCommand = new RelayCommand<object>(_ => QueryImportDate());
        LoginCommand = new RelayCommand<object>(_ => Login());
        TransInCommand = new RelayCommand<object>(_ => TransIn());
    }

    protected override void ClearAll()
    {
        purchaseDate = DateTime.Today.ToString("yyyy/MM/dd"); //訂購單建立日期
        base.ClearAll();
        OnPropertyChanged(nameof(purchaseDate));
        QueryImportDate();
    }
}