using TMPro;
using UnityEngine;

public class AmoController : MonoBehaviour
{	

	private SpriteRenderer sprites;
	[SerializeField] private TextMeshPro numberText;

	private int maxColorRange = 6;

	public Color color;
	public int number;

	private void Start()
	{
		sprites = GetComponent<SpriteRenderer>();
		InitAmo();
	}

	public void InitAmo()
	{
		int index = Random.Range(0, maxColorRange);

		number = ScoreManager.self.GetNumber(index);
		color = ScoreManager.self.GetNewColor(number);
		

		numberText.text = number.ToString();
		sprites.color = color;
	}

	
}
