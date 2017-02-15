using System;

namespace Adeptik.Preferences.Exceptions
{
    /// <summary>
    /// Настройки не найдены в хранилище
    /// </summary>
    public class PreferencesNotFoundException : Exception
    {
        public PreferencesNotFoundException(string message)
            : base(message)
        { }
    }
}
