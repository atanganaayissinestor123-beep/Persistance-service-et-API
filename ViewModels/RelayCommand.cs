using System.Windows.Input;
namespace CarnetDepenses.ViewModels;

// Version de secours : utilise celle de ton TP précédent si tu l'as
// (elle doit avoir Rafraichir() et un second paramètre Func<bool>).
public class RelayCommand : ICommand
{
    private readonly Action _exec;
    private readonly Func<bool>? _canExec;

    public RelayCommand(Action exec, Func<bool>? canExec = null)
    {
        _exec = exec;
        _canExec = canExec;
    }

    public bool CanExecute(object? p) => _canExec?.Invoke() ?? true;
    public void Execute(object? p) => _exec();
    public event EventHandler? CanExecuteChanged;
    public void Rafraichir() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
