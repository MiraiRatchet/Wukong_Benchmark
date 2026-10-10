using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace WukongBenchmarkAutomator
{
    public class PathFinder
    {
        private string ExeLauncherName;

        private string DevName;
        private string SteamAppId;

        private static string CachedAppPath = null;

        public PathFinder(string appId, string launcherName, string devName) {
            DevName = devName;
            SteamAppId = appId;
            ExeLauncherName = launcherName;
        }

        public string? FindConfigFileLocation()
        {
            string path = FindBenchmarkDirectory();
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var fileName = Path.Combine(path, DevName, "Saved", "Config", "Windows", "GameUserSettings.ini");
            if (File.Exists(fileName))
            {
                return fileName;
            }
            Console.Error.WriteLine("User settings file is not found");
            return null;
        }

        public string? FindBenchmarkExeLocation()
        {
            string path = FindBenchmarkDirectory();
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var fileName = Path.Combine(path, ExeLauncherName);
            if (File.Exists(fileName))
            {
                return fileName;
            }
            Console.Error.WriteLine($"Could not find Black Myth: Wukong Benchmark Tool. Check app installation.");
            return null;
        }

        private string? GetBenchmarkHistoryPath() 
        {
            var fileName = Path.Combine(Path.GetTempPath(), DevName, "BenchMarkHistory", "Tool");
            if (Directory.Exists(fileName))
            {
                return fileName;
            }
            Console.Error.WriteLine("Benchmark history folder is not found");
            return null;
        }

        private static string? GetSteamDirectory()
        {
            var path = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null) as string
                        ?? Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null) as string;

            if (string.IsNullOrEmpty(path))
            {
                Console.Error.WriteLine("Steam directory is not found");
                return null;
            }
            return Path.GetFullPath(path.Replace('/', '\\'));
        }

        private string? FindBenchmarkDirectory()
        {
            if (CachedAppPath != null)
            {
                return CachedAppPath;
            }
            string steamDir = GetSteamDirectory();
            if (string.IsNullOrEmpty(steamDir))
            {
                return null;
            }

            string? appPath = FindAppPath(steamDir, SteamAppId);
            if (appPath == null)
            {
                Console.Error.WriteLine(
                    $"Could not find Black Myth: Wukong Benchmark Tool. Check app installation.");
                return null;
            }

            CachedAppPath = appPath;
            return CachedAppPath;
        }

        private string FindAppPath(string steamDir, string appId)
        {
            foreach (var lib in GetLibraries(steamDir).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                string manifest = Path.Combine(lib, "steamapps", $"appmanifest_{appId}.acf");
                if (!File.Exists(manifest))
                    continue;

                string? installDir = ReadInstallDir(manifest);
                if (installDir == null)
                    continue;

                string appPath = Path.Combine(lib, "steamapps", "common", installDir);
                if (Directory.Exists(appPath))
                    return appPath;
            }

            return null;
        }

        private string ReadInstallDir(string manifest)
        {
            Dictionary<string, object> root = VdfParser.Parse(File.ReadAllText(manifest));

            object stateObj;
            if (!root.TryGetValue("AppState", out stateObj))
                return null;

            var state = stateObj as Dictionary<string, object>;
            if (state == null)
                return null;

            object dirObj;
            if (!state.TryGetValue("installdir", out dirObj))
                return null;

            return dirObj as string;
        }

        private List<string> GetLibraries(string steamDir)
        {
            var libraries = new List<string> { steamDir };

            string vdfPath = Path.Combine(steamDir, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdfPath))
                return libraries;

            Dictionary<string, object> root = VdfParser.Parse(File.ReadAllText(vdfPath));

            object foldersObj;
            if (!root.TryGetValue("libraryfolders", out foldersObj))
                return libraries;

            var folders = foldersObj as Dictionary<string, object>;
            if (folders == null)
                return libraries;

            foreach (object entryObj in folders.Values)
            {
                var lib = entryObj as Dictionary<string, object>;
                if (lib == null) continue;

                object pathObj;
                if (!lib.TryGetValue("path", out pathObj)) continue;

                string path = pathObj as string;
                if (string.IsNullOrWhiteSpace(path)) continue;

                libraries.Add(path);
            }

            return libraries;
        }

        public string FindNewResultFile(DateTime startTime)
        {
            var benchmarkHistoryPath = GetBenchmarkHistoryPath();
            if (string.IsNullOrEmpty(benchmarkHistoryPath)) return null;

            var candidate = new DirectoryInfo(benchmarkHistoryPath)
                .GetFiles()                                            
                .Where(f => string.IsNullOrEmpty(f.Extension))      
                .Where(f => long.TryParse(f.Name, out _))              
                .Where(f => f.LastWriteTime > startTime)               
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();

            return candidate?.FullName;
        }
    }
}
