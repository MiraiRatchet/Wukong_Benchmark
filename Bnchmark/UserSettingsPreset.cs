using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WukongBenchmarkAutomator
{
    public class SettingsPreset
    {
        public int ResolutionX { get; }
        public int ResolutionY { get; }

        public int ResolutionQuality { get; }
        public int QualityLevel { get; }
        public int Rtx { get; }
        public int Dlss { get; }

        public SettingsPreset(int resolutionX, int resolutionY, int resolutionQuality, int qualityLevel, int rtx, int dlss)
        {
            ResolutionX = resolutionX;
            ResolutionY = resolutionY;
            ResolutionQuality = resolutionQuality;
            QualityLevel = qualityLevel;
            Rtx = rtx;
            Dlss = dlss;
        }
    }

    public static class SettingsPresets
    {
        public static readonly SettingsPreset CPU = new SettingsPreset(
            resolutionX: 1280,
            resolutionY: 720,
            resolutionQuality: 30,
            qualityLevel: 0,
            rtx: 0,
            dlss: 0
        );

        public static readonly SettingsPreset GPU = new SettingsPreset(
            resolutionX: 3840,
            resolutionY: 2160,
            resolutionQuality: 100,
            qualityLevel: 4,
            rtx: 1,
            dlss: 1
        );
    }
}
