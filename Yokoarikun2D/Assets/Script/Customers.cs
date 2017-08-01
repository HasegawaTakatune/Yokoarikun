using UnityEngine;
using System.Collections;
//using System;

public class Customers : MonoBehaviour {

	// Customers Type
	const byte Boy=0,Boy2=1,Girl=2;
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
	float speed =0.05f;
	float range = 0.05f;
	public int direction = 0;
	const int Down = 0, Up = 1, Right = 2, Left = 3;

	Vector3[] movePosiResult = new Vector3[8];
	int Angle = 0;
	float time = 0;

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
		int i = 0;
		while (movePosiResult.Length != i) {
			movePosiResult [i] = new Vector3 (Mathf.Sin ((transform.localEulerAngles.y + 45 * i) * 3.14f / 180) * 0.02f, 
				Mathf.Cos ((transform.localEulerAngles.y + 45 * i) * 3.14f / 180) * 0.02f, 0);
			i++;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameStatus.stop) {
			if (Induction) {
				//********************************************************************************************************
				//お客さんを数珠繋ぎに誘導する処理部分
				//指定された座標まで移動をする。
				//********************************************************************************************************
				// Move to target
				angle = Mathf.Atan2 (target.y - transform.position.y, target.x - transform.position.x);
				if (Vector3.Distance (transform.position, target) >= range) {
					transform.position += new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0) * speed;
				}
				//********************************************************************************************************
				//********************************************************************************************************
			} else {
				switch(moveStatus){

				case InArea:
				// ランダム移動
					time -= Time.deltaTime;
					if (time <= 1) {
					SetAnimator (0);
					transform.position += movePosiResult [Angle];
					} 
					if (time <= 0) {
						Angle = (Random.Range (0, 8));
						time = Random.Range (1, 3);
						if (3 < transform.position.x || transform.position.x < -3)
							moveStatus = OutsideTheArea;
					}
					break;

				case OutsideTheArea:
					//エリア外行動
					if (transform.position.x >= 2)
						transform.position += movePosiResult [6];
					else if (transform.position.x <= -2)
						transform.position += movePosiResult [2];
					else
						moveStatus = InArea;
					
					SetAnimator (0);
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
	public void SetAnimator(int direc){
		direction = direc;
		switch (direc) {
		case Down:
			animator.Play (DOWN [type]);
			break;
		case Up:
			animator.Play (UP [type]);
			break;
		case Right:
			animator.Play (RIGHT [type]);
			spriteRenderer.flipX = false;
			break;
		case Left:
			animator.Play (RIGHT [type]);
			spriteRenderer.flipX = true;
			break;
		}
	}

	public void KillMe(){
		Destroy (gameObject);
	}

	// 性別決定
	public void GenderDetermination(){
		type = (byte)Mathf.Floor (Random.Range (0, 3));
	}

	public void SetPosition(Vector3 input){
		transform.position = input;
	}

}