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
        static bool is_pressed = false;
        static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                mod = modEntry;
                is_pressed = false;

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
                mod.Logger.Log("F1 pressed");
                is_pressed = true;
            }

            if(Input.GetKeyUp(KeyCode.F1))
            {
                mod.Logger.Log("F1 released");
                is_pressed = false;
            }
        }
    }
}
