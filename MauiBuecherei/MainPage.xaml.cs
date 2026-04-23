namespace MauiBuecherei
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSchülerInClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(SchülerInListPage));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fehler", $"Navigation Schüler fehlgeschlagen:\n{ex.Message}", "OK");
            }
        }

        private async void OnBuchClicked(object sender, EventArgs e)
        {
            try
            {
                await Shell.Current.GoToAsync(nameof(BuchListPage));
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fehler", $"Navigation Buch fehlgeschlagen:\n{ex.Message}", "OK");
            }
        }

        private async void OnAusleiheClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(AusleiheListPage));
        }

        private async void OnStatistikClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(StatistikPage));
        }

        private async void OnDatabaseClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(DatenbankExportImportPage));
        }
    }
}