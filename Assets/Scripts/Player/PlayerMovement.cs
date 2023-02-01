using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; //For easily finding controllers

//Associate this script with the XR.Player library
//On compile Unity will make a .dll for each assembly definition, allowing us to organize our code into libraries to promote modularity and reusability
//By default every script without a .asmdef will be put into Assembly-CSharp.dll
namespace XR.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        //General
        private Transform cam;

        //Movement
        public float speed = 10;
        //--
        private CharacterController movementController;

        //Input
        [SerializeField] private Vector2 input; //stick / keyboard value (0-1)
        private Gamepad controller; //only x-input supported
        [SerializeField] private bool useController = false;

        // Start is called before the first frame update
        void Start()
        {
            //Check for controller + set if available
            CheckForController();

            movementController = GetComponent<CharacterController>();
            cam = Camera.main.transform;
            //camScript = cam.GetComponent<GameCam>();
        }

        // Update is called once per frame
        void Update()
        {
            //Read input every frame
            HandleInput();

            //Handle player movement every frame
            HandleMovement();
        }

        //--------------
        //INPUT---------
        private void CheckForController()
        {
            controller = Gamepad.current; //Just set last connected controller to main
            useController = (controller != null);
        }

        //Handle controller input
        private void HandleXinput()
        {
            Vector2 leftStick = controller.leftStick.ReadValue();
            input = leftStick;
        }

        //Handle keyboard input
        private void HandleKeyboardInput()
        {
            float forward = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            input = new Vector2(horizontal, forward);
        }

        //Main handle input method
        private void HandleInput()
        {
            //Check if controller is plugged in every frame
            //CheckForController();

            //Read input (value 0-1 x/y)
            if (useController)
                HandleXinput();
            else
                HandleKeyboardInput();
        }
        //--------------
        //MOVEMENT------

        //Handle main movement
        private void HandleMovement()
        {
            Vector3 cam_right = cam.right;
            Vector3 cam_forward = Vector3.Cross(cam_right, Vector3.up); //Calculate forward as camera forward is messed up in top down
            cam_forward.y = 0;
            cam_right.y = 0;

            Vector3 normal_move = (cam_forward * input.y) + (cam_right * input.x);
            Vector3 full_move = normal_move * speed;
            Vector3 move = full_move * Time.deltaTime;

            movementController.Move(move);
        }
        //--------------

    }   

}
