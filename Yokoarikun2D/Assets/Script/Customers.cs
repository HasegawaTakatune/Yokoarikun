using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Const;

public class Customers : MonoBehaviour {
	//******************************************************************//
	//	誘導対象となるお客さんの制御	
	//
	//	呼び出し関係図
	//	Start　─┬─>SetAnimator
	//			└─>GenderDetermination
	//	Update ─┬─>Move
	//			└─>BreakTime
	//******************************************************************//
	const byte OutsideTheArea=0,InArea=1,Break=2;				// 移動タイプ　(OutsideTheArea : エリア外の移動  InArea : エリア内移動)
	byte moveStatus = 0;										// 移動タイプを格納
	public int type;											// キャラクターのタイプを設定 (Boy_1・Boy_2・Girl_1)

	public int CustomerNumber;									// キャラクターの番号
	public LeadControl player;									// 誘導される対象を格納
	public bool Induction = false;								// 誘導されているかの判定 (true : 誘導されている  	false : 誘導されていない)
	bool hit = false;											// 誰かに当たったかの判定 (true : 当たった  			false : 当たってない)

	public Vector3 target = new Vector3 (0, 6, 0);				// ターゲットの座標を格納
	const float speed =0.05f;									// 移動速度
	const float range = 0.05f;									// ターゲット座標に到達する許容範囲
	public byte direction = 0;									// アニメーションの向きを格納

	public List<Vector3> movePosResult = new List<Vector3> ();	// 移動量の格納
	int randomAngle = 0;										// ランダム移動の方向

	bool doOnce = false;										// 一度だけ実行

	[SerializeField]
	Animator animator;											// アニメーター格納
	static readonly int[] DOWN = new int[] {					// 下向きアニメーション
		Animator.StringToHash ("CustomersSprite@Down"),			// 男の子_1
		Animator.StringToHash ("CustomersBoySprite@Down"),		// 男の子_2
		Animator.StringToHash ("CustomersGirlSprite@Down")		// 女の子_1
	};
	static readonly int[] UP = new int[] {						// 上向きアニメーション
		Animator.StringToHash ("CustomersSprite@Up"),			// 男の子_1
		Animator.StringToHash ("CustomersBoySprite@Up"),		// 男の子_2
		Animator.StringToHash ("CustomersGirlSprite@Up")		// 女の子_1
	};
	static readonly int[] RIGHT = new int[] {					// 右向きアニメーション
		Animator.StringToHash ("CustomersSprite@Right"),		// 男の子_1
		Animator.StringToHash ("CustomersBoySprite@Right"),		// 男の子_2
		Animator.StringToHash ("CustomersGirlSprite@Right")		// 女の子_1
	};
	[SerializeField]
	SpriteRenderer spriteRenderer;								// スプライトレンダラ格納

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	移動量の初期化、キャラクターの種類を設定
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start(){
		GenderDetermination ();									// キャラクターの種類を設定
		if (!Induction) {										// 誘導されていなければ
			for (int j = 0; j < 8; j++) {						// 8方向分ループする
				movePosResult.Add (new Vector3 (				// 移動量初期化
					Mathf.Sin ((45 * j) * 3.14f / 180) * 0.02f,
					Mathf.Cos ((45 * j) * 3.14f / 180) * 0.02f, 
					0));
			}
		}
		SetAnimator (Key.DOWN);									// 下向きのアニメーションを設定
	}

	//**************************************************************//
	//	関数名　:	Update
	//	機能		:	メインループ・ステージ内をランダムに移動・ターゲットに追従する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Update () {
		if (!Game.stop) {							// ゲーム稼働中
			if (Induction) {						// 誘導されている時
				if (Vector3.Distance (transform.position, target) >= range) {										// ターゲット座標に到達していない時
					float angle = Mathf.Atan2 (target.y - transform.position.y, target.x - transform.position.x);	// ターゲットの方向を算出
					transform.position += new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0) * speed;			// ターゲットの方向に移動
				}
				if (!doOnce) {				// 一度だけ実行
					movePosResult.Clear ();	// 移動量のメモリ開放
					doOnce = true;			// 一度だけ実行を無効にする
				}
			} else {						// 誘導されていない時
				switch (moveStatus) {

				case InArea:											// エリア内移動
					transform.position += movePosResult [randomAngle];	// ランダム移動
					if (!doOnce) {										// 一度だけ実行
						StartCoroutine (Move ());						// 移動時間の制御
						doOnce = true;									// 一度だけ実行を無効にする
					}
					break;

				case Break:												// 休憩時間
					if (!doOnce) {										// 一度だけ実行
						StartCoroutine (BreakTime ());					// 休憩時間の制御
						doOnce = true;
					}
					break;

				case OutsideTheArea:									// エリア外移動
					if (transform.position.x >= 2)						// 右端に到達した時
						transform.position += movePosResult [6];		// 左に移動
					else if (transform.position.x <= -2)				// 左端に到達した時
						transform.position += movePosResult [2];		// 右に移動
					else
						moveStatus = InArea;							// 移動ステータスをエリア内に変更
					break;
				}
			}
		}
	}

	//**************************************************************//
	//	関数名　:	Move
	//	機能		:	ランダム移動をしている時間を制御（コルーチン）
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator Move(){
		yield return new WaitForSeconds (1);	// 1秒間ランダム移動
		moveStatus = Break;						// 移動ステータスを休憩に変更
		doOnce = false;							// 一度だけ実行を有効にする
	}

	//**************************************************************//
	//	関数名　:	BreakTime
	//	機能		:	ランダムに設定した時間が経過した後に、次の行動を設定する
	//				エリア内　ランダムに移動する方向を選択する
	//				エリア外　エリア内に戻るステータスに変更する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator BreakTime(){
		yield return new WaitForSeconds (Random.Range (1, 3));		// ランダムで休憩時間を決める
		randomAngle = (Random.Range (0, 8));						// 次のランダム移動の方向を決める
		moveStatus = InArea;										// 移動ステータスをエリア内移動に変更
		if (3 < transform.position.x || transform.position.x < -3)	// エリア外に出ていたら
			moveStatus = OutsideTheArea;							// 移動ステータスをエリア外移動に変更
		doOnce = false;												// 一度だけ実行を有効にする
	}

	//**************************************************************//
	//	関数名　:	OnCollisionEnter2D
	//	機能		:	敵に当たった時、プレイヤーに	当たった列の番号を伝える
	//	引数		:	Collision2D　other	衝突判定時の情報を受け取る
	//	戻り値	:	なし
	//**************************************************************//
	void OnCollisionEnter2D(Collision2D other){
		if (Induction && !hit) {					// 誘導されている状態かつ、まだ当たっていない時
			GameObject obj = other.gameObject;		// ゲームオブジェクトに変更
			if (obj.tag == "Enemy") {				// 敵に当たった時
				player.Hit(CustomerNumber,obj.GetComponent<LeadControl>());	// プレイヤーに列の何番目が当たったのか知らせる
				hit = true;							// 既に当たったことを知らせる
			}
		}
	}

	//**************************************************************//
	//	関数名　:	SetAnimator
	//	機能		:	再生するアニメーションの設定
	// 				左向きのアニメーションは、右向きのアニメーションを反転させて使い、
	// 				アニメーション画像の削減をしている。そのため、左向きの処理で右向きのアニメーションを再生している
	//	引数		:	byte direc	キャラの向きを受け取る	Up:1  Right:2  Down:4  Left:8
	//	戻り値	:	なし
	//**************************************************************//
	public void SetAnimator(byte direc){
		direction = direc;					// 向きの設定
		switch (direc) {
		case Key.DOWN:						// 下向き
			animator.Play (DOWN [type]);	// 下向きアニメーションを再生
			break;
		case Key.UP:						// 上向き
			animator.Play (UP [type]);		// 上向きアニメーションを再生
			break;
		case Key.RIGHT:						// 右向き
			animator.Play (RIGHT [type]);	// 右向きアニメーションを再生
			spriteRenderer.flipX = false;	// アニメーション画像の反転を無効にする
			break;
		case Key.LEFT:						// 左向き
			animator.Play (RIGHT [type]);	// 右向きアニメーションを再生
			spriteRenderer.flipX = true;	// アニメーション画像の反転を有効にする
			break;
		}
	}

	//**************************************************************//
	//	関数名　:	destroy
	//	機能		:	外部参照用のオブジェクト削除
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void destroy(){
		Destroy (gameObject);	// 自身のオブジェクトを削除する
	}

	//**************************************************************//
	//	関数名　:	GenderDetermination
	//	機能		:	キャラクターのタイプを再設定
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void GenderDetermination (){
		type = (int)Mathf.Floor (Random.Range (0, 3));	// ランダムでキャラクターのタイプを設定する
	}

	//**************************************************************//
	//	関数名　:	Position
	//	機能		:	ポジションを設定する
	//	引数		:	Vector3 value	座標を受け取る
	//	戻り値	:	なし
	//**************************************************************//
	public Vector3 Position{ set { transform.position = value; } }


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}