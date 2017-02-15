using Adeptik.Preferences;
using Adeptik.Preferences.Exceptions;
using Adeptik.Preferences.Extensions;
using Adeptik.Preferences.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Tests
{
    public class PreferencesServiceTests
    {
        private const string ExistingPrefsKey = "key";
        private const string ExistingPrefsKey_Inversed = "KEY";
        private const string NonExistingPrefsKey = "key_not_exists";

        [Theory]
        [InlineData("", false)]
        [InlineData(ExistingPrefsKey, true)]
        [InlineData(NonExistingPrefsKey, false)]
        [InlineData(ExistingPrefsKey_Inversed, false)]
        public void GetPreferencesTest(string key, bool exists)
        {
            var preferencesService = InitializePreferencesService();
            var preferences = preferencesService.GetPreferences(key);
            Assert.NotNull(preferences);
            if (!exists)
            {
                Assert.Throws<PreferencesNotFoundException>(() =>
                {
                    preferencesService.GetPreferences(key, true);
                });
            }
            else
            {
                var preferences2 = preferencesService.GetPreferences(key);
                Assert.NotNull(preferences2);
            }
        }

        [Fact]
        public void GetPreferencesWithNullKeyTest()
        {
            var preferencesService = InitializePreferencesService();
            Assert.Throws<ArgumentNullException>(() =>
            {
                preferencesService.GetPreferences(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                preferencesService.GetPreferences(null, true);
            });
        }

        private static IPreferencesService InitializePreferencesService()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IPreferencesStore, PreferencesStoreMock>();
            serviceCollection.AddPreferences();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var preferencesService = serviceProvider.GetRequiredService<IPreferencesService>();

            var prefs = preferencesService.GetPreferences(ExistingPrefsKey);
            prefs
                .Edit()
                .Set("propString", "value1")
                .Set("propInt", 42)
                .Save();

            return preferencesService;
        }
    }
}
