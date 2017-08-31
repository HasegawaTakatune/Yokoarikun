using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Const;

public class OnClickButtonSceneManager : OnClickButton {
	//******************************************************************//
	//	OnClickButton継承クラス
	//	メインゲーム・ランキングの難易度を選択して、シーン移動する。
	//	ボタンクリック時の処理は、基底クラスのOnClickButtonを使っている。
	//
	//	呼び出し関係図
	//	ボタンクリック	───>OnClickSceneManager	───>OnClick
	//******************************************************************//

	[SerializeField] Game.Difficulty setDifficulty;	//	難易度を保持

	//**************************************************************//
	//	関数名　:	OnClickSceneManager
	//	機能		:	ボタンを押されたらシーン遷移をする
	//				ゲームステータスの初期化・基底クラスのOnClickを呼ぶ
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void OnClickSceneManager(){
		Game.stop = false;						// ストップ初期化
		Game.start = false;						// スタート初期化
		Game.difficulty = setDifficulty;		// 難易度を設定
		Game.score = 0;							// スコア初期化
		if (sceneName == SceneName.PlayGame)	// メインゲームを開始する場合、難易度ごとのシーン名に設定する
			sceneName = (setDifficulty == Game.Difficulty.Normal) ? SceneName.Normal : SceneName.Hard;
		OnClick ();	// ボタン制御関数
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}
