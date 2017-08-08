using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Const;

public class TimeCount: MonoBehaviour {

	public GUIText TimeLimitText;
	public GUIText EndText;
	public GUIText StartCountText;
	public GUIText ScoreText;
	public float TimeLimit = 50;
	public float startCount = 4;
	public string RankScene = "Rank";
	int CreateTime;

	// Use this for initialization
	void Start () {
		CreateTime = (int)TimeLimit;
	}
	
	// Update is called once per frame
	void Update () {
		if (Game.start) {
			GameTimer ();
			ScoreText.text = Game.score.ToString() + "人";
		} else {
			GameCountDown ();
		}
			
	}

	// 残り時間
	void GameTimer(){
		if (CreateTime < 0) {
			EndText.text = "しゅうりょう";
			Game.start = false;
			if (CreateTime <= (-3)) {
				Rank.FromTitle = false;
				SceneManager.LoadScene (RankScene);
			}
		} else {
			TimeLimitText.text = CreateTime.ToString ();

			if (CreateTime <= 48) {
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
		} else {
			StartCountText.text = "すたーと!!";
			CreateTime = (int)TimeLimit;
			Game.start = true;
		}
	}
}
