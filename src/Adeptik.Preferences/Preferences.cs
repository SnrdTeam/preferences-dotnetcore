using Adeptik.Preferences.Exceptions;
using Newtonsoft.Json.Linq;
using System;

namespace Adeptik.Preferences
{
    /// <summary>
    /// Класс для управления настройками
    /// </summary>
    public class Preferences
    {
        private readonly string _key;
        private readonly IPreferencesStore _preferencesStore;
        private JObject _preferencesJson;
        private readonly object _editLock = new object();

        /// <summary>
        /// Создание экземпляра класса <see cref="Preferences"/>
        /// </summary>
        /// <param name="key">Идентификатор настроек</param>
        /// <param name="preferencesStore">Хранилище настроек</param>
        /// <param name="throwIfNotFound">Если true, то генерируется исключение при получении настроек из хранилища</param>
        internal Preferences(string key, IPreferencesStore preferencesStore, bool throwIfNotFound)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key));
            _preferencesStore = preferencesStore ?? throw new ArgumentNullException(nameof(preferencesStore));

            var preferencesJson = _preferencesStore.GetPreferences(key);
            if (throwIfNotFound && preferencesJson == null)
                throw new PreferencesNotFoundException($"Preferences with key \"{key}\" is not found");
            _preferencesJson = preferencesJson != null ? JObject.Parse(preferencesJson) : new JObject();
        }

        /// <summary>
        /// Получение значения настройки
        /// </summary>
        /// <typeparam name="T">Тип значения настройки</typeparam>
        /// <param name="path">Путь к настройке: цепочка имен настроек, разделенная ":"</param>
        /// <param name="defaultValue">Значение по умолчанию, возвращаемое если настройка не найдена</param>
        /// <returns>Значение настройки</returns>
        public T Get<T>(string path, T defaultValue = default(T))
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            lock (_editLock)
            {
                var token = GetValueToken(_preferencesJson, path, false);
                return token != null ? token.ToObject<T>() : defaultValue;
            }
        }

        /// <summary>
        /// Получение значения настройки
        /// </summary>
        /// <typeparam name="T">Тип значения настройки</typeparam>
        /// <param name="defaultValue">Значение по умолчанию, возвращаемое если настройка не найдена</param>
        /// <returns>Значение настройки</returns>
        public T Get<T>(T defaultValue = default(T)) where T : class
        {
            lock (_editLock)
            {
                if (_preferencesJson == null)
                    return defaultValue;
                return _preferencesJson.ToObject<T>();
            }

        }

        /// <summary>
        /// Редактирование настроек
        /// </summary>
        /// <returns>Объект редактирования настроек</returns>
        public PreferencesEditor Edit()
        {
            return new PreferencesEditor(this);
        }

        private static JToken GetValueToken(JToken token, string path, bool generateMissingNodes)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            JToken currentNode = token;
            foreach (var part in path.Split(':'))
            {
                var newNode = currentNode[part];
                if (newNode == null)
                {
                    if (generateMissingNodes)
                    {
                        newNode = JValue.CreateNull();
                        currentNode[part] = newNode;
                    }
                    else
                        return null;
                }
                currentNode = newNode;
            }

            return currentNode;
        }

        /// <summary>
        /// Класс для редактирования настроек
        /// </summary>
        public class PreferencesEditor
        {
            private readonly Preferences _preferences;
            private readonly JObject _editPreferencesJson;

            /// <summary>
            /// Создание экземпляра класса <see cref="PreferencesEditor"/>
            /// </summary>
            /// <param name="preferences">Редактируемые настройки</param>
            internal PreferencesEditor(Preferences preferences)
            {
                _preferences = preferences;
                _editPreferencesJson = _preferences._preferencesJson != null ? new JObject(_preferences._preferencesJson) : new JObject();
            }

            /// <summary>
            /// Установка значения настройки
            /// </summary>
            /// <typeparam name="T">Тип значения настройки</typeparam>
            /// <param name="path">Путь к настройке: цепочка имен настроек, разделенная ":".</param>
            /// <param name="value">Значение настройки</param>
            /// <returns>Этот объект</returns>
            public PreferencesEditor Set<T>(string path, T value)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException(nameof(path));

                GetValueToken(_editPreferencesJson, path, true).Replace(value != null ? JToken.FromObject(value) : JValue.CreateNull());
                return this;
            }

            /// <summary>
            /// Установка значения настройки
            /// </summary>
            /// <typeparam name="T">Тип значения настройки</typeparam>
            /// <param name="value">Значение настройки</param>
            /// <returns>Этот объект</returns>
            public PreferencesEditor Set<T>(T value) where T : class
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                _editPreferencesJson.ReplaceAll(JObject.FromObject(value));
                return this;
            }

            /// <summary>
            /// Удаление значения настройки
            /// </summary>
            /// <param name="path">Путь к настройке: цепочка имен настроек, разделенная ":".</param>
            /// <returns>Этот объект</returns>
            public PreferencesEditor Clear(string path)
            {
                if (string.IsNullOrEmpty(path))
                    throw new ArgumentNullException(nameof(path));

                var token = GetValueToken(_editPreferencesJson, path, false);
                if (token != null)
                    token.Parent.Remove();

                return this;
            }

            /// <summary>
            /// Удаление значения настройки
            /// </summary>
            /// <returns>Этот объект</returns>
            public PreferencesEditor Clear()
            {
                _editPreferencesJson.RemoveAll();
                return this;
            }

            /// <summary>
            /// Сохранение внесенных изменений
            /// </summary>
            public void Save()
            {
                lock (_preferences._editLock)
                {
                    _preferences._preferencesStore.SetPreferences(_preferences._key, _editPreferencesJson.ToString(Newtonsoft.Json.Formatting.None));
                    _preferences._preferencesJson = new JObject(_editPreferencesJson);
                }
            }
        }
    }
}
