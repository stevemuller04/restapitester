using System;
using System.Diagnostics;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    public RelayCommand(Action<object> execAction)
        : this(execAction, null)
    {
    }

    public RelayCommand(Action<object> execAction, Predicate<object> canExecPredicate)
    {
        if (execAction == null)
            throw new ArgumentNullException("execAction");
        _execAction = execAction;
        _canExecPredicate = canExecPredicate;
    }

    private Action<object> _execAction;
    private Predicate<object> _canExecPredicate;

    [DebuggerStepThrough]
    public bool CanExecute(object parameter)
    {
        return _canExecPredicate == null || _canExecPredicate(parameter);
    }

    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public void Execute(object parameter)
    {
        _execAction(parameter);
    }
}