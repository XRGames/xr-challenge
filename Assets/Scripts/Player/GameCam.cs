using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //For easily finding controllers

namespace XR.Player
{
    public class GameCam : MonoBehaviour
    {
        //General
        public Transform lookAt;
        //--
        private Gamepad myGamepad;
        private CharacterController controller;
        private const float YMin = -75.0f; //max y angle
        private const float YMax = 50.0f; //max x angle
        [SerializeField] private Vector3 myOffset;

        //LERP (zoom out cam)
        public AnimationCurve curve;
        public float zoomTime = 0.75f; //Time it takes to zoom out (cur value)
        //--
        private float originalZoomTime; //Original inspector value of zoom out time
        private float lerpStartZoom = 0f; //Starting value for lerp
        private float lerpStartTime = -1; //Start time for lerp
        private float lerpZoom = 0; //Lerp zoom value
        private bool isLerpingZoom = false; //Is lerping?
        private float lerpEvaluation; //Current lerp value
        
        //Controller
        private float currentX = 0.0f;
        private float currentY = 0.0f;
        //--
        public int sensivity_x = 75;
        public int sensivity_y = 50;

        //Main
        void Awake () {
            myOffset = transform.localPosition; //Initial camera offset (set in prefab)
            originalZoomTime = zoomTime + 0;//Store original zoom time (before we modify it)
            Transform MainTransform = transform.root;
            controller = MainTransform.GetComponent<CharacterController>();
            SetupController();

            transform.SetParent(null);
        }

        private void SetupController()
        {
            if (Gamepad.all.Count > 0)
                myGamepad = Gamepad.all[0];
        }

        //-- -- -- -- --
        //MAIN CAM METHODS
        //Get intial camera position
        private Vector3 getLookAtPos()
        {
            Vector3 lookAtPos = lookAt.position + (transform.right * myOffset.x);
            lookAtPos.y = lookAt.position.y + myOffset.y;

            return lookAtPos;
        }

        //Zoom camera in/out
        public void LerpZoom( float zoom, float newTime=-1f) //Zoom to val, time in seconds (optional)
        {
            lerpStartZoom = Mathf.Lerp( lerpStartZoom, lerpZoom, lerpEvaluation ); //Starting zoom is current evaluation of lerp start-end zoom
            lerpZoom = zoom; //Set new value to zoom to

            //Set zoom time (param/default value)
            if (newTime == -1)
                zoomTime = originalZoomTime;
            else
                zoomTime = newTime;

            lerpStartTime = Time.time;
            isLerpingZoom = true;
        }
    
        private void ReadControllerInput()
        {
            //Stick input this frame
            currentX += myGamepad.rightStick.x.ReadValue() * sensivity_x * Time.deltaTime;
            currentY -= myGamepad.rightStick.y.ReadValue() * sensivity_y * Time.deltaTime;
    
            //Clamp angles
            currentY = Mathf.Clamp(currentY, YMin, YMax);
            if (currentX < 0)
                currentX = 360;
            else if (currentX > 360)
                currentX = (currentX - 360);
        }

        //Update last after everything 
        private void LateUpdate()
        {
            //Zoom out if player is moving
            float speed = controller.velocity.magnitude;
            float zoomTo = 4;
            if (speed >= 2 && lerpZoom != 0 && !isLerpingZoom) //if moving, not zoomed out, not currently lerping
                LerpZoom(0, 2);//zoom in all the way (in 2 seconds)
            else if (lerpZoom == 0 && speed < 2) //If not zoomed out, not moving
                LerpZoom(zoomTo, 2); //Zoom out 4 units

            //-- --
            //MAIN
            Vector3 lookAtPos = getLookAtPos(); //Get intial Camera position (initial transform in World Co-ords)

            //Lerp zoom
            if (isLerpingZoom){ //If lerping zoom
                Vector3 finalPos = lookAtPos + (transform.forward * lerpZoom); //End position
                Vector3 lerpStartPos = lookAtPos + (transform.forward * lerpStartZoom); //Start position

                float endTime = lerpStartTime + zoomTime; //End time of lerp
                float timeLeft = (endTime - Time.time); //Time left
                float step = Mathf.Clamp((zoomTime - timeLeft)/zoomTime, 0, 1); //Max time - time left (divide by) max time
                step = curve.Evaluate( step ); //Give it a nice animation curve
                lerpEvaluation = step; //Store our current evaluation in case of zooming during a zoom

                Vector3 lerpedPos = Vector3.Lerp(lerpStartPos, finalPos, step); //Final lerped pos

                lookAtPos = lerpedPos; //Update current position of cam

                //End lerp
                if (step >= 1)
                    isLerpingZoom = false;
            } else {
                lookAtPos += (transform.forward * lerpZoom);
            }
            //--
            Vector3 back_vec = (transform.forward * myOffset.z);
            lookAtPos += back_vec; //Move camera back by initial z offset

            //-- --
            //Rotate based on controller/mouse movement
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0); //Mouse/Controller rotation
            Vector3 Direction = new Vector3(0,0,1); //Z-Axis

            transform.position = lookAtPos + rotation * Direction; //Initial position rotated around z-axis by camera angles
            //Pos (initial) + Quaternion(angles) *(multiplied by) Vector Direction 
            //Rotate around by camera angle values
            //-- -- --

            //Look at our player every frame
            transform.LookAt(lookAtPos);
        }

        //Read input every frame
        private void Update()
        {
            if (myGamepad != null)
                ReadControllerInput();
        }
    }
}
