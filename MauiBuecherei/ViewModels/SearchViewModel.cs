using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBuecherei.Models;
using MauiBuecherei.Services;
using System.Collections.ObjectModel;

namespace MauiBuecherei.ViewModels
{
    public partial class SearchViewModel : ObservableObject
    {
        private readonly BuchApiService _buchService;
        private readonly SchülerInApiService _schuelerService;
        private readonly AusleiheApiService _ausleiheService;

        // Gespeicherte Gesamtlisten
        private List<BuchDto> _alleBücher = new();
        private List<SchülerInDto> _alleSchüler = new();
        private List<AusleiheDto> _alleAusleihen = new();

        [ObservableProperty] private bool _isBusy;
        [ObservableProperty] private string _suchbegriff = string.Empty;

        // Ergebnisse
        public ObservableCollection<BuchDto> GefundeneBücher { get; } = new();
        public ObservableCollection<SchülerInDto> GefundeneSchüler { get; } = new();
        public ObservableCollection<AusleiheDto> AktiveAusleihen { get; } = new();
        public ObservableCollection<AusleiheDto> ÜberfälligeAusleihen { get; } = new();

        public IAsyncRelayCommand LoadDataCommand { get; }

        public SearchViewModel(
            BuchApiService buchService,
            SchülerInApiService schuelerService,
            AusleiheApiService ausleiheService)
        {
            _buchService = buchService;
            _schuelerService = schuelerService;
            _ausleiheService = ausleiheService;

            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            LoadDataCommand.Execute(null);
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var buchTask = _buchService.GetBuecherAsync();
                var schuelerTask = _schuelerService.GetSchülerInAsync();
                var ausleihenTask = _ausleiheService.GetAusleihenAsync();

                await Task.WhenAll(buchTask, schuelerTask, ausleihenTask);

                _alleBücher = buchTask.Result ?? new();
                _alleSchüler = schuelerTask.Result ?? new();
                _alleAusleihen = ausleihenTask.Result ?? new();

                AktiveAusleihenAktualisieren();
                ÜberfälligeAusleihenAktualisieren();
                SucheAusführen(); // Falls bereits ein Suchbegriff eingegeben wurde
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Fehler", $"Daten laden fehlgeschlagen: {ex.Message}", "OK");
            }
            finally { IsBusy = false; }
        }

        partial void OnSuchbegriffChanged(string value) => SucheAusführen();

        private void SucheAusführen()
        {
            if (string.IsNullOrWhiteSpace(Suchbegriff))
            {
                GefundeneBücher.Clear();
                GefundeneSchüler.Clear();
                return;
            }

            var bücher = _alleBücher.Where(b =>
                b.Titel.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                b.AutorInnen.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                b.Sachgebiet.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                b.ISBN.Contains(Suchbegriff) ||
                b.Buchnummer.Contains(Suchbegriff)
            ).ToList();

            var schüler = _alleSchüler.Where(s =>
                s.Vorname.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                s.Nachname.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                s.Ausweisnummer.ToString().Contains(Suchbegriff)
            ).ToList();

            GefundeneBücher.Clear();
            foreach (var b in bücher) GefundeneBücher.Add(b);

            GefundeneSchüler.Clear();
            foreach (var s in schüler) GefundeneSchüler.Add(s);
        }

        private void AktiveAusleihenAktualisieren()
        {
            var aktiv = _alleAusleihen.Where(a => a.Rueckgabedatum == null).ToList();
            AktiveAusleihen.Clear();
            foreach (var a in aktiv) AktiveAusleihen.Add(a);
        }

        private void ÜberfälligeAusleihenAktualisieren()
        {
            var now = DateTime.Now;
            var überfällig = _alleAusleihen.Where(a =>
                a.Rueckgabedatum == null && a.Ausleihdatum.AddDays(14) < now) // Beispiel: 14 Tage Leihfrist
                .ToList();
            ÜberfälligeAusleihen.Clear();
            foreach (var a in überfällig) ÜberfälligeAusleihen.Add(a);
        }
    }
}