using MauiBuecherei.ViewModels;

namespace MauiBuecherei
{
    public partial class AusleiheErstellenPage : ContentPage
    {
        private readonly AusleiheErstellenViewModel _viewModel;

        public AusleiheErstellenPage(AusleiheErstellenViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}