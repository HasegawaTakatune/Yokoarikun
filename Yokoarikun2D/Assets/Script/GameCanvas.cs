using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour {

	public Text TimeLimitText;
	public Text EndText;
	public Text StartCountText;
	public Text ScoreText;

	public float TimeToCleanUpText;
	public float TimeLimit;
	public float startCount=4;
	public const string RankScene = "Result";
	int CreateTime;

	// Use this for initialization
	void Start () {
		TimeLimit = (GameStatus.difficulty == GameStatus.Difficulty.Normal) ? 50 : 100;
		TimeToCleanUpText = (GameStatus.difficulty == GameStatus.Difficulty.Normal) ? 48 : 98;
		CreateTime = (int)TimeLimit;
	}

	// Update is called once per frame
	void Update () {
		if (!GameStatus.stop) {
			if (GameStatus.start) {
				GameTimer ();
				ScoreText.text = "アリーナ:" + ArrayCharracter.Score.ToString () + "人";
			} else {
				GameCountDown ();
			}
		}
	}

	// 残り時間
	void GameTimer(){
		if (CreateTime < 0) {
			EndText.text = "しゅうりょう";
			GameStatus.start = false;
			if (CreateTime <= (-3)) {
				Rank.FromTitle = false;
				SceneManager.LoadScene (RankScene);
			}
		} else {
			TimeLimitText.text = "のこり :" + CreateTime.ToString ();

			if (CreateTime <= TimeToCleanUpText) {
				StartCountText.text = "";
			}
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
			GameStatus.start = false;
		} else {
			StartCountText.text = "すたーと!!";
			CreateTime = (int)TimeLimit;
			GameStatus.start = true;
		}
	}
}
