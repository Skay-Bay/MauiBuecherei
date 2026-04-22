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
            var page = App.Current.Handler.MauiContext.Services.GetService<SchülerInListPage>();
            await Shell.Current.Navigation.PushAsync(page);
        }

        private async void OnBuchClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(BuchListPage));
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