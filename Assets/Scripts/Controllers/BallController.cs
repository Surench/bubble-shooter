using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;



public class BallController : MonoBehaviour
{
	[HeaderAttribute("Animation Curve")]
	public AnimationCurve animCurve;

	[SerializeField] private TextMeshPro numberText;
	[SerializeField] private SpriteRenderer spriteRenderer;

	private readonly string ballLayer = "Ball";
	private readonly string predictionLayer = "Prediction";

	private Transform similarBall;
	private Vector3 startPos;

	private Color color;
	public int number;
	

	public void Start()
	{
		startPos = transform.position;
	}



	public void BeRandomized()
	{
		gameObject.layer = LayerMask.NameToLayer(ballLayer);

		int random = Random.Range(0, 8);

		number = ScoreManager.self.GetNumber(random);
		color = ScoreManager.self.GetNewColor(number);

		numberText.text = number.ToString();
		spriteRenderer.color = color;

		StartCoroutine(SmoothStart());
	}
	
	IEnumerator SmoothStart()
	{
		float randomDuration = Random.Range(0f, 0.6f);
		float t = 0;
		float graphV = 0;
		float startTime = Time.time;
		Vector3 finalScale = transform.localScale;
		Vector3 startScale = Vector3.zero;
		while (t<1)
		{
			t = (Time.time - startTime) / randomDuration;

			graphV = animCurve.Evaluate(t);
			transform.localScale = finalScale * graphV;

			yield return new WaitForEndOfFrame();
		}
	}






	public void ActivateBall(Color newColor,int number)
	{
		this.number = number;
		this.color = newColor;
		gameObject.layer = LayerMask.NameToLayer(ballLayer);

		newColor.a = 1;		
		spriteRenderer.color = newColor;
		numberText.text = number.ToString();

		FindSimilarBalls();
		FilterFoundedBalls();
		UpdateBallScore(); 
	}

	
	[SerializeField] List<BallController> filteredBalls = new List<BallController>();
	List<BallController> colliders = new List<BallController>();

	public void FindSimilarBalls()
	{
		filteredBalls.Clear();
		colliders.Clear();

		RaycastHit2D[] hits = new RaycastHit2D[6];		
		ContactFilter2D filter = new ContactFilter2D() { };
		
		int numHits = Physics2D.CircleCast(transform.position,0.6f,Vector3.zero, filter,hits);
			   	
		for (int i = 0; i < numHits; i++)
		{
			BallController ball = hits[i].collider.GetComponent<BallController>();

			if (ball !=null) colliders.Add(ball);			
		}

		
	}

	void FilterFoundedBalls()
	{		
		foreach (BallController ball in colliders)
		{
			if ((ball.number == number) && (ball != this))
			{
				filteredBalls.Add(ball);
			}
		}

		for (int i = 0; i < filteredBalls.Count; i++)
		{
			filteredBalls[i].MoveToSimilarBall(transform);
			number += filteredBalls[i].number;
		}
		
	}	

	void UpdateBallScore()
	{	
		if (filteredBalls.Count > 0) 
		{
			number = GetFinalScore(number);
			color = ScoreManager.self.GetNewColor(number);
			spriteRenderer.color = color;
			numberText.text = number.ToString();
			ScoreManager.self.AddScore(number);
		}
	}

	public void MoveToSimilarBall(Transform ballPos)
	{
		this.similarBall = ballPos;
		UpdateBallPosC = StartCoroutine(UpdateBallPos());
	}

	Coroutine UpdateBallPosC;
	IEnumerator UpdateBallPos()
	{
		float startTime = Time.time;
		float duration = 0.2f;
		float t = 0;
		Vector3 starTPos = transform.position;

		while (t<1)
		{
			t = (Time.time - startTime) / duration;
			transform.position = Vector3.Lerp(starTPos, similarBall.position, t);
			yield return new WaitForEndOfFrame();
		}
		
		DeActivateBall();
	}

	public void DeActivateBall()
	{
		gameObject.layer = LayerMask.NameToLayer(predictionLayer);
		transform.position = startPos;
		this.number = 0;
		color = Color.white;
		color.a = 0f;
		spriteRenderer.color = color;
		numberText.text = "";
	}

	int GetFinalScore(int currScore)
	{
		if (currScore <=4) currScore = 4;
		else if (currScore <= 8) currScore = 8;		
		else if (currScore <= 16) currScore = 16;		
		else if (currScore <= 32) currScore = 32;
		else if (currScore <= 64) currScore = 64;
		else if (currScore <= 128) currScore = 128;
		else if (currScore <= 256) currScore = 256;
		else if (currScore <= 512) currScore = 512;
		else if (currScore <= 1024) currScore = 1024;
		else if (currScore <= 2048) currScore = 2048;
		
		return currScore;
	}

}
