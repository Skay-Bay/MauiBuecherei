using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MauiBuecherei.Services;
using MauiBuecherei.ViewModels;

namespace MauiBuecherei
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Basis-URL je nach Plattform
            string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5035/api/"
                : "http://localhost:5035/api/";

            builder.Services.AddHttpClient<SchülerInApiService>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                // Handler, der HTTP ohne SSL erlaubt und Umleitungen verhindert
                return new HttpClientHandler
                {
                    // Keine automatische Weiterleitung von HTTP auf HTTPS
                    AllowAutoRedirect = true,
                    // Akzeptiere alle Zertifikate (nur für Entwicklung)
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true,
                    // Verwende keine Standard-Proxy-Einstellungen
                    UseProxy = false
                };
            });

            builder.Services.AddTransient<SchülerInListViewModel>();
            builder.Services.AddTransient<SchülerInListPage>();
            builder.Services.AddTransient<SchülerInDetailPage>();

            Routing.RegisterRoute(nameof(SchülerInListPage), typeof(SchülerInListPage));
            Routing.RegisterRoute(nameof(SchülerInDetailPage), typeof(SchülerInDetailPage));

            return builder.Build();
        }
    }
}