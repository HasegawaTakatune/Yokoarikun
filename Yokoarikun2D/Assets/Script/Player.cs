using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Const;
using UnityEngine.UI;
public class Player : MonoBehaviour {
	//******************************************************************//
	//	プレイヤー制御
	// 	移動制御・アニメーション制御	
	//	スタート/ゴール処理・誘導関数の呼び出し制御
	//
	//	呼び出し関係図
	//	Start　─┬─>ManualAnimation
	//			└─>Update	─┬─>Move					┌─>Alignment
	//						 ├─>changeAnimation			├─>LeadControl.AddScore
	//						 │							├─>LeadControl.APositionIsInitialized
	//						 └─>ReturnToInitialPosition	┴─>LeadControl.AListIsInitialized
	//******************************************************************//
	[SerializeField]LeadControl leadControl;	// 誘導コンポーネントを参照
	byte platform = 0;							// プラットフォームの格納

	const float addPos = 0.1f;					// タッチした座標の有効幅

	const float RightFrame = 4;					// 右端
	const float LeftFrame = -4;					// 左端

	const int NORMAL=0,BOXER=1,GUITAR=2;		// キャラクタータイプ一覧 (ノーマル・ボクサー・ギター)

	[SerializeField]Transform StartPosition;	// 始めの座標
	[SerializeField]Transform EndPosition;		// 終わりの座標
	Vector3 startPos,endPos;			// 座標だけを格納する

	bool AddScore  = false;				// スコア加算の制御  (true : スコア処理をした  false : スコア処理をしていない)

	[SerializeField]Animator animator;	// アニメーター格納
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
	[SerializeField]SpriteRenderer spriteRenderer;			// スプライトレンダラ―格納
	byte type = NORMAL;					// キャラクタータイプ格納
	byte direc = 0;						// キャラクターの方向を格納

	const float speed = 0.05f;			// 移動速度

	delegate void Delegate();			// delegate型の宣言
	Delegate Move;						// 移動メソッドを格納する変数を宣言

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	プラットホーム設定、スコア・移動量の初期化、手動アニメーション制御開始
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start () {
		// プラットフォーム別に移動関数を登録する
		Move += (Application.platform == RuntimePlatform.Android) ? (Delegate)AndroidControl : (Delegate)PcControl;

		Game.score = 0;							// スコアの初期化
		startPos = StartPosition.position;		// スタート位置の初期化
		endPos = EndPosition.position;			// ゴール座標の初期化

		StartCoroutine (ManualAnimation ());	// アニメーション制御関数
	}

	//**************************************************************//
	//	関数名　:	Update
	//	機能		:	移動制御・アニメーション制御
	//				スタート地点からの入場・ゴール時の自動移動処理
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Update () {
		if (!Game.stop) {								// ポーズ状態じゃない時

			// 移動
			if (Game.start) {							// ゲーム開始
				Move();									// 移動関数

				changeAnimation (type, direc);			// アニメーション制御関数
				leadControl.direction = direc;			// 誘導するキャラの向きを設定
			}

			Vector3 nowPosition = gameObject.transform.position;	// 現在の座標を更新
			if (endPos.y >= nowPosition.y) {						// ゴール近くまで到達したら
				if (!AddScore) 										// スコアが加算されていなければ
					StartCoroutine (ReturnToInitialPosition ());	// ゴール後の初期化などの処理群

				// 自動ゴール
				if (Vector3.Distance (nowPosition, endPos) >= 0.5f) {// ゴールと同じ座標になるまで
					float angle;	// 移動方向を格納
					angle = Mathf.Atan2 (endPos.y - nowPosition.y, endPos.x - nowPosition.x);				// ゴールの方向を計算する
					transform.position += new Vector3 (Mathf.Cos (angle), -Mathf.Sin (angle), 0) * speed;	// ゴールまで移動をする
				}
				direc = Key.DOWN;	// 下を向かせる
			}

			if (startPos.y - 1 <= nowPosition.y) {			// ゲームステージに入場する処理
				transform.position += Vector3.down * speed;	// ステージに入場するまで下移動をする
				direc = Key.DOWN;							// 下を向く
				AddScore = false;							// スコア加算フラグの初期化
			}
		}
	}

	//**************************************************************//
	//	関数名　:	ReturnToInitialPosition
	//	機能		:	ゴール後の初期化処理群（コルーチン）
	//				スコア加算、座標・誘導キャラクターの初期化
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator ReturnToInitialPosition(){
		AddScore = true;										// スコアの加算をしたことを知らせる
		leadControl.AddScore ();								// 誘導したキャラクターの数分スコアに反映する
		yield return new WaitForSeconds (1);					// ゴールに入り込む待ち時間
		transform.position = startPos;							// 座標をスタート地点に設定
		Alignment ();											// キャラクターのタイプ変更
		leadControl.APositionIsInitialized (startPos);			// 誘導するキャラクターの座標初期化
		leadControl.AListIsInitialized (5);						// 誘導するキャラクターの誘導人数を初期化
	}

	//**************************************************************//
	//	関数名　:	AndroidControl
	//	機能		:	プラットフォームがAndroidの時の制御処理
	//				タッチ座標とキャラ座標の差分で移動量を計算して移動する
	//				移動量から向きを決める
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void AndroidControl(){
		if (Input.touchCount > 0) {	// タッチがされた時
			foreach (Touch t in Input.touches) {
				if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled) {// タッチが続行されている間
					Vector3 touchPosition = t.position;								// タッチポジションに代入
					touchPosition.z = 10f;											// タッチ座標で奥行は取れないので、変えを代入
					touchPosition = Camera.main.ScreenToWorldPoint (touchPosition);	// 画面上でタッチした位置を格納
					Vector3 pos = ((touchPosition - transform.position).normalized) * speed;	// キャラ座標とタッチ座標の差分を移動量に変換する
					transform.position += pos;													// 変換した移動量を加算する
					direc = (pos.x > 0.03f) ? Key.RIGHT : (pos.x < -0.03f) ? Key.LEFT : (pos.y <= 0) ? Key.DOWN : Key.UP; // 移動量から向く方向を決める
				}
			}
		}
	}

	//**************************************************************//
	//	関数名　:	PcControl
	//	機能		:	プラットフォームがPCの時の移動処理
	//				キー入力の戻り値を直接移動量として使う
	//				移動量から向きを決める
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void PcControl(){
		float horizontal = Input.GetAxis ("Horizontal") * speed;					// 水平入力を移動量に変換
		float vertical = Input.GetAxis ("Vertical") * speed;						// 垂直入力を移動量に変換
		if(horizontal != 0 && vertical != 0){
			horizontal = horizontal / 1.4f;
			vertical = vertical / 1.4f;
		}
		transform.Translate (horizontal, vertical, 0);								// 移動量を加算する
		direc = (horizontal > 0) ? Key.RIGHT : (horizontal < 0) ? Key.LEFT : (vertical <= 0) ? Key.DOWN : Key.UP; // 移動量から向く方向を決める
	}

	//**************************************************************//
	//	関数名　:	Alignment
	//	機能		:	キャラクターのタイプ変更（キャラクター画像の種類変更）
	// 				ノーマル　→　ボクサー　→　ギター　→　ノーマル　の順に変更していく
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void Alignment(){
		switch (type) {
		case NORMAL:type = BOXER;break;		// ノーマル　→　ボクサー
		case BOXER:type = GUITAR;break;		// ボクサー　→　ギター
		case GUITAR:type = NORMAL;break;	// ギター　　→　ノーマル
		}
		animator.Play (Down [type]);		// 下向きのアニメーションを再生
	}

	//**************************************************************//
	//	関数名　:	changeAnimation
	//	機能		:	アニメ画像の種類・向き別にアニメーション制御をする
	// 				右向きのアニメーションは、左向きのアニメーションを反転させて使い、アニメーション画像の削減をしている。
	//				そのため、右向きの処理で左向きのアニメーションを再生している
	//	引数		:	int type		キャラクターのタイプを受け取る NORMAL:0	BOXER:1	GUITAR:2
	//				byte direction	向きを受け取る	UP:1 RIGHT:2 DOWN:4 LEFT:8
	//	戻り値	:	なし
	//**************************************************************//
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

	//**************************************************************//
	//	関数名　:	ManualAnimation
	//	機能		:	アニメーション制御（コルーチン）
	// 				上下向きのアニメーションは、コルーチンで一定時間で反転をさせる事で表現する
	// 				アニメーション画像を削減するための処理
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator ManualAnimation(){
		while (true) {
			yield return new WaitForSeconds (.3f);				// アニメーションのフレームレート
			if (direc == Key.UP || direc == Key.DOWN)			// アニメーションが上・下向きの時
				spriteRenderer.flipX = !spriteRenderer.flipX;	// 画像を反転させる
		}
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}