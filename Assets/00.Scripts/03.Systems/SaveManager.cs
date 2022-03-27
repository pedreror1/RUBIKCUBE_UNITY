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
        public SaveData(Stack<KeyValuePair<int, int>> movements, int dimensions)
        {
            this.movements = movements;
            this.dimensions = dimensions;
        }

        /*public SaveData(Vector3[,,] cubletsOriginalPosition, Vector3[,,] cubletscurrentPosition, Queue<KeyValuePair<Slice, int>> movements)
        {
            this.cubletsOriginalPosition = cubletsOriginalPosition;
            this.cubletscurrentPosition = cubletscurrentPosition;
            this.movements = movements;
        }*/
    }
    public class SaveManager : MonoBehaviour
    {

      


        public void Save(Stack<KeyValuePair<Slice, int>> movements, int dimensions)
        {

            /* string Data = "Cublets {";
             for (int x = 0; x < dimension; x++)
             {
                 for (int y = 0; y < dimension; y++)
                 {
                     for (int z = 0; z < dimension; z++)
                     {
                         if (x == 0 || y == 0 || z == 0 || x == PlayerManager.Instance.dimension - 1 || y == PlayerManager.Instance.dimension - 1 || z == PlayerManager.Instance.dimension - 1)
                         {
                             if (cublets[x,y,z] != null)
                                 Data += $"{cublets[x, y, z].Originalposition}_{cublets[x, y, z].currentPosition}/";
                             else
                                 Data += $"/";
                         }
                     }
                 }
             }*/

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
                if(dataSegments.Length==3)
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
                    int size = -1;
                    var x = dataSegments[1].Replace("Size {", "");
                    if (int.TryParse(x,out size))
                    {
                        return new SaveData(movementList, size);
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