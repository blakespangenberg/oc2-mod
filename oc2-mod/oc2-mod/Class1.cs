using System;
using System.Reflection;

using UnityEngine;
using Harmony;
using UnityModManagerNet;

namespace DifficultyOptions
{
    static class Main
    {
        public static UnityModManager.ModEntry mod;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            mod = modEntry;
            mod.OnUpdate = OnUpdate;
            return true;
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                FileLog.Log("Restored health");
                HealthAndDamageNew component = GameObject.FindWithTag("Ship").GetComponent<HealthAndDamageNew>();
                component.health = component.initialHealth;
            }
        }
    }

    [HarmonyPatch] // Class
    [HarmonyPatch("processCollision")]    // Method
    static class SkipMacGuffinHealth
    {
        static bool Prefix(GameObject other)
        {
            if(other.CompareTag("MacGuffin"))
            {
                GameObject.FindWithTag("CampaignLevel").GetComponent<CampaignLevelController>().MacGuffinAcquired();
                other.GetComponent<PowerupController>().Poof();
                return false; // don't run the original
            }

            return true; // run the original
        }
    }
}