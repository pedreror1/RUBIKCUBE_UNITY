using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PEDREROR1.RUBIK.Editor
{ 
[CustomEditor(typeof(Slice))]
public class SliceEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        Slice Target = target as Slice;
        DrawDefaultInspector();
        if(GUILayout.Button("RotateCW"))
        {
            Target.Rotate(1);
        }
            if (GUILayout.Button("RotateCW"))
            {
                Target.Rotate(-1);
            }

            if (GUILayout.Button("TestParent"))
            {
                Target.TestParent();
    
            }
            if (GUILayout.Button("CheckFace"))
            {
                Target.CheckFace();

            }
            GUILayout.Label(Target.cublets.Count.ToString());
            foreach (var c in Target.cublets)
            {
                c.cublet= (Cublet) EditorGUILayout.ObjectField("cublet", c.cublet, typeof(Cublet));
                GUILayout.Label("Original pos="+c.originalPosition.ToString());
                GUILayout.Label("current pos=" + c.cublet.currentPosition.ToString());

            }

    }
}
}
