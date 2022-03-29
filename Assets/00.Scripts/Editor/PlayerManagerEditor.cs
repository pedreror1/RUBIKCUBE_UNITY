using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PEDREROR1.RUBIK.Editor
{
    /// <summary>
    /// Custom Editor for The Player Manager
    /// </summary>
    /// 
    [CustomEditor(typeof(PlayerManager))]
    public class PlayerManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var PME = target as PlayerManager;
            GUILayout.Label($"Current State: {PME.currentState}");

            DrawDefaultInspector();
              if(PME.currentState== PlayerManager.GameState.Playing && PME.debug) 
            {
                if(GUILayout.Button("Save"))
                {
                    PME.Save();
                }
                if (GUILayout.Button("load"))
                {
                    PME.Load();
                }
            }
        }      
    }
}