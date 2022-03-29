using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK.Utilities
{
    /// <summary>
    /// Utility Class for creating Singletons of Any kind of Monobehaviour based Class
    /// </summary>
    /// <typeparam name="T">Class Type</typeparam>
    public class SingletonComponent<T> :  MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
       

        public static T Instance
        {
            get
            {
                if (!instance)
                {
                    instance= FindObjectOfType<T>();
                    if(!instance)
                    {
                        instance = new GameObject($"{typeof(T).Name}_Singleton").AddComponent<T>();
                    }
                }

                return instance;
            }
            private set { }
        }
    }
}