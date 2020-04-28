using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityModManagerNet;

namespace OC2Mod
{
    public class Main
    {
        static void Load()
        {
            // initialization here //
        }

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

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                modEntry.Logger.Log("F1 pressed");
            }
        }
    }
}
