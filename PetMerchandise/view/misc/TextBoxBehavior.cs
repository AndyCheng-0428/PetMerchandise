using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PetMerchandise.view.misc;

public class TextBoxBehavior
{
    public static readonly DependencyProperty TextChangedCommandProperty = DependencyProperty.RegisterAttached(
        "TextChangedCommand", typeof(ICommand), typeof(TextBoxBehavior), new PropertyMetadata(null, TextChangedCommandChanged));

    public static readonly DependencyProperty TextChangedCommandParameterProperty = DependencyProperty.RegisterAttached(
        "TextChangedCommandParameter", typeof(object), typeof(TextBoxBehavior), new PropertyMetadata(null));

    private static void TextChangedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            textBox.TextChanged -= TextBoxOnTextChanged;
            textBox.TextChanged += TextBoxOnTextChanged;
        }
    }

    private static void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            ICommand command = GetTextChangedCommand(textBox);
            if (command != null && command.CanExecute(null))
            {
                command.Execute(GetTextChangedCommandParameter(textBox));
            }
        }
    }

    public static ICommand GetTextChangedCommand(TextBox textBox)
    {
        return (ICommand)textBox.GetValue(TextChangedCommandProperty);
    }

    public static void SetTextChangedCommand(TextBox textBox, ICommand value)
    {
        textBox.SetValue(TextChangedCommandProperty, value);
    }

    public static object GetTextChangedCommandParameter(TextBox textBox)
    {
        return textBox.GetValue(TextChangedCommandParameterProperty);
    }

    public static void SetTextChangedCommandParameter(TextBox textBox, object value)
    {
        textBox.SetValue(TextChangedCommandParameterProperty, value);
    }
}