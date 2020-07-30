using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
	[SerializeField] GameEvent initScoreManager;
	[SerializeField] GameEvent initGridManager;
	[SerializeField] GameObject WinigConfetti;
	[SerializeField]
	private AudioSource PerfectAudio;

	private void Awake()
	{
		//Application.targetFrameRate = 60;
	}

	private void Start()
	{
		initScoreManager.Raise();
		initGridManager.Raise();
	}
		

	public void ActivateWinConfett()
	{
		StartCoroutine(RaiseWinConffeti());
	}

	public void VibratrLight()
    {
		TapTicks.Vibrate(15);
    }

	public void VibratrShoft()
	{
		TapTicks.Vibrate(500);
	}

	public void PlayShootSound()
	{

		PerfectAudio.Play();
	}


	IEnumerator RaiseWinConffeti()
	{
		WinigConfetti.SetActive(true);
		yield return new WaitForSeconds(1f);
		WinigConfetti.SetActive(false);
	}
}
