using System.Net;
using System.Text;
namespace CarnetDepenses.Services;

// Le faux serveur : répond à la place d'une vraie API (pas d'Internet en salle).
public class FakeApiHandler : HttpMessageHandler
{
    public static bool SimulerPanne = false;  // true = panne

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage req, CancellationToken ct)
    {
        await Task.Delay(1500, ct);     // latence simulée
        if (SimulerPanne)
            throw new HttpRequestException("Pas de connexion");
        string json = "{\"base\":\"XAF\",\"rates\":" +
                      "{\"EUR\":0.001524,\"USD\":0.00178}}";
        return new HttpResponseMessage(HttpStatusCode.OK)
        { Content = new StringContent(json, Encoding.UTF8,
                                      "application/json") };
    }
}
