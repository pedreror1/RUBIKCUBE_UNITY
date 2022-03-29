using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK.Utilities
{
    /// <summary>
    /// Utility Class with a variety of Extensions methods for a better QOL
    /// </summary>
    public static class Extensions
    {
        public static Vector3 RoundToInt(this Vector3 vec3)
        {
            var newvec = new Vector3(Mathf.RoundToInt(vec3.x), Mathf.RoundToInt(vec3.y), Mathf.RoundToInt(vec3.z));

            return newvec;
        }

        public static Vector2 NegY(this Vector2 vec)
        {
            return new Vector2(vec.x, -vec.y);
        }
        public static Vector2 NegX(this Vector2 vec)
        {
            return new Vector2(-vec.x, vec.y);
        }
        public static Vector2 NegXY(this Vector2 vec)
        {
            return new Vector2(vec.x, vec.y) * -1;
        }
        public static Vector2 InvertVectorNegY(this Vector2 vec)
        {
            return new Vector2(-vec.y, vec.x);
        }
        public static Vector2 InvertVectorNegX(this Vector2 vec)
        {
            return new Vector2(vec.y, -vec.x);
        }
        public static Vector2 InvertVectorNegXY(this Vector2 vec)
        {
            return new Vector2(-vec.y, -vec.x);
        }
        public static Vector2 InvertVector(this Vector2 vec)
        {
            return new Vector2(vec.y, vec.x);
        }
        public static Vector3 ToVector3(this string Vector)
        {
            if (Vector.StartsWith("(") && Vector.EndsWith(")"))
            {
                Vector = Vector.Substring(1, Vector.Length - 2);
            }
            else
            {
                return Vector3.one * -1;
            }

            string[] sArray = Vector.Split(',');

            if (sArray.Length == 3)
            {

                Vector3 result = new Vector3(
                    float.Parse(sArray[0]),
                    float.Parse(sArray[1]),
                    float.Parse(sArray[2]));
                return result;
            }
            return Vector3.one * -1;

        }
        public static Vector2 ToVector2(this string Vector)
        {


            if (Vector.StartsWith("(") && Vector.EndsWith(")"))
            {
                Vector = Vector.Substring(1, Vector.Length - 2);
            }
            else
            {
                return Vector2.one * -1;
            }


            string[] sArray = Vector.Split(',');

            if (sArray.Length == 2)
            {

                Vector2 result = new Vector2(
                    float.Parse(sArray[0]),
                    float.Parse(sArray[1]));
                return result;
            }
            return Vector2.one * -1;

        }

        public static Vector3 FloorTo(this Vector3 vector, float x)
        {

            vector.x = Mathf.Round(vector.x / x) * x;
            vector.y = Mathf.Round(vector.y / x) * x;
            vector.z = Mathf.Round(vector.z / x) * x;
            return vector;
        }
    }
}