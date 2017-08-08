using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Const;

public class GameCanvas : MonoBehaviour {

	public Text TimeLimitText;
	public Text EndText;
	public Text StartCountText;
	public Text ScoreText;

	public float TimeToCleanUpText;
	public float TimeLimit;
	public float startCount = 4;
	int CreateTime;

	// Use this for initialization
	void Start () {
		TimeLimit = (Game.difficulty == Game.Difficulty.Normal) ? 50 : 100;
		TimeToCleanUpText = (Game.difficulty == Game.Difficulty.Normal) ? 48 : 98;
		CreateTime = (int)TimeLimit;
	}

	// Update is called once per frame
	void Update () {
		if (!Game.stop) {
			if (Game.start) {
				GameTimer ();
				ScoreText.text = "アリーナ:" + Game.score.ToString () + "人";
			} else {
				GameCountDown ();
			}
		}
	}

	// 残り時間
	void GameTimer(){
		if (CreateTime < 0) {
			EndText.text = "しゅうりょう";
			Game.stop = true;
			StartCoroutine (GameEnd ());
		} else {
			TimeLimitText.text = "のこり :" + CreateTime.ToString ();
			if (CreateTime <= TimeToCleanUpText)
				StartCountText.text = "";
		}

		TimeLimit -= Time.deltaTime;
		CreateTime = Mathf.FloorToInt (TimeLimit);

	}

	// 始まる前のカウントダウン
	void GameCountDown(){
		startCount -= Time.deltaTime;
		CreateTime = Mathf.FloorToInt (startCount);
		if (CreateTime >= 1) {
			StartCountText.text = CreateTime.ToString ();
			Game.start = false;
		} else {
			StartCountText.text = "すたーと!!";
			CreateTime = (int)TimeLimit;
			Game.start = true;
		}
	}

	IEnumerator GameEnd(){
		yield return new WaitForSeconds (3);
		Rank.FromTitle = false;
		SceneManager.LoadScene ("Result");
	}
}
