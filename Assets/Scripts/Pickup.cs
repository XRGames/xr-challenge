using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	[SerializeField]
	private int scoreValue;
	public int ScoreValue => scoreValue;

	[SerializeField, Tooltip("Your")]
	private PickupAnimator animator;

	/// <summary>
	/// Indicates if the Pickup has been collected or not
	/// </summary>
	public bool IsCollected { get; private set; }

	public event Action<Pickup> OnPickUp;

	private void Start()
	{
		Init();
	}

	/// <summary>
	/// Initialise and reset the properties.
	/// Make the Pickup available again.
	/// </summary>
	public void Init()
	{
		IsCollected = false;
		OnPickUp = null;
		animator.enabled = true;
	}

	/// <summary>
	/// Pick up the pickup.
	/// </summary>
	/// <returns>The score. Return -1 if failed to collect.</returns>
	public int GetPickedUp()
	{
		if (IsCollected) return -1;

		HandleOnPickedUp();

		return ScoreValue;
	}

	private void HandleOnPickedUp()
	{
		IsCollected = true;
		OnPickUp?.Invoke(this);
	}
}