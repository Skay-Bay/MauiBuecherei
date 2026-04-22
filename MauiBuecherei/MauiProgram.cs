using MauiBuecherei.Services;
using MauiBuecherei.Views;
using Microsoft.Extensions.Logging;
using System.Text.Json.Nodes;

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
            // HttpClient
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var client = new HttpClient { BaseAddress = new Uri("http://10.0.2.2:5035") };
                client.DefaultRequestHeaders.Add("Accept", "Application/JsonArray");
                return client;
            });

            builder.Services.AddSingleton<ApiClient>();
            builder.Services.AddTransient<SchülerInPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
