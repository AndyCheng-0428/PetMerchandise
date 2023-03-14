namespace PetMerchandise.view.misc;

/// <summary>
/// 正規表達式
/// </summary>
public class RegexExpression
{
    public const string MOBILE_PHONE = @"^09[0-9＊*]{8}\z"; //09開頭 0-9以及*＊共8碼
    public const string FB_ID = @"^[0-9]+?\z"; //開頭 0-9長度從0~32碼不等
    
}