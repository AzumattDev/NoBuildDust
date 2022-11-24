using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace NoBuildDust
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class NoBuildDustPlugin : BaseUnityPlugin
    {
        internal const string ModName = "NoBuildDust";
        internal const string ModVersion = "1.0.0";
        internal const string Author = "Azumatt";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;

        internal static string ConnectionError = "";

        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource NoBuildDustLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);
        

        public void Awake()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                NoBuildDustLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                NoBuildDustLogger.LogError($"There was an issue loading your {ConfigFileName}");
                NoBuildDustLogger.LogError("Please check your config entries for spelling and format!");
            }
        }
    }
}