using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	[SerializeField] 
	private GameObject Player;

	//Connect senders and receivers
	private void Awake()
	{
		InputManager.Event_RecognisedInput += RecognisedInput_Response;
	}

	//Disconnect senders and recievers
	private void OnApplicationQuit()
	{
		InputManager.Event_RecognisedInput -= RecognisedInput_Response;
	}



	void RecognisedInput_Response(string actionName, dynamic data)
	{
		Player.GetComponent<PlayerMovement>().UpdateMovement(actionName, data);
	}
}
