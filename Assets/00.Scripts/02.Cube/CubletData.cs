using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PEDREROR1.RUBIK.Utilities
{
    /// <summary>
    /// Cublet Data Is a helper Class That contains The original Cublet Position of each cublet as well
    /// as a reference to the cublet, it contains all the helper methods to allow the modification of
    /// each Cublet to get aligned with The Slices Rotations.
    /// </summary>
    public class CubletData
    {
#region PARAMETERS
        public Vector3 originalPosition;
        private Cublet cublet;
        public Cublet getCublet() => cublet;
        public Transform getCubletParent() => cublet ? cublet.transform.parent : null;
        public Vector3 GetCubletOriginalPosition() => cublet ? cublet.Originalposition : Vector3.zero;
        public Vector3 GetCubletCurrentPosition() => cublet ? cublet.currentPosition : Vector3.zero;

        #endregion

#region METHODS

        public CubletData(Cublet cublet)
        {
            this.cublet = cublet;
            originalPosition = cublet.Originalposition;
        }

        public void UpdateClubet(Cublet cublet)
        {
            this.cublet = cublet;
        }
        public void UpdateCubletSlice(Slice newSlice, Vector3 rotationAngle)
        {
            if(cublet)
            {
                cublet.UpdateSlices(newSlice, rotationAngle);
            }
        }
         public void UpdateCubletParent(Transform newParent)
        {
            cublet.transform.parent = newParent;
        }

        public void UpdateCubletPosition()
        {
            originalPosition = cublet.currentPosition = cublet.transform.position.RoundToInt();

        }

       
#endregion
    }
}