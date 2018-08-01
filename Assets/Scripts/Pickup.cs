using System;
using UnityEngine;

public class Pickup : MonoBehaviour
{
	[SerializeField]
	private Transform model;

	[SerializeField]
	private int scoreValue;
	public int ScoreValue => scoreValue;

	public bool IsCollected { get; private set; }

	public event Action<Pickup> OnPickUp;

	private void OnTriggerEnter(Collider other)
	{
		HandleOnPickedUp();
	}

	private void HandleOnPickedUp()
	{
		if (IsCollected) return;

		OnPickUp?.Invoke(this);
		IsCollected = true;
	}

	private void Update()
	{
		if (IsCollected) return;

		model.localPosition = new Vector3(0f, Mathf.Lerp(0.35f, 0.55f, (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f), 0f);
		model.localEulerAngles = new Vector3(0f, Time.time * 180f % 360f, 0f);
	}
}