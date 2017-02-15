namespace Adeptik.Preferences
{
    /// <summary>
    /// Хранилище настроек
    /// </summary>
    public interface IPreferencesStore
    {
        /// <summary>
        /// Возвращает значение настроек для указанного ключа
        /// </summary>
        /// <param name="key">Ключ - идентификатор настроек</param>
        /// <returns>Строковое представление настроек по указанному ключу или null, если настроек с указанным ключом не обнаружено</returns>
        string GetPreferences(string key);

        /// <summary>
        /// Установка нового значения настроек с указынным ключом
        /// </summary>
        /// <remarks>
        /// Если настроек с указанным ключом не обнаружено, то создается новый.
        /// Если в качестве значение указано null, то настройки удаляются из хранилища
        /// </remarks>
        /// <param name="key">Ключ - идентификатор настроек</param>
        /// <param name="value">Строковое представление настроек</param>
        void SetPreferences(string key, string value);
    }
}
