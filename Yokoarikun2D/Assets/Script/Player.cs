using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Const;

public class Player : MonoBehaviour {
	//******************************************************************//
	//							プレイヤー制御							//
	// 					移動制御・アニメーション制御						//
	//				スタート/ゴール処理・誘導関数の呼び出し制御			//
	//******************************************************************//
	[SerializeField]
	LeadControl leadControl;					// 誘導コンポーネントを参照
	byte platform = 0;							// プラットフォームの格納

	Vector3 touchPosition;						// タッチした座標
	Vector3 nowPosition;						// 現在の座標

	const float addPos = 0.1f;					// タッチした座標の有効幅

	const float RightFrame = 4;					// 右端
	const float LeftFrame = -4;					// 左端

	const int NORMAL=0,BOXER=1,GUITAR=2;		// キャラクタータイプ一覧 (ノーマル・ボクサー・ギター)

	int moveDirecResult = 0;					// 移動方向を格納
	Vector3[] movePosiResult = new Vector3[8];	// 8方向の移動量を格納

	[SerializeField]
	Transform StartPosition;			// 始めの座標
	[SerializeField]
	Transform EndPosition;				// 終わりの座標
	Vector3 startPos,endPos;			// 座標だけを格納する

	bool AddScore  = false;				// スコア加算の制御  (true : スコア処理をした  false : スコア処理をしていない)

	[SerializeField]
	Animator animator;					// アニメーター格納
	static readonly int[] Up = new int[] {		// 上向きアニメーション
		Animator.StringToHash ("PlayerSprite@Up"),			// 通常
		Animator.StringToHash ("PlayerBoxerSprite@Up"),		// ボクサー
		Animator.StringToHash ("PlayerGuitarSprite@Up")		// ギター
	};
	static readonly int[] Down = new int[] {	// 下向きアニメーション 
		Animator.StringToHash ("PlayerSprite@Down"),		// 通常
		Animator.StringToHash ("PlayerBoxerSprite@Down"),	// ボクサー
		Animator.StringToHash ("PlayerGuitarSprite@Down")	// ギター
	};
	static readonly int[] Left = new int[] {	// 左向きアニメーション
		Animator.StringToHash ("PlayerSprite@Left"),		// 通常
		Animator.StringToHash ("PlayerBoxerSprite@Left"),	// ボクサー
		Animator.StringToHash ("PlayerGuitarSprite@Left")	// ギター
	};
	[SerializeField]
	SpriteRenderer spriteRenderer;		// スプライトレンダラ―格納
	byte type = NORMAL;					// キャラクタータイプ格納
	byte direc = 0;						// キャラクターの方向を格納

	public const float speed = 0.05f;	// 移動速度

	void Start () {

		// 今使っているプラットフォーム
		if (Application.platform == RuntimePlatform.WindowsEditor) {		// Unityエディター
			platform = Platform.UnityEditor;
		} else if (Application.platform == RuntimePlatform.WindowsPlayer) { // Windows
			platform = Platform.Windows;
		} else if (Application.platform == RuntimePlatform.Android) {		// Android
			platform = Platform.Android;
		} else {															// どのプラットフォームでもなかった時
			platform = Platform.None;
		}

		Game.score = 0;							// スコアの初期化
		startPos = StartPosition.position;		// スタート位置の初期化
		endPos = EndPosition.position;			// ゴール座標の初期化

		int i = 0;
		while (movePosiResult.Length != i) {	// 8方向の移動量を計算しておく
			movePosiResult [i] = new Vector3 (Mathf.Sin ((transform.localEulerAngles.y + 45 * i) * 3.14f / 180) * speed * (720 / Screen.width), 
				Mathf.Cos ((transform.localEulerAngles.y + 45 * i) * 3.14f / 180) * speed * (1280 / Screen.height), 0);
			i++;
		}

		StartCoroutine (ManualAnimation ());	// アニメーション制御関数
	}

	void Update () {
		if (!Game.stop) {									// ポーズ状態じゃない時
			nowPosition = gameObject.transform.position;	// 現在の座標を更新

			// 移動
			if (Game.start) {								// ゲーム開始
				if (platform == Platform.Android) {			// Androidの時
					if (Input.touchCount > 0) {				// タッチがされた時
						foreach (Touch t in Input.touches) {
							if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)	// タッチが続行されている間
								touchPosition = Camera.main.ScreenToWorldPoint (t.position);	// 画面上でタッチした位置を格納
							else
								touchPosition = nowPosition;									// タッチされない限り、同じ座標を維持
						}
					} else
						touchPosition = nowPosition;		// タッチされない限り、同じ座標を維持

					if ((nowPosition.y) < (touchPosition.y - addPos)) {		// タッチした座標の近くにいなければ
						if (nowPosition.y < startPos.y - 1.1f) 				// ステージの範囲内であれば
							MovementSettings (Key.UP);						// 上に移動の設定をする
					}
					if ((nowPosition.y) > (touchPosition.y + addPos)) 		// タッチした座標の近くにいなければ
						MovementSettings (Key.DOWN);						// 下に移動の設定をする
					
					if ((nowPosition.x) > (touchPosition.x + addPos)) {		// タッチした座標の近くにいなければ
						if (nowPosition.x > LeftFrame) 						// ステージの範囲内であれば
							MovementSettings (Key.LEFT);					// 左に移動の設定をする
					}
					if ((nowPosition.x) < (touchPosition.x - addPos)) {		// タッチした座標の近くにいなければ
						if (nowPosition.x < RightFrame)						// ステージの範囲内であれば
							MovementSettings (Key.RIGHT);					// 右に移動の設定をする
					}
				} else if (platform == Platform.UnityEditor || platform == Platform.Windows) {	// Unityエディター・Windowsの時
					if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {		// 上キーが押されたら
						if (nowPosition.y < startPos.y - 1.1f) 								// ステージの範囲内であれば 
							MovementSettings (Key.UP);										// 上に移動の設定をする 
					}
					if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) 		// 下キーが押されたら
						MovementSettings (Key.DOWN);										// 下に移動の設定をする
					
					if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {		// 右キーが押されたら
						if (nowPosition.x > LeftFrame) 										// ステージの範囲内であれば
							MovementSettings (Key.LEFT);									// 右に移動の設定をする
					}
					if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {	// 左キーが押されたら
						if (nowPosition.x < RightFrame) 									// ステージの範囲内であれば
							MovementSettings (Key.RIGHT);									// 左に移動の設定をする
					}
				}

				changeAnimation (type, direc);			// アニメーション制御関数
				selectMoveDirection (moveDirecResult);	// 移動制御関数
				leadControl.direction = direc;			// 誘導するキャラの向きを設定
			}

			if (endPos.y >= nowPosition.y) {						// ゴール近くまで到達したら
				if (!AddScore) 										// スコアが加算されていなければ
					StartCoroutine (ReturnToInitialPosition ());	// ゴール後の初期化などの処理群

				// 自動ゴール
				if (Vector3.Distance (nowPosition, endPos) >= 0.5f) {// ゴールと同じ座標になるまで
					float angle;	// 移動方向を格納
					angle = Mathf.Atan2 (endPos.y - nowPosition.y, endPos.x - nowPosition.x);				// ゴールの方向を計算する
					transform.position += new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0) * speed;	// ゴールまで移動をする
				}
				direc = Key.DOWN;	// 下を向かせる
			}

			if (startPos.y - 1 <= nowPosition.y) {			// ゲームステージに入場する処理
				transform.position += movePosiResult [4];	// ステージに入場するまで下移動をする
				direc = Key.DOWN;							// 下を向く
				AddScore = false;							// スコア加算フラグの初期化
			}
		}
	}
		
	// ゴール後の初期化処理群（コルーチン）
	IEnumerator ReturnToInitialPosition(){
		AddScore = true;										// スコアの加算をしたことを知らせる
		leadControl.AddScore ();								// 誘導したキャラクターの数分スコアに反映する
		yield return new WaitForSeconds (1);					// ゴールに入り込む待ち時間
		transform.position = startPos;							// 座標をスタート地点に設定
		Alignment ();											// キャラクターのタイプ変更
		touchPosition = new Vector3 (nowPosition.x, -10, 0);	// タッチ座標の初期化
		leadControl.APositionIsInitialized (startPos);			// 誘導するキャラクターの座標初期化
		leadControl.AListIsInitialized (5);						// 誘導するキャラクターの誘導人数を初期化
	}

	// キャラクターのタイプ変更
	// ノーマル　→　ボクサー　→　ギター　→　ノーマル　の順に変更していく
	public void Alignment(){
		switch (type) {
		case NORMAL:type = BOXER;break;		// ノーマル　→　ボクサー
		case BOXER:type = GUITAR;break;		// ボクサー　→　ギター
		case GUITAR:type = NORMAL;break;	// ギター　　→　ノーマル
		}
		animator.Play (Down [type]);		// 下向きのアニメーションを再生
	}



	// 移動の設定をまとめて制御する
	void MovementSettings(byte key){
		moveDirecResult += key;		// 移動方向の設定
		direc = key;				// アニメーションの向きを設定
	}

	// アニメーション制御
	// 右向きのアニメーションは、左向きのアニメーションを反転させて使い、
	// アニメーション画像の削減をしている、そのため、右向きの処理で左向きのアニメーションを再生している
	void changeAnimation(int type,byte direction){
		switch (direction) {
		case Key.DOWN:						// 下向き
			animator.Play (Down [type]);	// 下向きのアニメーションを再生
			break;

		case Key.UP:						// 上向き
			animator.Play (Up [type]);		// 上向きのアニメーションを再生
			break;

		case Key.LEFT:						// 左向き
			animator.Play (Left [type]);	// 左向きのアニメーションを再生
			spriteRenderer.flipX = false;	// 画像反転を無効化
			break;

		case Key.RIGHT:						// 右向き
			animator.Play (Left [type]);	// 左向きのアニメーションを再生
			spriteRenderer.flipX = true;	// 画像反転を有効化
			break;

		default:
			//Debug.Log ("Errer");
			break;
		}
	}

	// アニメーション制御（コルーチン）
	// 上下向きのアニメーションは、コルーチンで一定時間で反転をさせる事で表現する
	// アニメーション画像を削減するための処理
	IEnumerator ManualAnimation(){
		while (true) {
			yield return new WaitForSeconds (.3f);				// アニメーションのフレームレート
			if (direc == Key.UP || direc == Key.DOWN)			// アニメーションが上・下向きの時
				spriteRenderer.flipX = !spriteRenderer.flipX;	// 画像を反転させる
		}
	}

	// 移動関数(switch)
	void selectMoveDirection(int direction){
		Vector3 move;				// 移動量を格納
		switch (direction) {
		case Key.UP:		move = movePosiResult [0];break;	// ↑に移動
		case Key.UPRIGHT:	move = movePosiResult [1];break;	// ↗に移動
		case Key.RIGHT:		move = movePosiResult [2];break;	// →に移動
		case Key.DOWNRIGHT:	move = movePosiResult [3];break;	// ↘に移動
		case Key.DOWN:		move = movePosiResult [4];break;	// ↓に移動
		case Key.DOWNLEFT:	move = movePosiResult [5];break;	// ↙に移動
		case Key.LEFT:		move = movePosiResult [6];break;	// ←に移動
		case Key.UPLEFT:	move = movePosiResult [7];break;	// ↖に移動
		default:			move = Vector3.zero;	  break;	// それ以外、移動量を０にする
		}
		transform.position += move;	// 移動
		moveDirecResult = 0;		// 移動方向の初期化
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}
