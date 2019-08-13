using UnityEngine;

public class PickupAnimator : MonoBehaviour
{
	[SerializeField]
	private Transform model;

	private void Update()
	{
		model.localPosition = new Vector3(0f, Mathf.Lerp(0.35f, 0.55f, (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f), 0f);
		model.localEulerAngles = new Vector3(0f, Time.time * 180f % 360f, 0f);
	}
}
