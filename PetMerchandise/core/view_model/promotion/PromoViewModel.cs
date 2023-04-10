using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PetMerchandise.core.db;
using PetMerchandise.core.db.entity;
using PetMerchandise.core.db.repository;
using PetMerchandise.core.db.temp_entity;
using PetMerchandise.core.view_model.misc;
using PetMerchandise.view.bean.promo_handler;

namespace PetMerchandise.core.view_model.promotion;

/// <summary>
/// 本類別用於處理 推廣 / 貼文 之ViewModel
/// </summary>
public class PromoViewModel : BaseViewModel
{
    private EFGenericRepository<Product> _productRepository;
    public PromotionBean PromotionBean { get; private set; }

    public ICommand GeneratePromoCommand { get; private set; }

    public PromoViewModel()
    {
        InitialRepository();
        InitialParameter();
        InitialCommand();
    }

    private void InitialCommand()
    {
        GeneratePromoCommand = new RelayCommand<object>(_ => { GeneratePromotionText(); });
    }

    private void GeneratePromotionText()
    {
        StringBuilder sb = new();
        DateTime today = DateTime.Today;
        sb.Append(string.Format("⏰{0:0000}/{1:00}/{2:00}修正商品品項及庫存⏰\n\n", today.Year, today.Month, today.Day));
        sb.Append("🔄本貼文 不定期更新 🔄\n\n");

        var inventory = from inv in ContextManager.GetInstance().Inventories
            where inv.Qty != 0
            orderby inv.ExpY, inv.ExpM, inv.ExpD
            group inv by new { inv.Uuid, inv.ExpY, inv.ExpM, inv.ExpD }
            into gInv
            select new
            {
                UUID = gInv.Key.Uuid,
                EXP_Y = gInv.First().ExpY,
                EXP_M = gInv.First().ExpM,
                EXP_D = gInv.First().ExpD,
                SALE_SUM = gInv.Sum(_ => _.Qty)
            };

        List<PromoTempEntity> resultCollection = null;
        if (PromotionBean.isBySaleName)
        {
            resultCollection = GeneratePromotionText(0);
        }
        else
        {
            IQueryable<PromoTempEntity> queryable = from p in ContextManager.GetInstance().Products
                join inv in inventory
                    on p.Uuid equals inv.UUID
                where (string.IsNullOrWhiteSpace(PromotionBean.brand) || p.Brand == PromotionBean.brand) &&
                      (string.IsNullOrWhiteSpace(PromotionBean.groupType1) ||
                       p.TypeGroup1 == PromotionBean.groupType1) &&
                      (string.IsNullOrWhiteSpace(PromotionBean.groupType2) ||
                       p.TypeGroup2 == PromotionBean.groupType2) &&
                      ((PromotionBean.isGenki && p.ChannelGenki == true) ||
                       (PromotionBean.isWanMeow && p.ChannelWanmiao == true))
                group new { p, inv } by new { p.Uuid, p.Brand, p.Animal }
                into gPInv
                select new PromoTempEntity()
                {
                    SaleName = String.Concat(gPInv.Single().p.Name,
                        string.IsNullOrEmpty(gPInv.Single().p.SubName) ? "" : gPInv.Single().p.SubName),
                    SaleSum = gPInv.Sum(c => c.inv.SALE_SUM),
                    SalePrice = gPInv.Single().p.SalePrice,
                    expY = gPInv.Single().inv.EXP_Y,
                    expM = gPInv.Single().inv.EXP_M,
                    expD = gPInv.Single().inv.EXP_D,
                    typeGroup1 = gPInv.Single().p.TypeGroup1,
                    uuid = gPInv.Single().p.Uuid,
                    Brand = gPInv.Single().p.Brand,
                    Animal = gPInv.Single().p.Animal,
                };

            resultCollection = queryable.OrderBy(p => p.SaleName).ThenBy(k => k.Brand).ThenBy(j => j.Animal).ToList();
        }

        int count = 1;
        foreach (var promotion in resultCollection)
        {
            sb.Append(string.Format(
                "\uD83D\uDCA8{0:00}. {1} \uD83C\uDE36{2}\uD83D\uDCE6 \uD83D\uDCB2{3}/包 效期⌛ {4:0000}/{5:00}/{6:00}\r\n\n",
                count++, promotion.SaleName, promotion.SaleSum,
                promotion.SalePrice.Value.ToString().TrimEnd('0').TrimEnd('.'), promotion.expY, promotion.expM,
                promotion.expD));
        }

        sb.Append("賣貨便\uD83D\uDE9A 運費35元自出\n\n");
        sb.Append("萊賣貨\uD83D\uDE9A 運費29元自出\n\n");
        sb.Append("高雄小港可自取\uD83D\uDEF5\n\n");
        sb.Append("所有商品均為現貨 - 賣貨便下單後24小時內\uD83D\uDD50即會出貨\n\n");
        sb.Append("有任何問題請私訊詢問 會於19:00~22:00回覆\n\n");
        sb.Append("商品價格以文字敘述為主\n\n");
        sb.Append("於寄件達三日未取貨 會發送FB訊息提醒！　屆時再煩請注意一下訊息唷！");
        PromotionBean.PromotionText = sb.ToString();
        PromotionBean.NotifyPropertyChanged();
    }

    public List<PromoTempEntity> GeneratePromotionText(int i)
    {
        List<PromoTempEntity> promoTempEntities = new();
        using (MySqlConnection lconn =
               new MySqlConnection(string.Format("{0};{1}",
                   ContextManager.GetInstance().Database.GetDbConnection().ConnectionString, "Password=aa123456")))
        {
            lconn.Open();
            using (MySqlCommand cmd = new MySqlCommand())
            {
                cmd.Connection = lconn;
                cmd.CommandText = "CALL_PROMOTION_BY_SALE_NAME";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TYPE_GROUP_1", PromotionBean.groupType1);
                cmd.Parameters.AddWithValue("@TYPE_GROUP_2", PromotionBean.groupType2);
                cmd.Parameters.AddWithValue("@BRAND", PromotionBean.brand);

                cmd.Parameters.AddWithValue("@SALE_NAME", MySqlDbType.VarChar);
                cmd.Parameters["@SALE_NAME"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@TOTAL_SUM", MySqlDbType.Int32);
                cmd.Parameters["@TOTAL_SUM"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@SALE_PRICE", MySqlDbType.Decimal);
                cmd.Parameters["@SALE_PRICE"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@EXP_Y", MySqlDbType.Int32);
                cmd.Parameters["@EXP_Y"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@EXP_M", MySqlDbType.Int32);
                cmd.Parameters["@EXP_M"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@EXP_D", MySqlDbType.Int32);
                cmd.Parameters["@EXP_D"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@O_TYPE_GROUP_1", MySqlDbType.VarChar);
                cmd.Parameters["@O_TYPE_GROUP_1"].Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@UUID", MySqlDbType.VarChar);
                cmd.Parameters["@UUID"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@SUB_NAME", MySqlDbType.VarChar);
                cmd.Parameters["@SUB_NAME"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@O_BRAND", MySqlDbType.VarChar);
                cmd.Parameters["@O_BRAND"].Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@ANIMAL", MySqlDbType.VarChar);
                cmd.Parameters["@ANIMAL"].Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@O_TYPE_GROUP_2", MySqlDbType.VarChar);
                cmd.Parameters["O_TYPE_GROUP_2"].Direction = ParameterDirection.InputOutput;
                cmd.Parameters.AddWithValue("@ROW_ID", MySqlDbType.Int32);
                cmd.Parameters["ROW_ID"].Direction = ParameterDirection.Output;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        promoTempEntities.Add(new PromoTempEntity()
                        {
                            SaleName = Convert.ToString(reader["SALE_NAME"]),
                            SaleSum = Convert.ToInt32(reader["TOTAL_SUM"]),
                            SalePrice = Convert.ToDecimal(reader["SALE_PRICE"]),
                            expY = Convert.ToInt32(reader["EXP_Y"]),
                            expM = Convert.ToInt32(reader["EXP_M"]),
                            expD = Convert.ToInt32(reader["EXP_D"]),
                            typeGroup1 = Convert.ToString(reader["TYPE_GROUP_1"]),
                            uuid = Convert.ToString(reader["UUID"]),
                            Brand = Convert.ToString(reader["BRAND"]),
                            Animal = Convert.ToString(reader["ANIMAL"]),
                        });
                    }
                }
            }
        }

        return promoTempEntities;
    }

    private void InitialParameter()
    {
        PromotionBean = new();

        PromotionBean.BrandSource.Add(" ");
        PromotionBean.GroupType1Source.Add(" ");
        PromotionBean.GroupType2Source.Add(" ");
        var result = _productRepository.Reads().ToList();
        foreach (var product in result)
        {
            // 處理品牌名稱
            if (!string.IsNullOrWhiteSpace(product.Brand))
            {
                PromotionBean.BrandSource.Add(product.Brand);
            }

            // 處理類別一
            if (!string.IsNullOrWhiteSpace(product.TypeGroup1))
            {
                PromotionBean.GroupType1Source.Add(product.TypeGroup1);
            }

            // 處理類別二
            if (!string.IsNullOrWhiteSpace(product.TypeGroup2))
            {
                PromotionBean.GroupType2Source.Add(product.TypeGroup2);
            }
        }
    }

    private void InitialRepository()
    {
        _productRepository = new EFGenericRepository<Product>(ContextManager.GetInstance());
    }
}