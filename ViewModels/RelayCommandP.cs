namespace CarnetDepenses.ViewModels;

public class RelayCommandP : System.Windows.Input.ICommand
{
    private readonly Action<object> _exec;
    public RelayCommandP(Action<object> exec) => _exec = exec;
    public bool CanExecute(object? p) => true;
    public void Execute(object? p) => _exec(p!);
    public event EventHandler? CanExecuteChanged;
}
