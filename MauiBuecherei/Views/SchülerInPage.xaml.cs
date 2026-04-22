using MauiBuecherei.Dtos;
using MauiBuecherei.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MauiBuecherei.Views
{
    public partial class SchülerInPage : ContentPage
    {
        private readonly ApiClient _apiClient;
        private ObservableCollection<SchülerInViewModel> _allSchülerInnen = new();

        public SchülerInPage(ApiClient apiClient)
        {
            InitializeComponent();
            _apiClient = apiClient;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadSchülerInnen();
        }

        private async Task LoadSchülerInnen()
        {
            LoadingIndicator.IsRunning = true;
            try
            {
                var dtos = await _apiClient.GetSchülerInAsync();
                var ausleihen = await _apiClient.GetAusleihenAsync();

                var viewModels = dtos.Select(dto => new SchülerInViewModel
                {
                    Ausweisnummer = dto.Ausweisnummer,
                    Vorname = dto.Vorname,
                    Nachname = dto.Nachname,
                    HatAusleihen = ausleihen.Any(a => a.Ausweisnummer == dto.Ausweisnummer && a.Rueckgabedatum == null)
                }).ToList();

                _allSchülerInnen = new ObservableCollection<SchülerInViewModel>(viewModels);
                SchülerInCollectionView.ItemsSource = _allSchülerInnen;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsRunning = false;
            }
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTerm = e.NewTextValue?.ToLower() ?? "";
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                SchülerInCollectionView.ItemsSource = _allSchülerInnen;
                return;
            }

            var filtered = _allSchülerInnen.Where(s =>
                s.Ausweisnummer.ToString().Contains(searchTerm) ||
                s.Vorname.ToLower().Contains(searchTerm) ||
                s.Nachname.ToLower().Contains(searchTerm)
            ).ToList();

            SchülerInCollectionView.ItemsSource = filtered;
        }

        private async void OnNewSchülerInClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Neue/r SchülerIn", "Funktion folgt in Kürze.", "OK");
        }

        private async void OnDetailsClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is SchülerInViewModel selected)
            {
                await DisplayAlert("Details", $"{selected.Vorname} {selected.Nachname}\nAusweis: {selected.Ausweisnummer}", "OK");
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is SchülerInViewModel selected)
            {
                await DisplayAlert("Bearbeiten", $"Bearbeite {selected.Vorname} {selected.Nachname}", "OK");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is SchülerInViewModel selected)
            {
                bool confirm = await DisplayAlert("Löschen", $"Möchten Sie {selected.Vorname} {selected.Nachname} wirklich löschen?", "Ja", "Nein");
                if (confirm)
                {
                    await _apiClient.DeleteSchülerInAsync(selected.Ausweisnummer);
                    await LoadSchülerInnen();
                }
            }
        }
    }

    // ViewModel für die Anzeige
    public class SchülerInViewModel
    {
        public int Ausweisnummer { get; set; }
        public string Vorname { get; set; } = string.Empty;
        public string Nachname { get; set; } = string.Empty;
        public bool HatAusleihen { get; set; }
    }
}
