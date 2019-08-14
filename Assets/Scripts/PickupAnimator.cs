using UnityEngine;

public class PickupAnimator : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private Renderer modelRenderer;

	[Header("Effects")]
	[SerializeField]
	private ParticleSystem idleEffect;

	[SerializeField]
	private ParticleSystem collectEffect;

	/// <summary>
	/// Play the idle animation and effects.
	/// </summary>
	public void PlayIdle()
	{
		enabled = true;
		modelRenderer.enabled = true;
		idleEffect.Play();
	}

	/// <summary>
	/// Play the collected animation and effects.
	/// </summary>
	public void PlayCollected()
	{
		enabled = false;
		modelRenderer.enabled = false;
		idleEffect.Stop();
		collectEffect.Play();
	}

	private void Update()
	{
		var modelTransform = modelRenderer.transform;
		modelTransform.localPosition = new Vector3(0f, Mathf.Lerp(0.35f, 0.55f, (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f), 0f);
		modelTransform.localEulerAngles = new Vector3(0f, Time.time * 180f % 360f, 0f);
	}
}
