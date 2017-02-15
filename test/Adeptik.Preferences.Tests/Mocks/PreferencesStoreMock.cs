using System.Collections.Generic;

namespace Adeptik.Preferences.Tests.Mocks
{
    public class PreferencesStoreMock : IPreferencesStore
    {
        private readonly Dictionary<string, string> _store = new Dictionary<string, string>();

        public string GetPreferences(string key)
        {
            string value;
            if (_store.TryGetValue(key, out value))
                return value;
            return null;
        }

        public void SetPreferences(string key, string value)
        {
            if (value == null && _store.ContainsKey(key))
                _store.Remove(key);
            else
                _store[key] = value;
        }
    }
}
