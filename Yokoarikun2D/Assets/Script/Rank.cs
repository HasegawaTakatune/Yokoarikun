using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Const;

public class Rank : MonoBehaviour {
	//******************************************************************//
	//	ランキングの表示・ソートを行う
	//
	//	呼び出し関係図
	//	Start	─┬─>Sort
	//			 └─>ShowRank
	//******************************************************************//
	string Key;						// 保存先の名前（ランキング）
	[SerializeField]
	Text[] Ranking = new Text[3];	// ランキング表示
	[SerializeField]
	Text PlayerRank;				// プレイヤーのランク表示用

	int[] ScoreArray = { 0, 0, 0 };	// 過去のスコアを格納
	int myScore;					// プレイヤーのスコアを格納

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	過去ランキングを取り出す、スコアのソート・表示をする
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start () {
		// ランキングが登録されていなければ
		if (PlayerPrefsX.GetIntArray ("Normal").Length <= 0 || PlayerPrefsX.GetIntArray ("Hard").Length <= 0) {
			PlayerPrefsX.SetIntArray ("Normal", ScoreArray);	// ノーマルモードのスコアを初期化
			PlayerPrefsX.SetIntArray ("Hard", ScoreArray);		// ハードモードのスコアを初期化
		}
		if(Game.score == 0)Ranking[3].gameObject.SetActive(false);				// スコアが0の時ランク外表示を非表示
		Key = (Game.difficulty == Game.Difficulty.Normal) ? "Normal" : "Hard";	// キー設定
		ScoreArray = PlayerPrefsX.GetIntArray (Key);	// 過去スコアを取得
		myScore = Game.score;	// プレイヤーのスコアを取得
		Sort ();				// ソート
		ShowRank ();			// ランキング表示
	}

	//**************************************************************//
	//	関数名　:	Sort
	//	機能		:	整列された配列を前提としたソート、
	//				ランクインした位置から入れ替えを始める
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Sort(){
		for (int max = 0; max < ScoreArray.Length; max++) {			// 上位スコアから探索する
			if (ScoreArray [max] <= myScore) {						// ランクインしていたら
				for (int under = ScoreArray.Length - 1; under > max; under--) {	// ランクインしている箇所から入れ替えをする
					ScoreArray [under] = ScoreArray [under - 1];	// 入れ替え（ランク繰り下げ）
				}
				ScoreArray [max] = myScore;							// プレイヤースコアを格納
				break;
			}
		}
		PlayerPrefsX.SetIntArray (Key, ScoreArray);					// ランキングの保存
	}

	//**************************************************************//
	//	関数名　:	ShowRank
	//	機能		:	ランキングの表示をする
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void ShowRank(){
		bool doOnce = true;									// 1度だけ実行
		PlayerRank.gameObject.SetActive (false);			// プレイヤーランクを非表示
		for (int index = 0; index < ScoreArray.Length; index++) {
			Ranking [index].text = (index + 1).ToString () + "  " + ScoreArray [index].ToString () + "人";	// ランク・スコア表示
			if (ScoreArray [index] == myScore && doOnce) {	// ランクインしてる場合
				PlayerRank.transform.position = (Ranking [index].transform.position + Vector3.right * 200);	// テキスト位置を設定
				PlayerRank.gameObject.SetActive (true);		// ランクインテキストを表示
				Ranking [3].gameObject.SetActive (false);	// ランク外テキストを非表示
				doOnce = false;								// 1度だけ実行
			}
		}
		Ranking [3].text = "あなた " + (Game.score).ToString () + "人";	// ランク外の表示
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}