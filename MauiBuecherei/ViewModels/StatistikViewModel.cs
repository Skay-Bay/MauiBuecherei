using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiBuecherei.Models;
using MauiBuecherei.Services;
using System.Collections.ObjectModel;

namespace MauiBuecherei.ViewModels
{
    public partial class StatistikViewModel : ObservableObject
    {
        private readonly StatistikApiService _statistikService;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private int _gesamtAusleihen;

        [ObservableProperty]
        private int _aktiveAusleihen;

        public ObservableCollection<TopBuchDto> TopBuecher { get; } = new();

        public IAsyncRelayCommand LoadStatistikCommand { get; }

        public StatistikViewModel(StatistikApiService statistikService)
        {
            _statistikService = statistikService;
            LoadStatistikCommand = new AsyncRelayCommand(LoadStatistikAsync);
            LoadStatistikCommand.Execute(null);
        }

        private async Task LoadStatistikAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var gesamtTask = _statistikService.GetGesamtAusleihenAsync();
                var aktivTask = _statistikService.GetAktiveAusleihenAsync();
                var topTask = _statistikService.GetTopBuecherAsync(10);

                await Task.WhenAll(gesamtTask, aktivTask, topTask);

                GesamtAusleihen = await gesamtTask;
                AktiveAusleihen = await aktivTask;

                var topListe = await topTask;
                TopBuecher.Clear();
                if (topListe != null)
                    foreach (var b in topListe)
                        TopBuecher.Add(b);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Fehler", $"Statistik laden fehlgeschlagen: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}