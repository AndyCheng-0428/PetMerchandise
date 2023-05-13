using PetMerchandise.core.constant;

namespace PetMerchandise.view.bean.setting_handler;

public class PromotionSettingBean : BindingBean
{
    public string Url { get; set; } = string.Format("https://www.facebook.com/dialog/oauth?client_id={0}&scope={1}&display=popup&redirect_uri=http://www.facebook.com/connect/login_success.html&response_type=token", FacebookApplicationConstant.APP_ID, "email,user_birthday");


    public void NotifyPropertyChanged()
    {
        OnPropertyChanged(nameof(Url));
    }
}