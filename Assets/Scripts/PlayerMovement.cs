using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
	[SerializeField]
	private Vector2 MoveForce;

	[SerializeField]
	private bool isJump, isSprint;

	private Rigidbody RB;
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

	private void Awake()
	{
		RB = GetComponent<Rigidbody>();
	}


	void Response(string actionName, dynamic dataType)
	{
		switch (actionName)
		{
			case "Jump":
				if (dataType == 1)
					isJump = true;
				else
					isJump = false;

				break;
			case "Sprint":
				if (dataType == 1)
					isSprint = true;
				else
					isSprint = false;
				break;
			case "Walk":
				MoveForce = dataType;
				break;
			default:
				Debug.Log("No valid input type");
				break;
		}
	}

	private void Update()
	{
		RB.velocity = new Vector3(MoveForce.x, 0f, MoveForce.y);
	}
}
