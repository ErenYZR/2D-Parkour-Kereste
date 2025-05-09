using UnityEngine;
using UnityEngine.UI;
using static FinishFlag;
using TMPro;

public class ScoreboardUI : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI scoreText; // Skorlarý gösterecek UI metni

	void Start()
	{
		ShowHighScores();
	}

	public void ShowHighScores()
	{
		HighScoreData highScores = HighScoreManager.LoadHighScores();

		scoreText.text = "";
		for (int i = 0; i < highScores.scores.Count; i++)
		{
			scoreText.text += (i + 1) + ". " + highScores.scores[i].ToString("F2") + "s\n";
		}
	}
}
