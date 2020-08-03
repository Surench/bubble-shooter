using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;



public class BallController : MonoBehaviour
{
	[HeaderAttribute("Animation Curve")]
	public AnimationCurve animCurve;

	[SerializeField] Animator anim;
	[SerializeField] private TextMeshPro numberText;
	[SerializeField] private TextMeshPro animNumberText;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private SpriteRenderer shadowRenderer;
	[SerializeField] private ParticleSystem explosionParticle;

	private readonly string ballLayer = "Ball";
	private readonly string predictionLayer = "Prediction";
	private readonly string animTrigger = "UpdateText";

	private Coroutine UpdateBallPosC;
	private Transform similarBall;
	public Vector3 startPos;

	private Color color;
	private Color shadowColor = Color.white;
	private float moveDuration;
	public bool lastball;
	public int number;
	public int index;
	
	

	public void InitBall()
	{
		gameObject.layer = LayerMask.NameToLayer(ballLayer);

		int random = Random.Range(0, 8); 

		number = ScoreManager.self.GetNumber(random);
		color = ScoreManager.self.GetNewColor(number);

		numberText.text = number.ToString();
		shadowColor.a = 0.2f;
		spriteRenderer.color = color;
		shadowRenderer.color = shadowColor;		

		StartCoroutine(SmoothStart());
	}	
	

	public void ActivateBall(Color newColor,int number)
	{
		this.number = number;
		this.color = newColor;
		gameObject.layer = LayerMask.NameToLayer(ballLayer);

		newColor.a = 1;
		shadowColor.a = 0.2f;
		spriteRenderer.color = newColor;
		shadowRenderer.color = shadowColor;
		numberText.text = number.ToString();

		GirdManager.self.FirstHit(index, number);
	}


	public void DeActivateBall()
	{
		gameObject.layer = LayerMask.NameToLayer(predictionLayer);
		transform.position = startPos;
		this.number = 0;
		color = Color.white; shadowColor = Color.white;
		color.a = 0f; shadowColor.a = 0;

		shadowRenderer.color = shadowColor;
		spriteRenderer.color = color;
		numberText.text = "";
	}


	public void MoveToSimilarBall(Transform ballPos, float moveDuration)
	{
		ActivateExplosion();

		this.moveDuration = moveDuration;
		this.similarBall = ballPos;
		numberText.text = "";
		UpdateBallPosC = StartCoroutine(UpdateBallPos());	
	}


	public void UpdateFinalScore(int multiplier)
	{		
		this.number = multiplier;

		numberText.text = number.ToString();

		color = ScoreManager.self.GetNewColor(number);
		spriteRenderer.color = color;
	}

	public void ActivateAnimation()
	{
		animNumberText.text = number.ToString();
		anim.SetTrigger(animTrigger);
	}

	IEnumerator SmoothStart()
	{
		float randomDuration = Random.Range(0f, 0.6f);
		float t = 0;
		float graphV = 0;
		float startTime = Time.time;
		Vector3 finalScale = transform.localScale;
		while (t < 1)
		{
			t = (Time.time - startTime) / randomDuration;

			graphV = animCurve.Evaluate(t);
			transform.localScale = finalScale * graphV;

			yield return new WaitForEndOfFrame();
		}
	}


	IEnumerator UpdateBallPos()
	{
		float startTime = Time.time;		
		float t = 0;
		
		while (t<1)
		{
			t = (Time.time - startTime) / moveDuration;
			transform.position = Vector3.Lerp(startPos, similarBall.position, t);
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(0.1f);
		DeActivateBall();
	}

	

	void ActivateExplosion()
	{
		ParticleSystem.MainModule settings;
		settings = explosionParticle.main;
		color.a = 0.4f;
		settings.startColor = new ParticleSystem.MinMaxGradient(color);
		explosionParticle.Play();
	}
	
	
}
