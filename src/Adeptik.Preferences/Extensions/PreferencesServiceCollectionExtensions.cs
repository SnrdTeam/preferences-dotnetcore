using System;
using Microsoft.Extensions.DependencyInjection;
using Adeptik.Preferences.Impl;

namespace Adeptik.Preferences.Extensions
{
    /// <summary>
    /// Методы расширения для <see cref="IServiceCollection"/>
    /// </summary>
    public static class PreferencesServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервиса управления настройками
        /// </summary>
        /// <param name="services">Экземпляр <see cref="IServiceCollection"/></param>
        /// <returns>Экземпляр <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddPreferences(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services
                .AddTransient<IPreferencesService, PreferencesService>();
        }
    }
}
