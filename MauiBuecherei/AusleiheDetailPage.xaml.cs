using MauiBuecherei.Models;
using MauiBuecherei.Services;

namespace MauiBuecherei
{
    [QueryProperty(nameof(Mode), "mode")]
    public partial class AusleiheDetailPage : ContentPage
    {
        private readonly AusleiheApiService _ausleiheService;
        public string Mode { get; set; } = string.Empty;

        public AusleiheDetailPage(AusleiheApiService ausleiheService)
        {
            InitializeComponent();
            _ausleiheService = ausleiheService;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EntryBuchnummer.Text))
            {
                await DisplayAlertAsync("Fehler", "Buchnummer eingeben.", "OK");
                return;
            }

            if (!int.TryParse(EntryAusweisnummer.Text, out int ausweisnummer))
                ausweisnummer = 0;

            LoadingIndicator.IsVisible = true;
            try
            {
                var dto = new AusleiheDto
                {
                    Buchnummer = EntryBuchnummer.Text,
                    Ausweisnummer = ausweisnummer
                };

                var created = await _ausleiheService.CreateAusleiheAsync(dto);
                if (created != null)
                {
                    await DisplayAlertAsync("Erfolg", $"Ausleihe erstellt (ID: {created.Id})", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await DisplayAlertAsync("Fehler", "Erstellung fehlgeschlagen.", "OK");
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
