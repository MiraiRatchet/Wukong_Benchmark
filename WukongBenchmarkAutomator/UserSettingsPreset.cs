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

        public override string ToString()
        {
            return
                "  ResolutionX          = " + ResolutionX + Environment.NewLine +
                "  ResolutionY          = " + ResolutionY + Environment.NewLine +
                "  ResolutionQuality    = " + ResolutionQuality + Environment.NewLine +
                "  QualityLevel         = " + QualityLevel + Environment.NewLine +
                "  Rtx                  = " + Rtx + Environment.NewLine +
                "  Dlss                 = " + Dlss + Environment.NewLine +
                "  sg.ViewDistanceQuality     = " + QualityLevel + Environment.NewLine +
                "  sg.ShadowQuality           = " + QualityLevel + Environment.NewLine +
                "  sg.TextureQuality          = " + QualityLevel + Environment.NewLine +
                "  sg.EffectsQuality          = " + QualityLevel + Environment.NewLine +
                "  sg.FoliageQuality          = " + QualityLevel + Environment.NewLine +
                "  sg.GlobalIlluminationQuality = " + QualityLevel + Environment.NewLine +
                "  sg.ReflectionQuality       = " + QualityLevel + Environment.NewLine +
                "  sg.AntiAliasingQuality     = " + QualityLevel + Environment.NewLine +
                "  sg.PostProcessQuality      = " + QualityLevel + Environment.NewLine +
                "  sg.ShadingQuality          = " + QualityLevel + Environment.NewLine +
                "  ViewDistance               = " + QualityLevel + Environment.NewLine;
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
