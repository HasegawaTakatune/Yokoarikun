using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Const;

public class LeadControl : MonoBehaviour {
	//******************************************************************//
	//	キャラクター誘導の制御群	
	//
	//	呼び出し関係図
	//	Start　───>UpdateTarget	───>Customers.SetAnimator
	//******************************************************************//
	public GameObject AddItem;					// 生成するオブジェクト
	public List<Customers> myList;				// 誘導をするキャラクターのリスト

	Vector3 tmpTarget = new Vector3 (0, 6, 0);	// 移動先を格納

	byte _direction = 0;						// キャラクターの向きを格納
	public byte direction{ get { return _direction; } set { _direction = value; } }	// 	キャラクターの向きのゲッター・セッター

	[SerializeField]
	AudioSource audioSource;					// オーディオソース格納
	[SerializeField]
	AudioClip audioClip;						// オーディオクリップ格納

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	サウンドの設定・ターゲット座標の更新を始める
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start(){
		if (audioClip != null && audioSource != null)	// オーディオソース・オーディオクリップが設定されていれば
			audioSource.clip = audioClip;				// 再生するサウンドの設定
		StartCoroutine (UpdateTarget ());				// ターゲット座標の更新
	}

	//**************************************************************//
	//	関数名　:	APositionIsInitialized
	//	機能		:	ターゲット座標の初期化
	//	引数		:	Vector3 position	初期座標を受け取る
	//	戻り値	:	なし
	//**************************************************************//
	public void APositionIsInitialized(Vector3 position){
		int index = 0;	// 指数
		while (index < myList.Count) {							// 要素数の分だけループ
			myList [index].GenderDetermination ();				// キャラクターの種類を再設定
			myList [index].target = position;					// ターゲット座標を指定座標に設定
			myList [index].Position = new Vector3 (0, 6, 0);	// 座標を初期化
			index++;											// 指数移動
		}
	}

	//**************************************************************//
	//	関数名　:	AListIsInitialized
	//	機能		:	誘導するキャラクターを指定された数確保する
	//	引数		:	int num		確保する数を指定する
	//	戻り値	:	なし
	//**************************************************************//
	public void AListIsInitialized(int num){
		while (myList.Count < num) 	// 指定された数より少なかったら、生成 & リストに追加をする
			GetCustomers (Instantiate (AddItem, new Vector3 (0, 6, 0), transform.rotation).GetComponent<Customers>());
		while (myList.Count > num)	// 指定された数より多かったら、余分を削除する
			DeleteCustomers ();
	}

	//**************************************************************//
	//	関数名　:	AddScore
	//	機能		:	誘導している数分スコアに加える
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void AddScore(){
		Game.score += myList.Count;								// スコアの加算
		int index = 0;											// 指数
		while (index < myList.Count) {							// 要素数の分だけループ
			GameStatus.AddCustomerCount (myList [index].type);	// キャラクターのタイプ別で獲得数を格納
			index++;											// 指数移動
		}
	}

	//**************************************************************//
	//	関数名　:	GetCustomers
	//	機能		:	誘導するキャラクターを追加する
	//	引数		:	Customers obj	誘導するキャラクターを受け取る
	//	戻り値	:	なし
	//**************************************************************//
	public void GetCustomers(Customers obj){
		myList.Add(obj);													// 引数のキャラクターをリストに追加
		int index = myList.Count - 1;										// 追加したキャラクターを指定
		myList [index].Induction = true;									// 獲得したことを表すフラグ
		myList [index].player = gameObject.GetComponent<LeadControl> ();	// 誘導してもらう対象を登録
		myList [index].CustomerNumber = myList.Count;						// 前から何番目かを指定する
	}

	//**************************************************************//
	//	関数名　:	DeleteCustomers
	//	機能		:	誘導している最後尾のキャラクターを削除する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void DeleteCustomers(){
		int index = myList.Count - 1;	// 最後尾を指定
		myList [index].destroy ();		// オブジェクトの削除
		myList.RemoveAt (index);		// リストから削除
	}

	//**************************************************************//
	//	関数名　:	RemoveAllCustomers
	//	機能		:	誘導している全キャラクターを削除する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	public void RemoveAllCustomers(){
		for (int i = myList.Count - 1; i >= 0; i--) {	// 要素数の分だけループ
			myList [i].destroy ();						// オブジェクトの削除
			myList.RemoveAt (i);						// リストから削除
		}
	}

	//**************************************************************//
	//	関数名　:	Hit
	//	機能		:	横取りされるキャラクターを先頭にして、その後ろのすべてのキャラクターを横取りされる
	//	引数		:	int number			横取りされる先頭の番号を受け取る
	//				LeadControl obj		横取りしてきた相手のLeadControlコンポーネントを受け取る
	//	戻り値	:	なし
	//**************************************************************//
	public void Hit(int number, LeadControl obj){
		ExecuteEvents.Execute<RecieveInterface>(target:gameObject,eventData:null,functor:(reciever,eventData)=>reciever.ISnatched());	// 横取りイベントを呼ぶ
		int index = myList.Count - 1;					// 最後尾の参照
		while (index >= number) {						// 横取りされる先頭の要素数までループ
			obj.GetCustomers (myList [index]);			// 横取りされる相手に、キャラクターを獲得させる
			myList.RemoveAt (index);					// 横取りされたキャラクターをリストから削除
			index--;									// 次のキャラクターを参照
		}
		if (audioClip != null && audioSource != null)	// オーディオソース・オーディオクリップが設定されていれば
			audioSource.PlayOneShot (audioClip);		// SEを再生する
	}

	//**************************************************************//
	//	関数名　:	OnCollisionEnter2D
	//	機能		:	当たり判定（当たった相手によって処理を変える）
	//				Customer	誘導するキャラクターとして追加をする
	//				Enemy		横取りをされる
	//	引数		:	Collision2D other	衝突判定時の情報を受け取る
	//	戻り値	:	なし
	//**************************************************************//
	void OnCollisionEnter2D(Collision2D other){
		if (gameObject.tag != "Enemy") {						// 敵の場合処理をしない
			GameObject obj = other.gameObject;					// ゲームオブジェクトに変換をする
			if (obj.tag == "Customer" && obj.GetComponent<Customers> ().Induction == false) {	// 当たったのがお客さんで、誘導をしていなかったら
				GetCustomers (obj.GetComponent<Customers> ());	// 誘導するリストに追加をする
			} else if (obj.tag == "Enemy") {					// 当たったのが敵だったら
				Hit (1, obj.GetComponent<LeadControl> ());		// キャラクターを横取りされる
			}
		}
	}

	//**************************************************************//
	//	関数名　:	UpdateTarget
	//	機能		:	キャラクターが移動するターゲット座標を更新する
	//				数珠繋ぎに移動をさせるために、更新前の座標を受け取るようにする。
	//				そのため、最後尾からターゲット座標の更新をする
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator UpdateTarget(){
		while (true) {
			yield return new WaitForSeconds (0.3f);								// キャラクター同士の距離を置く
			if (myList.Count > 0 && tmpTarget != transform.position) {			// リストに一人以上登録されていて、プレイヤーが移動した時
				for (int index = myList.Count - 1; index > 0; index--) {		// 最後尾から先頭の一つ手前までループ
					myList [index].SetAnimator (myList [index - 1].direction);	// アニメーションの向きを設定
					myList [index].target = myList [index - 1].target;			// ターゲット座標の更新
				}
				tmpTarget = transform.position;									// プレイヤーの座標を格納
				myList [0].SetAnimator (_direction);							// 先頭キャラクターのアニメーションする向きを設定
				myList [0].target = tmpTarget;									// 先頭キャラクターのターゲット座標を更新
			}
		}
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}
