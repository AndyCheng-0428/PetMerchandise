using System;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;

namespace PetMerchandise.core.view_model.misc;

/// <summary>
/// 在MVVM的結構中，若要在.xmal文件中使用DataBinding方式調用事件方法
/// 必須要每個命令皆建立類別，並在類別中寫入須要執行的方法
/// 本類別作為一個依賴方法，將要執行的方法作為RelayCommand的參數傳入即可執行任意方法
///
/// => 透過此類別將方法封裝成命令
/// </summary>
public class RelayCommand<T> : ICommand where T : class
{
    private readonly Action<T> _execute; //動作
    private readonly Func<T, bool> _canExecute; //是否可以執行

    public RelayCommand(Action<T> execute)
    {
        _execute = execute;
    }

    public RelayCommand(Action<T> execute, Func<T,bool> canExecute)
    {
        _canExecute = canExecute;
        _execute = execute;
    }

    public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter as T);

    public void Execute(object? parameter) => _execute(parameter as T);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}