using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PetMerchandise.core.db.entity;
using PetMerchandise.core.db.repository;
using PetMerchandise.core.view_model.misc;
using PetMerchandise.view.bean.product_handler;

namespace PetMerchandise.core.view_model.product;

public class ProductViewModel : BaseViewModel
{
    private const int PAGE_COUNT = 20; //每頁二十筆

    public int _currentPage { get; set; } = 1; //當前頁數

    public int _totalPage { get; set; } = 1; //總頁數

    private EFGenericRepository<Product> _productRepository;
    public ProductBean ProductBean { get; } = new();

    public ObservableCollection<Product> ProductList { get; } = new();

    public ICommand ChannelGenkiCheckCommand { get; set; }

    public ICommand ChannelGenkiUnCheckCommand { get; set; }

    public ICommand ChannelWanMeowCheckCommand { get; set; }

    public ICommand ChannelWanMeowUnCheckCommand { get; set; }

    public ICommand SaveUpdateCommand { get; set; }

    public ICommand SaveChangeCommand { get; set; }

    public ICommand TurnPageDownCommand { get; private set; }

    public ICommand TurnPageUpCommand { get; private set; }

    public ProductViewModel()
    {
        InitialRepository();
        InitialCommand();
        InitialElement();
    }

    private void InitialElement()
    {
        QueryProducts();
    }

    private void QueryProducts()
    {
        ProductList.Clear();
        _totalPage = (_productRepository.Reads().Count() / PAGE_COUNT) + 1;
        ProductList.AddRange(_productRepository.Reads().Skip(PAGE_COUNT * (_currentPage - 1)).Take(PAGE_COUNT)
            .ToList());
        NotifyPageParameterChanged();
    }

    private void InitialRepository()
    {
        DbContext dbContext = new SaleContext();
        _productRepository = new EFGenericRepository<Product>(dbContext);
    }

    private void InitialCommand()
    {
        ChannelGenkiCheckCommand = new RelayCommand<ProductBean>(p => p.ChannelGenki = true);
        ChannelGenkiUnCheckCommand = new RelayCommand<ProductBean>(p => p.ChannelGenki = false);
        ChannelWanMeowCheckCommand = new RelayCommand<ProductBean>(p => p.ChannelWanmiao = true);
        ChannelWanMeowUnCheckCommand = new RelayCommand<ProductBean>(p => p.ChannelWanmiao = false);
        SaveChangeCommand = new RelayCommand<object>(p => SaveChange());
        SaveUpdateCommand = new RelayCommand<object>(p => UpdateChange());
        TurnPageDownCommand = new RelayCommand<object>(_ =>
        {
            if (_currentPage >= _totalPage)
                return;
            _currentPage++;
            QueryProducts();
        });
        TurnPageUpCommand = new RelayCommand<object>(_ =>
        {
            if (_currentPage <= 1)
                return;
            _currentPage--;
            QueryProducts();
        });
    }

    private void SaveChange()
    {
        if (string.IsNullOrWhiteSpace(ProductBean.Name))
        {
            OnShowConfirmation("錯誤", "請輸入商品名稱");
            return;
        }

        Product product = new();
        product.Uuid = Guid.NewGuid().ToString().Replace("-", "");
        product.Animal = ProductBean.Animal;
        product.Brand = ProductBean.Brand;
        product.Cost = ProductBean.Cost;
        product.Ean13 = ProductBean.Ean13;
        product.Manufacture = ProductBean.Manufacture;
        product.Name = ProductBean.Name;
        product.SubName = ProductBean.SubName;
        product.Package = ProductBean.Package;
        product.AgeGroup = ProductBean.AgeGroup;
        product.ChannelGenki = ProductBean.ChannelGenki;
        product.ChannelWanmiao = ProductBean.ChannelWanmiao;
        product.CostLowest = ProductBean.CostLowest;
        product.SaleName = ProductBean.SaleName;
        product.SalePrice = ProductBean.SalePrice;
        product.TypeGroup1 = ProductBean.TypeGroup1;
        product.TypeGroup2 = ProductBean.TypeGroup2;
        _productRepository.Create(product);
        _productRepository.SaveChanges();
        OnShowConfirmation("成功", string.Format("成功新增商品{0} {1}", ProductBean.Name, ProductBean.SubName));
        ProductBean.Clear();
    }

    private void UpdateChange()
    {
        foreach (var product in ProductList)
        {
            Expression<Func<Product, object>>[]? updateProperties =
            {
                p => p.Name, p => p.SubName, p => p.Manufacture, p => p.Brand, p => p.Package, p => p.SalePrice,
                p => p.Animal, p => p.AgeGroup,
                p => p.TypeGroup1, p => p.TypeGroup2, p => p.Ean13, p => p.Cost, p => p.SaleName, p => p.CostLowest,
                p => p.ChannelGenki, p => p.ChannelWanmiao
            };
            _productRepository.Update(product, updateProperties);
            _productRepository.SaveChanges();
        }

        QueryProducts();
    }


    private void NotifyPageParameterChanged()
    {
        OnPropertyChanged(nameof(_totalPage));
        OnPropertyChanged(nameof(_currentPage));
    }
}