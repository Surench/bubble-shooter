using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

	[SerializeField] private float duration;
	[SerializeField] private float magnitude;


	public void Shake()
	{
		StartCoroutine(ShakeRoutine());
	}

    IEnumerator ShakeRoutine ()
	{
		Vector3 originPos = transform.localPosition;

		float t = 0;
		while (t <duration)
		{
			float x = Random.Range(-0.5f, 0.5f) * magnitude;
			float y = Random.Range(-0.5f, 0.5f) * magnitude;

			transform.localPosition = new Vector3(x, y, originPos.z);

			t += Time.deltaTime;

			yield return null;
		}

		transform.localPosition = originPos;
	}
}
