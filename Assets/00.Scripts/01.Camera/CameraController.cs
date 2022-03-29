using PEDREROR1.RUBIK.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK
{
    /// <summary>
    /// The CameraController Class Contains The Implementation To:
    /// *Rotate Around The Cube
    /// *Zoom In and Out from The Cube
    /// *Detect if a Cublet Is Selected
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        #region PARAMETERS
        private enum CameraState
        {
            Idle = 0,
            RotatingCamera = 1,
            ZoomingCamera = 2,
            Animating = 3,
            WaitingForFLick = 4
        }
        private CameraState currentCameraState;
        private Camera cam;
        private Vector3 target;
        Vector3 selectedCubletNormal;
        private float cameraDistance = 5;
        public Vector2 cameraZoomRange = new Vector2(3f, 15f);
        public LayerMask cubletMask;
        private Vector2 currentAngle = new Vector2(0, 0f);
        public Vector2 cameraXrotationLimit;
        #endregion

        #region METHODS
        public void UpdateState(int newState) => currentCameraState = (CameraState)newState;
        public Vector2 GetOrientedDirection(float yRotation, Vector2 mousePositionDelta, Vector3 normal)
        {
            if (Mathf.Round(normal.y) == 0f)
            {
                return mousePositionDelta;
            }
            var orientedVector = mousePositionDelta;
            print(yRotation);
            switch (yRotation)
            {
                case 0:
                case 360:
                    return mousePositionDelta.NegY();
                case 45://done
                    if (mousePositionDelta.x < 0 && mousePositionDelta.y <= 0)
                        orientedVector = new Vector2(mousePositionDelta.x, 0);
                    else if (mousePositionDelta.x < 0 && mousePositionDelta.y >= 0)
                        orientedVector = new Vector2(0, mousePositionDelta.x);
                    else if (mousePositionDelta.x > 0 && mousePositionDelta.y >= 0)
                        orientedVector = new Vector2(mousePositionDelta.x, 0);
                    else //x>0 y<0
                        orientedVector = new Vector2(0, mousePositionDelta.x);
                    return normal.y > 0 ? orientedVector : orientedVector.InvertVectorNegX();

                case 90:
                    return normal.y > 0 ? mousePositionDelta.InvertVector() : mousePositionDelta.InvertVectorNegXY(); //done

                case 135:
                    if (mousePositionDelta.x < 0 && mousePositionDelta.y <= 0)
                        orientedVector = new Vector2(0, mousePositionDelta.x);
                    else if (mousePositionDelta.x < 0 && mousePositionDelta.y >= 0)
                        orientedVector = new Vector2(-mousePositionDelta.x, 0);
                    else if (mousePositionDelta.x > 0 && mousePositionDelta.y >= 0)
                        orientedVector = new Vector2(0, mousePositionDelta.x);
                    else //x>0 y<0
                        orientedVector = new Vector2(-mousePositionDelta.x, 0);
                    if (normal.y < 0) orientedVector = orientedVector.InvertVectorNegY();
                    return orientedVector;

                case 180:
                    return mousePositionDelta.NegX();

                case 225:
                    if (mousePositionDelta.x < 0 && mousePositionDelta.y <= 0)
                        orientedVector = new Vector2(-mousePositionDelta.x, 0);
                    else if (mousePositionDelta.x < 0 && mousePositionDelta.y >= 0)
                        orientedVector = new Vector2(0, -mousePositionDelta.x);
                    else if (mousePositionDelta.x > 0 && mousePositionDelta.y >= 0)
                        orientedVector = new Vector2(-mousePositionDelta.x, 0);
                    else //x>0 y<0
                        orientedVector = new Vector2(0, -mousePositionDelta.x);
                    if (normal.y < 0) orientedVector = orientedVector.InvertVectorNegX();
                    return orientedVector;

                case 270:
                    return normal.y > 0 ? mousePositionDelta.InvertVectorNegXY() : mousePositionDelta.InvertVector();

                case 315:
                    if (mousePositionDelta.x < 0 && mousePositionDelta.y <= 0)
                        orientedVector = new Vector2(0, -mousePositionDelta.x);
                    else if (mousePositionDelta.x < 0 && mousePositionDelta.y >= 0)
                        orientedVector = new Vector2(mousePositionDelta.x, 0);
                    else if (mousePositionDelta.x > 0 && mousePositionDelta.y >= 0)
                        orientedVector = new Vector2(0, -mousePositionDelta.x);
                    else //x<0 y<0
                        orientedVector = new Vector2(mousePositionDelta.x, 0);

                    if (normal.y < 0) orientedVector = orientedVector.InvertVectorNegY();
                    return orientedVector;
            }
            return mousePositionDelta;
        }
        public bool CalculateFlickDirection(Vector3 mousePositionDelta)
        {
            var currentCameraAngle = ((transform.rotation.eulerAngles.y % 360 + 360) % 360);
            currentCameraAngle = Mathf.Round(currentCameraAngle / 45) * 45;
            PlayerManager.Instance.TryRotate(GetOrientedDirection(currentCameraAngle, mousePositionDelta, selectedCubletNormal));
            return false;

        }
        public Vector3 GetScreenToViewPort(Vector2 mousePosition)
        {
            return Vector3.ClampMagnitude(cam.ScreenToViewportPoint(Input.mousePosition), 1f);
        }
        public void Rotate(Vector2 offset)
        {
            currentAngle -= offset;
            currentAngle.x = Mathf.Clamp(currentAngle.x, cameraXrotationLimit.x, cameraXrotationLimit.y);
            if (currentAngle.y > 360f) currentAngle.y -= 360f;
            else if (currentAngle.y < 0f) currentAngle.y += 360f;
            Quaternion lookRotatipn = Quaternion.Euler(currentAngle);
            Vector3 lookDirection = lookRotatipn * Vector3.forward;
            Vector3 lookPosition = target - lookDirection * cameraDistance;
            transform.SetPositionAndRotation(lookPosition, lookRotatipn);
        }

        //TODO implement pinch in/out
        public void CalculateZoom(float touchZoom = 0, float MouseZoom = 0, float extraZoom = 0)
        {


            cameraDistance = Mathf.Clamp(cameraDistance - (touchZoom + MouseZoom + extraZoom), cameraZoomRange.x, cameraZoomRange.y);
            cam.orthographicSize = cameraDistance;

        }


        public bool TryGetCublet()
        {
            if (currentCameraState == CameraState.RotatingCamera) return false;

            Vector3 mousePosition3D = Input.mousePosition;
            mousePosition3D.z = 10;
            Ray camRay = cam.ScreenPointToRay(mousePosition3D);
            RaycastHit hit;
            if (Physics.Raycast(camRay, out hit, 10f, cubletMask))
            {
                PlayerManager.Instance.UpdateCurrentCublet(hit);
                selectedCubletNormal = hit.normal;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Awake()
        {
            PlayerManager.Instance.CubeGeneratedEvnt += Setup;
        }
        private void Setup()
        {

            cam = GetComponent<Camera>();
            target = PlayerManager.Instance.GetCenter;
            cameraDistance = PlayerManager.Instance.Dimension;
            cameraZoomRange = new Vector2(PlayerManager.Instance.Dimension * 1.5f, PlayerManager.Instance.Dimension * 3f);
            cam.orthographicSize = cameraZoomRange.x;
            transform.position = target - transform.forward * cameraDistance;
            CalculateZoom();
        }
    }
    #endregion
}

