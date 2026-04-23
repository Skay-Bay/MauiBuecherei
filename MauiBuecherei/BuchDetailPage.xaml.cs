using MauiBuecherei.Models;
using MauiBuecherei.Services;

namespace MauiBuecherei
{
    [QueryProperty(nameof(Mode), "mode")]
    [QueryProperty(nameof(Buchnummer), "buchnummer")]
    public partial class BuchDetailPage : ContentPage
    {
        private readonly BuchApiService _buchService;
        private BuchDto? _currentBuch;

        public string Mode { get; set; } = string.Empty;
        public string Buchnummer { get; set; } = string.Empty;

        public BuchDetailPage(BuchApiService buchService)
        {
            InitializeComponent();
            _buchService = buchService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (Mode == "new")
            {
                Title = "Neues Buch";
                if (!string.IsNullOrEmpty(Buchnummer) && Buchnummer != "0")
                    EntryBuchnummer.Text = Buchnummer;
                else
                    EntryBuchnummer.Text = "0";
                EntryBuchnummer.IsEnabled = true;
                DeleteButton.IsVisible = false;
                DatePickerErscheinungsdatum.Date = DateTime.Today;
            }
            else if (!string.IsNullOrEmpty(Buchnummer))
            {
                Title = "Buch bearbeiten";
                await LoadBuchAsync(Buchnummer);
                EntryBuchnummer.IsEnabled = false;
                DeleteButton.IsVisible = true;
            }
        }

        private async Task LoadBuchAsync(string buchNummer)
        {
            LoadingIndicator.IsVisible = true;
            try
            {
                _currentBuch = await _buchService.GetBuchAsync(buchNummer);
                if (_currentBuch != null)
                {
                    EntryBuchnummer.Text = _currentBuch.Buchnummer;
                    EntryTitel.Text = _currentBuch.Titel;
                    EntryAutorInnen.Text = _currentBuch.AutorInnen;
                    EntrySachgebiet.Text = _currentBuch.Sachgebiet;
                    EntryISBN.Text = _currentBuch.ISBN;
                    EntryVerlag.Text = _currentBuch.Verlag;
                    EntryVerlagsort.Text = _currentBuch.Verlagsort;
                    DatePickerErscheinungsdatum.Date = _currentBuch.Erscheinungsdatum;
                }
                else
                {
                    await DisplayAlertAsync("Fehler", "Buch nicht gefunden.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fehler", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EntryTitel.Text))
            {
                await DisplayAlertAsync("Fehler", "Titel darf nicht leer sein.", "OK");
                return;
            }

            LoadingIndicator.IsVisible = true;
            try
            {
                var dto = new BuchDto
                {
                    Buchnummer = EntryBuchnummer.Text,
                    Titel = EntryTitel.Text,
                    AutorInnen = EntryAutorInnen.Text,
                    Sachgebiet = EntrySachgebiet.Text,
                    ISBN = EntryISBN.Text,
                    Verlag = EntryVerlag.Text,
                    Verlagsort = EntryVerlagsort.Text,
                    Erscheinungsdatum = (DateTime)DatePickerErscheinungsdatum.Date
                };

                if (Mode == "new" || _currentBuch == null)
                {
                    var created = await _buchService.CreateBuchAsync(dto);
                    if (created != null)
                    {
                        await DisplayAlertAsync("Erfolg", $"Buch mit Nummer {created.Buchnummer} angelegt.", "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await DisplayAlertAsync("Fehler", "Anlegen fehlgeschlagen.", "OK");
                    }
                }
                else
                {
                    bool success = await _buchService.UpdateBuchAsync(_currentBuch.Buchnummer, dto);
                    if (success)
                    {
                        await DisplayAlertAsync("Erfolg", "Buch aktualisiert.", "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        await DisplayAlertAsync("Fehler", "Aktualisierung fehlgeschlagen.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fehler", ex.Message, "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if (_currentBuch == null) return;

            bool confirm = await DisplayAlertAsync("Löschen", $"'{_currentBuch.Titel}' löschen?", "Ja", "Nein");
            if (!confirm) return;

            LoadingIndicator.IsVisible = true;
            try
            {
                bool success = await _buchService.DeleteBuchAsync(_currentBuch.Buchnummer);
                if (success)
                {
                    await DisplayAlertAsync("Erfolg", "Buch gelöscht.", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await DisplayAlertAsync("Fehler", "Löschen fehlgeschlagen.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fehler", ex.Message, "OK");
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