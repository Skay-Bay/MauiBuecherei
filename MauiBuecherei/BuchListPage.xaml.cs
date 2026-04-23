using MauiBuecherei.ViewModels;

namespace MauiBuecherei
{
    public partial class BuchListPage : ContentPage
    {
        public BuchListPage(BuchListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}