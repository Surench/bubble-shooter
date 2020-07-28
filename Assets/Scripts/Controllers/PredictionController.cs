using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionController : MonoBehaviour
{
	[HeaderAttribute("Animation Curve")]
	public AnimationCurve animCurve;

	[SerializeField]
	private SpriteRenderer selfRenderer;
	private Coroutine ActivatePredictionC;
	private Coroutine DeActivatePredictionC;
	

	private void Start()
	{
		selfRenderer = GetComponent<SpriteRenderer>();	
	}

	public void ActivatePrediction(Color color)
	{
		gameObject.SetActive(true);
		color.a = 0.7f;
		selfRenderer.color = color;
		ActivatePredictionC = StartCoroutine(ActivatePredictionRoutin());
	}



	IEnumerator ActivatePredictionRoutin ()
	{
		float startTime = Time.time;
		float duration = 0.2f;
		float t = 0;
		float graphValue = 0;	

		Vector3 finalScale = new Vector3(0.65f, 0.65f,0);		
		

		while (t < 1)
		{			
			t = (Time.time - startTime) / duration;

			
			graphValue = animCurve.Evaluate(t);
			transform.localScale = finalScale * graphValue;

			yield return new WaitForEndOfFrame();
		}
	}

	


}
