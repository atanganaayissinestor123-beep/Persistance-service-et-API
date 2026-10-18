using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;
using CarnetDepenses.Models;
using CarnetDepenses.Services;
namespace CarnetDepenses.ViewModels;

public class DepensesViewModel : BaseViewModel
{
    private readonly IDepenseRepository _repo;
    private readonly IApiService _api;                       // PHASE B
    private string _libelle = "", _montantTexte = "";
    private bool _isBusy;                                     // PHASE B
    private string _totalConverti = "", _message = "";        // PHASE B

    public ObservableCollection<Depense> Depenses { get; } = new();
    public decimal Total => Depenses.Sum(d => d.Montant);

    public string Libelle
    {
        get => _libelle;
        set { if (SetField(ref _libelle, value))
                  AjouterCommand.Rafraichir(); }
    }
    public string MontantTexte
    {
        get => _montantTexte;
        set { if (SetField(ref _montantTexte, value))
                  AjouterCommand.Rafraichir(); }
    }

    // ---- PHASE B : propriétés ----
    public bool IsBusy
    {
        get => _isBusy;
        set { if (SetField(ref _isBusy, value))
                  ConvertirCommand.Rafraichir(); }
    }
    public string TotalConverti
    { get => _totalConverti; set => SetField(ref _totalConverti, value); }
    public string Message
    { get => _message; set => SetField(ref _message, value); }
    public RelayCommand ConvertirCommand { get; }

    // ---- Bouton "Simuler une panne" (relié au faux serveur) ----
    public bool SimulerPanne
    {
        get => FakeApiHandler.SimulerPanne;
        set
        {
            if (FakeApiHandler.SimulerPanne == value) return;
            FakeApiHandler.SimulerPanne = value;
            OnPropertyChanged(nameof(SimulerPanne));
        }
    }
    // -------------------------------

    public RelayCommand AjouterCommand { get; }
    public RelayCommandP SupprimerCommand { get; }

    // Repository et service API INJECTES par le conteneur.
    public DepensesViewModel(IDepenseRepository repo, IApiService api)
    {
        _repo = repo;
        _api = api;                                           // PHASE B
        AjouterCommand = new RelayCommand(
            async () => await AjouterAsync(),
            () => !string.IsNullOrWhiteSpace(Libelle)
               && !string.IsNullOrWhiteSpace(MontantTexte));
        SupprimerCommand = new RelayCommandP(
            async p => await SupprimerAsync((Depense)p));
        ConvertirCommand = new RelayCommand(                  // PHASE B
            async () => await ConvertirAsync(), () => !IsBusy);
    }

    // Relit le stockage et remplit la liste affichée.
    public async Task ChargerAsync()
    {
        Depenses.Clear();
        foreach (var d in await _repo.ListerAsync())
            Depenses.Add(d);
        OnPropertyChanged(nameof(Total));
    }

    private async Task AjouterAsync()
    {
        if (!decimal.TryParse(MontantTexte, out decimal m))
            return;
        var d = new Depense { Libelle = Libelle, Montant = m };
        await _repo.AjouterAsync(d);   // 1. on sauvegarde
        Depenses.Add(d);               // 2. on affiche
        OnPropertyChanged(nameof(Total));
        Libelle = ""; MontantTexte = "";
    }

    private async Task SupprimerAsync(Depense d)
    {
        await _repo.SupprimerAsync(d);
        Depenses.Remove(d);
        OnPropertyChanged(nameof(Total));
    }

    // ---- PHASE B : conversion via l'API ----
    private async Task ConvertirAsync()
    {
        IsBusy = true; Message = "";
        try
        {
            var t = await _api.GetTauxAsync();                // appel réseau
            decimal eur = t.Taux["EUR"];
            Preferences.Set("taux_eur", (double)eur);         // cache
            TotalConverti = $"{Total * eur:F2} EUR";
        }
        catch (HttpRequestException)                          // réseau coupé
        {
            double c = Preferences.Get("taux_eur", 0.0);
            if (c == 0) Message = "Hors ligne, pas de taux.";
            else
            {
                TotalConverti =
                    $"{Total * (decimal)c:F2} EUR (cache)";
                Message = "Hors ligne : dernier taux connu.";
            }
        }
        finally { IsBusy = false; }   // exécuté TOUJOURS
    }
}
