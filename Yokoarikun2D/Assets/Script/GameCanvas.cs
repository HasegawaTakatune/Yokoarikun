using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Const;

public class GameCanvas : MonoBehaviour {
	//**********************************************************************//
	//					ゲーム中のUIを制御する								//
	//ゲームスタート・ゲームセット・タイムリミット・スコア・連れている人数	//
	//**********************************************************************//
	[SerializeField]
	Text TimeLimitText;		// タイムリミットテキスト
	[SerializeField]
	Text GameStatusText;	// ゲームステータステキスト
	[SerializeField]
	Text ScoreText;			// スコアテキスト
	int TimeLimit;			// タイムリミット

	void Start () {
		TimeLimit = (Game.difficulty == Game.Difficulty.Normal) ? 30 : 300;		// 難易度ごとの制限時間	（Normal : 50  Hard : 100）
		StartCoroutine (CountDown ());		// スタートカウントダウン開始
		StartCoroutine (GameTimer ());		// ゲームタイマー開始
	}

	void Update () {
		if (!Game.stop && Game.start)
			ScoreText.text = "アリーナ:" + Game.score.ToString () + "人";		// スコアの更新
	}
	// 残り時間の更新（コルーチン）
	// 1秒ごとに更新して残り時間を更新する
	IEnumerator GameTimer(){
		while (true) {
			yield return new WaitForSeconds (1);
			if (!Game.stop && Game.start) {
				TimeLimit--;													// カウントダウン
				if (TimeLimit >= 0)												// ゲーム中
					TimeLimitText.text = "のこり :" + TimeLimit.ToString ();		// 残り時間更新
				else
					StartCoroutine (GameEnd ());								// ゲーム終了関数呼び出し
			}
		}
	}
	// 始まる前のカウントダウン（コルーチン）
	// 1秒ごとに更新して残り時間を更新する
	IEnumerator CountDown(){
		int count = 3;			// カウントダウン時間
		while (true) {
			yield return new WaitForSeconds (1);
			count--;			// カウントダウン
			if (count <= 0) {	// スタート
				GameStatusText.text = "すたーと!!";		// スタートテキスト
				Game.start = true;						// ゲームをスタートさせる
				StartCoroutine (ClearStatusText (1.5f));// すたーと!!テキストをクリアする
				break;
			}
			GameStatusText.text = count.ToString ();	// カウントダウンテキスト更新
		}
	}
	// GameStatusTextをクリアする（コルーチン）
	// interval（秒）経過後ににクリアする
	IEnumerator ClearStatusText(float interval){
		yield return new WaitForSeconds (interval);		// 遅延
		GameStatusText.text = "";						// テキストクリア
	}
	// ゲーム終了の処理群
	IEnumerator GameEnd(){
		GameStatusText.text = "しゅうりょう";	// 終了テキスト
		Game.stop = true;						// ゲームを止める
		yield return new WaitForSeconds (3);	// 3秒遅延
		SceneManager.LoadScene ("Result");		// リザルト画面に移行
	}

	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}
