﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnClickButton : MonoBehaviour {
	//******************************************************************//
	//	OnClickButton系列の基底クラス
	//	オーディオの制御・シーン移動制御を受け持つ
	//
	//	呼び出し関係図
	//	Start
	//	ボタンクリック	───>MoveToScene
	//******************************************************************//
	[SerializeField] protected bool IChoose = false;	// 選択状態の管理　（true : 選択  false : 非選択）
	[SerializeField] protected AudioClip audioClip;		// オーディオクリップ保持
	[SerializeField] protected AudioSource audioSource;	// オーディオソース保持
	// シーン名一覧
	public enum SceneName
	{
		None,		// なし
		Title,		// タイトル
		PlayGame,	// ゲーム
		Normal,		// ノーマルモード
		Hard,		// ハードモード
		Result,		// リザルト
		Rank		// ランキング
	}
	protected static SceneName sceneName;				// シーン名保持
	[SerializeField] SceneName loadSceneName;			// インスタンスに表示する用のシーン名選択

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	サウンドの設定・ボタンの選択状態を設定・シーン名の設定
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	protected void Start () {
		audioSource.clip = audioClip;						// オーディオクリップの設定

		// Unityエディター・Windowsで実行している時、かつ選択してほしいボタンの時
		if ((Application.platform == RuntimePlatform.WindowsEditor ||
			 Application.platform == RuntimePlatform.WindowsPlayer) && IChoose)
			gameObject.GetComponent<Button> ().Select ();	// ボタンを選択状態にする

		if (loadSceneName != SceneName.None)
			sceneName = loadSceneName;			// インスタンスでシーンを指定している時、そのシーンを適応させる
	}

	//**************************************************************//
	//	関数名　:	OnClick
	//	機能		:	ボタンクリック時の処理
	//				（ボタンクリック音の再生・シーン遷移関数を呼ぶ）
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void OnClick(){
		audioSource.PlayOneShot (audioClip);	// オーディオの再生
		StartCoroutine ("MoveToScene");			// シーン遷移の関数
	}

	//**************************************************************//
	//	関数名　:	MoveToScene
	//	機能		:	シーン遷移の関数（コルーチン）
	//				ボタンクリック音が終わるまで遅延させる
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	private IEnumerator MoveToScene(){
		yield return new WaitForSeconds (audioClip.length);	// ボタンクリック音が終わるまで遅延
		SceneManager.LoadScene (sceneName.ToString ());		// シーン遷移
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}
