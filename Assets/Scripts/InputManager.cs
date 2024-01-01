using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
	public delegate void Delegate_RecognisedInput(string actionName, dynamic data);
	public static event Delegate_RecognisedInput Event_RecognisedInput;
	
	private PlayerInput PlayerInput;

	private void Awake()
	{
		PlayerInput = GetComponent<PlayerInput>();
	}

	private void OnEnable()
	{

		//Reminds what input map you have selected in editor
		PlayerInput.SwitchCurrentActionMap("RH_Movement");
		Debug.Log("Current Action Map is: " + PlayerInput.currentActionMap.name);

		/*
		 * These subscribe (successful inputs understood inside the input 
		 * asset) to (an event which broadcasts info about the occured input)
		 */
		PlayerInput.currentActionMap["Walk"].performed += Detected_Input;
		PlayerInput.currentActionMap["Walk"].canceled += Detected_Input;

		PlayerInput.currentActionMap["Sprint"].performed += Detected_Input;
		PlayerInput.currentActionMap["Sprint"].canceled += Detected_Input;

		PlayerInput.currentActionMap["Jump"].performed += Detected_Input;
		PlayerInput.currentActionMap["Jump"].canceled += Detected_Input;

	}

	//private void OnDisable()
	//{

	//	/*
	//	 * These unsubscribe the existing inputs defined above. Unsubscribing is
	//	 * not automatic, therefore needs manual clean-up when the gameObject 
	//	 * is disabled
	//	 */

	//	PlayerInput.currentActionMap["Walk"].performed -= Detected_Input;
	//	PlayerInput.currentActionMap["Walk"].canceled -= Detected_Input;

	//	PlayerInput.currentActionMap["Sprint"].performed -= Detected_Input;
	//	PlayerInput.currentActionMap["Sprint"].canceled -= Detected_Input;

	//	PlayerInput.currentActionMap["Jump"].performed -= Detected_Input;
	//	PlayerInput.currentActionMap["Jump"].canceled -= Detected_Input;

	//	Debug.Log("Input Actions successfully unsubscribed on " + this.gameObject);

	//}

	private void OnApplicationQuit()
	{

		/*
			* A last resort which kicks in if the movement events don't 
			* unsubscribe properly
			*/
		PlayerInput.currentActionMap["Walk"].performed -= Detected_Input;
		PlayerInput.currentActionMap["Walk"].canceled -= Detected_Input;

		PlayerInput.currentActionMap["Sprint"].performed -= Detected_Input;
		PlayerInput.currentActionMap["Sprint"].canceled -= Detected_Input;

		PlayerInput.currentActionMap["Jump"].performed -= Detected_Input;
		PlayerInput.currentActionMap["Jump"].canceled -= Detected_Input;

		Debug.LogWarning("Input Actions failed to un-subscribe on " + this.gameObject + ". They have been un-subscribed using OnApplicationQuit()");

	}

	private void Detected_Input(InputAction.CallbackContext actionContext)
	{

		switch (actionContext.action.name)
		{
			case "Jump":
				Event_RecognisedInput(actionContext.action.name, actionContext.ReadValue<float>());
				break;
			case "Sprint":
				Event_RecognisedInput(actionContext.action.name, actionContext.ReadValue<float>());
				break;
			case "Walk":
				Event_RecognisedInput(actionContext.action.name, actionContext.ReadValue<Vector2>());
				break;
			default:
				Debug.Log("No valid input type");
				break;
		}

	}
}
