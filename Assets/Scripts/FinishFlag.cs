using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class FinishFlag : MonoBehaviour
{
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] UI ui;
    public float highScore;
	public ScoreboardUI scoreboardui;
	[SerializeField] TextMeshProUGUI highScoreText;


	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			highScore = ui.timer;
			HighScoreManager.SaveTime(highScore);
			pauseMenu.EndGame();
			Time.timeScale = 0;
			scoreboardui.ShowHighScores();
			highScoreText.text = "Your Score: " + highScore.ToString("F2");
			print(highScoreText.text);
		}
	}

	[Serializable] public class HighScoreData
	{
		public List<float> scores = new List<float>();//skor listesi
	}
}
