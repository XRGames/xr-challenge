using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
	void OnEnable()
	{
		EventManager.BasicEvent += Response;
		//do function to enable subscriptions
	}


	void OnDisable()
	{
		EventManager.BasicEvent -= Response;
		//do function which checks if they are full and if so, clear subscriptions
	}


	void Response(string a)
	{
		Debug.Log("BasicEvent() called. Info ¦ " + a);
	}
}
