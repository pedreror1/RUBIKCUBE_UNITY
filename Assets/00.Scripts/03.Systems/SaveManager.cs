using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PEDREROR1.RUBIK.Utilities
{
    /// <summary>
    /// The SaveData Class serves as a Definition of the data that should be saved by the
    /// SaveManager Class
    /// </summary>
    public class SaveData
    {
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

    /// <summary>
    /// The Save Manager Class is in charge Of The Saving and Loading of the data to a persistent
    /// Text File
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public void Save(Stack<KeyValuePair<Slice, int>> movements, int dimensions,float timer)
        {
            string data= "Movements {";
            while (movements.Count > 0)
            {
                var movement = movements.Pop();
                int sliceIndex = -1;
                if (int.TryParse(movement.Key.name.Replace("Slice_", ""), out sliceIndex))
                {
                    data += $"({sliceIndex},{movement.Value})/";
                }
            }
            data += "}";
            data +="Size {"+dimensions+"}";
            data += "Timer {" + timer+ "}";

            if (!Directory.Exists(Application.dataPath + "/SaveData"))
            {
                Directory.CreateDirectory(Application.dataPath + "/SaveData");
            }
            File.WriteAllText(Application.dataPath+"/SaveData/data.json",data);            
        }
        public SaveData LoadData()
        {          
            if (Directory.Exists(Application.dataPath + "/SaveData"))
            {
                var savedData=  File.ReadAllText(Application.dataPath + "/SaveData/data.json");
                var dataSegments = savedData.Split('}');
                if(dataSegments.Length==4)
                {
                    //Movements
                    var movementData= dataSegments[0].Split('{');
                    var movements= movementData[1].Replace("/}","").Split('/');
                    Stack<KeyValuePair<int, int>> movementList = new Stack<KeyValuePair<int, int>>();
                    if (movements.Length > 0)
                    {
                        foreach (var movement in movements)
                        {
                            var movementasVector = movement.ToVector2();
                            if(movementasVector.x!=-1)
                            {
                                movementList.Push(new KeyValuePair<int, int>((int)movementasVector.x, (int)movementasVector.y));
                            }
                        }                        
                    }
                    //Dimension and Timer
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
    }
}