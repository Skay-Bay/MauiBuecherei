namespace MauiBuecherei
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnGoToSchuelerClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(Views.SchülerInPage));
        }
    }
}