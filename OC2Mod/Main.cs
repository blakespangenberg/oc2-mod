using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;
using UnityModManagerNet;
using Harmony;

/*
 * These are functions that the mod manager will call if implemented
 * 
OnGUI         - Called to draw UI.
OnSaveGUI     - Called while saving.
OnUpdate      - Called by MonoBehaviour.Update.
OnLateUpdate  - Called by MonoBehaviour.LateUpdate.
OnFixedUpdate - Called by MonoBehaviour.FixedUpdate.
OnShowGUI     - Called when opening mod GUI.
OnHideGUI     - Called when closing mod GUI.
*/

namespace OC2Mod
{
    public class Main
    {
        static UnityModManager.ModEntry mod;
        const float HORDE_SPEED_MULTIPLIER    = 1.0f;
        const float DISH_WASH_TIME_MULTIPLIER = 1.0f;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                mod = modEntry;

                var harmony = HarmonyInstance.Create(mod.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                mod.OnUpdate = OnUpdate;
            }
            catch (Exception e)
            {
                mod.Logger.Log("Load Exception:");
                mod.Logger.Log(e.ToString());
                return false;
            }

            return true;
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                //mod.Logger.Log("F1 pressed");
                
            }

            if(Input.GetKeyUp(KeyCode.F1))
            {
                //mod.Logger.Log("F1 released");
                
            }
        }

        /*
         * Speeds up/Slows down dish wash speed
         */
        [HarmonyPatch(typeof(ServerWashingStation))] // Class
        [HarmonyPatch("UpdateSynchronising")]        // Method
        static class FastWash
        {
            static bool Prefix(ref WashingStation ___m_washingStation)
            {
                ___m_washingStation.m_cleanPlateTime = 2.0f * DISH_WASH_TIME_MULTIPLIER;
                return true; // execute original
            }
        }

        /*
         * Increases/Decreases staggering of horde spawn in each wave
         */
        [HarmonyPatch(typeof(GameModes.Horde.ServerHordeFlowController))] // Class
        [HarmonyPatch("NextSpawn")]                                       // Method
        static class FastHorde
        {
            static void Postfix(List<GameModes.Horde.HordeSpawnData> spawns, double waveTime, ref int __result)
            {
                mod.Logger.Log("FastHorde.PostFix()");

                double waveTimeScaled = waveTime*HORDE_SPEED_MULTIPLIER;
                for (int i = 0; i < spawns.Count; i++)
                {
                    if (spawns[i].CanSpawn(waveTimeScaled))
                    {
                        __result = i;
                        return;
                    }
                }

                __result = -1;
            }
        }
    }
}
