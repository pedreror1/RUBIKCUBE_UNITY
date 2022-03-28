using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PEDREROR1.RUBIK.Editor
{ 
    /// <summary>
    /// Custom Editor with Test Buttons for the Slice Game Component
    /// </summary>
[CustomEditor(typeof(Slice))]
public class SliceEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        Slice Target = target as Slice;
        DrawDefaultInspector();
        if(GUILayout.Button("RotateCW"))
        {
            Target.TryRotate(1);
        }
            if (GUILayout.Button("RotateCW"))
            {
                Target.TryRotate(-1);
            }

            if (GUILayout.Button("TestParent"))
            {
                Target.TestParent();
    
            }
            if (GUILayout.Button("CheckFace"))
            {
                Target.CheckFace();

            }
             

    }
}
}
