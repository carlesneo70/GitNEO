using System.Linq;
using System.Configuration;

namespace GitNEO
{
    public static class AppConfigHelper
    {
        private const string SECTION_NAME = "appSettings";

        private static bool IsSectionExist(string sectionName, AppSettingsSection appSetting)
        {
            var keyCount = appSetting.Settings.AllKeys
                                     .Where(key => key == sectionName).Count();

            return keyCount > 0;
        }

        public static string GetValue(string sectionName, string appConfigFile, string defaultValue = "")
        {
            var configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = appConfigFile;

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            var section = (AppSettingsSection)configuration.GetSection(SECTION_NAME);

            var result = string.Empty;

            try
            {
                if (IsSectionExist(sectionName, section))
                    result = section.Settings[sectionName].Value;
                else
                    result = defaultValue;
            }
            catch
            {
            }

            return result;
        }

        public static void SaveValue(string sectionName, string value, string appConfigFile)
        {
            var configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = appConfigFile;

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            var section = (AppSettingsSection)configuration.GetSection(SECTION_NAME);

            try
            {
                if (IsSectionExist(sectionName, section))
                {
                    section.Settings[sectionName].Value = value;
                }
                else
                {
                    section.Settings.Add(sectionName, value);
                }

                configuration.Save(ConfigurationSaveMode.Modified, false);
                ConfigurationManager.RefreshSection(SECTION_NAME);
            }
            catch
            {
            }
        }
    }
}
