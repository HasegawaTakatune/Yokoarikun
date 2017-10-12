using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Const;

public class Enemy : MonoBehaviour,RecieveInterface{
	//******************************************************************//
	//	敵の制御	
	// 	移動制御・アニメーション制御	
	//
	//	呼び出し関係図
	//	Start	───>Update ───>Move	─┬─>Escape		─┬─>Game.IsItHard
	//			 			 		 └─>ScrollEnd	 └─>LeadControl.RemoveAllCustomers
	//******************************************************************//


	[SerializeField] LeadControl leadControl;			// 誘導コンポーネントを参照
	Transform player;									// プレイヤー座標を参照
	const byte START=0,STOP=1,EXIT=2;					// 行動ステータス
	[SerializeField] byte status = START; 				// 行動パターン
	const int FEINT=0,RUN=1;							// 行動タイプ一覧
	[SerializeField] int type = FEINT;					// 行動タイプ
	[SerializeField] float speed = 1.0f;				// 移動速度
	[SerializeField] int direction = 1;					// 移動方向
	Vector3 position;									// ステータス変更の基準座標を格納
	float movement;										// 移動量を格納
	[SerializeField] SpriteRenderer spriteRenderer;		// スプライトレンダラ―格納
	delegate void Delegate();							// delegate型の宣言
	Delegate Move;										// 移動メソッドを格納する変数を宣言

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	プレイヤーの探索、移動基準の座標・移動量の初期化
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start(){
		player = GameObject.Find ("Player").transform;	// プレイヤー座標を初期化
		position = transform.position;					// 基準座標の初期化
		movement = direction * speed * Time.deltaTime;	// 移動量の初期化
		Move += (Game.difficulty == Game.Difficulty.Normal) ? (Delegate)EasyMove : (Delegate)HardMove; // 移動メソッドを選択
	}

	//**************************************************************//
	//	関数名　:	Update
	//	機能		:	メインループ、難易度・移動パターン別に移動を制御する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Update () {
		if (!Game.stop && Game.start) {		// 停止していない・ゲームが稼働中の時
			Move();							// 移動関数の呼び出し
		}
	}

	//**************************************************************//
	//	関数名　:	EasyMove
	//	機能		:	イージーモードの行動パターン、移動途中に一時停止するモノと
	//				まっすぐに移動するパターンを用意してある
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void EasyMove(){
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
	}

	//**************************************************************//
	//	関数名　:	HardMove
	//	機能		:	ハードモードの行動パターン、プレイヤーに追尾ずる
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void HardMove(){
		transform.position += new Vector3 (movement,(player.position.y - transform.position.y) * 0.002f,0);	// プレイヤーを追尾
		if (transform.position.x >= -position.x || transform.position.x <= position.x)						// フレームアウトした時 
			ScrollEnd ();		// ステータスを再設定する関数
	}

	//**************************************************************//
	//	関数名　:	ScrollEnd
	//	機能		:	画面の端まで到着したら、次の行動ステータスを再設定する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
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

	//**************************************************************//
	//	関数名　:	ISnatched
	//	機能		:	キャラクターを横取りした時のメッセージ処理（受信）
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void ISnatched(){
		status = EXIT;		// 逃げる行動に移る
		speed = 3;			// 早足で逃げる
		movement = direction * speed * Time.deltaTime;	// 移動量の再設定
	}

	//**************************************************************//
	//	関数名　:	Escape
	//	機能		:	一定時間をおいて逃げる行動に移る（コルーチン）
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator Escape(){
		yield return new WaitForSeconds (Random.Range (1, 3));	// 逃げ始めるタイミングを図る
		status = EXIT;		// 逃げる行動に移る
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}