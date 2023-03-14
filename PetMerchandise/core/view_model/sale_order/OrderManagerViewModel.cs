using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PetMerchandise.core.db.entity;
using PetMerchandise.core.db.repository;
using PetMerchandise.core.view_model.misc;
using PetMerchandise.view.order_handler;

namespace PetMerchandise.core.view_model.sale_order;

public class OrderManagerViewModel : BaseViewModel
{
    private const int PAGE_COUNT = 20; //每頁二十筆

    public int _currentPage { get; set; } = 1; //當前頁數

    public int _totalPage { get; set; } = 1; //總頁數

    private EFGenericRepository<SaleOrder> _saleOrderRepository;

    private delegate void QueryHandler();

    private event QueryHandler _queryEvent;

    public ICommand AddItemToProcessListCommand { get; private set; }

    public ICommand RemoveItemFromProcessListCommand { get; private set; }

    public ICommand TransOutOrderCommand { get; private set; }

    public ICommand InitAllOrderCommand { get; private set; }

    public ICommand CompleteOrderCommand { get; private set; }

    public ICommand DiscardOrderCommand { get; private set; }

    public ICommand InitHandlingOrderCommand { get; private set; }

    public ICommand InitDeliveringOrderCommand { get; private set; }

    public ICommand InitCompletedOrderCommand { get; private set; }

    public ICommand InitDiscardedOrderCommand { get; private set; }

    public ICommand TurnPageDownCommand { get; private set; }

    public ICommand TurnPageUpCommand { get; private set; }

    public ObservableCollection<OrderStatusBean> OrderStatusBeanList { get; } = new();

    private readonly List<OrderStatusBean> _processList = new(); //待處理資料集合

    public OrderManagerViewModel()
    {
        InitialRepository();
        InitialCommand();
    }

    private void InitialRepository()
    {
        var context = new SaleContext();
        _saleOrderRepository = new EFGenericRepository<SaleOrder>(context);
    }

    private void InitialCommand()
    {
        AddItemToProcessListCommand = new RelayCommand<OrderStatusBean>(bean => AddItemToProcessList(bean));
        RemoveItemFromProcessListCommand = new RelayCommand<OrderStatusBean>(bean => RemoveItemFromProcessList(bean));
        TransOutOrderCommand = new RelayCommand<object>(_ => TransOutOrder()); //訂單寄出
        CompleteOrderCommand = new RelayCommand<object>(_ => CompleteOrder()); //買家已收貨
        DiscardOrderCommand = new RelayCommand<object>(_ => DiscardOrder()); //買家棄單
        TurnPageDownCommand = new RelayCommand<object>(_ =>
        {
            if (_currentPage < _totalPage)
                _currentPage++;
            else return;
            _queryEvent.Invoke();
        });
        TurnPageUpCommand = new RelayCommand<object>(_ =>
        {
            if (_currentPage > 1)
                _currentPage--;
            else return;
            _queryEvent.Invoke();
        });

        InitAllOrderCommand = new RelayCommand<object>(_ =>
        {
            _queryEvent += QueryAllOrder;
            _queryEvent.Invoke();
        });
        InitHandlingOrderCommand = new RelayCommand<object>(_ =>
        {
            _queryEvent += QueryHandlingOrder;
            _queryEvent.Invoke();
        });
        InitDeliveringOrderCommand = new RelayCommand<object>(_ =>
        {
            _queryEvent += QueryDeliveringOrder;
            _queryEvent.Invoke();
        });
        InitCompletedOrderCommand = new RelayCommand<object>(_ =>
        {
            _queryEvent += QueryCompletedOrder;
            _queryEvent.Invoke();
        });
        InitDiscardedOrderCommand = new RelayCommand<object>(_ =>
        {
            _queryEvent += QueryDiscardedOrder;
            _queryEvent.Invoke();
        });
    }

    /// <summary>
    /// 新增待處理項目至待處理資料集合
    /// </summary>
    /// <param name="bean">待處理項目</param>
    private void AddItemToProcessList(OrderStatusBean bean)
    {
        if (_processList.Contains(bean))
        {
            return;
        }

        _processList.Add(bean);
    }

    /// <summary>
    /// 從待處理資料集合移除待處理項目
    /// </summary>
    /// <param name="bean">待處理項目</param>
    private void RemoveItemFromProcessList(OrderStatusBean bean)
    {
        if (!_processList.Contains(bean))
        {
            return;
        }

        _processList.Remove(bean);
    }

    /// <summary>
    /// 訂單從處理中轉為出貨
    /// 此時會先檢查訂單狀態
    /// 檢查庫存是否足夠(在建立訂單時已經檢查過,此時作複檢查) -> 預防同時兩位買家買同一商品
    /// 若庫存充足，檢查無誤，則以Stored Procedure進行庫存異動、並更新訂單狀態
    /// </summary>
    private void TransOutOrder()
    {
        List<OrderStatusBean> filterList = new();
        foreach (var content in _processList)
        {
            if (content.OrderStatus != -1)
            {
                continue;
            }

            filterList.Add(content);
        }

        var saleContext = _saleOrderRepository.DbContext as SaleContext;
        // 首先, 取出本次異動訂單編號相對應的細項內容
        var first = from so in filterList
            join sod in saleContext.SaleOrderDetails on so.OrderNo equals sod.OrderNo
            select new SaleOrderDetail()
            {
                OrderNo = so.OrderNo, ProductUuid = sod.ProductUuid, ExpY = sod.ExpY, ExpM = sod.ExpM, ExpD = sod.ExpD,
                Qty = sod.Qty
            };
        // 將細項內容依照自己的商品代碼、效期分組後將數量相加 (避免大家都買同一個東西 而此東西可能有不同效期)
        var a = from sod in first
            group sod by new { sod.ProductUuid, sod.ExpY, sod.ExpM, sod.ExpD }
            into sod1
            select new
            {
                sod1.Key.ProductUuid, sod1.Key.ExpY, sod1.Key.ExpM, sod1.Key.ExpD, Qty = sod1.Sum(sod => sod.Qty)
            };

        var res = from x in a
            join inv in saleContext.Inventories on
                new
                {
                    UUID = x.ProductUuid, EXP_Y = x.ExpY.ToString(), EXP_M = x.ExpM.ToString(),
                    EXP_D = x.ExpD.ToString()
                } equals new
                {
                    UUID = inv.Uuid, EXP_Y = inv.ExpY.ToString(), EXP_M = inv.ExpM.ToString(),
                    EXP_D = inv.ExpD.ToString()
                }
            where inv.Qty < x.Qty
            select new
            {
                inv.Uuid, inv.ExpY, inv.ExpM, inv.ExpD, ConsumeQty = x.Qty, InventoryQty = inv.Qty
            };

        // 進行商品資料關聯 用以提醒當庫存不足時，哪個商品　效期　數量　與　庫存數量比對後數量不足
        var result = from k in res
            join p in saleContext.Products on k.Uuid.ToString() equals p.Uuid.ToString()
            select new
            {
                p.SaleName, p.Ean13, p.Uuid, k.ExpY, k.ExpD, k.ExpM, k.InventoryQty, k.ConsumeQty
            };
        var checkerList = result.ToList();

        if (checkerList.Count > 0)
        {
            // 商品數量不足集合
            StringBuilder sb = new();
            foreach (var checker in checkerList)
            {
                sb.Append(string.Format(
                    "{0}數量不足！商品條碼：{1}，商品唯一辨識碼：{2}，效期：{3:0000}/{4:00}/{5:00}，庫存：{6:000}，訂單所需:{7:000}\n",
                    checker.SaleName, checker.Ean13, checker.Uuid, checker.ExpY, checker.ExpM, checker.ExpD,
                    checker.InventoryQty, checker.ConsumeQty));
            }

            OnShowConfirmation("錯誤", sb.ToString());
            return;
        }

        // 商品數量檢查結束，確保所有商品數量充足，使用Stored Procedure執行單筆轉入
        // 此Stored Procedure執行以下步驟
        // 1. 再次檢查訂單狀態，若不為待處理則不予處理
        // 2. 檢查訂單細項，若根本沒有商品也不予處理
        // 3. 依照訂單細項，逐一檢查庫存，若庫存不足，則跳錯 錯誤代碼 10001
        // 4. 若無錯誤則進行庫存更新
        // 5. 若過程均無錯誤，則更新此訂單狀態，修正為出貨，並對此次交易進行Commit
        foreach (var content in filterList)
        {
            MySqlParameter[] @parameters =
            {
                new("@CHANNEL", content.OrderChannel),
                new("@CHANNEL_ORDER_NO", content.OrderChannelNo)
            };
            saleContext.Database.ExecuteSqlRaw("call SALE_TRANS_OUT(@CHANNEL, @CHANNEL_ORDER_NO)", @parameters);
        }

        _queryEvent.Invoke();
    }

    /// <summary>
    /// 訂單從出貨轉為棄單
    /// 此步驟無須檢查，其內部會自我檢查訂單狀態必須為出貨方會異動
    /// 執行此功能者，FB_ID會同步加入黑名單中
    /// </summary>
    private void DiscardOrder()
    {
        foreach (var content in _processList)
        {
            MySqlParameter[] @parameters =
            {
                new("@CHANNEL", content.OrderChannel),
                new("@CHANNEL_ORDER_NO", content.OrderChannelNo)
            };
            _saleOrderRepository.DbContext.Database.ExecuteSqlRaw(
                "call SALE_TRANS_DISCARD(@CHANNEL, @CHANNEL_ORDER_NO)", @parameters);
        }

        _queryEvent.Invoke();
    }

    /// <summary>
    /// 訂單從出貨轉為完成
    /// 此步驟無須檢查，其內部會自我檢查訂單狀態必須為出貨方會異動
    /// </summary>
    private void CompleteOrder()
    {
        foreach (var content in _processList)
        {
            MySqlParameter[] @parameters =
            {
                new("@CHANNEL", content.OrderChannel),
                new("@CHANNEL_ORDER_NO", content.OrderChannelNo)
            };
            _saleOrderRepository.DbContext.Database.ExecuteSqlRaw(
                "call SALE_TRANS_COMPLETED(@CHANNEL, @CHANNEL_ORDER_NO)", @parameters);
        }

        _queryEvent.Invoke();
    }

    /// <summary>
    /// 查詢所有訂單
    /// </summary>
    private void QueryAllOrder()
    {
        QueryOrderStatus(null);
    }

    private void QueryHandlingOrder()
    {
        QueryOrderStatus(-1);
    }

    private void QueryDeliveringOrder()
    {
        QueryOrderStatus(-2);
    }

    private void QueryCompletedOrder()
    {
        QueryOrderStatus(0);
    }

    private void QueryDiscardedOrder()
    {
        QueryOrderStatus(-99);
    }

    /// <summary>
    /// 查詢訂單主要方法
    /// </summary>
    /// <param name="status"></param>
    private void QueryOrderStatus(int? status)
    {
        InitialData();
        SaleContext context = (_saleOrderRepository.DbContext as SaleContext)!;

        var result = from so in context.SaleOrders
            join a in from sod in context.SaleOrderDetails
                group sod by sod.OrderNo
                into sodu
                select new
                {
                    sodu.Key, Total = sodu.Sum(sod => Convert.ToDecimal(sod.UnitPrice * sod.Qty))
                }
                on so.OrderNo equals a.Key
            orderby so.OrderY, so.OrderM, so.OrderD
            where so.OrderStatus == status || status == null
            select new OrderStatusBean
            {
                OrderNo = so.OrderNo,
                OrderDate = String.Format("{0:0000}/{1:00}/{2:00}", so.OrderY, so.OrderM, so.OrderD),
                OrderChannel = so.Channel,
                OrderStatus = so.OrderStatus,
                OrderChannelNo = so.ChannelOrderNo,
                PurchaserName = so.Purchaser,
                PurchaserFacebookId = so.PurchaserFbId,
                OrderDeliveryFee = so.DeliveryFee,
                OrderDeliveryFeeType = so.DeliveryFeeType.ToString(),
                OrderDeliveryFeeTypeName = so.DeliveryFeeType == 0 ? "含運" : "不含運",
                PurchaserPhoneNo = so.PurchaserPhone,
                OrderSaleSum = decimal.Ceiling(a.Total)
            };
        // if (status != null)
        // {
        //     result.Where(bean => bean.OrderStatus == status);
        // }

        int SkipCount = (_currentPage - 1) * PAGE_COUNT;
        _totalPage = (result.Count() / PAGE_COUNT) + 1;
        OrderStatusBeanList.AddRange(result.Skip(SkipCount).Take(PAGE_COUNT).ToList());
        NotifyPageParameterChanged();
    }

    private void InitialData()
    {
        OrderStatusBeanList.Clear();
        _processList.Clear();
    }

    private void NotifyPageParameterChanged()
    {
        OnPropertyChanged(nameof(_totalPage));
        OnPropertyChanged(nameof(_currentPage));
    }
}