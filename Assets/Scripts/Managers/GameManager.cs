using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
	[SerializeField] GameEvent initScoreManager;
	[SerializeField] GameEvent initGridManager;
	
	private void Awake()
	{
		Application.targetFrameRate = 60;
	}

	private void Start()
	{
		initScoreManager.Raise();
		initGridManager.Raise();
	}

	public void VibratrLight()
    {
		TapTicks.Vibrate(25);
    }

	public void VibratrShoft()
	{
		TapTicks.Vibrate(500);
	}


}
