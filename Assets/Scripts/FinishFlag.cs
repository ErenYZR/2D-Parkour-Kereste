using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishFlag : MonoBehaviour
{
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] UI ui;
    public float highScore;


	private void OnTriggerEnter2D(Collider2D collision)
	{
        highScore = ui.timer;
		pauseMenu.EndGame();
		Time.timeScale = 0;
		print(highScore);
	}
}
