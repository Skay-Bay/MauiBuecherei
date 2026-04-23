using MauiBuecherei.ViewModels;

namespace MauiBuecherei
{
    public partial class SearchPage : ContentPage
    {
        private readonly SearchViewModel _viewModel;

        public SearchPage(SearchViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadDataCommand.Execute(null);
        }
    }
}