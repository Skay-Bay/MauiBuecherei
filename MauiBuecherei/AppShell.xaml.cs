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
            Routing.RegisterRoute(nameof(AusleiheListPage), typeof(AusleiheListPage));
            Routing.RegisterRoute(nameof(AusleiheErstellenPage), typeof(AusleiheErstellenPage));
            Routing.RegisterRoute(nameof(StatistikPage), typeof(StatistikPage));
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
        }
    }
}