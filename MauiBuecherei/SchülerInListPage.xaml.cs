using MauiBuecherei.ViewModels;

namespace MauiBuecherei
{
    public partial class SchülerInListPage : ContentPage
    {
        public SchülerInListPage(SchülerInListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SchülerInListViewModel vm)
            {
                vm.LoadSchülerInCommand.Execute(null);
            }
        }
    }
}