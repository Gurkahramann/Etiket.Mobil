using Microsoft.Extensions.DependencyInjection;
using System;
using Etiket.DtoClient;
using System.Collections.Generic;
using System.Text;

namespace Etiket.DtoClient.IoC
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApiClientService(this IServiceCollection services, Action<ApiClientOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddSingleton<ApiClientService>();

            return services;
        }
    }
}
