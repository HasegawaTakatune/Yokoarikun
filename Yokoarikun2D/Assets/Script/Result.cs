using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Const;

public class Result : MonoBehaviour {
	//******************************************************************//
	//	スコアの結果発表（カウントアップ）制御
	//
	//	呼び出し関係図
	//	Start　───>CountUpScore
	//******************************************************************//
	[SerializeField]
	GameObject[] customers;		// 生成するオブジェクト
	[SerializeField]
	Text ResultText;			// 結果表示テキスト

	[SerializeField]
	AudioClip DrumRoll;			// サウンド（ドラムロール）
	[SerializeField]
	AudioClip DrumEnd;			// サウンド（ドラムロールラスト）
	[SerializeField]
	AudioSource audioSource;	// オーディオソース格納

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	リザルトテキストの初期化・カウントアップ開始
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start () {
		ResultText.text = "あつめた人数:" + 0;			// テキストの初期化
		StartCoroutine(CountUpScore ());				// スコアのカウントアップ
	}

	//**************************************************************//
	//	関数名　:	CountUpScore
	//	機能		:	スコアのカウントアップ・ドラムロール制御
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator CountUpScore(){
		int countUp = 1;								// カウントアップ用
		bool DoOnce = false;							// 1度だけ実行
		while (true) {
			if (countUp <= Game.score) {				// 獲得スコアまでループする
				ResultText.text = "あつめた人数:" + countUp;					// スコアのカウントアップテキストを更新
				Instantiate (customers [GameStatus.GetCustomerCount ()],	// 獲得キャラクターの生成
					new Vector3 (Random.Range (-3.0f, 3.0f), Random.Range (-4.5f, 0.5f), countUp),
					Quaternion.identity);
				countUp++;								// スコアのカウントアップ
			}else if(!DoOnce){
				audioSource.Stop ();					// ドラムロールを停止
				audioSource.PlayOneShot (DrumEnd);		// ドラムロールラストを再生
				DoOnce = true;
			}else if (Input.anyKeyDown || Input.touchCount > 0)		// 集計が終了・入力をしたら
					SceneManager.LoadScene ("Rank");				// ランキング画面に遷移する
			yield return new WaitForSeconds (.01f);
		}
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}
