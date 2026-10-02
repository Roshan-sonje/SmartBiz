using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartBiz.Desktop.Services;

namespace SmartBiz.Desktop
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts => { });

            // Simple configuration for development. This can be swapped for file-based or cloud configuration later.
            builder.Configuration.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string,string>("ApiBaseUrl","https://localhost:5001/")
            });

            // Register named HttpClient and API service
            var baseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001/";
            builder.Services.AddHttpClient("SmartBizApi", client => client.BaseAddress = new Uri(baseUrl));
            builder.Services.AddSingleton<IApiService, ApiService>();

            return builder.Build();
        }
    }
}
