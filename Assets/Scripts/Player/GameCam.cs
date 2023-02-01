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
        
        //Input
        public int sensivity_controller_x = 75;
        public int sensivity_controller_y = 50;
        public int sensivity_mouse_x = 50;
        public int sensivity_mouse_y = 35;
        //--
        [SerializeField] private float currentX = 0.0f;

        //--
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
        //READ INPUT  --
        private void ReadControllerInput()
        {
            //Stick input this frame
            currentX += myGamepad.rightStick.x.ReadValue() * sensivity_controller_x * Time.deltaTime;
    
            //Clamp angles
            if (currentX < 0)
                currentX = 360;
            else if (currentX > 360)
                currentX = (currentX - 360);
        }

        private void ReadMouseInput()
        {
            currentX += Input.GetAxis("Mouse X") * sensivity_mouse_x * Time.deltaTime;
    
            //Clamp angles
            if (currentX < 0)
                currentX = 360 + currentX;
            else if (currentX > 360)
                currentX = (currentX - 360);            
        }
        //-- -- -- -- -- --
        //MAIN CAM METHODS-
        //Get intial camera position
        private Vector3 getLookAtPos()
        {
            Vector3 lookAtPos = lookAt.position + (transform.right * myOffset.x);
            lookAtPos.y = lookAt.position.y + myOffset.y;

            Vector3 back_vec = (transform.forward * myOffset.z);
            lookAtPos += back_vec; //Move camera back by initial z offset

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

        //Update last after everything 
        void LateUpdate()
        {
            //Zoom out if player is moving
            float speed = controller.velocity.magnitude;
            float zoomTo = 4;
            if (speed >= 2 && lerpZoom != 0) //if moving, not zoomed out, not currently lerping
                LerpZoom(0, 2);//zoom in all the way (in 2 seconds)
            else if (lerpZoom == 0 && speed < 2) //If not zoomed out, not moving
                LerpZoom(zoomTo, 2); //Zoom out 4 units

            //-- -- --
            //MAIN  --
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
            //Rotate based on controller/mouse movement
            Quaternion rotation = Quaternion.Euler(0, currentX, 0); //Mouse/Controller rotation
            Vector3 Direction = new Vector3(0,0,1); //Z-Axis

            transform.position = lookAtPos + rotation * Direction;//Initial position rotated around z-axis by camera angles
            //Pos (initial) + Quaternion(angles) *(multiplied by) Vector Direction 
            //Rotate around by camera angle values
            //-- -- --
            //Look at our player every frame
            transform.LookAt(lookAt.position);
        }

        //Read input every frame
        void Update()
        {
            if (myGamepad != null)
                ReadControllerInput();
            else
                ReadMouseInput();

            //CameraUpdate();
        }
    }
}
