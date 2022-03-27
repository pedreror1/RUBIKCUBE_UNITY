using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK
{
    public class Slice : MonoBehaviour
    {
        public class CubletData
        {
            public Cublet cublet;
            public Vector3 originalPosition;

            public CubletData(Cublet cublet)
            {
                this.cublet = cublet;
                originalPosition = cublet.Originalposition;
            }

            internal void UpdateClubet(Cublet cublet)
            {
                this.cublet = cublet;
            }
        }
        public List<CubletData> cublets = new List<CubletData>();

        public Vector3 RotationAngle;
        public enum FaceType
        {
            innerFace,
            Front,
            Back,
            Left,
            Right,
            Top,
            Down
        }
        public FaceType sliceFaceType;
        public LayerMask cubletFaceMask;
        public void Destroy()
        {
            cublets.Clear();
            Destroy(gameObject);
        }
        // Start is called before the first frame update

        public KeyValuePair<Slice, int> Rotate(int direction, float speed = 1)
        {
            
            if (transform.childCount > 0)
            {
                return new KeyValuePair<Slice, int>();
            }
            
            foreach (var cublet in cublets)
            {
                cublet.cublet.setParent(transform);
            }
            

            StartCoroutine(RotateSmoothly(RotationAngle, direction, speed));
            return new KeyValuePair<Slice, int>(this, direction);
        }

        public Vector3 startRotation = Vector3.zero;
        IEnumerator RotateSmoothly(Vector3 finalRotation, int direction, float speedMultiplier)
        {

            Vector3 currentRotation = startRotation;
            float delta = 0;
            while (delta < 1f)
            {
                currentRotation = Vector3.Lerp(currentRotation, startRotation + (finalRotation * direction), delta);
                delta += Time.deltaTime * PlayerManager.Instance.animationSpeed * speedMultiplier;
                transform.rotation = Quaternion.Euler(currentRotation);
                yield return new WaitForEndOfFrame();

            }
            foreach (var cublet in cublets)
            {
                cublet.cublet.setParent(null);
               
                cublet.cublet.setParent(PlayerManager.Instance.cubeGenerator.transform);
                cublet.originalPosition= cublet.cublet.currentPosition = cublet.cublet.transform.position.CeilToInt();

            }
            startRotation = currentRotation;
            PlayerManager.Instance.UpdateSlices(RotationAngle, this);
            PlayerManager.Instance.CheckWinningCondition();
        }
        public void TestParent()
        {
            foreach (var cublet in cublets)
            {
                if (cublet.cublet.transform.parent == transform)
                {
                    cublet.cublet.transform.parent = PlayerManager.Instance.cubeGenerator.transform;

                }
                else
                    cublet.cublet.transform.parent = transform;
            }
        }

        public void UpdateCublets()
        {
            foreach (var cublet in cublets)
            {
                cublet.cublet.currentPosition = cublet.cublet.transform.position.CeilToInt();
            }
        }
        void Start()
        {
            cubletFaceMask = LayerMask.GetMask("CubletFace");
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Setup(FaceType faceType)
        {
            if (cublets.Count > 0)
            {
                var pos1 = cublets[0].cublet.Originalposition;
                var pos2 = cublets[cublets.Count - 1].cublet.Originalposition;

                if (pos1.x == pos2.x)
                    RotationAngle = new Vector3(90, 0, 0);
                else if (pos1.z == pos2.z)
                    RotationAngle = new Vector3(0, 0, 90);
                else if (pos1.y == pos2.y)
                    RotationAngle = new Vector3(0, 90, 0);
                foreach (var cublet in cublets)
                {
                     
                    cublet.cublet.UpdateSlices(this);
                }
            }
            sliceFaceType = faceType;
        }
        public bool CheckFace()
        {
            if (sliceFaceType != FaceType.innerFace)
            {
                Ray FaceRay = new Ray(cublets[0].cublet.currentPosition + getFaceOffset(), cublets[0].cublet.currentPosition - (cublets[0].cublet.currentPosition + getFaceOffset()));
                RaycastHit FaceRayHit;
                string faceTag = "";
                if (Physics.Raycast(FaceRay, out FaceRayHit, 100f, cubletFaceMask))
                {
                    faceTag = FaceRayHit.transform.tag;
                }

                for (int i = 1; i < cublets.Count; i++)
                {
                    FaceRay = new Ray(cublets[i].cublet.currentPosition + getFaceOffset(), cublets[i].cublet.currentPosition - (cublets[i].cublet.currentPosition + getFaceOffset()));
                    if (Physics.Raycast(FaceRay, out FaceRayHit, 100f, cubletFaceMask))
                    {
                        if (!FaceRayHit.transform.CompareTag(faceTag))
                            return false;
                    }
                }
                return true;
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            if (sliceFaceType != FaceType.innerFace)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + getFaceOffset(), 1f);

                Gizmos.color = Color.green;
                for (int i = 0; i < cublets.Count; i++)
                {
                    Gizmos.DrawRay(new Ray(cublets[i].cublet.currentPosition + getFaceOffset(),
                                           cublets[i].cublet.currentPosition - (cublets[i].cublet.currentPosition + getFaceOffset())));
                    Gizmos.DrawSphere(cublets[i].cublet.currentPosition + getFaceOffset(), 0.5f);
                }
            }
        }
        public Vector3 getFaceOffset()
        {
            switch (sliceFaceType)
            {
                case FaceType.innerFace:
                    return Vector3.zero;
                    break;
                case FaceType.Front:
                    return Vector3.forward * 5f;
                case FaceType.Back:
                    return Vector3.forward * -5f;
                case FaceType.Left:
                    return Vector3.right * -5f;
                case FaceType.Right:
                    return Vector3.right * 5f;
                case FaceType.Top:
                    return Vector3.up * 5f;
                case FaceType.Down:
                    return Vector3.up * -5f;
            }
            return Vector3.zero;
        }

        public int changed=3;
        public void updateSlice(Slice changedSlice)
        {
            for (int i = 0; i < cublets.Count; i++)
            {
                var replacementCublet= changedSlice.getCubletAtPosition(cublets[i].originalPosition);
                if(replacementCublet!=null)
                {
                    cublets[i].cublet = replacementCublet;
                      cublets[i].cublet.UpdateSlices(this);
                }
            }

             /*   changed = 3;
            foreach (var changedCublet in changedSlice.cublets)
            {
                
                for (int i = 0; i < cublets.Count; i++)
                {
                    Debug.Log($"is {cublets[i].originalPosition} == to {changedCublet.cublet.currentPosition}?");
                    if (cublets[i].originalPosition == changedCublet.cublet.currentPosition)
                    {
                        UpdateClubet(i, changedCublet.cublet);
                        changedCublet.cublet.UpdateSlices(this);
                        changed--;
                        break;

                    }

                }
               

            }
            if (changed != 0)
            {
                Debug.LogError($"Error updating face {name}from {changedSlice.name} ");
            }*/

        }

        private Cublet getCubletAtPosition(Vector3 originalPosition)
        {
            foreach (var cublet in cublets)
            {
                if (cublet.cublet.currentPosition == originalPosition)
                    return cublet.cublet;
            }
            return null;
        }

        private void UpdateClubet(int i, Cublet cublet)
        {
            if (i < cublets.Count)
            {
                cublets[i].cublet = cublet;
            }
        }
    }
}



//TODO move from here
public static partial class Extensions
{
    public static Vector3 CeilToInt(this Vector3 vec3)
    {
        var newvec = new Vector3(Mathf.RoundToInt(vec3.x), Mathf.RoundToInt(vec3.y), Mathf.RoundToInt(vec3.z));

        return newvec;
    }

    public static Vector2 InvertVector(this Vector2 vec)
    {
        return new Vector2(-vec.y, vec.x);
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
}
