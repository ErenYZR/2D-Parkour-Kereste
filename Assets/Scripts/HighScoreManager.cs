using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static FinishFlag;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
	[SerializeField] public TextMeshProUGUI firstScore;

	private static string savePath => Application.persistentDataPath + "/highscores.json";

	public static void SaveTime(float newTime)
	{
		HighScoreData highScores = LoadHighScores();

		// Yeni süreyi ekle
		highScores.scores.Add(newTime);

		// Süreleri küçükten büyüðe sýrala
		highScores.scores.Sort();

		// En fazla 10 kayýt tut
		if (highScores.scores.Count > 10)
		{
			highScores.scores.RemoveAt(10);
		}

		for (int i = 0; i< highScores.scores.Count; i++)
		{
			print("A"+highScores.scores[i]);
			print(savePath);
		}


		// JSON olarak kaydet
		string json = JsonUtility.ToJson(highScores, true);
		File.WriteAllText(savePath, json);
	}

	public static HighScoreData LoadHighScores()
	{
		if (File.Exists(savePath))
		{
			string json = File.ReadAllText(savePath);
			return JsonUtility.FromJson<HighScoreData>(json);
		}
		else
		{
			return new HighScoreData(); // Dosya yoksa boþ liste döndür
		}
	}
}
