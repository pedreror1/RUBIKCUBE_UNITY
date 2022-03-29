using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PEDREROR1.RUBIK.Editor
{
    [CustomEditor(typeof(CubeGenerator))]
    public class CubeGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var Target = target as CubeGenerator;
            if (GUILayout.Button("GenerateCube"))
            {
                Target.GenerateCube();
            }
            if (GUILayout.Button("Destroy Cube"))
            {
                Target.DestroyCube();
            }
            foreach(var slice in PlayerManager.Instance.slices)
            {
                GUILayout.Label(slice.name);
                if (GUILayout.Button("Rotate CW"))
                {
                    slice.TryRotate(1);
                }
                if (GUILayout.Button("Rotate CCW"))
                {
                    slice.TryRotate(-1);
                }
                if (GUILayout.Button("TestParent"))
                {
                    slice.TestParent();
                }
            }

        }
    }

}