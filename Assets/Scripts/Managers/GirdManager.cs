using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirdManager : MonoBehaviour
{
	

	[SerializeField] private GameEvent ComobEvent;
	[SerializeField] private GameEvent gameOverEvent;
	[SerializeField] public List<BallController> Balls;
	[SerializeField] private List<BallController> SimilarNeighburs;
	[SerializeField] private List<BallController> PredictionFounders;
	[SerializeField] private List<BallController> PredictedBalls;

	public Transform hexPrefab;
	public static GirdManager self;

	public int gridWidth;
	public int gridHeight;
	public int gridActiveRaw;

	public float hexWidth = 1.732f;
	public float hexHeight = 2.0f;
	public float gap = 0.0f;
	private int totalBall;

	public int index;
	public int newScore;

	public Vector3 startPos = new Vector3(-2.19f, 5, 0);


	private void Awake()
	{
		self = this;
	}

	public void InitGridManager()
	{
		AddGap();
		CreateGrid();
	}

	void AddGap()
	{
		hexWidth += hexWidth * gap;
		hexHeight += hexHeight * gap;
	}

	void CalcStartPos()
	{
		float offset = 0;
		if (gridHeight / 2 % 2 != 0)
			offset = hexWidth / 2;

		float x = -hexWidth * (gridWidth / 2) - offset;
		float y = hexHeight * 0.75f * (gridHeight / 2);

		startPos = new Vector3(x, y, 0);
	}

	Vector3 CalcWorldPos(Vector2 gridPos)
	{
		float offset = 0;
		if (gridPos.y % 2 != 0)
			offset = hexWidth / 2;

		float x = startPos.x + gridPos.x * hexWidth + offset;
		float y = startPos.y - gridPos.y * hexHeight * 0.75f;

		return new Vector3(x, y, 0);
	}

	void CreateGrid()
	{
		for (int y = 0; y < gridHeight; y++)//9
		{
			for (int x = 0; x < gridWidth; x++)//6
			{
				Transform hex = Instantiate(hexPrefab) as Transform;
				BallController ballCont = hex.GetComponent<BallController>();

				Vector2 gridPos = new Vector2(x, y);
				Vector3 newPos = CalcWorldPos(gridPos);

				hex.position = newPos;
				ballCont.startPos = newPos;
				ballCont.index = totalBall;
				hex.parent = this.transform;
				hex.name = "Hexagon" + x + "|" + y;

				if (y >= gridActiveRaw) ballCont.DeActivateBall();
				else ballCont.InitBall();

				Balls.Add(ballCont);
				totalBall++;
			}
		}
	}



	public void FirstHit(int newindex, int score)
	{
		this.index = newindex;
		this.newScore = score;

		if (newindex >= 48)
		{
			GameOver();
			return;
		}

		List<int> indexes = GetNeighboursIndexes(index);

		CleanUpGridLists();

		UpdateGridChainReaction(index, indexes);		
	}

	void UpdateGridChainReaction(int index, List<int> neighbourID)
	{

		//****** arajin xpvac gtnuma koxkinerin

		BallController firstBall = Balls[index].GetComponent<BallController>();
		SimilarNeighburs.Add(firstBall);

		for (int i = 0; i < neighbourID.Count; i++)
		{
			BallController similarBall = Balls[index + neighbourID[i]].GetComponent<BallController>();
			if (newScore.Equals(similarBall.number))
				SimilarNeighburs.Add(similarBall);
		}

		//****** myusnern gtnumen eli iranc nmanerin

		for (int i = 0; i < SimilarNeighburs.Count; i++)
		{
			int ind = SimilarNeighburs[i].index;
			List<int> newNeighbours = GetNeighboursIndexes(ind);

			for (int j = 0; j < newNeighbours.Count; j++)
			{
				BallController predictedBall = Balls[ind + newNeighbours[j]].GetComponent<BallController>();

				if ((!SimilarNeighburs.Contains(predictedBall)) && (newScore.Equals(predictedBall.number)))
				{
					SimilarNeighburs.Add(predictedBall);
				}

			}
		}

		//****** Predection

		newScore = GetFinalScore(newScore * SimilarNeighburs.Count);


		for (int i = 0; i < SimilarNeighburs.Count; i++)
		{
			int ind = SimilarNeighburs[i].index;
			List<int> neighboursIDS = GetNeighboursIndexes(ind);

			for (int j = 0; j < neighboursIDS.Count; j++)
			{
				BallController similarBall = Balls[ind + neighboursIDS[j]].GetComponent<BallController>();

				if ((!PredictedBalls.Contains(similarBall)) && (newScore.Equals(similarBall.number)))
				{
					PredictionFounders.Add(SimilarNeighburs[i]);
					PredictedBalls.Add(similarBall);
				}

			}
		}

		ActivateChain();

	}

	void ActivateChain()
	{
		StartCoroutine(UpdateChainMerge());
	}

	public float waitDelay;
	public float movmentDelay;

	IEnumerator UpdateChainMerge()
	{
		if (PredictionFounders.Count > 0)
		{
			int i;

			for (i = 0; i < SimilarNeighburs.Count-1; i++)
			{
				if (SimilarNeighburs[i] == PredictionFounders[0]) break;

				SimilarNeighburs[i].MoveToSimilarBall(SimilarNeighburs[i + 1].transform, movmentDelay);				
			}

			for (int j = SimilarNeighburs.Count - 1; j >= i+1; j--)
			{
				SimilarNeighburs[j].MoveToSimilarBall(SimilarNeighburs[j - 1].transform, movmentDelay);				
			}


			yield return new WaitForSeconds(0.05f);

			PredictionFounders[0].UpdateFinalScore(newScore);


			yield return new WaitForSeconds(waitDelay);

			PredictionFounders[0].MoveToSimilarBall(PredictedBalls[0].transform, movmentDelay/2);


			yield return new WaitForSeconds(0.05f);

			PredictedBalls[0].UpdateFinalScore(newScore * 2);
			PredictedBalls[0].ActivateAnimation();
			ScoreManager.self.AddScore(newScore * 2);


			yield return new WaitForSeconds(0.15f);
			FirstHit(PredictedBalls[0].index, PredictedBalls[0].number);
			ComobEvent.Raise();


		}
		else
		{

			for (int i = 0; i < SimilarNeighburs.Count - 1; i++)
			{
				SimilarNeighburs[i].MoveToSimilarBall(SimilarNeighburs[i + 1].transform, movmentDelay);			
			}

			if (SimilarNeighburs.Count >1)
			{
				SimilarNeighburs[SimilarNeighburs.Count - 1].UpdateFinalScore(newScore);
				SimilarNeighburs[SimilarNeighburs.Count - 1].ActivateAnimation();
				ScoreManager.self.AddScore(newScore);
			}

		}
	}


	private List<int> indexesStartsfromLeft = new List<int> {13,14,15,16,25,26,27,28,37,38,39,40};
	List<int> GetNeighboursIndexes(int currIndex)
	{
		List<int> newIndexes = new List<int>();

		int remainder = currIndex / 6;
		remainder %= 2;

		if (currIndex < 6)
		{
			if (currIndex.Equals(0)) newIndexes = new List<int>() { 1, 6 };
			else if (currIndex.Equals(5)) newIndexes = new List<int>() { -1, 5, 6 };
			else newIndexes = new List<int>() { -1, 1, 5, 6 };
		}
		else if (currIndex % 6 == 0)
		{
			if (remainder.Equals(1)) newIndexes = new List<int>() { -6, -5, 1, 6, 7 };
			else newIndexes = new List<int>() { -6, 1, 6 };
		}
		else if ((currIndex + 1) % 6 == 0)
		{
			if (remainder.Equals(1)) newIndexes = new List<int>() { -6, -1, 6 };
			else newIndexes = new List<int>() { -7, -6, -1, 5, 6 };
		}		
		else
		{
			if (indexesStartsfromLeft.Contains(currIndex))
				newIndexes = new List<int>() { -7, -6, -1, 1, 5, 6 };
			else
				newIndexes = new List<int>() { -6, -5, -1, 1, 6, 7 };			
		}

		return newIndexes;
	}


	void CleanUpGridLists()
	{
		SimilarNeighburs.Clear();
		PredictionFounders.Clear();
		PredictedBalls.Clear();
	}

	   
	public void LevelPassed()
	{
		StopAllCoroutines();
		ResetGrid();
	}

	public void GameOver()
	{
		ResetGrid();
		gameOverEvent.Raise();
	}

	void ResetGrid()
	{
		for (int i = 0; i < Balls.Count; i++)
		{
			if (i < gridWidth * gridActiveRaw) Balls[i].InitBall();
			else Balls[i].DeActivateBall();
		}
	}

	public int GetFinalScore(int currScore)
	{
		if (currScore <= 2) currScore = 2;
		else if (currScore <= 4) currScore = 4;
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
