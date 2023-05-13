using System;
using System.Windows.Input;
using PetMerchandise.core.view_model.misc;
using PetMerchandise.view.bean.setting_handler;

namespace PetMerchandise.core.view_model.setting;

public class PromotionSettingViewModel : BaseViewModel
{
    public PromotionSettingBean PromotionSettingBean { get; } = new();

    // public RelayCommand<object> LoadedCommand { get; }

    public RelayCommand<Tuple<IntPtr, int, IntPtr, IntPtr, bool>> MessageHookCommand { get; }

    public ICommand LoginCommand { get; }

    public PromotionSettingViewModel()
    {
        LoginCommand = new RelayCommand<object>(_ => Login());
        // LoadedCommand = new RelayCommand<object>(_ => Loaded());
        MessageHookCommand = new RelayCommand<Tuple<IntPtr, int, IntPtr, IntPtr, bool>>(tuple => MessageHook(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5));
    }

    private void Login()
    {
        throw new NotImplementedException();
    }

    public IntPtr MessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, bool handled)
    {
        if (msg == 130)
        {
            // Messenger.Default.Send();  
            // Close();
        }

        return IntPtr.Zero;
    }
}