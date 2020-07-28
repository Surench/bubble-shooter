using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetBallColor : ScoreManager
{
	public GetBallColor(int index) : base(index)
	{

	}
}

public class ScoreManager : MonoBehaviour
{

	[SerializeField] GameEvent levelPassed;

	[SerializeField] private TextMeshProUGUI currLvelTex;
	[SerializeField] private TextMeshProUGUI nextLvelTex;
	[SerializeField] private Slider lvelSlider;
	[SerializeField] private List<Color> newColors;
	[SerializeField] private List<Color> colorss;
	[SerializeField] private List<int> numbers;

	LevelSettings levelSettings = new LevelSettings();
	


	public static ScoreManager self;
	private int currentGameLevel;
	protected int index;

	public int totalScore;
	public int scoreForNextLvl;


	public ScoreManager()
	{

	}

	public ScoreManager(int index)
	{
		this.index = index;
	}

	

	public void InitScoreManger()
	{
		self = this;
		currentGameLevel = DataManager.GetLevelSettings().currentLevel;
		CalculateScoreForNextLvl();
		ResetLevelSlider();
	}

	public void LevelPassed()
	{
		currentGameLevel++;
		totalScore = 0;
		CalculateScoreForNextLvl();
		ResetLevelSlider();
		levelSettings.currentLevel = currentGameLevel;
		DataManager.SetLevelSettings(levelSettings);
	}

	private void ResetLevelSlider()
	{
		lvelSlider.value = 0;
		currLvelTex.text = (currentGameLevel+1).ToString();
		nextLvelTex.text = (currentGameLevel +2).ToString();
	}

	
	


	public void AddScore(int newScore)
	{
		totalScore += newScore;
		if (totalScore >= scoreForNextLvl) levelPassed.Raise();		
		else UpdateSliderValue();
				
	}

	void UpdateSliderValue()
	{
		StartCoroutine(UpdateSliderValu());
	}

	
	IEnumerator UpdateSliderValu()
	{
		float time = Time.time;
		float duration = 0.2f;
		float t = 0;
		float startV = lvelSlider.value;
		float endV = (float)totalScore / (float)scoreForNextLvl;
		while (t<1)
		{
			t = (Time.time - time) / duration;
			float v = Mathf.Lerp(startV, endV, t);
			lvelSlider.value = v;
			yield return new WaitForEndOfFrame();
		}
	}

	void CalculateScoreForNextLvl()
	{
		if (currentGameLevel < 3) scoreForNextLvl = 500;
		else if (currentGameLevel < 6) scoreForNextLvl = 800;
		else scoreForNextLvl = 1000;
	}
		
	
	public int GetNumber(int index)
	{
		return numbers[index];
	}
	public Color GetNewColor(int index)
	{
		switch (index)
		{
			case 2:
				index = 0;
				break;
			case 4:
				index = 1;
				break;
			case 8:
				index = 2;
				break;
			case 16:
				index = 3;
				break;
			case 32:
				index = 4;
				break;
			case 64:
				index = 5;
				break;
			case 128:
				index = 6;
				break;
			case 256:
				index = 7;
				break;
			default:
				index = 8;
				break;
		}
		return newColors[index];
	}

}
