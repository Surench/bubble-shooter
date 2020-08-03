using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
	[SerializeField] GameEvent initScoreManager;
	[SerializeField] GameEvent initGridManager;
	[SerializeField] GameObject WinigConfetti;
	public static GameManager self;
	[SerializeField]
	private AudioSource Audio;

	private void Awake()
	{
		self = this;
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

		Audio.Play();
	}


	IEnumerator RaiseWinConffeti()
	{
		WinigConfetti.SetActive(true);
		yield return new WaitForSeconds(1f);
		WinigConfetti.SetActive(false);
	}


	
	IEnumerator UpdateSoundEffectRoutin(int amount)
	{
		float pitch = 1.4f;

		for (int i = 0; i < amount; i++)
		{
			pitch += 0.1f;

			Audio.pitch = pitch;
			Audio.Play();

			yield return new WaitForSeconds(0.1f);
		}

		Audio.pitch = 1.4f;
	}
	
}
