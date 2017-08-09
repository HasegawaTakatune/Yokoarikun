using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Const;

public class Customers : MonoBehaviour {

	// Customers Type
	const byte OutsideTheArea=0,InArea=1;
	public byte moveStatus = 0;
	public byte type;

	public int CustomerNumber;
	public ArrayCharracter player;
	Enemy enemy;
	public bool Induction = true;
	bool hit = false;

	public Vector3 target = new Vector3 (0, 6, 0);
	float angle;
	const float speed =0.05f;
	const float range = 0.05f;
	public byte direction = 0;

	public List<Vector3> movePosResult = new List<Vector3> ();
	int Angle = 0;
	float time = 0;

	bool doOnce = false;

	Animator animator;
	static readonly int[] DOWN = new int[] {
		Animator.StringToHash ("CustomersSprite@Down"),
		Animator.StringToHash ("CustomersBoySprite@Down"),
		Animator.StringToHash ("CustomersGirlSprite@Down")
	};
	static readonly int[] UP = new int[] {
		Animator.StringToHash ("CustomersSprite@Up"),
		Animator.StringToHash ("CustomersBoySprite@Up"),
		Animator.StringToHash ("CustomersGirlSprite@Up")
	};
	static readonly int[] RIGHT = new int[] {
		Animator.StringToHash ("CustomersSprite@Right"),
		Animator.StringToHash ("CustomersBoySprite@Right"),
		Animator.StringToHash ("CustomersGirlSprite@Right")
	};
	SpriteRenderer spriteRenderer;

	void Awake(){
		// アニメーター取得
		animator = GetComponent<Animator> ();
	}

	void Start(){
		spriteRenderer = GetComponent<SpriteRenderer> ();
		GenderDetermination ();
		// 移動量初期化
		if (!Induction) {
			for (int j = 0; j < 8; j++) {
				movePosResult.Add (new Vector3 (
					Mathf.Sin ((45 * j) * 3.14f / 180) * 0.02f,
					Mathf.Cos ((45 * j) * 3.14f / 180) * 0.02f, 
					0));
			}
		}
		SetAnimator (Key.DOWN);
	}
	
	// Update is called once per frame
	void Update () {
		if (!Game.stop) {
			if (Induction) {
				//********************************************************************************************************
				//お客さんを数珠繋ぎに誘導する処理部分
				//指定された座標まで移動をする。
				//********************************************************************************************************
				// Move to target
				if (Vector3.Distance (transform.position, target) >= range) {
					angle = Mathf.Atan2 (target.y - transform.position.y, target.x - transform.position.x);
					transform.position += new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0) * speed;
				}
				//********************************************************************************************************
				//********************************************************************************************************
				if (!doOnce) {
					// 移動量の解放
					movePosResult.Clear ();
					doOnce = true;
				}
			} else {
				switch (moveStatus) {

				case InArea:
				// ランダム移動
					time -= Time.deltaTime;
					if (time <= 0) {
						Angle = (Random.Range (0, 8));
						time = Random.Range (1, 3);
						if (3 < transform.position.x || transform.position.x < -3)
							moveStatus = OutsideTheArea;
					}else if (time <= 1) {
						transform.position += movePosResult [Angle];
					}
					break;

				case OutsideTheArea:
					//エリア外行動
					if (transform.position.x >= 2)
						transform.position += movePosResult [6];
					else if (transform.position.x <= -2)
						transform.position += movePosResult [2];
					else
						moveStatus = InArea;
					break;
				}
			}
		}
	}

	void OnCollisionEnter2D(Collision2D other){
		// Hit
		if (Induction && !hit) {
			if (other.gameObject.tag != "Customer" && other.gameObject.tag != "Player" && Induction) {
				enemy = other.gameObject.GetComponent<Enemy> ();
				player.Hit (CustomerNumber, enemy);
				hit = true;
			}
		}
	}

	// change Animetion
	public void SetAnimator(byte direc){
		direction = direc;
		switch (direc) {
		case Key.DOWN:
			animator.Play (DOWN [type]);
			break;
		case Key.UP:
			animator.Play (UP [type]);
			break;
		case Key.RIGHT:
			animator.Play (RIGHT [type]);
			spriteRenderer.flipX = false;
			break;
		case Key.LEFT:
			animator.Play (RIGHT [type]);
			spriteRenderer.flipX = true;
			break;
		}
	}

	public void KillMe(){
		Destroy (gameObject);
	}

	public void GenderDetermination (){
		// 性別決定
		type = (byte)Mathf.Floor (Random.Range (0, 3));
	}

	public Vector3 Position{ set { transform.position = value; } }

}