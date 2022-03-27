using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace PEDREROR1.RUBIK.Editor
{
    public class PlayerManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var PME = target as PlayerManager;

            if(PME.saveManager)
            {
                if(GUILayout.Button("Save"))
                {
                    PME.save();

                }
                if (GUILayout.Button("Save"))
                {
                    PME.load();

                }

            }

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}