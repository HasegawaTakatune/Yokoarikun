using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Const;

public class Enemy : MonoBehaviour {

	public Transform player;
	const byte START=0,STOP=1,EXIT=2;
	const int min = 1,max = 3;

	public List<Customers> GetList;

	public float speed = 1.0f;
	public byte status = START; //敵の行動パターン
	public int type = 0;
	public int direction = 1;
	Vector3 position;
	bool isItHard;
	float movement;

	// Animator
	SpriteRenderer spriteRenderer;

	void Awake(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Start(){
		player = GameObject.Find ("Player").transform;
		position = transform.position;
		isItHard = Game.IsItHard ();
		movement = direction * speed * Time.deltaTime;
	}

	void Update () {
		if (!Game.stop && Game.start) {
			switch (Game.difficulty) {
			case Game.Difficulty.Normal:
				if (type == 0) {
					switch (status) {
					case START:
					// ステージ入場
						transform.Translate (movement, 0, 0);
					// ステータス変更
						if (transform.position.x >= -1 || transform.position.x >= 1) {
							status = STOP;
							StartCoroutine (Escape ());
						}
						break;

					case STOP:
						break;

					case EXIT:
					// ステージ退場
						transform.Translate (movement, 0, 0);
					// ステータス変更
						if (transform.position.x >= -position.x || transform.position.x <= position.x) {
							ScrollEnd ();
							status = START;
						}
						break;
					}
			
				} else if (type == 1) {
					//敵キャラの移動
					transform.Translate (movement, 0, 0);
					if (transform.position.x >= -position.x || transform.position.x <= position.x) {
						ScrollEnd ();
					}
				}
				break;
	
			case Game.Difficulty.Hard:
			// 追尾
				transform.position += new Vector3 (
					movement,(player.position.y - transform.position.y) * 0.002f,0);
			// 行動終了
				if (transform.position.x >= -position.x || transform.position.x <= position.x) {
					ScrollEnd ();
				}
				break;

			default:
			//Debug.Log ("Errer : difficulty");
				break;

			}
		}
	}

	/// Function/////////////////////////////////////////////////

	// 画面端まで到着したら
	void ScrollEnd(){
		float y = 0;
		if (isItHard) {
			y = Random.Range (-6, 0);
			position = new Vector3 (position.x, player.position.y, position.z);
		}
		type = (Random.Range (0, 2));										// 行動パターン
		direction = (Random.Range (0, 2)) == 0 ? 1 : -1;					// 初期方向
		speed = Random.Range (1, 2) + (0.015f * Game.score);				// スピード　スコアによって速度が上がる
		transform.position = new Vector3 (position.x * direction, position.y + y, position.z);	// 初期位置
		spriteRenderer.flipX = (direction == 1) ? false : true;								// 向きの変換
		RemoveAllCustomers ();																// お客さんを開放
		movement = direction * speed * Time.deltaTime;
	}

	// お客さんの奪取
	public void GetCustomers(Customers customer){
		GetList.Add (customer);
		if (type == 0)status = 2;
		speed = 3;
		// お客さん誘導
		StartCoroutine (UpdateTarget ());
	}

	// 全客さんを開放
	public void RemoveAllCustomers(){
		for (int i = GetList.Count - 1; i >= 0; i--) {
			GetList [i].KillMe ();
			GetList.RemoveAt (i);
		}
	}

	// ターゲット位置の更新
	IEnumerator UpdateTarget(){
		while (true) {
			yield return new WaitForSeconds (0.3f);
			for (int i = GetList.Count - 1; i >= 0; i--) {
				if (i != 0)
					GetList [i].target = GetList [i - 1].target;
				else
					GetList [0].target = transform.position;
			}
		}
	}

	// 逃げる
	IEnumerator Escape(){
		yield return new WaitForSeconds (Random.Range (min, max));
		status = EXIT;
	}
		
}