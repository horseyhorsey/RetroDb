using System;
using System.Collections.Generic;
using System.IO;

namespace RetroDb.Data.Frontend.RocketLauncher
{
    public class Settings
    {
        public string DefaultEmulator { get; set; }
        public IEnumerable<string> RomExtensions { get; set; }
        public IEnumerable<string> RomPaths { get; set; }
    }

    public class Stat
    {
        public string GlobalStatKey { get; set; }
        public string Rom { get; set; }
        public string StatsPath { get; set; }
        public string SystemName { get; set; }

        public DateTime LastTimePlayed { get; set; }
        public int TimesPlayed { get; set; }
        public TimeSpan AvgTimePlayed { get; set; }
        public TimeSpan TotalOverallTime;
        public TimeSpan TotalTimePlayed { get; set; }

        public override string ToString()
        {
            return $"{SystemName} - {Rom} + {TimesPlayed}";
        }
    }

    public enum GeneralStat
    {
        General,
        TopTen_Time_Played,
        TopTen_Times_Played,
        Top_Ten_Average_Time_Played,
        Number_of_Times_Played,
        Last_Time_Played,
        Average_Time_Played,
        Total_Time_Played
    }

    public static class MediaPaths
    {
        public const string _Default = "_Default";
        public const string Artwork = "Artwork";
        public const string Backgrounds = "Backgrounds";
        public const string Bezels = "Bezels";
        public const string Controller = "Controller";
        public const string Fade = "Fade";
        public const string Guides = "Guides";
        public const string Manuals = "Manuals";
        public const string MultiGame = "MultiGame";
        public const string Music = "Music";
        public const string SavedGames = "Saved Games";
        public const string Video = "Videos";
        public const string Wheels = "Wheels";
    }

    public static class LauncherPaths
    {
        public const string Settings = "Settings";
        public const string Statistics = @"Data\Statistics";
    }

    public static class EmulatorPaths
    {
        /// <summary>
        /// Gets the global emulators.ini config for this system
        /// </summary>
        /// <param name="rlPath"></param>
        /// <param name="systemName"></param>
        /// <returns></returns>
        public static string GlobalEmulatorConfig(string rlPath) => Path.Combine(rlPath, LauncherPaths.Settings, "Global Emulators.ini");

        /// <summary>
        /// Gets the emulators.ini config for this system
        /// </summary>
        /// <param name="rlPath"></param>
        /// <param name="systemName"></param>
        /// <returns></returns>
        public static string EmulatorConfig(string rlPath, string systemName) => Path.Combine(rlPath, LauncherPaths.Settings, systemName, "Emulators.ini");
    }    
}
