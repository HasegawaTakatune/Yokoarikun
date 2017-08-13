using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Const;

public class OnClickButtonSceneManager : OnClickButton {
	//******************************************************************//
	//					OnClickButton継承クラス							//
	//			メインゲーム・ランキングの難易度を選択するボタン			//
	//******************************************************************//

	[SerializeField]
	Game.Difficulty setDifficulty;	//	難易度を保持

	// 難易度を選択するボタンを押した時に呼ばれる関数
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
