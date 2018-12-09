using RetroDb.Data.Extensions;
using RetroDb.Data.Frontend.RocketLauncher;
using RetroDb.Data.Helper;
using RetroDb.Engine.Win;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetroDb.Engine.Frontends
{
    public class RocketLauncher
    {
        private static Process _process;
        private static ProcessStartInfo _startInfo;
        private static IniFile ini = new IniFile();
        string[] _genStats = { "General", "TopTen_Time_Played", "TopTen_Times_Played", "Top_Ten_Average_Time_Played" };
        public Settings CurrentSystemSettings { get; private set; }
        public string InstallPath { get; }
        #region Constructors
        static RocketLauncher()
        {
            if (_process == null)
                _process = new Process();
        }

        public RocketLauncher(string rlInstallPath)
        {           
            InstallPath = rlInstallPath;
            CurrentSystemSettings = new Settings();
            Setup();            
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets all Rocketlauncher stats for a system
        /// </summary>
        /// <param name="rlPath"></param>
        /// <param name="systemName"></param>
        /// <returns></returns>
        public Task<IEnumerable<Stat>> GetStatsAsync(string systemName)
        {
            return Task.Run(() =>
            {
                var iniPath = Path.Combine(InstallPath, LauncherPaths.Statistics, systemName + ".ini");
                if (!File.Exists(iniPath)) throw new FileNotFoundException($"Rocket launcher ini stat file not found: {iniPath}");

                var _ini = new IniFile();
                var statList = new List<Stat>();

                _ini.Load(iniPath);
                int count = _ini.Sections.Count;

                foreach (IniFile.IniSection s in _ini.Sections)
                {
                    string section = s.Name.ToString();
                    if (_genStats[0] != section)
                        if (_genStats[1] != section)
                            if (_genStats[2] != section)
                                if (_genStats[3] != section)
                                {
                                    if (section == GeneralStat.General.ToString())
                                    {
                                    }
                                    else
                                    {
                                        var gameStat = new Stat();
                                        gameStat.Rom = section;

                                        gameStat.SystemName = systemName;
                                        try { gameStat.TimesPlayed = Convert.ToInt32(_ini.GetKeyValue(section, "Number_of_Times_Played")); }
                                        catch { }

                                        try { gameStat.LastTimePlayed = Convert.ToDateTime(_ini.GetKeyValue(section, "Last_Time_Played")); }
                                        catch { }

                                        try
                                        {
                                            var avgTime = TimeSpan.Parse(_ini.GetKeyValue(section, "Average_Time_Played")).Days;
                                            gameStat.AvgTimePlayed = new TimeSpan(0, 0, avgTime);
                                        }
                                        catch { }

                                        try
                                        {
                                            var TotalTime = TimeSpan.Parse(_ini.GetKeyValue(section, "Total_Time_Played")).Days;
                                            gameStat.TotalTimePlayed = new TimeSpan(0, 0, TotalTime);
                                            gameStat.TotalOverallTime = gameStat.TotalOverallTime + gameStat.TotalTimePlayed;
                                        }
                                        catch { }

                                        statList.Add(new Stat
                                        {
                                            AvgTimePlayed = gameStat.AvgTimePlayed,
                                            LastTimePlayed = gameStat.LastTimePlayed,
                                            TimesPlayed = gameStat.TimesPlayed,
                                            Rom = gameStat.Rom,
                                            TotalTimePlayed = gameStat.TotalTimePlayed,
                                            TotalOverallTime = gameStat.TotalOverallTime,
                                            SystemName = gameStat.SystemName
                                        });
                                    }
                                }
                }

                return statList.AsEnumerable();
            });
        }

        public void Launch(string gameName, string systemName)
        {

            gameName = gameName.WrapInQuotes();
            systemName = systemName.WrapInQuotes();
            _startInfo.Arguments = $@"-s {systemName} -r {gameName}";

            //TODO: enable the frontend launched from
            //_startInfo.Arguments += $@" -f ";

            _process.Start();
            _process.WaitForExit();
        }

        public Task<int> NavigatePause(string pauseCommand)
        {
            return Task.Run(() =>
            {
                try
                {
                    return SendBroadcastMessage(pauseCommand);
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        public Task<int> Pause()
        {
            return Task.Run(() =>
            {
                try
                {
                    return SendBroadcastMessage(Commands.RL_PAUSE);
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        /// <summary>
        /// Quits Rocketlauncher if running by using the Broadcast system
        /// </summary>
        /// <returns></returns>
        public Task<int> Quit()
        {
            return Task.Run(() =>
            {
                try
                {
                    return SendBroadcastMessage(Commands.RL_EXIT);
                }
                catch (Exception)
                {                    
                    throw;
                }      
            });            
        }

        public async Task SetCurrentSystemAsync(string systemName)
        {
            this.CurrentSystemSettings = await this.GetSystemSettingsAsync(systemName);
        }

        #endregion

        #region private Methods       

        private IntPtr GetRocklauncherMessageHwnd()
        {
            return Win.Window.FindWindow(null, "RocketLauncherMessageReceiver");
        }

        private IEnumerable<string> GetRomPaths(string[] paths)
        {                        
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].Contains(@"..\") || paths[i].Contains(@".\"))
                {
                    var romPath = new Uri(paths[i], UriKind.RelativeOrAbsolute);
                    var comb = Path.Combine(InstallPath, romPath.OriginalString);
                    var combined = new Uri(comb, UriKind.RelativeOrAbsolute);
                    paths[i] = combined.LocalPath;
                }
            }

            return paths;
        }

        private Task<Settings> GetSystemSettingsAsync(string systemName)
        {
            var settings = new Settings();
            return Task.Run(() =>
            {
                var globalIni = EmulatorPaths.GlobalEmulatorConfig(InstallPath);
                var sysEmuIni = EmulatorPaths.EmulatorConfig(InstallPath, systemName);
                if (!File.Exists(globalIni)) throw new FileNotFoundException($"{systemName} Global launcher ini stat file not found: {globalIni}");
                if (!File.Exists(sysEmuIni)) throw new FileNotFoundException($"{systemName} Emulator ini stat file not found: {sysEmuIni}");

                ini.Load(sysEmuIni);
                string section = "ROMS";
                //Emulator                
                string key = "Default_Emulator";
                settings.DefaultEmulator = ini.GetKeyValue(section, key) ?? string.Empty;

                //Get rom paths
                key = "Rom_Path";
                var dirs = ini.GetKeyValue(section, key)?.Split('|')?.ToArray();
                settings.RomPaths = GetRomPaths(dirs) ?? new List<string>();

                //Global
                ini.Load(globalIni);
                section = settings.DefaultEmulator;
                key = "Rom_Extension";
                settings.RomExtensions = ini.GetKeyValue(section, key)?.Split('|')?.AsEnumerable() ?? new List<string>();

                return settings;
            });

        }
        private int SendBroadcastMessage(string message)
        {
            var command = message;
            var cpds = new COPYDATASTRUCT()
            {
                dwData = new IntPtr(3),
                cbData = command.Length + 1,
                lpData = command
            };

            return Win.Window.SendMessage(GetRocklauncherMessageHwnd(), Win.Window.WM_COPYDATA, 0, ref cpds);
        }

        /// <summary>
        /// Sets up the Process to be used to launch RL
        /// </summary>
        private void Setup()
        {
            if (_startInfo == null)
            {
                _startInfo = new ProcessStartInfo();
                _startInfo.UseShellExecute = false;
                _startInfo.WorkingDirectory = InstallPath;
                _startInfo.FileName = InstallPath + "\\Rocketlauncher.exe";
                _process.StartInfo = _startInfo;
            }            
        }
        #endregion        
    }
}
