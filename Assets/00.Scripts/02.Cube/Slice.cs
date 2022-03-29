using PEDREROR1.RUBIK.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK
{
    /// <summary>
    /// The Slice Class is a N*N container of Cublets
    /// itÅLs in charged of rotating each Slice of the Rubik Cube
    /// It also contains a helper metod to verify if the Face Is complete
    /// </summary>
    public class Slice : MonoBehaviour
    {

#region PARAMETERS
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
        public Vector3 RotationAngle { get; private set; }

        private List<CubletData> cublets = new List<CubletData>();        
        private LayerMask cubletFaceMask;
        private Vector3 startRotation = Vector3.zero;
        #endregion

#region METHODS

        #region SLICE_LIFETIME
        public void Setup(FaceType faceType)
        {
            cubletFaceMask = LayerMask.GetMask("CubletFace");
            if (cublets.Count > 0)
            {
                var pos1 = cublets[0].GetCubletOriginalPosition();
                var pos2 = cublets[cublets.Count - 1].GetCubletOriginalPosition();

                if (pos1.x == pos2.x)
                    RotationAngle = new Vector3(90, 0, 0);
                else if (pos1.z == pos2.z)
                    RotationAngle = new Vector3(0, 0, 90);
                else if (pos1.y == pos2.y)
                    RotationAngle = new Vector3(0, 90, 0);
                foreach (var cublet in cublets)
                {
                    cublet.UpdateCubletSlice(this, RotationAngle);
                }
            }
            sliceFaceType = faceType;
        }
        public void AddCublet(CubletData newData)
        {
            cublets.Add(newData);
        }
        public void updateSlice(Slice changedSlice)
        {
            for (int i = 0; i < cublets.Count; i++)
            {
                var replacementCublet = changedSlice.getCubletAtPosition(cublets[i].originalPosition);
                if (replacementCublet != null)
                {
                    cublets[i].UpdateClubet(replacementCublet);
                    cublets[i].UpdateCubletSlice(this, RotationAngle);
                }
            }
        }
        public KeyValuePair<Slice, int> TryRotate(int direction, float speed = 1)
        {
            if (transform.childCount > 0)
            {
                return new KeyValuePair<Slice, int>();
            }

            foreach (var cublet in cublets)
            {
                cublet.UpdateCubletParent(transform);
            }

            StartCoroutine(RotateSmoothly(RotationAngle, direction, speed));
            return new KeyValuePair<Slice, int>(this, direction);
        }
        IEnumerator RotateSmoothly(Vector3 finalRotation, int direction, float speedMultiplier)
        {
            Vector3 currentRotation = startRotation;
            float delta = 0;
            while (delta < 1f)
            {
                currentRotation = Vector3.Lerp(currentRotation, startRotation + (finalRotation * direction), delta);
                delta += Time.deltaTime * PlayerManager.Instance.animationSpeed * speedMultiplier;
                transform.eulerAngles = currentRotation;
                yield return new WaitForEndOfFrame();
            }
            transform.eulerAngles = transform.eulerAngles.FloorTo(90);
            foreach (var cublet in cublets)
            {
                cublet.UpdateCubletParent(PlayerManager.Instance.GetCubeTransform);
                cublet.UpdateCubletPosition();
            }

           
            startRotation = currentRotation.FloorTo(90);
            PlayerManager.Instance.UpdateSlices(RotationAngle, this);
            PlayerManager.Instance.CheckWinningCondition();
        }
        public void Destroy()
        {
            cublets.Clear();
            Destroy(gameObject);
        }
        #endregion


        #region HELPERS
        public bool CheckFace()
        {
            if (sliceFaceType != FaceType.innerFace)
            {
                Ray FaceRay = new Ray(cublets[0].GetCubletCurrentPosition() + getFaceOffset(), cublets[0].GetCubletCurrentPosition() - (cublets[0].GetCubletCurrentPosition() + getFaceOffset()));
                RaycastHit FaceRayHit;
                string faceTag = "";
                if (Physics.Raycast(FaceRay, out FaceRayHit, 100f, cubletFaceMask))
                {
                    faceTag = FaceRayHit.transform.tag;
                }

                for (int i = 1; i < cublets.Count; i++)
                {
                    FaceRay = new Ray(cublets[i].GetCubletCurrentPosition() + getFaceOffset(), cublets[i].GetCubletCurrentPosition() - (cublets[i].GetCubletCurrentPosition() + getFaceOffset()));
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
        private Vector3 getFaceOffset()
        {
            switch (sliceFaceType)
            {
                case FaceType.innerFace:
                    return Vector3.zero;                     
                case FaceType.Front:
                    return Vector3.forward * 15f;
                case FaceType.Back:
                    return Vector3.forward * -15f;
                case FaceType.Left:
                    return Vector3.right * -15f;
                case FaceType.Right:
                    return Vector3.right * 15f;
                case FaceType.Top:
                    return Vector3.up * 15f;
                case FaceType.Down:
                    return Vector3.up * -15f;
            }
            return Vector3.zero;
        }
        private Cublet getCubletAtPosition(Vector3 originalPosition)
        {
            foreach (var cublet in cublets)
            {
                if (cublet.GetCubletCurrentPosition() == originalPosition)
                {
                    return cublet.getCublet();
                }
            }
            return null;
        }
        #endregion
        #region DEBUG
        public void TestParent()
        {
            if (!PlayerManager.Instance.debug) return;
            foreach (var cublet in cublets)
            {
                if (cublet.getCubletParent() == transform)
                {
                    cublet.UpdateCubletParent(PlayerManager.Instance.GetCubeTransform);

                }
                else
                    cublet.UpdateCubletParent(transform);
            }
        }



        private void OnDrawGizmos()
        {
            if (!PlayerManager.Instance.debug) return;
            if (sliceFaceType != FaceType.innerFace)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(transform.position + getFaceOffset(), 1f);

                Gizmos.color = Color.green;
                for (int i = 0; i < cublets.Count; i++)
                {
                    Gizmos.DrawRay(new Ray(cublets[i].GetCubletCurrentPosition() + getFaceOffset(),
                                           cublets[i].GetCubletCurrentPosition() - (cublets[i].GetCubletCurrentPosition() + getFaceOffset())));
                    Gizmos.DrawSphere(cublets[i].GetCubletCurrentPosition() + getFaceOffset(), 0.5f);
                }
            }
        }
        #endregion
#endregion

    }
}
