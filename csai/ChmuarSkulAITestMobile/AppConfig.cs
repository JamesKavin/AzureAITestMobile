using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ChmurSkulAITestMobile
{
    public static class AppConfig
    {
        public const string ComputerVisionKey = "ComputerVisionKey";
        public const string TranslatorTextKey = "TranslatorTextKey";
        public const string BingSpeechKey = "BingSpeechKey";

        private const string AppConfigFileName = "App.config";

        private static IEnumerable<XElement> _settings;
        
        public static string Get(string key)
        {
            if (_settings == null)
            {
                InitConfig();
            }

            return _settings.FirstOrDefault(e => e.Attribute("key").Value == key)?.Attribute("value")?.Value;
        }

        private static void InitConfig()
        {
            var assembly = typeof(AppConfig).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("ChmurSkulAITestMobile." + AppConfigFileName);
            var xml = XDocument.Load(stream);
            var configNode = xml.Descendants("configuration");
            var appSettingsNode = configNode.Descendants("appSettings");

            _settings = appSettingsNode.Elements("add");
        }
    }
}
