using UnityEngine;
using System.Collections;

public class ArenaIcon : MonoBehaviour {
	//**************************************************************//
	//	ゴールの位置を知らせるアイコン
	//
	//	呼び出し関係図
	//	Start　─┬─>Change
	//			└─>Update
	//**************************************************************//
	Vector2 pos;				// 現在の座標
	Vector2 ScreenPos;			// スクリーン座標
	public bool Shake = true;	// 揺れ(true : 上移動　false : 下移動)
	const float speed = 0.5f;	// 揺れる速度

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	座標の初期化・フラグ変更関数を呼ぶ
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start () {
		// 座標の初期化
		pos = new Vector2 (transform.position.x, transform.position.y);
		// フラグ変更関数
		StartCoroutine (Change ());
	}

	//**************************************************************//
	//	関数名　:	Update
	//	機能		:	メインループ・アイコンを上下に揺らす・アイコン表示の制御
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Update () {
		// 揺らす(true : 上移動　false : 下移動)
		pos.y += (Shake) ? speed : -speed;
		transform.position = pos;

		// 画面座標に変換
		ScreenPos = Camera.main.ScreenToWorldPoint (pos);
		// ゴールに近づいたら表示を消す
		if (ScreenPos.y <= -20)
			gameObject.SetActive (false);
	}

	//**************************************************************//
	//	関数名　:	Change
	//	機能		:	フラグ変更関数（コルーチン）
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator Change(){
		while (true) {
			yield return new WaitForSeconds (1f);	// 遅延
			Shake = !Shake;							// フラグの反転
		}
	}


	//**************************************************************//
	//								End of class					//
	//**************************************************************//
}
