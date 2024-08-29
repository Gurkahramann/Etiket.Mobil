using Etiket.DtoClient;
using Microsoft.Extensions.Logging;
using Etiket.DtoClient.IoC;
using Etiket.MAUI.Pages;

namespace Etiket.MAUI
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
            builder.Services.AddApiClientService(options =>
            {
                options.ApiBaseAddress = "http://192.168.1.108:5281/";
            });
            builder.Services.AddSingleton<ApiClientService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<PrinterConfig>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
