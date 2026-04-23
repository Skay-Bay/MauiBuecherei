using MauiBuecherei.ViewModels;

namespace MauiBuecherei
{
    public partial class AusleiheListPage : ContentPage
    {
        public AusleiheListPage(AusleiheListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
