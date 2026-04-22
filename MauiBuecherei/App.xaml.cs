namespace MauiBuecherei
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(SchülerInListPage), typeof(SchülerInListPage));
            Routing.RegisterRoute(nameof(BuchListPage), typeof(BuchListPage));
            Routing.RegisterRoute(nameof(AusleiheListPage), typeof(AusleiheListPage));
            Routing.RegisterRoute(nameof(StatistikPage), typeof(StatistikPage));
            Routing.RegisterRoute(nameof(DatenbankExportImportPage), typeof(DatenbankExportImportPage));

            MainPage = new AppShell();
        }
    }
}