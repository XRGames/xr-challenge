using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class EventManager : MonoBehaviour
{
	public delegate void BasicAction(string actionName, dynamic dataType);
	public static event BasicAction BasicEvent;

	[SerializeField]
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
		PlayerInput.currentActionMap["Walk"].performed += Subscrip_Test_Func;
		PlayerInput.currentActionMap["Walk"].canceled += Subscrip_Test_Func;

		PlayerInput.currentActionMap["Sprint"].performed += Subscrip_Test_Func;
		PlayerInput.currentActionMap["Sprint"].canceled += Subscrip_Test_Func;

		PlayerInput.currentActionMap["Jump"].performed += Subscrip_Test_Func;
		PlayerInput.currentActionMap["Jump"].canceled += Subscrip_Test_Func;

	}

	private void OnDisable()
	{
		/*
		 * These unsubscribe the existing inputs defined above. Unsubscribing is
		 * not automatic, therefore needs manual clean-up when the gameObject 
		 * is disabled
		 */
		if (PlayerInput.currentActionMap.actions.Count > 0)
		{
			PlayerInput.currentActionMap["Walk"].performed -= Subscrip_Test_Func;
			PlayerInput.currentActionMap["Walk"].canceled -= Subscrip_Test_Func;

			PlayerInput.currentActionMap["Sprint"].performed -= Subscrip_Test_Func;
			PlayerInput.currentActionMap["Sprint"].canceled -= Subscrip_Test_Func;

			PlayerInput.currentActionMap["Jump"].performed -= Subscrip_Test_Func;
			PlayerInput.currentActionMap["Jump"].canceled -= Subscrip_Test_Func;
		}
	}

	private void OnApplicationQuit()
	{
		/*
		 * A last resort which kicks in if the movement events don't 
		 * unsubscribe properly
		 */
		PlayerInput.currentActionMap["Walk"].performed -= Subscrip_Test_Func;
		PlayerInput.currentActionMap["Walk"].canceled -= Subscrip_Test_Func;

		PlayerInput.currentActionMap["Sprint"].performed -= Subscrip_Test_Func;
		PlayerInput.currentActionMap["Sprint"].canceled -= Subscrip_Test_Func;

		PlayerInput.currentActionMap["Jump"].performed -= Subscrip_Test_Func;
		PlayerInput.currentActionMap["Jump"].canceled -= Subscrip_Test_Func;

		Debug.LogWarning("Input Actions failed to un-subscribe on " + this.gameObject + ". They have been un-subscribed using OnApplicationQuit()");

	}

	//private void Subscrip_Test_Func3(InputAction.CallbackContext actionContext)
	//{
	//	BasicEvent("Input info: " + actionContext.ReadValue<Vector2>());

	//}

	//private void Subscrip_Test_Func2(InputAction.CallbackContext actionContext)
	//{
	//	BasicEvent("Input info: " + actionContext.ReadValue<float>());
	//}

	private void Subscrip_Test_Func(InputAction.CallbackContext actionContext)
	{
		//if (actionContext.action.name == "Walk")
		//{
		//	BasicEvent( "Input info: " + actionContext.action.name + " " + actionContext.ReadValue<Vector2>());
		//}
		//else if (actionContext.action.name == "Sprint")
		//{
		//	BasicEvent("Input info: " + actionContext.action.name + " " + actionContext.ReadValue<float>());
		//}
		//else if (actionContext.action.name == "Jump")
		//{
		//	BasicEvent("Input info: " + actionContext.action.name + " " + actionContext.ReadValue<float>());
		//}

		switch(actionContext.action.name)
		{
			case "Jump":
				BasicEvent(actionContext.action.name, actionContext.ReadValue<float>());
				break;
			case "Sprint":
				BasicEvent(actionContext.action.name, actionContext.ReadValue<float>());
				break;
			case "Walk":
				BasicEvent(actionContext.action.name, actionContext.ReadValue<Vector2>());
				break;
			default:
				Debug.Log("No valid input type");
				break;
		}
	}
}
