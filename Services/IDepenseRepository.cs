using CarnetDepenses.Models;
namespace CarnetDepenses.Services;

// Le ViewModel ne connaît QUE cette interface.
public interface IDepenseRepository
{
    Task<List<Depense>> ListerAsync();
    Task AjouterAsync(Depense d);
    Task SupprimerAsync(Depense d);
}
