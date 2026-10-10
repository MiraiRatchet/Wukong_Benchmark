using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WukongBenchmarkAutomator
{
    class Program
    {
        public static ConfigManager configManager = null;
        public static PathFinder pathFinder = null;

        const string SteamAppId = "3132990";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== Wukong Benchmark Run ===");

                pathFinder = new PathFinder(SteamAppId, "b1_benchmark.exe", "b1");

                string appFileName = pathFinder.FindBenchmarkExeLocation();
                if (string.IsNullOrEmpty(appFileName))
                    return;
                Console.WriteLine($"Wukong Benchmark Tool executable found: {appFileName}");

                configManager = new ConfigManager(pathFinder);
                if (!configManager.BackupOriginalSettings())
                    return;

                Console.WriteLine("Starting benchmark run....");
                var fileName = RunBenchmarkWithConfig(appFileName, ConfigType.CPU);
                if (fileName == null)
                {
                    return;
                }
                BenchmarkResult result = ResultParser.Parse(fileName);
                result.Print();

                Thread.Sleep(1000);

                fileName = RunBenchmarkWithConfig(appFileName, ConfigType.GPU);
                if (fileName == null)
                {
                    return;
                }
                result = ResultParser.Parse(fileName);
                result.Print();

                configManager.RestoreOriginalSettings();

                Console.WriteLine("Benchmark run is successful!");
            }
            finally { Console.ReadKey(); }
        }

        private static string? RunBenchmarkWithConfig(string appName, ConfigType settingsType)
        {

            configManager.ApplyTestSettings(settingsType);

            DateTime startTime = DateTime.Now;
            Process process = RunBenchmark(appName);

            string foundFile = null;
            var deadline = DateTime.Now.AddMinutes(15);

            while (DateTime.Now < deadline)
            {
                Thread.Sleep(5000);

                foundFile = pathFinder.FindNewResultFile(startTime);
                if (foundFile != null)
                {
                    Console.WriteLine($"Benchmark results for {settingsType} settings added: {foundFile}");
                    break;
                }

                if (process.HasExited)
                {
                    Console.WriteLine("Something went wrong, could not retrieve benchmark results. Benchmark process was canceled.");
                    break;
                }
            }

            if (!process.HasExited)
            {
                try
                {
                    if (!process.WaitForExit(5000)) // ждём 5 сек
                    {
                        KillBenchmarkProcess();
                    }
                }
                catch (Exception ex)
                {
                    try { KillBenchmarkProcess(); } catch { /* ignore */ }
                }
            }

            if (foundFile != null)
            {
                Thread.Sleep(1000);
                return foundFile;
            }

            return null;
        }

        static Process? RunBenchmark(string AppName)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(AppName)
            {
                WorkingDirectory = Path.GetDirectoryName(AppName),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            startInfo.Arguments = $"-benchmark";
            startInfo.Environment["SteamAppId"] = SteamAppId;
            startInfo.Environment["SteamGameId"] = SteamAppId;
            Process? process = Process.Start(startInfo);
            return process;
        }

        private static void KillBenchmarkProcess()
        {
            string processName = "b1-Win64-Shipping";
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                Console.WriteLine($"Could no find process '{processName}'.");
                return;
            }

            foreach (var p in processes)
            {
                try
                {
                    Console.WriteLine($"Terminating '{processName}' (PID={p.Id})...");
                    p.Kill();
                    p.WaitForExit(5000);
                    Console.WriteLine($"PID={p.Id} terminated.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not terminate PID={p.Id}: {ex.GetType().Name}: {ex.Message}");
                }
                finally
                {
                    p.Dispose();
                }
               
        }
        }
    }
}