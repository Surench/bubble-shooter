using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	private Animator anim; 
    [SerializeField] private TextMeshProUGUI currLvelTex;
    [SerializeField] private TextMeshProUGUI lostText;
	[SerializeField] List<string> texts;

	void Start()
    {
		anim = GetComponent<Animator>();
    }

    public void LevelUP ()
	{
		currLvelTex.text = texts[Random.Range(0, texts.Count)];
		anim.SetTrigger("LevelUP");	}

	public void LevelLost()
	{
		lostText.text ="-" + ScoreManager.totalScore.ToString();
		anim.SetTrigger("LevelLost");
	}

}
