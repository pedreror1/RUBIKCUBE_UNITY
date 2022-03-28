using PEDREROR1.RUBIK.Utilities;
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
        enum CameraState
        {
            Idle=0,
            RotatingCamera=1,
            ZoomingCamera=2,
            Animating=3,
            WaitingForFLick=4
        }
        CameraState currentCameraState;
        
        private Camera _camera;

        private Vector3 target;

        Vector3 selectedCubletNormal;


        private Transform _transform;

        private float cameraDistance = 5;
        public Vector2 CameraZoomRange = new Vector2(3f, 15f);      

        public LayerMask CubletMask; 

        Vector2 orbitAngles = new Vector2(0, 0f);
        public void UpdateState(int newState) => currentCameraState = (CameraState)newState;

        private void Awake()
        {
            PlayerManager.Instance.CubeGeneratedEvnt += setup;
        }
        void setup()
        {
            _transform = transform;
            _camera = GetComponent<Camera>();
            target = PlayerManager.Instance.getCenter;
            cameraDistance = PlayerManager.Instance.dimension ;
            CameraZoomRange = new Vector2(PlayerManager.Instance.dimension * 1.5f,PlayerManager.Instance.dimension * 3f);
            _camera.orthographicSize = CameraZoomRange.x;
            _transform.position = target - _transform.forward * cameraDistance;
            CalculateZoom();
        }
        public Vector2 GetOrientedDirection(float yRotation, Vector2 mousePositionDelta, Vector3 normal)
        {
            if(Mathf.Round(normal.y)!=1f)
            {
                return mousePositionDelta;
            }
            switch(yRotation)
            {
                case 0:
                case 360:
                    print(mousePositionDelta+" / " +mousePositionDelta.NegY());
                    return mousePositionDelta.NegY();
                case 90:
                    return mousePositionDelta.invertVector();
                case 180:
                    return mousePositionDelta.NegX();
                case 270:
                    return mousePositionDelta.invertVector().NegXY();

            }
            return mousePositionDelta;
        }

        public bool CalculateFlickDirection(Vector3 mousePositionDelta )
        {
                var currentCameraAngle=((transform.rotation.eulerAngles.y%360 +360)%360);
                currentCameraAngle=Mathf.Round(currentCameraAngle / 90) * 90;               
                PlayerManager.Instance.TryRotate(GetOrientedDirection(currentCameraAngle,mousePositionDelta,selectedCubletNormal));
                return false;
            
        }
        public Vector3 getScreenToViewPort(Vector2 mousePosition)
        {
            return Vector3.ClampMagnitude(_camera.ScreenToViewportPoint(Input.mousePosition), 1f);
        }       

        public void Rotate(Vector2 offset)
        {
            orbitAngles -= offset;
            Quaternion lookRotatipn = Quaternion.Euler(orbitAngles);

            Vector3 lookDirection = lookRotatipn * Vector3.forward;
            Vector3 lookPosition = target - lookDirection * cameraDistance;
            _transform.SetPositionAndRotation(lookPosition, lookRotatipn);
        }
      
        //TODO implement pinch in/out
        public void CalculateZoom(float touchZoom=0, float MouseZoom=0, float extraZoom=0)
        {
            
               
            cameraDistance = Mathf.Clamp(cameraDistance - (touchZoom + MouseZoom + extraZoom), CameraZoomRange.x, CameraZoomRange.y);
            _camera.orthographicSize = cameraDistance;

        }

        
        public bool tryGetCublet()
        {
            if (currentCameraState== CameraState.RotatingCamera) return false;
            
            Vector3 mousePosition3D = Input.mousePosition;
            mousePosition3D.z = 10;
            Ray camRay = _camera.ScreenPointToRay(mousePosition3D);
            RaycastHit hit;
            if (Physics.Raycast(camRay, out hit,10f,CubletMask))
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
    }
}

