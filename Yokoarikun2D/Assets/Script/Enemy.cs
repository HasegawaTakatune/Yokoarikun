using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Const;

public class Enemy : MonoBehaviour,RecieveInterface{
	//******************************************************************//
	//								敵の制御								//
	// 					移動制御・アニメーション制御						//
	//******************************************************************//


	[SerializeField]
	LeadControl leadControl;			// 誘導コンポーネントを参照
	Transform player;					// プレイヤー座標を参照
	const byte START=0,STOP=1,EXIT=2;	// 行動ステータス
	[SerializeField]
	byte status = START; 				// 行動パターン
	const int FEINT=0,RUN=1;			// 行動タイプ一覧
	[SerializeField]
	int type = FEINT;					// 行動タイプ
	[SerializeField]
	float speed = 1.0f;					// 移動速度
	[SerializeField]
	int direction = 1;					// 移動方向
	Vector3 position;					// ステータス変更の基準座標を格納
	float movement;						// 移動量を格納
	[SerializeField]
	SpriteRenderer spriteRenderer;		// スプライトレンダラ―格納

	void Start(){
		player = GameObject.Find ("Player").transform;	// プレイヤー座標を初期化
		position = transform.position;					// 基準座標の初期化
		movement = direction * speed * Time.deltaTime;	// 移動量の初期化
	}

	void Update () {
		if (!Game.stop && Game.start) {		// 停止していない・ゲームが稼働中の時
			switch (Game.difficulty) {		// 難易度分岐
			case Game.Difficulty.Normal:	// ノーマルモード
				if (type == FEINT) {		// フェイント行動
					switch (status) {		// ステータスで行動分岐
					case START:				// 移動開始
						transform.Translate (movement, 0, 0);	// 移動量の分だけ移動する
						if (transform.position.x >= -1 || transform.position.x >= 1) {	// 指定座標に到達した時
							status = STOP;						// 一時停止をさせる
							StartCoroutine (Escape ());			// ステータスを逃げるに変更する関数
						}
						break;

					case STOP:				// 一時停止（何もしない）
						break;

					case EXIT:									// 退出
						transform.Translate (movement, 0, 0);	// 移動量の分だけ移動する
						if (transform.position.x >= -position.x || transform.position.x <= position.x)	// フレームアウトした時
							ScrollEnd ();						// ステータスを再設定する関数
						break;
					}
			
				} else if (type == RUN) {					// 走る行動
					transform.Translate (movement, 0, 0);	// 移動量の分だけ移動する
					if (transform.position.x >= -position.x || transform.position.x <= position.x)		// フレームアウトした時
						ScrollEnd ();						// ステータスを再設定する関数
				}
				break;
	
			case Game.Difficulty.Hard:	// ハードモード
				transform.position += new Vector3 (movement,(player.position.y - transform.position.y) * 0.002f,0);	// プレイヤーを追尾
				if (transform.position.x >= -position.x || transform.position.x <= position.x)						// フレームアウトした時 
					ScrollEnd ();		// ステータスを再設定する関数
				break;

			default:
			//Debug.Log ("Errer : difficulty");
				break;

			}
		}
	}


	// 画面端まで到着したら
	void ScrollEnd(){
		float y = 0;		// 次に出撃するy座標を設定
		status = START;		// ステータスを始めに戻す
		if (Game.IsItHard ()) {			// ハードモードの時
			y = Random.Range (-6, 0);	// y座標をランダム指定
			position = new Vector3 (position.x, player.position.y, position.z);
		}
		type = (Random.Range (0, 2));								// 行動パターンを再設定
		direction = (Random.Range (0, 2)) == 0 ? 1 : -1;			// 移動方向を再設定
		speed = Random.Range (1, 2) + (0.015f * Game.score);		// スピード　スコアによって速度が上がる
		transform.position = new Vector3 (position.x * direction, position.y + y, position.z);	// 初期位置
		spriteRenderer.flipX = (direction == 1) ? false : true;		// 向きの変換
		movement = direction * speed * Time.deltaTime;				// 移動量の再設定
		leadControl.RemoveAllCustomers ();							// 横取りしたキャラクターを全開放する
	}

	// キャラクターを横取りした時のメッセージ処理（受信）
	public void ISnatched(){
		status = EXIT;		// 逃げる行動に移る
		speed = 3;			// 早足で逃げる
		movement = direction * speed * Time.deltaTime;	// 移動量の再設定
	}
		
	// 逃げる（コルーチン）
	IEnumerator Escape(){
		yield return new WaitForSeconds (Random.Range (1, 3));	// 逃げ始めるタイミングを図る
		status = EXIT;		// 逃げる行動に移る
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}