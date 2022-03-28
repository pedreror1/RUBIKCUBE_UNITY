using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// The CameraController Class Contains The Implementation To:
/// *Rotate Around The Cube
/// *Zoom In and Out from The Cube
/// *Detect if a Cublet Is Selected
/// </summary>
namespace PEDREROR1.RUBIK
{

    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Camera _camera;

        private Vector3 target;
      

        bool MovingCamera = false;

        private Transform _transform;

        private float cameraDistance = 5;

        public float RotationSpeed = 0.1f;
        public float deadZone = 0.25f;
        // Update is called once per frame

        private Vector3 mousePosition, mousePreviousPosition;
        public Vector3 mousePositionDelta;

        public Vector2 CameraZoomRange = new Vector2(3f, 15f);
        //TODO move to CUbe controller
        [SerializeField] CubeGenerator cubeGenerator;

        public float minRotationMagnitude = 0.5f;

        public LayerMask CubletMask;

        private void Awake()
        {
            PlayerManager.Instance.CubeGeneratedEvnt += setup;
        }
        void setup()
        {
            _transform = transform;
            _camera = GetComponent<Camera>();
            target = cubeGenerator ? cubeGenerator.getCenter : Vector3.zero;
            cameraDistance = PlayerManager.Instance.dimension ;

            CameraZoomRange = cubeGenerator ? new Vector2(PlayerManager.Instance.dimension * 1.5f,PlayerManager.Instance.dimension * 3f) : CameraZoomRange;
            _camera.orthographicSize = CameraZoomRange.x;
            _transform.position = target - _transform.forward * cameraDistance;
            CalculateZoom();
        }
        void Update()
        {
            if (PlayerManager.Instance.currentState != PlayerManager.GameState.Playing) return;
            if (Input.GetMouseButton(0))
            {

                if (!PlayerManager.Instance.hasCublet && !CheckCollision())
                {
                    MovingCamera = true;
                    CalculateCameraRotationInput();
                   
                }
                if (!MovingCamera && PlayerManager.Instance.hasCublet)
                {
                    CalculateFlickDirection();
                }

            }
            if (Input.GetMouseButtonUp(0))
            {
                MovingCamera = false;
                PlayerManager.Instance.RemoveCurrentCublet();
            }
            CalculateZoom();
        }



        private void CalculateFlickDirection()
        {

            mousePosition = _camera.ScreenToViewportPoint(Input.mousePosition);
            mousePositionDelta = Vector3.ClampMagnitude(mousePreviousPosition - mousePosition, 1f);

            if (mousePositionDelta.magnitude > minRotationMagnitude)
            {
                PlayerManager.Instance.TryRotate(mousePositionDelta);
            }
        }
        Vector2 orbitAngles = new Vector2(0, 0f);

        public void CalculateCameraRotationInput()
        {
            if (Input.touchCount < 2)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mousePreviousPosition = Vector3.ClampMagnitude(_camera.ScreenToViewportPoint(Input.mousePosition), 1f);
                }

                if (Input.GetMouseButton(0))
                {
                    mousePosition = Vector3.ClampMagnitude(_camera.ScreenToViewportPoint(Input.mousePosition), 1f);
                    mousePositionDelta = mousePreviousPosition - mousePosition;



                    Rotate(((Vector2)(mousePositionDelta)).InvertVector());
                }
            }
        }

        public void Rotate(Vector2 offset)
        {
            orbitAngles -= offset;
            Quaternion lookRotatipn = Quaternion.Euler(orbitAngles);

            Vector3 lookDirection = lookRotatipn * Vector3.forward;
            Vector3 lookPosition = target - lookDirection * cameraDistance;
            _transform.SetPositionAndRotation(lookPosition, lookRotatipn);
        }
        public Vector2 pos1, pos2, pos1Delta, pos2Delta;
        public float dist1, pinchZoom;
        public float touchSensitivity = 0.1f;
        //TODO implement pinch in/out
        public void CalculateZoom(float extraZoom=0)
        {
            
                if (Input.touchCount >= 2)
                {
                    var Touch0 = Input.GetTouch(0);
                    var Touch1 = Input.GetTouch(1);

                    if (Touch0.phase == TouchPhase.Began || Touch1.phase == TouchPhase.Began)
                    {
                        pos1 = Touch0.position;
                        pos2 = Touch1.position;
                        dist1 = Vector2.Distance(pos1, pos2);
                    }


                    if (Touch0.phase == TouchPhase.Moved || Touch1.phase == TouchPhase.Moved)
                    {
                        pos1Delta = pos1 + Touch0.deltaPosition;
                        pos2Delta = pos2 + Touch1.deltaPosition;
                        pinchZoom = PlayerManager.Instance.currentState== PlayerManager.GameState.Playing? Vector2.Distance(pos1Delta, pos2Delta)-dist1:0;
                        print(pinchZoom);
                        

                    }
                 





            }
            cameraDistance = Mathf.Clamp(cameraDistance - ((pinchZoom * touchSensitivity) + Input.mouseScrollDelta.y + extraZoom), CameraZoomRange.x, CameraZoomRange.y);
            _camera.orthographicSize = cameraDistance;

        }
     

        public bool CheckCollision()
        {
            if (MovingCamera) return false;
           
            Vector3 mousePosition3D = Input.mousePosition;
            mousePosition3D.z = 10;
            Ray camRay = _camera.ScreenPointToRay(mousePosition3D);
            RaycastHit hit;
            if (Physics.Raycast(camRay, out hit,10f,CubletMask))
            {
                //Debug.Log($"{hit.transform.name} normal {hit.normal} ");
                if (!PlayerManager.Instance.hasCublet)
                    PlayerManager.Instance.UpdateCurrentCublet(hit);
                mousePreviousPosition = Vector3.ClampMagnitude(_camera.ScreenToViewportPoint(Input.mousePosition), 1f);
                
                return true;
            }
            else
            {

                return false;
            }





        }
    }
}

