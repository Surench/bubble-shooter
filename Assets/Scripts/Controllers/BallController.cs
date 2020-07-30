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
	[SerializeField] private SpriteRenderer shadowRenderer;

	private readonly string ballLayer = "Ball";
	private readonly string predictionLayer = "Prediction";

	private Transform similarBall;
	private Vector3 startPos;

	private Color color;
	private Color shadowColor = Color.white;
	private float moveDuration;
	public bool lastball;
	public int number;
	

	public void Start()
	{
		startPos = transform.position;
	}



	public void BeRandomized()
	{
		gameObject.layer = LayerMask.NameToLayer(ballLayer);

		int random = Random.Range(0, 6); //was 8

		number = ScoreManager.self.GetNumber(random);
		color = ScoreManager.self.GetNewColor(number);

		numberText.text = number.ToString();
		shadowColor.a = 0.2f;
		spriteRenderer.color = color;
		shadowRenderer.color = shadowColor;		

		StartCoroutine(SmoothStart());
	}
	
	IEnumerator SmoothStart()
	{
		float randomDuration = Random.Range(0f, 0.6f);
		float t = 0;
		float graphV = 0;
		float startTime = Time.time;
		Vector3 finalScale = transform.localScale;
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
		shadowColor.a = 0.2f;
		spriteRenderer.color = newColor;
		shadowRenderer.color = shadowColor;
		numberText.text = number.ToString();

		if (!lastball) StartChainSearch();
		else ScoreManager.self.GameOver();
		
	}

	public void StartChainSearch()
	{
		RegisterBallToChain();

		FindSimilarBalls(number, false);
	}


	[SerializeField] List<BallController> neighborBalls = new List<BallController>();
	[SerializeField] List<BallController> filteredBalls = new List<BallController>();

	public void FindSimilarBalls(int searchNum,bool extraSearch)
	{
		RaycastHit2D[] hits = new RaycastHit2D[6];		
		ContactFilter2D filter = new ContactFilter2D() { };

		int numHits = Physics2D.CircleCast(transform.position,0.5f,Vector3.zero, filter,hits);
			   	
		for (int i = 0; i < numHits; i++)
		{
			BallController ball = hits[i].collider.GetComponent<BallController>();

			if (ball !=null) neighborBalls.Add(ball);			
		}

		foreach (BallController ball in neighborBalls)
		{
			if (ball.number.Equals(searchNum)) 
			{
				filteredBalls.Add(ball);
				if (extraSearch) BallsManager.self.FounderOfExtraBall(this);				
			} 			
		}

		foreach (var item in filteredBalls)
		{
			if(extraSearch) item.RegusterExtraBall();
			else item.RegisterBallToChain();
		}

		if (extraSearch) ExtraSearchDone();		
		else SearchDone();
		
		
	}
		

	void ExtraSearchDone()
	{
		
	}

	void SearchDone()
	{
		BallsManager.self.ContinueChainSearch();
		neighborBalls.Clear();
		filteredBalls.Clear();
	}

	public void RegisterBallToChain()
	{		
		BallsManager.self.AddToChain(this);		
	}

	private void RegusterExtraBall()
	{
		BallsManager.self.AddToExtraChain(this);
	}

	void CheckIsTheBallAlone()
	{
		filteredBalls.Clear();
		neighborBalls.Clear();

		//FindSimilarBalls();

		if (neighborBalls.Count > 0) 
		{
			for (int i = 0; i < neighborBalls.Count; i++)
			{
				print(neighborBalls[i].name);
			}
			
		}
		else
		{
			print("Alone");
		}


	}

	public void UpdateFinalScore(int multiplier)
	{		
		number *= multiplier;
		number = GetFinalScore(number);
		ScoreManager.self.AddScore(number);

		numberText.text = number.ToString();

		color = ScoreManager.self.GetNewColor(number);
		spriteRenderer.color = color;
	}

	public void MoveToSimilarBall(Transform ballPos,float moveDuration)
	{
		this.moveDuration = moveDuration;
		this.similarBall = ballPos;
		UpdateBallPosC = StartCoroutine(UpdateBallPos());
	}

	Coroutine UpdateBallPosC;
	IEnumerator UpdateBallPos()
	{
		numberText.text = "";

		float startTime = Time.time;		
		float t = 0;
		Vector3 starTPos = transform.position;
		
		while (t<1)
		{
			t = (Time.time - startTime) / moveDuration;
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
		color = Color.white; shadowColor = Color.white;
		color.a = 0f; shadowColor.a = 0;

		shadowRenderer.color = shadowColor;
		spriteRenderer.color = color;
		numberText.text = "";
	}

	public int GetFinalScore(int currScore)
	{
		if (currScore <= 2) currScore = 2;
		else if(currScore <=4) currScore = 4;
		else if (currScore <= 8) currScore = 8;		
		else if (currScore <= 16) currScore = 16;		
		else if (currScore <= 32) currScore = 32;
		else if (currScore <= 64) currScore = 64;
		else if (currScore <= 128) currScore = 128;
		else if (currScore <= 256) currScore = 256;
		else if (currScore <= 512) currScore = 512;
		else if (currScore <= 1024) currScore = 1024;
		else if (currScore <= 2048) currScore = 2048;
		else currScore = 2048;
		
		return currScore;
	}

}
