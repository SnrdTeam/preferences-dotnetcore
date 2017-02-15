using Adeptik.Preferences.Extensions;
using Adeptik.Preferences.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Adeptik.Preferences.Tests
{
    public class PreferencesTests
    {
        [Fact]
        public void PreferencesGetValueTest()
        {
            var prefs = InitializePreferences();
            PreferencesGetValueTestInternal(prefs);
        }

        private void PreferencesGetValueTestInternal(Preferences prefs)
        {
            var strVal = prefs.Get<string>(path: "propStringNull");
            Assert.Null(strVal);
            var intVal = prefs.Get<int?>(path: "propIntNull");
            Assert.Null(intVal);
            strVal = prefs.Get<string>(path: "propString");
            Assert.Equal("value1", strVal);
            var int32Val = prefs.Get<int>("propInt32");
            Assert.Equal(42, int32Val);
            var int64Val = prefs.Get<long>("propInt64");
            Assert.Equal(420L, int64Val);
            var doubleVal = prefs.Get<double>("propDouble");
            Assert.Equal(42.546841d, doubleVal);

            var prefVal = prefs.Get<TestPreference>("propClass");
            Assert.Equal(4200, prefVal.propInt32);
            Assert.Equal("value11", prefVal.propString);

            prefVal = prefs.Get<TestPreference>();
            Assert.Equal(42, prefVal.propInt32);
            Assert.Equal("value1", prefVal.propString);

            var nonExistentVal = prefs.Get("nonexistent", "default value");
            Assert.Equal("default value", nonExistentVal);

            Assert.Throws<FormatException>(() =>
            {
                prefs.Get<int>(path: "propString");
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                prefs.Get<int>(path: null);
            });
        }

        [Fact]
        public void PreferencesGetValueByRouteTest()
        {
            var prefs = InitializePreferences();

            var strVal = prefs.Get<string>(path: "propClass:propString");
            Assert.Equal("value11", strVal);
        }

        [Fact]
        public void PreferencesSetValueTest()
        {
            var prefs = InitializePreferences();

            prefs
               .Edit()
               .Set("propString", "value1")
               .Set("propInt32", 42)
               .Save();
            PreferencesGetValueTestInternal(prefs);

            Assert.Throws<ArgumentNullException>(() =>
            {
                prefs
                .Edit()
                .Set<string>(null)
                .Save();
            });
        }

        [Fact]
        public void PreferencesClearTest()
        {
            var prefs = InitializePreferences();
            Assert.NotNull(prefs.Get<string>(path: "propString"));
            prefs
                .Edit()
                .Clear("propString")
                .Clear("nonexistent")
                .Save();
            Assert.Null(prefs.Get<string>(path: "propString"));

            prefs
                .Edit()
                .Clear("propInt32");
            Assert.Equal(42, prefs.Get<int>("propInt32"));

            prefs
                .Edit()
                .Clear()
                .Save();
            var longVal = prefs.Get("propInt64", 11L);
            Assert.Equal(11, longVal);
        }

        private static Preferences InitializePreferences()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IPreferencesStore, PreferencesStoreMock>();
            serviceCollection.AddPreferences();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var preferencesService = serviceProvider.GetRequiredService<IPreferencesService>();

            var existingPrefsKey = "key";

            var prefs = preferencesService.GetPreferences(existingPrefsKey);
            prefs
                .Edit()
                .Set("propStringNull", (string)null)
                .Set("propIntNull", (int?)null)
                .Set("propString", "value1")
                .Set("propInt32", 42)
                .Set("propInt64", 420L)
                .Set("propDouble", 42.546841d)
                .Set("propClass", new TestPreference { propInt32 = 4200, propString = "value11" })
                .Save();

            return preferencesService.GetPreferences(existingPrefsKey);
        }
    }
}
