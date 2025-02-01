using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    public float timer { get; private set; }

    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] FinishFlag finishFlag;


    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timerText.text = timer.ToString("#0.00");
        highScoreText.text = "Your Score: " +finishFlag.highScore.ToString();
    }
}
