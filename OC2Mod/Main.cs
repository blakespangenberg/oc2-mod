using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;
using UnityModManagerNet;
using Harmony;
using System.Collections;

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
        const float DISH_WASH_TIME_MULTIPLIER    = 0.5f; // larger means longer wash time
        const float HORDE_SPAWN_SPEED_MULTIPLIER = 2.2f; // larger means less stagger

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

                double waveTimeScaled = waveTime* HORDE_SPAWN_SPEED_MULTIPLIER;
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

        const float HORDE_PLATE_RETURN_TIME_MULTIPLIER       = 0.7f; // untested
        const float HORDE_TARGET_HEALTH_MULTIPLIER           = 1.0f; // untested
        // const float HORDE_TARGET_REPAIR_SPEED_MULTIPLIER     = 1.0f; // broken
        const float HORDE_TARGET_REPAIR_THRESHOLD_MULTIPLIER = 1.0f; // untested
        const float HORDE_TARGET_REPAIR_COST_MULTIPLIER      = 0.5f; // untested
        const float HORDE_HEALTH_MULTIPLIER                  = 1.0f; // untested
        
        // const float HORDE_ENEMY_COUNT_MULTIPLIER = 1.0f;

        [HarmonyPatch(typeof(GameUtils))]                             // Class
        [HarmonyPatch("GetLevelConfig")]                              // MethodPostfix
        static class GetLevelConfigPatch
        {
            static void Postfix(ref LevelConfigBase __result)
            {
                // mod.Logger.Log("GetLevelConfigPatch.Postfix()");
                
                GameModes.Horde.HordeLevelConfig result = (__result as GameModes.Horde.HordeLevelConfig);
                if (result == null) return; // It's not a horde level

                result.m_plateReturnTime        =        10f * HORDE_PLATE_RETURN_TIME_MULTIPLIER;
                result.m_targetHealth           = (int) (100 * HORDE_TARGET_HEALTH_MULTIPLIER);
                // result.m_targetRepairSpeed      =       0.5f * HORDE_TARGET_REPAIR_SPEED_MULTIPLIER;
                result.m_targetRepairThreshold  = (int) (10f * HORDE_TARGET_REPAIR_THRESHOLD_MULTIPLIER);
                result.m_targetRepairCostMax    = (int) (200 * HORDE_TARGET_REPAIR_COST_MULTIPLIER);
                result.m_health                 = (int) (100 * HORDE_HEALTH_MULTIPLIER);

                /*
                // waves
                for (int i = 0; i < result.m_waves.Count; i++)
                {
                    // spawns for each wave
                    for(int j = 0; j < result.m_waves[i].m_spawns.Count * HORDE_ENEMY_COUNT_MULTIPLIER; j++)
                    {
                        result.m_waves[i].m_spawns.Add(result.m_waves[i].m_spawns[j % result.m_waves[i].m_spawns.Count]);
                    }
                }
                */
            }
        }

        const float HORDE_ENEMY_KITCH_ATTACK_SPEED_MULTIPLIER  = 0.3f; // larger means slower attack rate
        const float HORDE_ENEMY_TARGET_DAMAGE_MULTIPLIER       = 1.0f; // larger means more damage
        const float HORDE_ENEMY_TARGET_ATTACK_SPEED_MULTIPLIER = 0.5f; // larger means slower attack rate
        const float HORDE_ENEMY_KITCH_DAMAGE_MULTIPLIER        = 0.5f; // larger means more damage
        const int   HORDE_ENEMY_RECIPIE_COUNT                  = 1; // doesn't work
        const float HORDE_ENEMY_MOVEMENT_SPEED_MULTIPLIER      = 0.4f; // larger means move to window faster
        [HarmonyPatch(typeof(GameModes.Horde.ServerHordeEnemy))]     // Class
        [HarmonyPatch("OnUpdateState")]                              // MethodPostfix
        static class HordeEnemyPath
        {
            static bool Prefix(ref GameModes.Horde.HordeEnemy ___m_enemy)
            {
                ___m_enemy.m_attackKitchenFrequencySeconds = 5f * HORDE_ENEMY_KITCH_ATTACK_SPEED_MULTIPLIER;
                ___m_enemy.m_targetDamage = (int) (25 * HORDE_ENEMY_TARGET_DAMAGE_MULTIPLIER);
                ___m_enemy.m_attackTargetFrequencySeconds = 5f * HORDE_ENEMY_TARGET_ATTACK_SPEED_MULTIPLIER;
                ___m_enemy.m_kitchenDamage = (int) (10 * HORDE_ENEMY_KITCH_DAMAGE_MULTIPLIER);
                ___m_enemy.m_recipeCount   = HORDE_ENEMY_RECIPIE_COUNT;
                ___m_enemy.m_movementSpeed = 1f * HORDE_ENEMY_MOVEMENT_SPEED_MULTIPLIER;

                return true;
            }
        }
    }
}
