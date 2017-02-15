using System;

namespace Adeptik.Preferences.Impl
{
    internal class PreferencesService : IPreferencesService
    {
        private readonly IPreferencesStore _preferencesStore;

        public PreferencesService(IPreferencesStore preferencesStore)
        {
            if (preferencesStore == null)
                throw new ArgumentNullException(nameof(preferencesStore));

            _preferencesStore = preferencesStore;
        }

        public Preferences GetPreferences(string preferencesKey, bool throwIfNotFound = false)
        {
            return new Preferences(preferencesKey, _preferencesStore, throwIfNotFound);
        }
    }
}
