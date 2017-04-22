using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

	public GameObject player;
	const byte NONE=0,NORMAL=1,HARD=2;
	const byte START=0,STOP=1,EXIT=2;

	public List<Customers> GetList;

	byte difficulty;
	public float speed=1.0f;
	public byte status = START; //敵の行動パターン
	public float timer = 0; //敵の動きを止めておくための変数
	public int direction = 1;
	bool SetDirection = false;
	float randTime;
	byte min = 1,max = 3;
	public int type=0;
	Vector3 position;
	Vector3 pos;

	float delay = 0;
	public float reSpawnTimer = 0;
	bool move = false;

	//Animator
	Animator animator;

	void Awake(){
		animator = gameObject.GetComponent<Animator> ();
	}

	void Start(){
		difficulty = (byte)OnClickButtonSceneManager.difficulty;
		position = transform.position;
		pos.x = transform.position.x;
	}

	void Update () {
		if (!GameStatus.stop) {
			switch (difficulty) {

			case NORMAL:
				if (type == 0) {
					switch (status) {
					case START:
					// ステージ入場
						timer = 0;
						transform.Translate (direction * speed * Time.deltaTime, 0, 0);
					// ステータス変更
						if (direction == 1) {
							if (transform.position.x * direction >= (-1) * direction) {
								randTime = Random.Range (min, max);
								status = STOP;
							}
						} else {
							if (transform.position.x >= (-1) * direction) {
								randTime = Random.Range (min, max);
								status = STOP;
							}
						}
						break;

					case STOP:
					// 一時停止
						timer += Time.deltaTime;
					// ステータス変更
						if (timer >= randTime) {
							status = EXIT;
						}
						break;

					case EXIT:
					// ステージ退場
						transform.Translate (direction * speed * Time.deltaTime, 0, 0);
					// ステータス変更
						if (direction == 1) {
							if ((transform.position.x * direction) >= ((-position.x) * direction)) {
								ScrollEnd ();
								status = START;
							}
						} else {
							if (transform.position.x <= position.x) {
								ScrollEnd ();
								status = START;
							}
						}
						break;
					}
			
				} else if (type == 1) {
					//敵キャラの移動
					transform.Translate (direction * speed * Time.deltaTime, 0, 0);
					if (direction == 1) {
						if ((transform.position.x * direction) >= ((-position.x) * direction)) {
							ScrollEnd ();
						}
					} else {
						if (transform.position.x <= position.x) {
							ScrollEnd ();
						}
					}
				}
				break;
	
			case HARD:
			// 追尾
				position = transform.position;
				transform.position = new Vector3 (
					position.x + (Time.deltaTime * speed) * direction,
					position.y + (player.transform.position.y - position.y) * 0.002f,
					position.z
				);
			// 行動終了
				if (direction == 1) {
					if (transform.position.x >= -pos.x) {
						ScrollEnd ();
					}
				} else {
					if (transform.position.x <= pos.x) {
						ScrollEnd ();
					}
				}
				break;

			default:
			//Debug.Log ("Errer : difficulty");
				break;

			}

			// お客さん誘導
			if (move) {
				for (int i = GetList.Count - 1; i >= 0; i--) {
					if (i != 0) {
						GetList [i].target = GetList [i - 1].target;
					} else {
						GetList [0].target = transform.position;
					}
				}
				move = !move;
			}

			UpdateTarget ();
		}
	}

	/// Function/////////////////////////////////////////////////

	// 画面端まで到着したら
	void ScrollEnd()
	{
		float y = 0;
		if (difficulty == HARD) {
			y = Random.Range (-6, 0);
			position = new Vector3 (position.x, player.transform.position.y, position.z);
		}
		type = Mathf.FloorToInt (Random.Range (0, 2));										// 行動パターン
		direction = (Mathf.FloorToInt (Random.Range (0, 2))) == 0 ? 1 : -1;					// 初期方向
		speed = (Mathf.FloorToInt (Random.Range (1, 2))) + (0.015f * ArrayCharracter.Score);	// スピード
		transform.position = new Vector3 (pos.x * direction, position.y + y, position.z);	// 初期位置
		SetDirection = (direction == 1) ? false : true;										// 
		animator.SetBool ("Direction", SetDirection);										//
		RemoveAllCustomers ();																// お客さんを開放
		reSpawnTimer = Random.Range (0, 3);													// 
		Debug.Log("Watching speed:"+speed);
	}

	// お客さんの奪取
	public void GetCustomers(Customers customer){
		GetList.Add (customer);
		if (type == 0)status = 2;
		speed = 3;
	}

	// 全客さんを開放
	public void RemoveAllCustomers(){
		for (int i = GetList.Count - 1; i >= 0; i--) {
			GetList [i].KillMe ();
			GetList.RemoveAt (i);
		}
	}

	// ターゲット位置の更新
	void UpdateTarget(){
		delay += Time.deltaTime;
		if (delay >= 0.1f) {
			move = !move;
			delay = 0;
		}
	}
		
}