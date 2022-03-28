using PEDREROR1.RUBIK.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEDREROR1.RUBIK
{
    /// <summary>
    /// The Input Manager is In charge of Handling The Input for the Game for moth PC and Mobile
    /// The Input Manager Comunicates to the Camera Controller To do the proper calculations for 
    /// The Camera Rotation, Zooming in/out and the Cube Flick Gesture.
    /// </summary>
    /// 

    [RequireComponent(typeof(CameraController))]
    sealed class InputManager : MonoBehaviour
    {
#region PARAMETERS

        [Tooltip("Touch Sensitivity for Zoom Calculation")]
        public float touchSensitivity = 0.025f;
        [Tooltip("Treshold for Starting the Rotation")]
        public float minRotationDistance = 0.1f;
        [Tooltip("Camera Controller Component")]
        [SerializeField] CameraController cameraController;
        [Tooltip("Rotation Movement Speed")]
        public float RotationSpeed = 0.1f;
        [Tooltip("Rotation Movement dead zone")]
        public float deadZone = 0.25f;

        private Vector2 touch0StartPosition, touch1StartPosition, touch0DeltaPosition, touch1DeltaPosition;
        private float startZoomDistance, deltaZoomDistance, pinchZoom;
        private Vector3 mousePosition, mousePreviousPosition;
        private Vector3 mousePositionDelta;
        private bool FlickAvailable = true;
        #endregion

#region METHODS
        private void Awake()
        {
            if(cameraController==null)
            {
                cameraController = GetComponent<CameraController>();
            }
        }
        private void Update()
        {
            if (!PlayerManager.Instance.isPlaying) return;
            if (Input.GetMouseButtonDown(0))
            {
                mousePreviousPosition = cameraController.getScreenToViewPort(Input.mousePosition);
                if (!PlayerManager.Instance.hasCublet)
                {
                    cameraController.tryGetCublet();
                }
            }
            if (Input.GetMouseButton(0))
            {
                if (!PlayerManager.Instance.hasCublet && Input.touchCount < 2)
                {
                    RotationInput();
                    
                }
                else if (PlayerManager.Instance.hasCublet && FlickAvailable)
                {
                    FlickInput();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                FlickAvailable = true;
                cameraController.UpdateState(0);
                PlayerManager.Instance.RemoveCurrentCublet();
            }
            ZoomInput();

        }

        private void FlickInput()
        {
            cameraController.UpdateState(4);
            mousePosition = cameraController.getScreenToViewPort(Input.mousePosition);
            mousePositionDelta = Vector3.ClampMagnitude(mousePreviousPosition - mousePosition, 1f);

            if (mousePositionDelta.magnitude > minRotationDistance)
            {
                FlickAvailable = cameraController.CalculateFlickDirection(mousePositionDelta);
            }
        }

        private void RotationInput()
        {
            mousePosition = cameraController.getScreenToViewPort(Input.mousePosition);
            mousePositionDelta = mousePreviousPosition - mousePosition;
            cameraController.UpdateState(1);
            cameraController.Rotate(((Vector2)(mousePositionDelta)).InvertVectorNegY());
        }

        private void ZoomInput()
        {
            if (Input.touchCount >= 2)
            {
                var Touch0 = Input.GetTouch(0);
                var Touch1 = Input.GetTouch(1);

                if (Touch0.phase == TouchPhase.Began || Touch1.phase == TouchPhase.Began)
                {
                    touch0StartPosition = Touch0.position;
                    touch1StartPosition = Touch1.position;
                    startZoomDistance = Vector2.Distance(touch0StartPosition, touch1StartPosition);
                }

                if (Touch0.phase == TouchPhase.Moved || Touch1.phase == TouchPhase.Moved)
                {
                    touch0DeltaPosition = touch0StartPosition + Touch0.deltaPosition;
                    touch1DeltaPosition = touch1StartPosition + Touch1.deltaPosition;
                    deltaZoomDistance = Vector2.Distance(touch0DeltaPosition, touch1DeltaPosition);
                    pinchZoom = deltaZoomDistance - startZoomDistance;
                }

            }

            if (cameraController && (Input.touchCount >= 2 || Input.mouseScrollDelta.y != 0))
            {
                cameraController.UpdateState(2);
                cameraController.CalculateZoom(pinchZoom * touchSensitivity, Input.mouseScrollDelta.y);
            }
        }
        #endregion
    }
}