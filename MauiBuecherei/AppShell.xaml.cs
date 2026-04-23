namespace MauiBuecherei
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SchülerInListPage), typeof(SchülerInListPage));
            Routing.RegisterRoute(nameof(SchülerInDetailPage), typeof(SchülerInDetailPage));
            Routing.RegisterRoute(nameof(BuchListPage), typeof(BuchListPage));
            Routing.RegisterRoute(nameof(BuchDetailPage), typeof(BuchDetailPage));
            // Neue Ausleihe-Routen
            Routing.RegisterRoute(nameof(AusleiheListPage), typeof(AusleiheListPage));
            Routing.RegisterRoute(nameof(AusleiheDetailPage), typeof(AusleiheDetailPage));
            Routing.RegisterRoute(nameof(BulkAusleihePage), typeof(BulkAusleihePage));
        }
    }
}