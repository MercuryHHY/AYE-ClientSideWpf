
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AYE_Commom.Command;


public class AyeBaseCommand : DelegateCommand
{
    private readonly Func<Task> _executeMethod;
    private readonly Func<bool> _canExecuteMethod;
    private int _isExecuting;

    public AyeBaseCommand(Func<Task> executeMethod)
        : this(executeMethod, () => true)
    {
    }

    public AyeBaseCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
        : base(() => executeMethod().Wait(), canExecuteMethod)
    {
        _executeMethod = executeMethod ?? throw new ArgumentNullException(nameof(executeMethod));
        _canExecuteMethod = canExecuteMethod ?? throw new ArgumentNullException(nameof(canExecuteMethod));
    }

    public new async void Execute()
    {
        if (CanExecute())
        {
            await ExecuteAsync();
        }
    }

    private async Task ExecuteAsync()
    {
        try
        {
            Interlocked.Exchange(ref _isExecuting, 1);
            RaiseCanExecuteChanged();
            await _executeMethod();
        }
        finally
        {
            Interlocked.Exchange(ref _isExecuting, 0);
            RaiseCanExecuteChanged();
        }
    }

    public new bool CanExecute()
    {
        return Interlocked.CompareExchange(ref _isExecuting, 0, 0) == 0 && _canExecuteMethod();
    }

    protected override void Execute(object parameter)
    {
        Execute();
    }

    protected override bool CanExecute(object parameter)
    {
        return CanExecute();
    }

    private void RaiseCanExecuteChanged()
    {
        CommandManager.InvalidateRequerySuggested();
    }
}


