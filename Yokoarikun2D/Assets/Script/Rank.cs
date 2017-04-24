using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Rank : MonoBehaviour {
	public struct MYDATA{
		public int difficulty;
		public 	int yourRank;
		public int tmpScore;
		public bool changed;
	}
	MYDATA myData;
	public Text[] Ranking;
	public Text PlayerRank;
	public static string RankName;
	public static bool FromTitle = false;
	int[] ScoreArray = { 0, 0, 0, 0 };
	void Init(){
		myData.difficulty = (int)OnClickButtonSceneManager.difficulty;
		myData.changed = true;
	}

	// Use this for initialization
	void Start () {
		Init ();
		// ランキングが登録されていなければ
		if (PlayerPrefsX.GetIntArray ("Normal").Length <= 0 || PlayerPrefsX.GetIntArray ("Hard").Length <= 0) {
			// ランキングの登録を行う
			PlayerPrefsX.SetIntArray ("Normal", ScoreArray);
			PlayerPrefsX.SetIntArray ("Hard", ScoreArray);
		}
	
		// Save Rank
		RankName = (myData.difficulty == 1) ? "Normal" : "Hard";
		PlayerRank.gameObject.SetActive (false);
		// Score Ranking
		ScoreArray = PlayerPrefsX.GetIntArray (RankName);
		ScoreArray [3] = ArrayCharracter.Score;
		myData.tmpScore = ScoreArray [3];
		Sort ();

		// ランキング表示
		for (int i = 0; i < ScoreArray.Length - 1; i++) {
			Ranking [i].text = (i + 1).ToString () + "  " + ScoreArray [i].ToString () + "人";
			// 自分のランキング位置を知らせる
			if (!FromTitle) {
				if (ScoreArray [i] == myData.tmpScore && myData.changed) {
					if (i >= 3)
						break;
					
					PlayerRank.transform.position = (Ranking [i].transform.position + Vector3.right * 200);
					Ranking [3].gameObject.SetActive (false);
					PlayerRank.gameObject.SetActive (true);
					myData.changed = false;
				} 
				Ranking [3].text = "あなた " + (ArrayCharracter.Score).ToString () + "人";
			} else {
				Ranking [3].text = "";
			}
		}
	}

	// ソート
	void Sort(){
		for (int Comparison = 0; Comparison < 3; Comparison++) {
			for (int Source = Comparison + 1; Source < 4; Source++) {
				if (ScoreArray [Comparison] < ScoreArray [Source]) {
					int tmp;
					tmp = ScoreArray [Comparison];
					ScoreArray [Comparison] = ScoreArray [Source];
					ScoreArray [Source] = tmp;
				}

			}
		}
		PlayerPrefsX.SetIntArray (RankName, ScoreArray);
	}
}