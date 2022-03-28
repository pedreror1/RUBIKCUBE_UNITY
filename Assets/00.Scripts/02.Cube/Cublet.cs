using PEDREROR1.RUBIK.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK
{
    /// <summary>
    /// The Cublet Class serves as a Container for each of the Cublets in the Rubik Cube
    /// itÅLs used as a bridge for comunicating from the Player Manager To the Slices 
    /// To rotate The Correct Slice Depending on the Flick Direction
    /// </summary>
    public class Cublet : MonoBehaviour
    {
#region PARAMETERS
        public Vector3 Originalposition { get; set; }
        public Vector3 currentPosition { get; set; }

        private Slice xRotSlice, yRotSlice, zRotSlice;
        #endregion

#region METHODS
        public void UpdateSlices(Slice newSlice, Vector3 rotationAngle)
        {
            if (rotationAngle.x != 0)
                xRotSlice = newSlice;
            else if (rotationAngle.y != 0)
                yRotSlice = newSlice;
            if (rotationAngle.z != 0)
                zRotSlice = newSlice;
        }

        public void setPositionAndName(Vector3 position)
        {
            this.Originalposition = position;
            currentPosition = position;
            name = $"Cublet_{position.x},{position.y},{position.z}";
        }
        public void DestroyCublet()
        {
            Destroy(gameObject);
        }
        public void setParent(Transform parent)
        {
            if (transform)
            {
                transform.SetParent(parent);
            }
        }

        public KeyValuePair<Slice, int> Rotate(Vector3 Direction, Vector3 Face)
        {
            //Cubes left Side
            if (Face.RoundToInt() == new Vector3(-1, 0, 0))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (yRotSlice)
                    {
                        return yRotSlice.TryRotate(Direction.x > 0 ? 1 : -1);
                    }
                }
                else
                {
                    if (zRotSlice)
                    {
                        return zRotSlice.TryRotate(Direction.y > 0 ? 1 : -1);
                    }
                }
            }
            //Cubes right Side
            if (Face.RoundToInt() == new Vector3(1, 0, 0))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (yRotSlice)
                    {
                        return yRotSlice.TryRotate(Direction.x > 0 ? 1 : -1);
                    }
                }
                else
                {
                    if (zRotSlice)
                    {
                        return zRotSlice.TryRotate(Direction.y > 0 ? -1 : 1);
                    }
                }
            }

            //Cubes Top Side
            if (Face.RoundToInt() == new Vector3(0, 1, 0))
            {

                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (zRotSlice)
                    {
                        return zRotSlice.TryRotate(Direction.x > 0 ? 1 : -1);
                    }
                }
                else
                {
                    if (xRotSlice)
                    {
                        return xRotSlice.TryRotate(Direction.y > 0 ? 1 : -1);
                    }
                }
            }
            //Cubes bottom Side
            if (Face.RoundToInt() == new Vector3(0, -1, 0))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (zRotSlice)
                    {
                        return zRotSlice.TryRotate(Direction.x > 0 ? -1 : 1);
                    }
                }
                else
                {
                    if (xRotSlice)
                    {
                        return xRotSlice.TryRotate(Direction.y > 0 ? 1 : -1);
                    }
                }
            }

            //Cubes front Side
            if (Face.RoundToInt() == new Vector3(0, 0, -1))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (yRotSlice)
                    {
                        return yRotSlice.TryRotate(Direction.x > 0 ? 1 : -1);
                    }
                }
                else
                {
                    if (xRotSlice)
                    {
                        return xRotSlice.TryRotate(Direction.y > 0 ? -1 : 1);
                    }
                }
            }
            //Cubes back Side
            if (Face.RoundToInt() == new Vector3(0, 0, 1))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (yRotSlice)
                    {
                        return yRotSlice.TryRotate(Direction.x > 0 ? 1 : -1);
                    }
                }
                else
                {
                    if (xRotSlice)
                    {
                        return xRotSlice.TryRotate(Direction.y > 0 ? 1 : -1);
                    }
                }
            }

            return new KeyValuePair<Slice, int>(null, 0);
        }
        #endregion
    }
}