using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	[SerializeField] AnimationCurve curve;

	[SerializeField] private TextMeshProUGUI currLvelTex;
    [SerializeField] private TextMeshProUGUI lostText;

	[SerializeField] List<string> winingTexts;
		

    public void LevelUP ()
	{
		currLvelTex.text = winingTexts[Random.Range(0, winingTexts.Count)];
		StartCoroutine(UpdateTextSize(currLvelTex,45));		
	}

	public void ComboAnim()
	{
		currLvelTex.text = "2X";
		StartCoroutine(UpdateTextSize(currLvelTex,85));
	}

	public void LevelLost()
	{
		lostText.text ="-" + ScoreManager.totalScore.ToString();		
		StartCoroutine(UpdateTextSize(lostText,20));
	}
	


	IEnumerator UpdateTextSize(TextMeshProUGUI text, float maxSize)
	{		

		float cureveTime = 0;
		float curevAnim = curve.Evaluate(cureveTime);

		while (curevAnim <1)
		{
			cureveTime += Time.deltaTime * 1f;
			curevAnim = curve.Evaluate(cureveTime);

			text.fontSize = (maxSize * curevAnim);

			yield return null;
		}

		yield return new WaitForSeconds(0.2f);

		text.fontSize = 0;
	}
}
