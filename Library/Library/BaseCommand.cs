using System;
using System.Windows.Input;

internal class BaseCommand : ICommand
{

    private Predicate<object> _canExecute;
    private Action<object> _method;
    public event EventHandler CanExecuteChanged;

    public BaseCommand(Action<object> method)
    {
        _method = method;
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public void Execute(object parameter)
    {
        _method.Invoke(parameter);
    }
}