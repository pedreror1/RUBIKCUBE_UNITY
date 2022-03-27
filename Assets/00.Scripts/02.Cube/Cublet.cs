using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK
{
    public class Cublet : MonoBehaviour
    {
        // Start is called before the first frame update

        public Vector3 Originalposition, currentPosition;

        Transform _transform;
        GameObject _gameObject;

        [SerializeField]
        private Slice xRotSlice, yRotSlice, zRotSlice;
        
        public void UpdateSlices(Slice newSlice )
        {
            if (newSlice.RotationAngle.x != 0)
                xRotSlice = newSlice;
            else if (newSlice.RotationAngle.y != 0)
                yRotSlice = newSlice;
            if (newSlice.RotationAngle.z != 0)
                zRotSlice = newSlice;

            
        }

       

        private void Awake()
        {
            _transform = transform;
            _gameObject = gameObject;
        }

        private void OnMouseDown()
        {

        }
        public void setPositionAndName(Vector3 position)
        {
            this.Originalposition = position;
            currentPosition = position;
            name = $"Cublet_{position.x},{position.y},{position.z}";
        }
        public void DestroyCublet()
        {
            Destroy(_gameObject);
        }
        public void setParent(Transform parent)
        {
            if (_transform)
            {
                _transform.SetParent(parent);
            }
        }

        public KeyValuePair<Slice,int> Rotate(Vector3 Direction, Vector3 Face)
        {

            Debug.Log($"Cublet{name} direction {Direction} Face{Face} ");
            //Cubes left Side
            if(Face.CeilToInt() == new Vector3(-1,0,0))
            {
                if(Mathf.Abs(Direction.x)> Mathf.Abs(Direction.y))
                {
                    if(yRotSlice)
                    {
                        var rotationDirection = Direction.x > 0 ? 1 : -1;
                        return yRotSlice.Rotate(rotationDirection);
                        
                    }
                }
                else
                {
                    if (zRotSlice)
                    {
                        var rotationDirection = Direction.y > 0 ? 1 : -1;
                        return zRotSlice.Rotate(rotationDirection);
                     }
                }
            }
            //Cubes right Side
            if (Face.CeilToInt() == new Vector3(1, 0, 0))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (yRotSlice)
                    {
                        var rotationDirection = Direction.x > 0 ? 1 : -1;
                        return yRotSlice.Rotate(rotationDirection);
                     }
                }
                else
                {
                    if (zRotSlice)
                    {
                        
                        var rotationDirection = Direction.y > 0 ? -1 : 1;
                        return zRotSlice.Rotate(rotationDirection);
                     }
                }
            }

            //Cubes Top Side
            if (Face.CeilToInt() == new Vector3(0, 1, 0))
            {

                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (zRotSlice)
                    {
                        

                        var rotationDirection = Direction.x > 0 ? 1 : -1;
                        return zRotSlice.Rotate(rotationDirection);
                         
                    }
                }
                else
                {
                    if (xRotSlice)
                    {

                        

                        var rotationDirection = Direction.y > 0 ? 1 : -1;
                        return xRotSlice.Rotate(rotationDirection);
                         
                    }
                }
            }
            //Cubes bottom Side
            if (Face.CeilToInt() == new Vector3(0, -1, 0))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (zRotSlice)
                    {
                      

                        var rotationDirection = Direction.x > 0 ? -1 : 1;
                        return zRotSlice.Rotate(rotationDirection);
                        
                    }
                }
                else
                {
                    if (xRotSlice)
                    {
 
                        var rotationDirection = Direction.y > 0 ? 1 : -1;
                        return xRotSlice.Rotate(rotationDirection);
                         
                    }
                   
                }
            }

            //Cubes front Side
            if (Face.CeilToInt() == new Vector3(0, 0,-1))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (yRotSlice)
                    {
 
                        var rotationDirection = Direction.x > 0 ? 1 : -1;
                        return yRotSlice.Rotate(rotationDirection);
                         
                    }
                }
                else
                {
                    if (xRotSlice)
                    {
                        

                        var rotationDirection = Direction.y > 0 ? -1 : 1;
                        return xRotSlice.Rotate(rotationDirection);
                         
                    }
                    
                }
            }
            //Cubes back Side
            if (Face.CeilToInt() == new Vector3(0, 0, 1))
            {
                if (Mathf.Abs(Direction.x) > Mathf.Abs(Direction.y))
                {
                    if (yRotSlice)
                    {
                        var rotationDirection = Direction.x > 0 ? 1 : -1;
                        return yRotSlice.Rotate(rotationDirection);
                        
                    }
                }
                else
                {
                    if (xRotSlice)
                    {
                        var rotationDirection = Direction.y > 0 ? 1 : -1;
                        return xRotSlice.Rotate(rotationDirection);
                         
                    }
                }
            }

            return new KeyValuePair<Slice, int>(null,0);
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}