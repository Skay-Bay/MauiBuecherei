using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBuecherei.Models;
using MauiBuecherei.Services;
using System.Collections.ObjectModel;

namespace MauiBuecherei.ViewModels
{
    public partial class SchülerInListViewModel : ObservableObject
    {
        private readonly SchülerInApiService _schuelerService;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _suchbegriff = string.Empty;

        public ObservableCollection<SchülerInDto> SchülerIn { get; } = new();

        public SchülerInListViewModel(SchülerInApiService schuelerService)
        {
            _schuelerService = schuelerService;
            LoadSchülerInCommand = new AsyncRelayCommand(LoadSchülerInAsync);
            LoadSchülerInCommand.Execute(null);
        }

        public IAsyncRelayCommand LoadSchülerInCommand { get; }

        private async Task LoadSchülerInAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var liste = await _schuelerService.GetSchülerInAsync();
                SchülerIn.Clear();
                if (liste != null)
                    foreach (var s in liste)
                        SchülerIn.Add(s);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Fehler", $"Laden fehlgeschlagen: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task CreateSchülerIn()
        {
            // Zur Detailseite navigieren mit Parameter "neu"
            await Shell.Current.GoToAsync($"{nameof(SchülerInDetailPage)}?mode=new");
        }

        [RelayCommand]
        private async Task EditSchülerIn(SchülerInDto schueler)
        {
            if (schueler == null) return;
            // Zur Detailseite navigieren mit Ausweisnummer
            await Shell.Current.GoToAsync($"{nameof(SchülerInDetailPage)}?ausweisnummer={schueler.Ausweisnummer}");
        }

        [RelayCommand]
        private async Task DeleteSchülerIn(SchülerInDto schueler)
        {
            if (schueler == null) return;
            bool confirm = await Shell.Current.DisplayAlertAsync("Löschen", $"{schueler.Vorname} {schueler.Nachname} löschen?", "Ja", "Nein");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                bool success = await _schuelerService.DeleteSchülerInAsync(schueler.Ausweisnummer);
                if (success)
                {
                    SchülerIn.Remove(schueler);
                    await Shell.Current.DisplayAlertAsync("Erfolg", "Schüler gelöscht.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Fehler", "Löschen fehlgeschlagen.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Fehler", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Search()
        {
            // Einfache clientseitige Suche (optional)
            if (string.IsNullOrWhiteSpace(Suchbegriff))
            {
                LoadSchülerInCommand.Execute(null);
                return;
            }
            var gefiltert = SchülerIn.Where(s =>
                s.Vorname.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                s.Nachname.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                s.Ausweisnummer.ToString().Contains(Suchbegriff)
            ).ToList();
            SchülerIn.Clear();
            foreach (var s in gefiltert)
                SchülerIn.Add(s);
        }
    }
}