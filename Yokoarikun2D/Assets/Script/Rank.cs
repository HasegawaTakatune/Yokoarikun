using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Rank : MonoBehaviour {

	public static string RankName;
	int difficulty = (int)GameStatus.difficulty;

	public Text[] Ranking = new Text[3];
	public Text PlayerRank;

	int[] ScoreArray = { 0, 0, 0, 0 };

	int yourRank;
	int tmpScore;
	bool changed = true;
	public static bool FromTitle = false;

	// Use this for initialization
	void Start () {
		// ランキングが登録されていなければ
		if (PlayerPrefsX.GetIntArray ("Normal").Length <= 0 || PlayerPrefsX.GetIntArray ("Hard").Length <= 0) {
			// ランキングの登録を行う
			PlayerPrefsX.SetIntArray ("Normal", ScoreArray);
			PlayerPrefsX.SetIntArray ("Hard", ScoreArray);
		}

		PlayerRank.gameObject.SetActive (false);

		// Save Rank
		RankName = (difficulty == 1) ? "Normal" : "Hard";

		// Score Ranking
		ScoreArray = PlayerPrefsX.GetIntArray (RankName);
		ScoreArray [3] = tmpScore = ArrayCharracter.Score;
		// ソート
		Sort ();
		// ランキング表示
		ShowRank ();
	}

	// ソート
	void Sort(){
		for (int i = 0; i < 3; i++) {
			for (int j = i + 1; j < 4; j++) {
				if (ScoreArray [i] < ScoreArray [j]) {
					int tmp;
					tmp = ScoreArray [i];
					ScoreArray [i] = ScoreArray [j];
					ScoreArray [j] = tmp;
				}
			}
		}
		PlayerPrefsX.SetIntArray (RankName, ScoreArray);
	}

	void ShowRank(){
		// ランキング表示
		for (int i = 0; i < ScoreArray.Length - 1; i++) {
			Ranking [i].text = (i + 1).ToString () + "  " + ScoreArray [i].ToString () + "人";
			// 自分のランキング位置を知らせる
			if (!FromTitle) {
				if (ScoreArray [i] == tmpScore && changed) {
					if (i >= 3)
						break;

					PlayerRank.transform.position = (Ranking [i].transform.position + Vector3.right * 200);
					Ranking [3].gameObject.SetActive (false);
					PlayerRank.gameObject.SetActive (true);
					changed = false;
				} 
				Ranking [3].text = "あなた " + (ArrayCharracter.Score).ToString () + "人";
			} else {
				Ranking [3].text = "";
			}
		}
	}
}