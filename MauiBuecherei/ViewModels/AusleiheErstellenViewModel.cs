using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBuecherei.Models;
using MauiBuecherei.Services;
using System.Collections.ObjectModel;

namespace MauiBuecherei.ViewModels
{
    public partial class AusleiheErstellenViewModel : ObservableObject
    {
        private readonly SchülerInApiService _schülerService;
        private readonly BuchApiService _buchService;
        private readonly AusleiheApiService _ausleiheService;

        private List<SchülerInDto> _alleSchüler = new();
        private List<BuchDto> _alleBücher = new();

        [ObservableProperty] private bool _isBusy;

        [ObservableProperty] private string _suchbegriffSchüler = string.Empty;
        public ObservableCollection<SchülerInDto> SchülerListe { get; } = new();
        [ObservableProperty] private SchülerInDto? _ausgewählterSchüler;
        [ObservableProperty] private bool _kannSchülerErstellen;

        [ObservableProperty] private string _suchbegriffBuch = string.Empty;
        public ObservableCollection<BuchDto> BuchListe { get; } = new();
        [ObservableProperty] private bool _kannBuchErstellen;

        public ObservableCollection<BuchDto> AusgewählteBücher { get; } = new();

        public IRelayCommand<SchülerInDto> SchülerAuswählenCommand { get; }
        public IAsyncRelayCommand SchülerErstellenCommand { get; }
        public IRelayCommand<BuchDto> BuchHinzufügenCommand { get; }
        public IAsyncRelayCommand BuchErstellenCommand { get; }
        public IRelayCommand<BuchDto> BuchEntfernenCommand { get; }
        public IAsyncRelayCommand AusleiheDurchführenCommand { get; }

        public AusleiheErstellenViewModel(
            SchülerInApiService schülerService,
            BuchApiService buchService,
            AusleiheApiService ausleiheService)
        {
            _schülerService = schülerService;
            _buchService = buchService;
            _ausleiheService = ausleiheService;

            SchülerAuswählenCommand = new RelayCommand<SchülerInDto>(SchülerAuswählen);
            SchülerErstellenCommand = new AsyncRelayCommand(SchülerErstellenAsync);
            BuchHinzufügenCommand = new RelayCommand<BuchDto>(BuchHinzufügen);
            BuchErstellenCommand = new AsyncRelayCommand(BuchErstellenAsync);
            BuchEntfernenCommand = new RelayCommand<BuchDto>(BuchEntfernen);
            AusleiheDurchführenCommand = new AsyncRelayCommand(AusleiheDurchführenAsync);
        }

        public async Task InitializeAsync()
        {
            IsBusy = true;
            try
            {
                var schülerTask = _schülerService.GetSchülerInAsync();
                var bücherTask = _buchService.GetBuecherAsync();
                await Task.WhenAll(schülerTask, bücherTask);

                _alleSchüler = schülerTask.Result ?? new List<SchülerInDto>();
                _alleBücher = bücherTask.Result ?? new List<BuchDto>();

                SchülerFiltern();
                BuchFiltern();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Fehler", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }

        partial void OnSuchbegriffSchülerChanged(string value) => SchülerFiltern();
        partial void OnSuchbegriffBuchChanged(string value) => BuchFiltern();

        private void SchülerFiltern()
        {
            var gefiltert = string.IsNullOrWhiteSpace(SuchbegriffSchüler)
                ? _alleSchüler
                : _alleSchüler.Where(s =>
                    s.Vorname.Contains(SuchbegriffSchüler, StringComparison.OrdinalIgnoreCase) ||
                    s.Nachname.Contains(SuchbegriffSchüler, StringComparison.OrdinalIgnoreCase) ||
                    s.Ausweisnummer.ToString().Contains(SuchbegriffSchüler)).ToList();

            SchülerListe.Clear();
            foreach (var s in gefiltert)
                SchülerListe.Add(s);

            KannSchülerErstellen = !string.IsNullOrWhiteSpace(SuchbegriffSchüler) &&
                                   int.TryParse(SuchbegriffSchüler, out int _) &&
                                   gefiltert.Count == 0;
        }

        private void BuchFiltern()
        {
            var gefiltert = string.IsNullOrWhiteSpace(SuchbegriffBuch)
                ? _alleBücher
                : _alleBücher.Where(b =>
                    b.Titel.Contains(SuchbegriffBuch, StringComparison.OrdinalIgnoreCase) ||
                    b.AutorInnen.Contains(SuchbegriffBuch, StringComparison.OrdinalIgnoreCase) ||
                    b.Buchnummer.Contains(SuchbegriffBuch) ||
                    b.ISBN.Contains(SuchbegriffBuch)).ToList();

            BuchListe.Clear();
            foreach (var b in gefiltert)
                BuchListe.Add(b);

            KannBuchErstellen = !string.IsNullOrWhiteSpace(SuchbegriffBuch) && gefiltert.Count == 0;
        }

        private void SchülerAuswählen(SchülerInDto? schüler)
        {
            if (schüler == null) return;
            AusgewählterSchüler = schüler;
            SuchbegriffSchüler = $"{schüler.Vorname} {schüler.Nachname} ({schüler.Ausweisnummer})";
            SchülerListe.Clear();
        }

        private async Task SchülerErstellenAsync()
        {
            await Shell.Current.GoToAsync(
                $"{nameof(SchülerInDetailPage)}?mode=new&ausweisnummer={SuchbegriffSchüler}");
        }

        private void BuchHinzufügen(BuchDto? buch)
        {
            if (buch == null) return;
            if (AusgewählteBücher.Any(b => b.Buchnummer == buch.Buchnummer))
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                    await Shell.Current.DisplayAlertAsync("Hinweis", "Buch bereits in der Liste.", "OK"));
                return;
            }
            AusgewählteBücher.Add(buch);
            SuchbegriffBuch = string.Empty;
            BuchFiltern();
        }

        private void BuchEntfernen(BuchDto? buch)
        {
            if (buch != null)
                AusgewählteBücher.Remove(buch);
        }

        private async Task BuchErstellenAsync()
        {
            await Shell.Current.GoToAsync(
                $"{nameof(BuchDetailPage)}?mode=new&buchnummer={SuchbegriffBuch}");
        }

        private async Task AusleiheDurchführenAsync()
        {
            if (AusgewählteBücher.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Fehler", "Keine Bücher ausgewählt.", "OK");
                return;
            }

            int ausweisnummer = AusgewählterSchüler?.Ausweisnummer ?? 0;

            IsBusy = true;
            try
            {
                if (AusgewählteBücher.Count == 1)
                {
                    var dto = new AusleiheDto
                    {
                        Buchnummer = AusgewählteBücher[0].Buchnummer,
                        Ausweisnummer = ausweisnummer
                    };
                    var erg = await _ausleiheService.CreateAusleiheAsync(dto);
                    if (erg != null)
                    {
                        await Shell.Current.DisplayAlertAsync("Erfolg", $"Ausleihe erstellt (ID: {erg.Id})", "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else await Shell.Current.DisplayAlertAsync("Fehler", "Ausleihe fehlgeschlagen.", "OK");
                }
                else
                {
                    var bulk = new BulkAusleiheDto
                    {
                        SchülerInAusweisnummer = ausweisnummer,
                        BuchNummern = AusgewählteBücher.Select(b => b.Buchnummer).ToList()
                    };
                    var ergebnisse = await _ausleiheService.BulkAusleiheAsync(bulk);
                    if (ergebnisse != null)
                    {
                        string meldung = string.Join("\n", ergebnisse.Select(r =>
                            $"{r.BuchNummer}: {(r.Erfolg ? "OK" : r.Fehlermeldung)}"));
                        await Shell.Current.DisplayAlertAsync("Ergebnis", meldung, "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else await Shell.Current.DisplayAlertAsync("Fehler", "Bulk-Ausleihe fehlgeschlagen.", "OK");
                }
            }
            catch (Exception ex) { await Shell.Current.DisplayAlertAsync("Fehler", ex.Message, "OK"); }
            finally { IsBusy = false; }
        }
    }
}