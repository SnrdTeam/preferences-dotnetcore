using System;

namespace Adeptik.Preferences.Impl
{
    /// <summary>
    /// Реализация сервиса управления настройками
    /// </summary>
    internal class PreferencesService : IPreferencesService
    {
        private readonly IPreferencesStore _preferencesStore;

        /// <summary>
        /// Создание экземпляра класса <see cref="PreferencesService"/>
        /// </summary>
        /// <param name="preferencesStore"></param>
        public PreferencesService(IPreferencesStore preferencesStore)
        {
            _preferencesStore = preferencesStore ?? throw new ArgumentNullException(nameof(preferencesStore));
        }

        /// <inheritdoc />
        public Preferences GetPreferences(string preferencesKey, bool throwIfNotFound = false)
        {
            return new Preferences(preferencesKey, _preferencesStore, throwIfNotFound);
        }
    }
}
