using MauiBuecherei.Models;
using MauiBuecherei.Services;

namespace MauiBuecherei
{
    public partial class BulkAusleihePage : ContentPage
    {
        private readonly AusleiheApiService _ausleiheService;

        public BulkAusleihePage(AusleiheApiService ausleiheService)
        {
            InitializeComponent();
            _ausleiheService = ausleiheService;
        }

        private async void OnBulkClicked(object sender, EventArgs e)
        {
            if (!int.TryParse(EntryAusweisnummer.Text, out int ausweisnummer) || ausweisnummer <= 0)
            {
                await DisplayAlertAsync("Fehler", "Gültige Ausweisnummer eingeben.", "OK");
                return;
            }

            var buchnummern = EditorBuchnummern.Text
                ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            if (buchnummern == null || buchnummern.Count == 0)
            {
                await DisplayAlertAsync("Fehler", "Mindestens eine Buchnummer angeben.", "OK");
                return;
            }

            LoadingIndicator.IsVisible = true;
            try
            {
                var bulkDto = new BulkAusleiheDto
                {
                    SchülerInAusweisnummer = ausweisnummer,
                    BuchNummern = buchnummern
                };

                var ergebnisse = await _ausleiheService.BulkAusleiheAsync(bulkDto);
                if (ergebnisse != null)
                {
                    string meldung = string.Join("\n", ergebnisse.Select(r =>
                        $"{r.BuchNummer}: {(r.Erfolg ? "OK" : r.Fehlermeldung)}"));
                    await DisplayAlertAsync("Ergebnis", meldung, "OK");
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

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
