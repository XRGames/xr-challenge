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

	[SerializeField]
	private float MoveMultiplier;

	private Rigidbody RB;

	private void Awake()
	{
		RB = GetComponent<Rigidbody>();
		MoveMultiplier = 1.0f;
	}

	public void UpdateMovement(string actionName, dynamic dataType)
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

	private void FixedUpdate()
	{
		if (isJump)
		{
			RB.velocity = Vector3.up * 10;
		}

		if (isSprint)
		{
			RB.velocity = new Vector3(MoveForce.x * (MoveMultiplier * 2), RB.velocity.y, MoveForce.y * (MoveMultiplier * 2));
		}
		else
		{
			RB.velocity = new Vector3(MoveForce.x * MoveMultiplier, RB.velocity.y, MoveForce.y * MoveMultiplier);
		}
	}
}
