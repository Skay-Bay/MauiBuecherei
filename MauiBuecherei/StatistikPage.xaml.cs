using MauiBuecherei.ViewModels;

namespace MauiBuecherei
{
    public partial class StatistikPage : ContentPage
    {
        private readonly StatistikViewModel _viewModel;

        public StatistikPage(StatistikViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadStatistikCommand.Execute(null);
        }
    }
}