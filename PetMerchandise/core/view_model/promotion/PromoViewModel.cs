using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
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
        StringBuilder consoleSb = new();
        DateTime today = DateTime.Today;
        sb.Append(string.Format("⏰{0:0000}/{1:00}/{2:00}修正商品品項及庫存⏰\n\n", today.Year, today.Month, today.Day));
        sb.Append("🔄本貼文 不定期更新 🔄\n\n");
        List<PromoTempEntity> resultCollection = GeneratePromotionText(PromotionBean.isBySaleName);


        int count = 1;
        Dictionary<string, string> sourceDict = new();
        Dictionary<string, bool> targetDict = new(); //將所有已輸出檔案路徑盡數加入，若有重複則不輸出，若最終結果為false則刪除

        foreach (string typeGroup1 in Directory.GetDirectories(PromotionBean.imageOutputPath))
        {
            foreach (var filePath in Directory.GetFiles(typeGroup1))
            {
                // 如果使用者選擇的類別不為空，且檔案路徑不包含使用者選擇的類別，則跳過　（避免誤刪其他圖片）
                if (!string.IsNullOrWhiteSpace(PromotionBean.groupType1) &&
                    !filePath.Contains(PromotionBean.groupType1))
                {
                    continue;
                }

                targetDict.Add(filePath, false);
            }
        }

        if (!Directory.Exists(PromotionBean.imageSourcePath))
        {
            consoleSb.Append("商品來源資料夾不存在");
        }
        else
        {
            string[] filesPath = Directory.GetFiles(PromotionBean.imageSourcePath);
            foreach (string file in filesPath)
            {
                int dotIndex = file.IndexOf('.');
                int lastSlashIndex = file.LastIndexOf('\\') + 1;
                string referUuid = file.Substring(lastSlashIndex, dotIndex - lastSlashIndex);
                if (sourceDict.ContainsKey(referUuid))
                {
                    continue; //避免相同檔案名稱 不同副檔名造成錯誤
                }

                sourceDict.Add(referUuid, file);
            }
        }

        foreach (var promotion in resultCollection)
        {
            sb.Append(string.Format(
                "\uD83D\uDCA8{0:00}. {1} \uD83C\uDE36{2}\uD83D\uDCE6 \uD83D\uDCB2{3}/包 效期⌛ {4:0000}/{5:00}/{6:00}\r\n\n",
                count++, promotion.SaleName, promotion.SaleSum,
                promotion.SalePrice.Value.ToString().TrimEnd('0').TrimEnd('.'), promotion.expY, promotion.expM,
                promotion.expD));
            consoleSb.Append(CompressImage(promotion.uuid, promotion.SaleName, promotion.Brand, promotion.typeGroup1,
                sourceDict, targetDict));
        }

        sb.Append("賣貨便\uD83D\uDE9A 運費35元自出\n\n");
        sb.Append("萊賣貨\uD83D\uDE9A 運費23元自出\n\n");
        sb.Append("高雄小港可自取\uD83D\uDEF5\n\n");
        sb.Append("所有商品均為現貨 - 賣貨便下單後24小時內\uD83D\uDD50即會出貨\n\n");
        sb.Append("有任何問題請私訊詢問 會於19:00~22:00回覆\n\n");
        sb.Append("商品價格以文字敘述為主\n\n");
        sb.Append("於寄件達三日未取貨 會發送FB訊息提醒！　屆時再煩請注意一下訊息唷！");
        PromotionBean.PromotionText = sb.ToString();
        PromotionBean.ConsoleText = consoleSb.ToString();
        // 若無須存在之檔案則刪除
        foreach (var filePath in targetDict.Keys)
        {
            if (!targetDict[filePath])
            {
                File.Delete(filePath);
            }
        }

        PromotionBean.NotifyPropertyChanged();
    }

    public List<PromoTempEntity> GeneratePromotionText(bool bySaleName)
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
                cmd.CommandText = bySaleName ? "CALL_PROMOTION_BY_SALE_NAME" : "CALL_PROMOTION_BY_UUID";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IS_GENKI", PromotionBean.isGenki ? 1 : 0);
                cmd.Parameters.AddWithValue("@IS_WANMIAO", PromotionBean.isWanMeow ? 1 : 0);
                cmd.Parameters.AddWithValue("@IS_ALL", PromotionBean.isAll ? 1 : 0);
                cmd.Parameters.AddWithValue("@TYPE_GROUP_1",
                    string.IsNullOrWhiteSpace(PromotionBean.groupType1) ? null : PromotionBean.groupType1);
                cmd.Parameters.AddWithValue("@TYPE_GROUP_2",
                    string.IsNullOrWhiteSpace(PromotionBean.groupType2) ? null : PromotionBean.groupType2);
                cmd.Parameters.AddWithValue("@BRAND",
                    string.IsNullOrWhiteSpace(PromotionBean.brand) ? null : PromotionBean.brand);

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

    // Compress Image from imageSourcePath to imageTargetPath 
    // Max width is 600 and Max height is 800
    // If image is smaller than 600*800, it will not compress
    // If image width or height is larger than max width or height, it will compress keep ratio
    private string CompressImage(String fileName, string saleName, string brand, string groupType1,
        Dictionary<string, string> sourceDict, Dictionary<string, bool> targetDict)
    {
        if (!Directory.Exists(PromotionBean.imageSourcePath))
        {
            return "";
        }

        if (!sourceDict.ContainsKey(fileName))
        {
            return string.Format("{0} {1} {2} 沒有圖片\r\n", saleName, brand, fileName);
        }

        String sourcePath = sourceDict[fileName];

        if (!Directory.Exists(string.Format("{0}\\{1}", PromotionBean.imageOutputPath, groupType1)))
        {
            Directory.CreateDirectory(string.Format("{0}\\{1}", PromotionBean.imageOutputPath, groupType1));
        }

        string outputFilePath = string.Format("{0}\\{1}\\{2}.jpg", PromotionBean.imageOutputPath, groupType1, fileName);
        if (File.Exists(outputFilePath))
        {
            targetDict[outputFilePath] = true;
            return "";
        }

        var maxWidth = 600;
        var maxHeight = 800;
        var image = Image.FromFile(sourcePath);
        var ratioX = (double)maxWidth / image.Width;
        var ratioY = (double)maxHeight / image.Height;
        var ratio = Math.Min(ratioX, ratioY);
        var newWidth = (int)(image.Width * ratio);
        var newHeight = (int)(image.Height * ratio);
        var newImage = new Bitmap(newWidth, newHeight);
        Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
        newImage.Save(outputFilePath);
        return "";
    }
}