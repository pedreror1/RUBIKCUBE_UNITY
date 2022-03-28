using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PEDREROR1.RUBIK.Utilities
{
    public class SaveData
    {
        //   public Vector3[,,] cubletsOriginalPosition;
        // public Vector3[,,] cubletscurrentPosition;
        public Stack<KeyValuePair<int, int>> movements = new Stack<KeyValuePair<int, int>>();
        public int dimensions;
        public float timer;
        public SaveData(Stack<KeyValuePair<int, int>> movements, int dimensions, float timer)
        {
            this.movements = movements;
            this.dimensions = dimensions;
            this.timer = timer;
        }


    }
    public class SaveManager : MonoBehaviour
    {

      


        public void Save(Stack<KeyValuePair<Slice, int>> movements, int dimensions,float timer)
        {
            string Data= "Movements {";
            while (movements.Count > 0)
            {
                var movement = movements.Pop();
                int sliceIndex = -1;
                if (int.TryParse(movement.Key.name.Replace("Slice_", ""), out sliceIndex))
                {
                    Data += $"({sliceIndex},{movement.Value})/";
                }
            }
            Data += "}";
            Data +="Size {"+dimensions+"}";
            Data += "Timer {" + timer+ "}";


            if (!Directory.Exists(Application.dataPath + "/SaveData"))
            {
                Directory.CreateDirectory(Application.dataPath + "/SaveData");
            }
            File.WriteAllText(Application.dataPath+"/SaveData/data.json",Data);
            
        }
        public SaveData LoadData()
        {
          
            if (Directory.Exists(Application.dataPath + "/SaveData"))
            {
                var jsonData=  File.ReadAllText(Application.dataPath + "/SaveData/data.json");
                var dataSegments = jsonData.Split('}');
                if(dataSegments.Length==4)
                {
                    //Movements
                    var MovementData= dataSegments[0].Split('{');
                    var Movements= MovementData[1].Replace("/}","").Split('/');
                    Stack<KeyValuePair<int, int>> movementList = new Stack<KeyValuePair<int, int>>();
                    if (Movements.Length > 0)
                    {
                        foreach (var movement in Movements)
                        {
                            var movementasVector = movement.ToVector2();
                            if(movementasVector.x!=-1)
                            {
                                movementList.Push(new KeyValuePair<int, int>((int)movementasVector.x, (int)movementasVector.y));
                            }
                        }
                        
                    }
                    //Dimension
                    int size = 3;
                    float timer = 0;
                    var x = dataSegments[1].Replace("Size {", "");
                    var y = dataSegments[2].Replace("Timer {", "");
                    if (int.TryParse(x, out size) && float.TryParse(y, out timer))
                    {
                        return new SaveData(movementList, size,timer);
                    }


                }
                return null;

            }
            return null;
            
            
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