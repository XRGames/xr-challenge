using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class EventManager : MonoBehaviour
{
	public delegate void BasicAction(string a);
	public static event BasicAction BasicEvent;

	[SerializeField]
	private PlayerInput PlayerInput;


	//void Start()
	//{
	//	BasicEvent();
	//}

	private void Awake()
	{
		PlayerInput = GetComponent<PlayerInput>();
	}

	private void OnEnable()
	{
		PlayerInput.currentActionMap.FindAction("Action").started += Subscrip_Func;

		PlayerInput.currentActionMap["Walk"].performed += Subscrip_Func;
		PlayerInput.currentActionMap["Walk"].canceled += Subscrip_Func;
	}

	private void OnDisable()
	{
		PlayerInput.currentActionMap.FindAction("Action").started -= Subscrip_Func;

		PlayerInput.currentActionMap["Walk"].performed -= Subscrip_Func;
		PlayerInput.currentActionMap["Walk"].canceled -= Subscrip_Func;
	}

	private void OnApplicationQuit()
	{
		if (PlayerInput.currentActionMap.actions.Count > 0)
		{
			PlayerInput.currentActionMap.FindAction("Action").started -= Subscrip_Func;

			PlayerInput.currentActionMap["Walk"].performed -= Subscrip_Func;
			PlayerInput.currentActionMap["Walk"].canceled -= Subscrip_Func;

			Debug.LogWarning("Input Actions failed to un-subscribe on " + this.gameObject + ". They have been un-subscribed using OnApplicationQuit()");
		}
	}

	private void Subscrip_Func(InputAction.CallbackContext actionContext)
	{
		BasicEvent("Input info: " + actionContext.ReadValue<Vector2>());
	}
}
