using System.Windows;

namespace PetMerchandise.view.utils;

public static class WebBrowserUtils
{
    public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
        "Source",
        typeof(string),
        typeof(WebBrowserUtils),
        new UIPropertyMetadata(OnSourceChanged));

    public static string GetSource(DependencyObject obj)
    {
        return (string)obj.GetValue(SourceProperty);
    }
    
    public static void SetSource(DependencyObject obj, string value)
    {
        obj.SetValue(SourceProperty, value);
    }
    
    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not System.Windows.Controls.WebBrowser browser)
        {
            return;
        }
        
        browser.Navigate((string)e.NewValue);
    }
}