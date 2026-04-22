using MauiBuecherei.Models;
using MauiBuecherei.Services;

namespace MauiBuecherei
{
    [QueryProperty(nameof(Mode), "mode")]
    [QueryProperty(nameof(Ausweisnummer), "ausweisnummer")]
    public partial class SchülerInDetailPage : ContentPage
    {
        private readonly SchülerInApiService _schuelerService;
        private SchülerInDto? _currentSchülerIn;

        public string Mode { get; set; } = string.Empty;
        public string Ausweisnummer { get; set; } = string.Empty;

        public SchülerInDetailPage(SchülerInApiService schuelerService)
        {
            InitializeComponent();
            _schuelerService = schuelerService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (Mode == "new")
            {
                Title = "Neuer Schüler";
                EntryAusweisnummer.Text = "0";
                EntryAusweisnummer.IsEnabled = true;
                DeleteButton.IsVisible = false;
            }
            else if (!string.IsNullOrEmpty(Ausweisnummer) && int.TryParse(Ausweisnummer, out int id))
            {
                Title = "Schüler bearbeiten";
                await LoadSchülerInAsync(id);
                EntryAusweisnummer.IsEnabled = false; // Nummer nicht änderbar
                DeleteButton.IsVisible = true;
            }
        }

        private async Task LoadSchülerInAsync(int id)
        {
            LoadingIndicator.IsVisible = true;
            try
            {
                var liste = await _schuelerService.GetSchülerInAsync();
                _currentSchülerIn = liste?.FirstOrDefault(s => s.Ausweisnummer == id);
                if (_currentSchülerIn != null)
                {
                    EntryAusweisnummer.Text = _currentSchülerIn.Ausweisnummer.ToString();
                    EntryVorname.Text = _currentSchülerIn.Vorname;
                    EntryNachname.Text = _currentSchülerIn.Nachname;
                }
                else
                {
                    await DisplayAlert("Fehler", "Schüler nicht gefunden.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EntryVorname.Text) || string.IsNullOrWhiteSpace(EntryNachname.Text))
            {
                await DisplayAlert("Fehler", "Vor- und Nachname dürfen nicht leer sein.", "OK");
                return;
            }

            if (!int.TryParse(EntryAusweisnummer.Text, out int ausweisnummer))
            {
                await DisplayAlert("Fehler", "Ungültige Ausweisnummer.", "OK");
                return;
            }

            LoadingIndicator.IsVisible = true;
            try
            {
                var dto = new SchülerInDto
                {
                    Ausweisnummer = ausweisnummer,
                    Vorname = EntryVorname.Text,
                    Nachname = EntryNachname.Text
                };

                if (Mode == "new" || _currentSchülerIn == null)
                {
                    // Neuanlage – POST mit Nummer (0 generiert automatisch)
                    var created = await _schuelerService.CreateSchülerInAsync(dto);
                    if (created != null)
                    {
                        await DisplayAlert("Erfolg", $"Schüler mit Nummer {created.Ausweisnummer} angelegt.", "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await DisplayAlert("Fehler", "Anlegen fehlgeschlagen.", "OK");
                    }
                }
                else
                {
                    // Bearbeitung – PUT
                    bool success = await _schuelerService.UpdateSchülerInAsync(_currentSchülerIn.Ausweisnummer, dto);
                    if (success)
                    {
                        await DisplayAlert("Erfolg", "Schüler aktualisiert.", "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await DisplayAlert("Fehler", "Aktualisierung fehlgeschlagen.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_currentSchülerIn == null) return;
            bool confirm = await DisplayAlert("Löschen", $"{_currentSchülerIn.Vorname} {_currentSchülerIn.Nachname} löschen?", "Ja", "Nein");
            if (!confirm) return;

            LoadingIndicator.IsVisible = true;
            try
            {
                bool success = await _schuelerService.DeleteSchülerInAsync(_currentSchülerIn.Ausweisnummer);
                if (success)
                {
                    await DisplayAlert("Erfolg", "Schüler gelöscht.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await DisplayAlert("Fehler", "Löschen fehlgeschlagen.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}