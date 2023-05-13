using System.Globalization;
using System.Threading;
using System.Windows;
using Microsoft.Win32;

namespace PetMerchandise
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";
            Thread.CurrentThread.CurrentCulture = ci;
            
            
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
            registryKey.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", 11001, RegistryValueKind.DWord);
            registryKey.Close();
        }
    }
}