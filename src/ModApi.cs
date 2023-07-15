using HarmonyLib;
using ProtectHighTierPrefabs.Utilities;
using System;
using System.Reflection;

namespace ProtectHighTierPrefabs
{
    internal class ModApi : IModApi
    {
        private static readonly ModLog<ModApi> _log = new ModLog<ModApi>();

        public static bool DebugMode { get; set; } = false;

        public void InitMod(Mod _modInstance)
        {
            var harmony = new Harmony(GetType().ToString());
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            ModEvents.GameStartDone.RegisterHandler(OnGameStartDone);
            ModEvents.GameShutdown.RegisterHandler(OnGameShutdown);
        }

        private void OnGameStartDone()
        {
            try
            {
                ZoneManager.OnGameStartDone();
            }
            catch (Exception e)
            {
                _log.Error("OnGameStartDone", e);
            }
        }

        private void OnGameShutdown()
        {
            try
            {
                ZoneManager.OnGameShutdown();
            }
            catch (Exception e)
            {
                _log.Error("OnGameStartDone", e);
            }
        }
    }
}
