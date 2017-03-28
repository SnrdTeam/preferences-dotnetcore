using System;

namespace Adeptik.Preferences.Exceptions
{
    /// <summary>
    /// Настройки не найдены в хранилище
    /// </summary>
    public class PreferencesNotFoundException : Exception
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="PreferencesNotFoundException"/>
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public PreferencesNotFoundException(string message)
            : base(message)
        { }
    }
}
