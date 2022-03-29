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
        public float rotationSpeed = 0.1f;
        [Tooltip("Rotation Movement dead zone")]
        public float deadZone = 0.25f;

        private Vector2 touch0StartPosition, touch1StartPosition, touch0DeltaPosition, touch1DeltaPosition;
        private float startZoomDistance, deltaZoomDistance, pinchZoom;
        private Vector3 mousePosition, mousePreviousPosition;
        private Vector3 mousePositionDelta;
        private bool flickAvailable = true;
        private bool isZooming = false;
        #endregion

        #region METHODS
        private void Awake()
        {
            if (cameraController == null)
            {
                cameraController = GetComponent<CameraController>();
            }
        }
        private void Update()
        {
            if (PlayerManager.Instance.currentState != PlayerManager.GameState.Playing) return;

            if (Input.GetMouseButtonDown(0))
            {
                mousePreviousPosition = cameraController.GetScreenToViewPort(Input.mousePosition);
                if (!PlayerManager.Instance.hasCublet)
                {
                    cameraController.TryGetCublet();
                }
            }

            if (mousePreviousPosition == Vector3.zero) return;

            if (Input.GetMouseButton(0))
            {
                if (!PlayerManager.Instance.hasCublet && Input.touchCount < 2)
                {
                    if (!isZooming)
                        RotationInput();
                }
                else if (PlayerManager.Instance.hasCublet && flickAvailable)
                {
                    FlickInput();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                flickAvailable = true;
                cameraController.UpdateState(0);
                PlayerManager.Instance.RemoveCurrentCublet();
            }
            ZoomInput();

        }

        Vector3 previouseMouseposition;

        private void FlickInput()
        {
            cameraController.UpdateState(4);
            mousePosition = cameraController.GetScreenToViewPort(Input.mousePosition);
            mousePositionDelta = Vector3.ClampMagnitude(mousePreviousPosition - mousePosition, 1f);

            if (mousePositionDelta.magnitude > minRotationDistance)
            {
                flickAvailable = cameraController.CalculateFlickDirection(mousePositionDelta);
            }
        }

        private void RotationInput()
        {
            mousePosition = cameraController.GetScreenToViewPort(Input.mousePosition);
            mousePositionDelta = mousePreviousPosition - mousePosition;
            cameraController.UpdateState(1);
            cameraController.Rotate(((Vector2)(mousePositionDelta)).InvertVectorNegY() * rotationSpeed);
            mousePreviousPosition = mousePosition;
        }

        private void ZoomInput()
        {
            if (Input.touchCount >= 2)
            {

                var Touch0 = Input.GetTouch(0);
                var Touch1 = Input.GetTouch(1);

                if (Touch0.phase == TouchPhase.Began || Touch1.phase == TouchPhase.Began)
                {
                    isZooming = true;
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
                if (Touch0.phase == TouchPhase.Ended && Touch1.phase == TouchPhase.Ended)
                {
                    isZooming = false;
                }
            }
            else if (Input.touchCount == 0)
            {
                isZooming = false;
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