using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PetMerchandise.core.db;
using PetMerchandise.core.db.entity;
using PetMerchandise.core.db.repository;
using PetMerchandise.core.view_model.misc;
using PetMerchandise.view.bean.order_handler;
using PetMerchandise.view.bean.product_handler;

namespace PetMerchandise.core.view_model;

public class BaseImportExportViewModel : BaseViewModel
{
    public ObservableCollection<OrderDetailViewBean> OrderDetailBeanList { get; set; }

    protected EFGenericRepository<Product> _productRepository { get; set; }
    protected EFGenericRepository<SaleOrder> _saleOrderRepository { get; set; }
    protected EFGenericRepository<SaleOrderDetail> _saleOrderDetailRepository { get; set; }
    protected EFGenericRepository<BlackList?> _blackListRepository { get; set; }
    protected EFGenericRepository<Inventory> _inventoryRepository { get; set; }

    protected EFGenericRepository<PurchaseOrder> _purchaseOrderRepository { get; set; }

    protected EFGenericRepository<Product> ProductRepository => _productRepository;

    public ICommand AddDetailCommand { get; set; }
    public ICommand QueryProductCommand { get; set; }
    public ICommand QueryProductNameCommand { get; set; }
    public ICommand QueryUuidCommand { get; set; }
    public ICommand ClearAllCommand { get; set; }
    public ICommand DeleteDetailCommand { get; set; }
    public ICommand QueryBrandNameCommand { get; set; }
    public ICommand QuerySubNameCommand { get; set; }
    public ICommand QueryPackageCommand { get; set; }
    public ObservableHashSet<string> groupType1Source { get; set; } = new(); //此欄位為固定不變 由進入時直接取得類型之列表

    public BaseImportExportViewModel()
    {
        InitialRepository();
        InitialElement();
        InitialCommand();
        InitialParameter();
    }

    /// <summary>
    /// 初始化執行初期會使用到之參數以及通知請求
    /// </summary>
    private void InitialParameter()
    {
        List<Product> products = _productRepository.Reads().ToList();
        groupType1Source.Add(" ");
        foreach (var product in products)
        {
            if (!string.IsNullOrWhiteSpace(product.TypeGroup1))
            {
                groupType1Source.Add(product.TypeGroup1);
            }
        }
    }

    /// <summary>
    /// 初始化所需使用之元素   
    /// </summary>
    protected virtual void InitialElement()
    {
        OrderDetailBeanList = new();
    }

    /// <summary>
    /// 建立一個與Database之連線，並根據需要聯繫到相應資料表
    /// </summary>
    private void InitialRepository()
    {
        var context = ContextManager.GetInstance();
        _productRepository = new EFGenericRepository<Product>(context);
        _blackListRepository = new EFGenericRepository<BlackList?>(context);
        _saleOrderRepository = new EFGenericRepository<SaleOrder>(context);
        _saleOrderDetailRepository = new EFGenericRepository<SaleOrderDetail>(context);
        _inventoryRepository = new EFGenericRepository<Inventory>(context);
        _purchaseOrderRepository = new(context);
    }

    /// <summary>
    /// 初始化與UI層可能會使用到之命令
    /// </summary>
    protected virtual void InitialCommand()
    {
        AddDetailCommand = new RelayCommand<object>(_ => { AddItem(); });
        QueryProductCommand = new RelayCommand<OrderDetailViewBean>(Query);
        QueryBrandNameCommand = new RelayCommand<OrderDetailViewBean>(QueryBrand);
        QueryProductNameCommand = new RelayCommand<OrderDetailViewBean>(QueryProductName);
        QueryUuidCommand = new RelayCommand<OrderDetailViewBean>(QueryUuid);
        QuerySubNameCommand = new RelayCommand<OrderDetailViewBean>(QuerySubName);
        QueryPackageCommand = new RelayCommand<OrderDetailViewBean>(QueryPackage);
        DeleteDetailCommand = new RelayCommand<OrderDetailViewBean>(RemoveItem);
        ClearAllCommand = new RelayCommand<object>(_ => { ClearAll(); });
    }

    /// <summary>
    /// 新增商品資訊
    /// </summary>
    private void AddItem()
    {
        OrderDetailBeanList.Add(new OrderDetailViewBean());
    }

    /// <summary>
    /// 移除特定商品資料
    /// </summary>
    /// <param name="orderDetailViewBean">特定商品資訊</param>
    private void RemoveItem(OrderDetailViewBean orderDetailViewBean)
    {
        OrderDetailBeanList.Remove(orderDetailViewBean);
    }

    protected virtual void ClearAll()
    {
        OrderDetailBeanList.Clear();
    }

    /// <summary>
    /// 根據EAN13輸入後 查詢Uuid及相關資料
    /// </summary>
    /// <param name="orderDetailViewBean"></param>
    protected void Query(OrderDetailViewBean orderDetailViewBean)
    {
        if (null == orderDetailViewBean || string.IsNullOrEmpty(orderDetailViewBean.ean13))
        {
            return;
        }

        // 若有填入EAN13 則直接查找結果
        Product result = _productRepository.Read(predicate: f => f.Ean13 == orderDetailViewBean.ean13);
        if (null == result)
        {
            orderDetailViewBean.productNameSource = new();
            orderDetailViewBean.productSubNameSource = new();
            orderDetailViewBean.packageSource = new();
            orderDetailViewBean.uuidSource = new();
            orderDetailViewBean.brand = null;
            orderDetailViewBean.groupType1 = null;
            orderDetailViewBean.groupType2 = null;
            orderDetailViewBean.package = null;
            orderDetailViewBean.productName = null;
            orderDetailViewBean.productSubName = null;
            orderDetailViewBean.Uuid = null;
            orderDetailViewBean.resetNeed = true;
        }
        else
        {
            orderDetailViewBean.brand = result.Brand;
            orderDetailViewBean.groupType1 = result.TypeGroup1;
            orderDetailViewBean.groupType2 = result.TypeGroup2;
            orderDetailViewBean.productName = result.Name;
            orderDetailViewBean.package = result.Package;
            orderDetailViewBean.productSubName = result.SubName;
            orderDetailViewBean.Uuid = result.Uuid;
            orderDetailViewBean.resetNeed = false;
        }

        orderDetailViewBean.NotifyPropertyChanged();
    }

    protected void QueryBrand(OrderDetailViewBean orderDetailViewBean)
    {
        orderDetailViewBean.brandNameSource = new();
        orderDetailViewBean.productNameSource = new();
        orderDetailViewBean.productSubNameSource = new();
        orderDetailViewBean.packageSource = new();
        orderDetailViewBean.uuidSource = new();
        if (orderDetailViewBean.resetNeed)
        {
            orderDetailViewBean.brand = null;
            orderDetailViewBean.productName = null;
            orderDetailViewBean.productSubName = null;
            orderDetailViewBean.package = null;
            orderDetailViewBean.Uuid = null;
        }

        var result = from p in (_productRepository.DbContext as SaleContext).Products
            where p.TypeGroup1 == orderDetailViewBean.groupType1
            group p by p.Brand
            into pTemp
            select pTemp.Key;

        orderDetailViewBean.brandNameSource.Add(" ");
        foreach (var brandName in result.ToHashSet())
        {
            if (string.IsNullOrWhiteSpace(brandName))
            {
                continue;
            }

            orderDetailViewBean.brandNameSource.Add(brandName);
        }

        orderDetailViewBean.NotifyPropertyChanged();
    }

    protected void QueryProductName(OrderDetailViewBean orderDetailViewBean)
    {
        orderDetailViewBean.productNameSource = new();
        orderDetailViewBean.productSubNameSource = new();
        orderDetailViewBean.packageSource = new();
        orderDetailViewBean.uuidSource = new();
        if (orderDetailViewBean.resetNeed)
        {
            orderDetailViewBean.productName = null;
            orderDetailViewBean.productSubName = null;
            orderDetailViewBean.package = null;
            orderDetailViewBean.Uuid = null;
        }

        var result = from p in (_productRepository.DbContext as SaleContext).Products
            where p.TypeGroup1 == orderDetailViewBean.groupType1 &&
                  (string.IsNullOrWhiteSpace(orderDetailViewBean.brand)
                      ? true
                      : p.Brand == orderDetailViewBean.brand)
            group p by p.Name
            into pTemp
            select pTemp.Key;

        // orderDetailViewBean.productNameSource.Add(" ");
        HashSet<string> productNameContent = result.ToHashSet();
        foreach (var productName in result.ToHashSet())
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                continue;
            }

            orderDetailViewBean.productNameSource.Add(productName);
        }

        if (productNameContent.Count == 1)
        {
            orderDetailViewBean.productName = productNameContent.Single();
            QuerySubName(orderDetailViewBean);
            return;
        }

        orderDetailViewBean.NotifyPropertyChanged();
    }

    /// <summary>
    /// 根據所選類別、品牌、產品名稱 進行產品次品名的重構
    /// </summary>
    /// <param name="orderDetailViewBean"></param>
    protected void QuerySubName(OrderDetailViewBean orderDetailViewBean)
    {
        orderDetailViewBean.productSubNameSource = new();
        orderDetailViewBean.packageSource = new();
        if (orderDetailViewBean.resetNeed)
        {
            orderDetailViewBean.productSubName = null;
            orderDetailViewBean.package = null;
            orderDetailViewBean.Uuid = null;
        }

        var result = from p in (_productRepository.DbContext as SaleContext).Products
            where p.TypeGroup1 == orderDetailViewBean.groupType1
                  && (string.IsNullOrWhiteSpace(orderDetailViewBean.brand)
                      ? true
                      : p.Brand == orderDetailViewBean.brand)
                  && (string.IsNullOrWhiteSpace(orderDetailViewBean.productName)
                      ? true
                      : p.Name == orderDetailViewBean.productName)
            group p by p.SubName
            into pTemp
            select pTemp.Key;

        // orderDetailViewBean.productSubNameSource.Add(" ");
        HashSet<string> subNameContent = result.ToHashSet();
        foreach (var content in subNameContent)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                continue;
            }

            orderDetailViewBean.productSubNameSource.Add(content);
        }

        if (subNameContent.Count == 1)
        {
            orderDetailViewBean.productSubName = subNameContent.Single();
            QueryPackage(orderDetailViewBean);
            return;
        }

        orderDetailViewBean.NotifyPropertyChanged();
    }

    protected void QueryPackage(OrderDetailViewBean orderDetailViewBean)
    {
        orderDetailViewBean.packageSource = new();
        orderDetailViewBean.uuidSource = new();
        if (orderDetailViewBean.resetNeed)
        {
            orderDetailViewBean.package = null;
            orderDetailViewBean.Uuid = null;
        }

        var result = from p in (_productRepository.DbContext as SaleContext).Products
            where p.TypeGroup1 == orderDetailViewBean.groupType1
                  && (string.IsNullOrWhiteSpace(orderDetailViewBean.brand)
                      ? true
                      : p.Brand == orderDetailViewBean.brand)
                  && (string.IsNullOrWhiteSpace(orderDetailViewBean.productName)
                      ? true
                      : p.Name == orderDetailViewBean.productName)
                  && (
                      string.IsNullOrWhiteSpace(orderDetailViewBean.productSubName)
                          ? true
                          : p.SubName == orderDetailViewBean.productSubName)
            select p.Package;
        HashSet<string> packageContent = result.ToHashSet();
        foreach (var content in packageContent)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                continue;
            }

            orderDetailViewBean.packageSource.Add(content);
        }

        if (packageContent.Count == 1)
        {
            orderDetailViewBean.package = packageContent.Single();
            QueryUuid(orderDetailViewBean);
            return;
        }

        orderDetailViewBean.NotifyPropertyChanged();
    }

    /// <summary>
    /// 根據商品的名稱、副名稱、包裝 查詢其唯一辨識碼
    /// </summary>
    /// <param name="orderDetailViewBean"></param>
    protected void QueryUuid(OrderDetailViewBean orderDetailViewBean)
    {
        orderDetailViewBean.uuidSource = new();
        if (orderDetailViewBean.resetNeed)
        {
            orderDetailViewBean.Uuid = null;
        }

        Expression<Func<Product, bool>> typeGroup1 = p => p.TypeGroup1 == orderDetailViewBean.groupType1; //類別一
        Expression<Func<Product, bool>> brand = p => p.Brand == orderDetailViewBean.brand; //品牌
        Expression<Func<Product, bool>> productName = p => p.Name == orderDetailViewBean.productName; //商品名稱
        Expression<Func<Product, bool>> productSubName = p => p.SubName == orderDetailViewBean.productSubName; //商品次品名
        Expression<Func<Product, bool>> package = p => p.Package == orderDetailViewBean.package; //包裝

        IQueryable<Product> queryable = _productRepository.Reads();
        queryable = queryable.Where(typeGroup1);

        if (!string.IsNullOrWhiteSpace(orderDetailViewBean.brand))
        {
            queryable = queryable.Where(brand);
        }

        if (!string.IsNullOrWhiteSpace(orderDetailViewBean.productName))
        {
            queryable = queryable.Where(productName);
        }

        if (!string.IsNullOrWhiteSpace(orderDetailViewBean.productSubName))
        {
            queryable = queryable.Where(productSubName);
        }

        if (!string.IsNullOrWhiteSpace(orderDetailViewBean.package))
        {
            queryable = queryable.Where(package);
        }

        List<Product> products = queryable.ToList();
        foreach (var product in products)
        {
            orderDetailViewBean.uuidSource.Add(product.Uuid);
        }

        if (products.Count == 1)
        {
            orderDetailViewBean.Uuid = products[0].Uuid;
        }

        orderDetailViewBean.NotifyPropertyChanged();
    }
}