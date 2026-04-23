using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBuecherei.Models;
using MauiBuecherei.Services;
using System.Collections.ObjectModel;

namespace MauiBuecherei.ViewModels
{
    public partial class AusleiheListViewModel : ObservableObject
    {
        private readonly AusleiheApiService _ausleiheService;

        [ObservableProperty]
        private bool _isBusy;

        public ObservableCollection<AusleiheDto> Ausleihen { get; } = new();

        public IAsyncRelayCommand LoadAusleihenCommand { get; }

        public AusleiheListViewModel(AusleiheApiService ausleiheService)
        {
            _ausleiheService = ausleiheService;
            LoadAusleihenCommand = new AsyncRelayCommand(LoadAusleihenAsync);
            LoadAusleihenCommand.Execute(null);
        }

        private async Task LoadAusleihenAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var liste = await _ausleiheService.GetAusleihenAsync();
                Ausleihen.Clear();
                if (liste != null)
                    foreach (var a in liste)
                        Ausleihen.Add(a);
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
        private async Task CreateAusleihe()
        {
            await Shell.Current.GoToAsync(nameof(AusleiheErstellenPage));
        }

        [RelayCommand]
        private async Task ReturnBook(AusleiheDto? ausleihe)
        {
            if (ausleihe == null || ausleihe.Rueckgabedatum != null) return;
            bool confirm = await Shell.Current.DisplayAlertAsync("Rückgabe", $"Buch {ausleihe.Buchnummer} wirklich zurückgeben?", "Ja", "Nein");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                bool success = await _ausleiheService.ReturnBookAsync(ausleihe.Buchnummer);
                if (success)
                {
                    ausleihe.Rueckgabedatum = DateTime.Now;
                    ausleihe.Ausweisnummer = 0;
                    var index = Ausleihen.IndexOf(ausleihe);
                    if (index >= 0)
                    {
                        Ausleihen.RemoveAt(index);
                        Ausleihen.Insert(index, ausleihe);
                    }
                    await Shell.Current.DisplayAlertAsync("Erfolg", "Buch wurde zurückgegeben.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Fehler", "Rückgabe fehlgeschlagen.", "OK");
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
        private async Task DeleteAusleihe(AusleiheDto? ausleihe)
        {
            if (ausleihe == null) return;
            bool confirm = await Shell.Current.DisplayAlertAsync("Löschen", $"Ausleihe #{ausleihe.Id} löschen?", "Ja", "Nein");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                bool success = await _ausleiheService.DeleteAusleiheAsync(ausleihe.Id);
                if (success)
                {
                    Ausleihen.Remove(ausleihe);
                    await Shell.Current.DisplayAlertAsync("Erfolg", "Ausleihe gelöscht.", "OK");
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
    }
}