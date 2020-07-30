using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BallsManager : MonoBehaviour
{
	
	[SerializeField] List<BallController> similarBalls;
	[SerializeField] List<BallController> extraBalls;
	[SerializeField] List<BallController> extraBallFounders;

	public static BallsManager self;

	private int searchCount;
	private int chainStartNum;

	private void Awake()
	{
		self = this;
	}

	public void AddToChain(BallController ballController)
	{
		if (!similarBalls.Contains(ballController)) similarBalls.Add(ballController);				
	}
	public void AddToExtraChain(BallController ballController)
	{
		if ((!similarBalls.Contains(ballController)) && (!extraBalls.Contains(ballController))) extraBalls.Add(ballController);
	}
	public void FounderOfExtraBall(BallController extraBallFounder)
	{
		if (!extraBallFounders.Contains(extraBallFounder)) extraBallFounders.Add(extraBallFounder);		
	}


	public void ContinueChainSearch()
	{
		searchCount++;
	
		if (searchCount < similarBalls.Count)
		{			
			SearchMoreSimilarCans();
		}
		else
		{
			UpdateChainBasicMove();
		}
	}

	void ChainSearchDone()
	{
		
		
	}

	BallController extraball;
	void FindExtraBall()
	{
		if (similarBalls.Count >1)
		{
			chainStartNum = similarBalls[0].GetFinalScore(similarBalls.Count * similarBalls[0].number);


			foreach (var item in similarBalls)
			{
				item.FindSimilarBalls(chainStartNum, true);
			}

			if (extraBalls.Count >0) UpdateChainExtraMove();
			else UpdateChainBasicMove();			
		}	
	}

	IEnumerator ChainMovment()
	{
		float delay = 0.45f / searchCount;
		for (int i = similarBalls.Count - 1; i >= 1; i--)
		{
			similarBalls[i].MoveToSimilarBall(similarBalls[i - 1].transform, delay);
			yield return new WaitForSeconds(delay);
		}

		similarBalls[0].UpdateFinalScore(searchCount);
		ResetChain();
	}

	IEnumerator ChainExtraMovment()
	{
		float delay = 0.45f / searchCount;

		for (int i = 0; i < similarBalls.Count; i++)
		{
			if (similarBalls[i] != extraBallFounders[extraBallFounders.Count - 1])
			{
				similarBalls[i].MoveToSimilarBall(extraBallFounders[extraBallFounders.Count - 1].transform, delay);
			}
			yield return new WaitForSeconds(delay);
		}

		similarBalls[extraBallFounders.Count - 1].UpdateFinalScore(searchCount);
		yield return new WaitForSeconds(0.2f);
		extraBallFounders[extraBallFounders.Count - 1].MoveToSimilarBall(extraBalls[extraBalls.Count - 1].transform, delay);

		similarBalls[0].UpdateFinalScore(searchCount *2);
		ResetChain();
		
	}


	void UpdateChainBasicMove()
	{
		StartCoroutine(ChainMovment());		
	}
	void UpdateChainExtraMove()
	{
		StartCoroutine(ChainExtraMovment());
	}

	void SearchMoreSimilarCans()
	{
		similarBalls[searchCount].FindSimilarBalls(similarBalls[searchCount].number, false);
	}	

	void ResetChain()
	{
		searchCount = 0;
		similarBalls.Clear();
		extraBalls.Clear();
		extraBallFounders.Clear();
	}
}
