using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
	[SerializeField] AnimationCurve animationCurve;

	private Vector3 initialScale;
	private Vector3 finalScale;
	private float graphValue;

	private void Awake()
	{
		initialScale = transform.localScale;
		finalScale = Vector3.one;
		animationCurve.postWrapMode = WrapMode.PingPong;
		animationCurve.preWrapMode = WrapMode.PingPong;
	}

	public void Awae()
	{		
		StartCoroutine(BouncingEffect());
	}


	IEnumerator BouncingEffect()
	{	

		float startTime = Time.time;
		float duration = 0.2f;
		float t = 0;

		Vector3 start = transform.position;
		Vector3 dir = transform.position - a.transform.position;

		float x = Mathf.Round(dir.normalized.x);
		float y = Mathf.Round(dir.normalized.y);

		while (t<2)
		{
			t = (Time.time - startTime) / duration;

			transform.position = Vector3.Lerp(start, new Vector3((animationCurve.Evaluate(t) * x) / 8, (animationCurve.Evaluate(t ) * y)/ 8, 0), t);

			yield return new WaitForEndOfFrame();
		}
	}

	[SerializeField] GameObject a;
	public void TakeNormal()
	{
		

	}


}
