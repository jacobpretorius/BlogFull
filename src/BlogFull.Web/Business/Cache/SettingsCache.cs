using BlogFull.Web.Business.Helpers;
using BlogFull.Web.Models.Data;

namespace BlogFull.Web.Business.Cache
{
    public static class SettingsCache
    {
        private static Settings _settings { get; set; }

        static SettingsCache()
        {
            //fill the cache with settings on load
            _settings = StorageHelper.ReadSettings();
        }

        /// <summary>
        /// Rewrite the settings in memory
        /// </summary>
        /// <param name="settings">The new settings to use</param>
        public static void SetSettings(Settings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Get the setting from memory
        /// </summary>
        /// <returns>The latest settings object</returns>
        public static Settings GetSettings()
        {
            //if there was an issue loading (1st spin of site, retry)
            if (_settings == null)
            {
                _settings = StorageHelper.ReadSettings();
            }

            return _settings;
        }

        /// <summary>
        /// Get the saved author name
        /// </summary>
        /// <returns>Author full name</returns>
        public static string GetAuthor()
        {
            return _settings.AuthorName;
        }
    }
}