// ViewModels/BuchListViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBuecherei.Models;
using MauiBuecherei.Services;
using System.Collections.ObjectModel;

namespace MauiBuecherei.ViewModels
{
    public partial class BuchListViewModel : ObservableObject
    {
        private readonly BuchApiService _buchService;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _suchbegriff = string.Empty;

        public ObservableCollection<BuchDto> Buecher { get; } = new();

        public BuchListViewModel(BuchApiService buchService)
        {
            _buchService = buchService;
            LoadBuecherCommand = new AsyncRelayCommand(LoadBuecherAsync);
            LoadBuecherCommand.Execute(null);
        }

        public IAsyncRelayCommand LoadBuecherCommand { get; }

        private async Task LoadBuecherAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var liste = await _buchService.GetBuecherAsync();
                Buecher.Clear();
                if (liste != null)
                    foreach (var b in liste)
                        Buecher.Add(b);
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
        private async Task CreateBuch()
        {
            await Shell.Current.GoToAsync($"{nameof(BuchDetailPage)}?mode=new");
        }

        [RelayCommand]
        private async Task EditBuch(BuchDto buch)
        {
            if (buch == null) return;
            await Shell.Current.GoToAsync($"{nameof(BuchDetailPage)}?buchnummer={buch.Buchnummer}");
        }

        [RelayCommand]
        private async Task DeleteBuch(BuchDto buch)
        {
            if (buch == null) return;
            bool confirm = await Shell.Current.DisplayAlertAsync("Löschen", $"'{buch.Titel}' löschen?", "Ja", "Nein");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                bool success = await _buchService.DeleteBuchAsync(buch.Buchnummer);
                if (success)
                {
                    Buecher.Remove(buch);
                    await Shell.Current.DisplayAlertAsync("Erfolg", "Buch gelöscht.", "OK");
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
            // Einfache clientseitige Suche (alternativ serverseitige Suche verwenden)
            if (string.IsNullOrWhiteSpace(Suchbegriff))
            {
                LoadBuecherCommand.Execute(null);
                return;
            }

            var gefiltert = Buecher.Where(b =>
                b.Titel.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                b.AutorInnen.Contains(Suchbegriff, StringComparison.OrdinalIgnoreCase) ||
                b.Buchnummer.Contains(Suchbegriff) ||
                b.ISBN.Contains(Suchbegriff)
            ).ToList();

            Buecher.Clear();
            foreach (var b in gefiltert)
                Buecher.Add(b);
        }

        // Optional: Serverseitige Suche aufrufen
        [RelayCommand]
        private async Task ServerSearch()
        {
            if (string.IsNullOrWhiteSpace(Suchbegriff))
            {
                await LoadBuecherAsync();
                return;
            }

            IsBusy = true;
            try
            {
                var ergebnisse = await _buchService.SucheBuecherAsync(Suchbegriff);
                Buecher.Clear();
                if (ergebnisse != null)
                    foreach (var b in ergebnisse)
                        Buecher.Add(b);
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
    }
}