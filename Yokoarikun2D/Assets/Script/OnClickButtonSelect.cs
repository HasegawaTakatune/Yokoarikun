using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Const;

public class OnClickButtonSelect : OnClickButton {
	//******************************************************************//
	//	OnClickButton継承クラス
	//	選択メニューボタンの表示・非表示の制御群
	//******************************************************************//

	[SerializeField]
	GameObject[] ToMakeItTransparent;	// 非表示にするボタンを格納
	[SerializeField]
	GameObject[] ToMakeItOpaque;		// 表示させるボタンを格納

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	基底クラスの初期化を流用
	//				サウンドの設定・ボタンの選択状態を設定・シーン名の設定
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	new void Start () {					// スタート関数に上書きする
		base.Start ();					// 基底クラスのスタート関数を呼ぶ
	}

	//**************************************************************//
	//	関数名　:	OnClickRank
	//	機能		:	ランキングボタンを押した時に、
	//				遷移先をランキングに設定して、難易度選択に移行する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void OnClickRank(){
		sceneName = SceneName.Rank;				// 遷移するシーンを"ランク"に設定
		StartCoroutine (MoveToSelect());		// 選択ボタンの表示を制御する関数を呼ぶ
	}

	//**************************************************************//
	//	関数名　:	OnClickStart
	//	機能		:	スタートボタンを押した時に、
	//				遷移先をゲームに設定して、難易度選択に移行する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void OnClickStart(){
		sceneName = SceneName.PlayGame;			// 遷移するシーンを"ゲーム"に設定
		StartCoroutine (MoveToSelect());		// 選択ボタンの表示を制御する関数を呼ぶ
	}

	//**************************************************************//
	//	関数名　:	OnClickStop
	//	機能		:	ストップボタンを押した時に、
	//				ゲームを停止状態にさせ、選択ボタンを表示する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void OnClickStop(){
		if (Game.start) {
			Game.stop = true;					// ゲームが始まっている時、ゲームを停止する
			StartCoroutine (MoveToSelect());	// 選択ボタンの表示を制御する関数を呼ぶ
		}
	}

	//**************************************************************//
	//	関数名　:	OnClickResume
	//	機能		:	再開ボタンを押した時に、
	//				ゲーム停止状態を解除し、選択ボタンを非表示にする
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void OnClickResume(){
		StartCoroutine (AStopIsReleased ());	// 停止状態を解除する関数
		StartCoroutine (MoveToSelect());		// 選択ボタンの表示を制御する関数を呼ぶ
	}

	//**************************************************************//
	//	関数名　:	AStopIsReleased
	//	機能		:	停止状態を解除する（コルーチン）
	// 				ボタンクリック音が終わるまで遅延させる
	// 				(表示されている選択メニューが消えるまで、ゲームを始めさせないため)
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator AStopIsReleased(){
		yield return new WaitForSeconds (audioClip.length); // ボタンクリック音が終わるまで遅延
		Game.stop = false;									// 停止状態を解除
	}

	//**************************************************************//
	//	関数名　:	MoveToSelect
	//	機能		:	選択メニューの表示・非表示を制御（コルーチン）
	// 				ボタンクリック音が終わるまで遅延させる
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	private IEnumerator MoveToSelect(){
		audioSource.PlayOneShot (audioClip);				// ボタンクリック音を再生
		yield return new WaitForSeconds (audioClip.length);	// ボタンクリック音が終わるまで遅延
		int i = 0;
		while (ToMakeItTransparent.Length != i) {
			ToMakeItTransparent [i].SetActive (false);		// 配列内のボタンを非表示にする
			i++;
		}
		i = 0;
		while (ToMakeItOpaque.Length != i) {
			ToMakeItOpaque [i].SetActive (true);			// 配列内のボタンを表示にする
			i++;
		}
		gameObject.SetActive (false);						// 自分自身を非表示にする
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}