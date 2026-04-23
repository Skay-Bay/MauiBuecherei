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
            await Shell.Current.GoToAsync(nameof(SchülerInListPage));
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

        private async void OnSearchClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SearchPage));
        }
    }
}