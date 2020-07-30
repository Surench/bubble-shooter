using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirdManager : MonoBehaviour
{
	[SerializeField] List<BallController> empyGrid;
	[SerializeField] List<BallController> Balls;
	
	public Transform hexPrefab;

	public int gridWidth = 6;
	public int gridHeight = 6;

	public float hexWidth = 1.732f;
	public float hexHeight = 2.0f;	
	public float gap = 0.0f;

	public Vector3 startPos = new Vector3(-2.19f, 5, 0);


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

	[SerializeField]List<BallController> TestingBalls; //3 
	void CreateGrid()
	{
		for (int y = 0; y < gridHeight; y++)//3
		{
			for (int x = 0; x < gridWidth; x++)//6
			{
				Transform hex = Instantiate(hexPrefab) as Transform;
				BallController ballCont = hex.GetComponent<BallController>();

				ballCont.BeRandomized();

				Balls.Add(ballCont);				

				Vector2 gridPos = new Vector2(x, y);
				//stex tanq if gridPos(x,y) == orinak 32
				hex.position = CalcWorldPos(gridPos);

				hex.parent = this.transform;
				hex.name = "Hexagon" + x + "|" + y;
			}
		}
	}

	//es i, j -i poxaren ari grenq x,y -ov positionnerov iranc ancni u vortex irar koxq linen nuyn numbeer unecox hex-er irarnc miacni iar ha?



	public void LevelPassed()
	{
		foreach (var item in empyGrid)
		{
			item.DeActivateBall();
		}

		foreach (var item in Balls)
		{
			item.BeRandomized();
		}
    }


}
