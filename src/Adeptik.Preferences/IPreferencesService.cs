namespace Adeptik.Preferences
{
    /// <summary>
    /// Сервис управления настройками
    /// </summary>
    public interface IPreferencesService
    {
        /// <summary>
        /// Возвращает набор настроек с указанным ключом
        /// </summary>
        /// <param name="preferencesKey">Ключ - идентификатор настроек</param>
        /// <param name="throwIfNotFound">Если false, то настройки создаются, если еще не существуют в хранилище настроек; если true - генерируется исключение <see cref="PreferencesNotFoundException"/></param>
        /// <returns>Объект для доступа к содержимому настроек</returns>
        Preferences GetPreferences(string preferencesKey, bool throwIfNotFound = false);
    }
}
