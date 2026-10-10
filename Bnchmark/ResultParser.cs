using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Xml.Linq;

namespace WukongBenchmarkAutomator
{
    public class BenchmarkResult
    {
        public int FPSAvg { get; set; }
        public int FPSMax { get; set; }
        public int FPSMin { get; set; }
        public int FPS95 { get; set; }
        public string CPUModel { get; set; }
        public string GPUModel { get; set; }
        public string ScreenResolution { get; set; }
        public int QualityLevel { get; set; }
        public int Rtx { get; set; }
        public int Dlss { get; set; }

        public void Print()
        {
            Console.WriteLine();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"   TEST RESULTS   ");
            Console.WriteLine(new string('=', 60));

            Console.WriteLine("  --- System ---");
            Console.WriteLine($"    CPU          : {CPUModel}");
            Console.WriteLine($"    GPU          : {GPUModel}");
            Console.WriteLine($"    Resolution   : {ScreenResolution}");

            Console.WriteLine();
            Console.WriteLine("  --- Graphics ---");
            Console.WriteLine($"    QualityLevel : {QualityLevel}");
            Console.WriteLine($"    RTX          : {(Rtx > 0 ? "on" : "off")}");
            Console.WriteLine($"    DLSS         : {Dlss}");

            Console.WriteLine();
            Console.WriteLine("  --- FPS ---");
            Console.WriteLine($"    Avg FPS  : {FPSAvg}");
            Console.WriteLine($"    Min. FPS     : {FPSMin}");
            Console.WriteLine($"    Max FPS    : {FPSMax}");
            Console.WriteLine($"    FPS95 : {FPS95}");

            Console.WriteLine(new string('=', 60));
            Console.WriteLine();
        }
    }

    public class ResultParser
    {
        public static BenchmarkResult Parse(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
            {
                Console.Error.WriteLine($"Could not find {jsonFilePath}");
                return null;
            }

            string jsonContent = File.ReadAllText(jsonFilePath);
            JObject jsonObject = JObject.Parse(jsonContent);

            var result = new BenchmarkResult
            {
                FPSAvg = (int)jsonObject["FPSAvg"],
                FPSMax = (int)jsonObject["FPSMax"],
                FPSMin = (int)jsonObject["FPSMin"],
                FPS95 = (int)jsonObject["FPS95"],
                CPUModel = (string)jsonObject["CPUModel"],
                GPUModel = (string)jsonObject["GPUModel"],
                ScreenResolution = (string)jsonObject["ScreenResolution"],
                QualityLevel = (int)jsonObject["QualityLevel"],
                Rtx = (int)jsonObject["Rtx"],
                Dlss = (int)jsonObject["Dlss"]
            };

            return result;
        }
    }
}