using System.Net.Http.Json;
using CarnetDepenses.Models;
namespace CarnetDepenses.Services;

public interface IApiService
{ Task<TauxReponse> GetTauxAsync(); }

public class ApiService : IApiService
{
    private readonly HttpClient _http;
    // HttpClient fourni par l'injection de dépendances.
    public ApiService(HttpClient http) => _http = http;

    // GET + contrôle du code + lecture + désérialisation.
    public async Task<TauxReponse> GetTauxAsync() =>
        await _http.GetFromJsonAsync<TauxReponse>(
            "https://api.exemple.cm/taux")
        ?? throw new InvalidOperationException("Réponse vide");
}
