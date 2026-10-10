using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WukongBenchmarkAutomator
{
    public enum ConfigType : byte
    {
        CPU,
        GPU
    };

    public class ConfigManager
    {

        private static readonly Dictionary<ConfigType, SettingsPreset> PresetMap =
        new Dictionary<ConfigType, SettingsPreset>
        {
            { ConfigType.CPU, SettingsPresets.CPU },
            { ConfigType.GPU, SettingsPresets.GPU }
        };

        private string ConfigFileName;
        private string BackupFileName;

        public ConfigManager(PathFinder pathFinder)
        {
            if (pathFinder == null)
            {
                throw new ArgumentNullException(nameof(pathFinder));
            }
            ConfigFileName = pathFinder.FindConfigFileLocation();
            BackupFileName = ConfigFileName + ".backup";
        }

        public bool BackupOriginalSettings()
        {
            Console.WriteLine($"Backing up original user settings");
            if (!File.Exists(ConfigFileName))
            {
                Console.Error.WriteLine("Could not find user settings. Try running the app manually.");
                return false;
            }

            if (File.Exists(BackupFileName))
            {
                Console.WriteLine("Backup file already exists. Skipping...");
                return true;
            }

            File.Copy(ConfigFileName, BackupFileName);
            Console.WriteLine($"User settings backup created: {BackupFileName}");
            return true;
        }

        public bool RestoreOriginalSettings()
        {

            if (!File.Exists(BackupFileName))
            {
                Console.Error.WriteLine("Backup not found, no user settings to restore.");
                return false;
            }

            File.Copy(BackupFileName, ConfigFileName, overwrite: true);
            Console.WriteLine("Original user settings restored.");
            return true;
 
        }

        public void ApplyTestSettings(ConfigType configType)
        {
            Console.WriteLine($"Applying {configType} Test settings...");
            string content = File.ReadAllText(ConfigFileName);

            var preset = PresetMap[configType];

            content = Regex.Replace(content, @"(?m)^ResolutionSizeX=\d+", $"ResolutionSizeX={preset.ResolutionX}");
            content = Regex.Replace(content, @"(?m)^ResolutionSizeY=\d+", $"ResolutionSizeY={preset.ResolutionY}");

            content = Regex.Replace(content, @"sg\.RayTracingQuality=\d+", $"sg.RayTracingQuality={preset.Rtx}");
            content = Regex.Replace(content, @"\(""Rtx"", ""\d+""\)", $"(\"Rtx\", \"{preset.Rtx}\")");

            string[] qualityKeys =
            {
                "sg.ViewDistanceQuality", "sg.ShadowQuality", "sg.TextureQuality",
                "sg.EffectsQuality", "sg.FoliageQuality", "sg.GlobalIlluminationQuality",
                "sg.ReflectionQuality", "sg.AntiAliasingQuality", "sg.PostProcessQuality",
                "sg.ShadingQuality", "ViewDistance"
            };

            foreach (var key in qualityKeys)
            {
                content = Regex.Replace(content, $@"{Regex.Escape(key)}=\d+",
                    $"{key}={preset.QualityLevel}");
            }
            content = Regex.Replace(content, @"sg\.ResolutionQuality=\d+", $"sg.ResolutionQuality={preset.ResolutionQuality}");

            content = Regex.Replace(content, @"\(""Dlss"", ""\d+""\)", $"(\"Dlss\", \"{preset.Dlss}\")");

            File.WriteAllText(ConfigFileName, content);
            Console.WriteLine($"{configType} test settings applied.");
        }
    }
}