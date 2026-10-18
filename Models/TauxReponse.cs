using System.Text.Json.Serialization;
namespace CarnetDepenses.Models;

public class TauxReponse
{
    [JsonPropertyName("base")]
    public string Base { get; set; } = "";
    // Nom exact de la clé dans le JSON reçu.
    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Taux { get; set; }
        = new();
}
