using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Const;

public class GameCanvas : MonoBehaviour {
	//**********************************************************************//
	//	ゲーム中のUIを制御する
	//	ゲームスタート・ゲームセット・タイムリミット・スコア・連れている人数
	//
	//	呼び出し関係図
	//	Start	─┬─>CountDown	───>ClearStatusText
	//			 ├─>GameTimer	───>GameEnd
	//			 └─>Update
	//**********************************************************************//
	[SerializeField]
	Text TimeLimitText;		// タイムリミットテキスト
	[SerializeField]
	Text GameStatusText;	// ゲームステータステキスト
	[SerializeField]
	Text ScoreText;			// スコアテキスト
	int TimeLimit;			// タイムリミット

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	難易度ごとにタイムリミットを設定
	//				スタートカウントダウン開始
	//				ゲームタイマー開始
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start () {
		TimeLimit = (Game.difficulty == Game.Difficulty.Normal) ? 50 : 100;		// 難易度ごとの制限時間	（Normal : 50  Hard : 100）
		StartCoroutine (CountDown ());		// スタートカウントダウン開始
		StartCoroutine (GameTimer ());		// ゲームタイマー開始
	}

	//**************************************************************//
	//	関数名　:	Update
	//	機能		:	スコアの更新
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Update () {
		if (!Game.stop && Game.start)
			ScoreText.text = "アリーナ:" + Game.score.ToString () + "人";		// スコアの更新
	}

	//**************************************************************//
	//	関数名　:	GameTimer
	//	機能		:	残り時間の更新（コルーチン）
	//				1秒ごとに遅延して残り時間を更新する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
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

	//**************************************************************//
	//	関数名　:	CountDown
	//	機能		:	始まる前のカウントダウン（コルーチン）
	//				1秒ごとに遅延して残り時間を更新する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
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

	//**************************************************************//
	//	関数名　:	ClearStatusText
	//	機能		:	GameStatusTextをクリアする（コルーチン）
	//	引数		:	float interval	遅延させる時間（秒）を受け取る
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator ClearStatusText(float interval){
		yield return new WaitForSeconds (interval);		// 遅延
		GameStatusText.text = "";						// テキストクリア
	}

	//**************************************************************//
	//	関数名　:	GameEnd
	//	機能		:	ゲーム終了時、テキスト表示・ゲームを止める・リザルトに移行する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
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
