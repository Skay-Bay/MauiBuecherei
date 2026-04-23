using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using MauiBuecherei.Services;
using MauiBuecherei.ViewModels;

namespace MauiBuecherei
{
    public static class MauiProgram
    {
        // MauiProgram.cs
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

            // HttpClient für API
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var client = new HttpClient
                {
                    BaseAddress = new Uri("http://10.0.2.2:5035/api/"),
                    Timeout = TimeSpan.FromSeconds(10)
                };
                return client;
            });

            // Services
            builder.Services.AddSingleton<SchülerInApiService>();
            builder.Services.AddSingleton<BuchApiService>();
            builder.Services.AddSingleton<AusleiheApiService>();
            builder.Services.AddSingleton<StatistikApiService>();

            // ViewModels
            builder.Services.AddTransient<SchülerInListViewModel>();
            builder.Services.AddTransient<BuchListViewModel>();
            builder.Services.AddTransient<AusleiheErstellenViewModel>();
            builder.Services.AddTransient<AusleiheListViewModel>();
            builder.Services.AddTransient<StatistikViewModel>();
            builder.Services.AddTransient<SearchViewModel>();

            // Pages
            builder.Services.AddTransient<SchülerInListPage>();
            builder.Services.AddTransient<SchülerInDetailPage>();
            builder.Services.AddTransient<BuchListPage>();
            builder.Services.AddTransient<BuchDetailPage>();
            builder.Services.AddTransient<AusleiheListPage>();
            builder.Services.AddTransient<AusleiheErstellenPage>();
            builder.Services.AddTransient<StatistikPage>();
            builder.Services.AddTransient<SearchPage>();

            return builder.Build();
        }
    }
}