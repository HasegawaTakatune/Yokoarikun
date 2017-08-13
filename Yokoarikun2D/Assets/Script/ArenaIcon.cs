using UnityEngine;
using System.Collections;

public class ArenaIcon : MonoBehaviour {
	//**************************************************************//
	//					ゴールの位置を知らせるアイコン				//
	//**************************************************************//
	Vector2 pos;				// 現在の座標
	Vector2 ScreenPos;			// スクリーン座標
	public bool Shake = true;	// 揺れ(true : 上移動　false : 下移動)
	const float speed = 0.5f;	// 揺れる速度

	void Start () {
		// 座標の初期化
		pos = new Vector2 (transform.position.x, transform.position.y);
		// フラグ変更関数
		StartCoroutine (Change ());
	}
	
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

	// フラグ変更関数（コルーチン）
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
