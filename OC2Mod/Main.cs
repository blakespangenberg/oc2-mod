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
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            // var harmony = HarmonyInstance.Create(modEntry.Info.Id);
            // harmony.PatchAll(Assembly.GetExecutingAssembly());

            modEntry.OnUpdate = OnUpdate;
            return true;
        }

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                modEntry.Logger.Log("F1 pressed");
            }
        }

        /*
        [HarmonyPatch(typeof(SurfaceMovable))] // Class
        [HarmonyPatch("GetVelocity")]          // Method
        static class DoubleSpeed
        {
            static bool Prefix(Vector3 __result)
            {
                __result = new Vector3();
                __result.Set(10,10,10);

                return false;
            }
        }
        */
    }
}
