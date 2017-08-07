using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Const;

public class ArrayCharracter : MonoBehaviour {
	// Use platform
	byte platform = 0;

	// touch
	Vector3 touchPosition;
	Vector3 nowPosition;

	float addPos = 0.1f;

	float RightFrame = 4;
	float LeftFrame = -4;

	const int NORMAL=0,BOXER=1,GUITAR=2;

	int moveDirecResult = 0;
	Vector3[] movePosiResult = new Vector3[8];

	public GameObject StartPosition;
	public GameObject EndPosition;
	//
	bool Create=false;
	public GameObject AddItem;
	public List<Customers> myScriptList;
	public int CustomersNum;
	int maxCustomersNum;
	int myListNum;

	Vector3 tmpTarget = new Vector3 (0, 6, 0);
	float timer=0;
	// Audio
	public AudioClip audioClip;
	AudioSource audioSource;

	Vector3 startPos,endPos;
	// Score
	public static int Score = 0;
	bool AddScore  = false;

	// Animator
	Animator animator;
	byte playerDirection = Key.DOWN;
	static readonly int[] Up = new int[] { 
		Animator.StringToHash ("PlayerSprite@Up"),
		Animator.StringToHash ("PlayerBoxerSprite@Up"),
		Animator.StringToHash ("PlayerGuitarSprite@Up")
	};
	static readonly int[] Down = new int[] { 
		Animator.StringToHash ("PlayerSprite@Down"),
		Animator.StringToHash ("PlayerBoxerSprite@Down"),
		Animator.StringToHash ("PlayerGuitarSprite@Down")
	};
	static readonly int[] Left = new int[] { 
		Animator.StringToHash ("PlayerSprite@Left"),
		Animator.StringToHash ("PlayerBoxerSprite@Left"),
		Animator.StringToHash ("PlayerGuitarSprite@Left")
	};
	SpriteRenderer spriteRenderer;
	byte type = NORMAL;
	byte direc = 0;

	public const float speed = 0.05f;
	float delay=0;
	float topDelay=0;
	bool move = true;

	void Awake(){
		animator = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {

		// 今使っているプラットホーム
		if (Application.platform == RuntimePlatform.WindowsEditor) {
			platform = Platform.UnityEditor;
		} else if (Application.platform == RuntimePlatform.WindowsPlayer) {
			platform = Platform.Windows;
		} else if (Application.platform == RuntimePlatform.Android) {
			platform = Platform.Android;
		} else {
			platform = Platform.None;
		}
		// Audio
		audioSource = gameObject.GetComponent<AudioSource> ();
		audioSource.clip = audioClip;
		//
		Score = 0;
		CustomersNum = myListNum = maxCustomersNum = myScriptList.Count;
		startPos = StartPosition.transform.position;
		endPos = EndPosition.transform.position;

		int i = 0;
		while (movePosiResult.Length != i) {
			movePosiResult [i] = new Vector3 (Mathf.Sin ((transform.localEulerAngles.y + 45 * i) * 3.14f / 180) * speed * (720 / Screen.width), 
				Mathf.Cos ((transform.localEulerAngles.y + 45 * i) * 3.14f / 180) * speed * (1280 / Screen.height), 0);
			i++;
		}
		// SpriteRenderer
		spriteRenderer = GetComponent<SpriteRenderer>();

		StartCoroutine (ManualAnimation ());
	}

	// Update is called once per frame
	void Update () {
		if (!GameStatus.stop) {
			nowPosition = gameObject.transform.position;

			// 移動
			if (GameStatus.start && !AddScore) {
				if (platform == Platform.Android){
					// Android
					// タッチした座標
					if (Input.touchCount > 0) {
						foreach (Touch t in Input.touches) {
							if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
								touchPosition = Camera.main.ScreenToWorldPoint (t.position);
							else
								touchPosition = nowPosition;			
						}
					} else
						touchPosition = nowPosition;

					if ((nowPosition.y) < (touchPosition.y - addPos)) {
						if (nowPosition.y < startPos.y - 1.1f) {
							moveDirecResult += Key.UP;
							direc = Key.UP;
							UpdateTarget ();
						}
						playerDirection = Key.UP;
					}
					if ((nowPosition.y) > (touchPosition.y + addPos)) {
						moveDirecResult += Key.DOWN;
						playerDirection = Key.DOWN;
						direc = Key.DOWN;
						UpdateTarget ();
					}
					if ((nowPosition.x) > (touchPosition.x + addPos)) {
						if (nowPosition.x > LeftFrame) {// 移動制限 
							moveDirecResult += Key.LEFT;
							playerDirection = Key.LEFT;
							direc = Key.LEFT;
							UpdateTarget ();
						}
					}
					if ((nowPosition.x) < (touchPosition.x - addPos)) {
						if (nowPosition.x < RightFrame) {
							moveDirecResult += Key.RIGHT;
							playerDirection = Key.RIGHT;
							direc = Key.RIGHT;
							UpdateTarget ();
						}
					}
				} else if (platform == Platform.UnityEditor || platform == Platform.Windows){
					// Windows:UnityEditor
					if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.UpArrow)) {
						if (nowPosition.y < startPos.y - 1.1f) {
							moveDirecResult += Key.UP;
							direc = Key.UP;
							UpdateTarget ();
						}
						playerDirection = Key.UP;
					}
					if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
						moveDirecResult += Key.DOWN;
						playerDirection = Key.DOWN;
						direc = Key.DOWN;
						UpdateTarget ();
					}
					if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow)) {
						if (nowPosition.x > LeftFrame) {// 移動制限 
							moveDirecResult += Key.LEFT;
							direc = Key.LEFT;
							UpdateTarget ();
						}
						playerDirection = Key.LEFT;
					}
					if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
						if (nowPosition.x < RightFrame) {
							moveDirecResult += Key.RIGHT;
							direc = Key.RIGHT;
							UpdateTarget ();
						}
						playerDirection = Key.RIGHT;
					}
				}

				// アニメーション設定
				changeAnimation (type, direc);//playerDirection);
				selectMoveDirection (moveDirecResult);
				moveDirecResult = 0;
			}
			//********************************************************************************************************
			//お客さんを数珠繋ぎに誘導する処理部分
			//先頭のお客さんから順に、どこまで移動するかの座標を指定してあげる
			//********************************************************************************************************
			// プレイヤに追尾
			for (int i = CustomersNum - 1; i >= 0; i--) {
				// ターゲット座標更新
				if (move) {
					if (i != 0) {
						// 自分の前にいるキャラのステータスを参照する
						//int Direction = myScriptList [i - 1].direction;
						myScriptList [i].SetAnimator (myScriptList [i - 1].direction);//(Direction);
						myScriptList [i].target = myScriptList [i - 1].target;
					}
				}
			}
			// 最前列にいるお客さんは、プレイヤーのステータスを参照する
			if (topDelay >= 0.1f) {
				myScriptList [0].SetAnimator (direc);
				myScriptList [0].target = tmpTarget;
				tmpTarget = nowPosition;
				topDelay = 0;
			}
			move = false;
			//********************************************************************************************************
			//********************************************************************************************************

			// 到着後の処理
			if (endPos.y >= nowPosition.y) {
				// Score
				if (!AddScore) {
					Score += myScriptList.Count;
					AddScore = true;
					// 各お客さんごとの取得数を格納
					int j = 0;
					while (j < CustomersNum) {
						GameStatus.AddCustomerCount (myScriptList [j].type);
						j++;
					}
				}

				// 自動ゴール
				{
					float angle;
					angle = Mathf.Atan2 (endPos.y - nowPosition.y, endPos.x - nowPosition.x);
					if (Vector3.Distance (nowPosition, endPos) >= 0.5f) {
						transform.position += new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0) * speed;
					}
				}

				timer += Time.deltaTime;
				if (timer >= 1) {
					transform.position = startPos;
					Alignment ();
					timer = 0;
				}
				move = true;
				Create = true;
				touchPosition = new Vector3 (nowPosition.x, -10, 0);
				// Set direction
				playerDirection = Key.DOWN;
				direc = Key.DOWN;
				UpdateTarget ();
			}

			// ステージ入場
			if (startPos.y - 1 <= nowPosition.y) {
				// お客さんの追加
				if (Create) {
					int i = 0;
					int createNum = (maxCustomersNum ) - CustomersNum;
					// 生成 & リストに追加
					while (i < createNum) {
						GetCustomers ((GameObject)Instantiate (AddItem, new Vector3 (0, 6, 0), transform.rotation));
						i++;
					}
					// 余分を削除
					while (i > createNum) {
						DeleteCustomers ();
						i--;
					}
					// 位置の初期化
					int j = 0;
					while (j < CustomersNum) {
						myScriptList [j].GenderDetermination ();
						myScriptList [j].target = new Vector3 (0, 6, 0);
						myScriptList [j].SetPosition (nowPosition);
						j++;
					}
					Create = false;
				}
				transform.position += movePosiResult [4];

				AddScore = false;
				touchPosition = new Vector3 (nowPosition.x, -10, 0);
				// Position
				for (int i = CustomersNum - 1; i >= 0; i--) {
					// ターゲット座標更新
					if (i != 0) {
						//byte Direction = myScriptList [i - 1].direction;
						myScriptList [i].SetAnimator (myScriptList [i - 1].direction);//Direction);
						myScriptList [i].target = myScriptList [i - 1].target;
					}
				}
				myScriptList [0].SetAnimator (direc);
				myScriptList [0].target = tmpTarget;
				tmpTarget = nowPosition;
				// Set direction
				playerDirection = Key.DOWN;
				direc = Key.DOWN;
				UpdateTarget ();
			}
		}
	}


	// Function/////////////////////////////////////////////////////////////////// 

	// お客さんがはぐれてしまう
	public void Hit(int Number, Enemy enemy){
		int i = myListNum - 1;
		// 迷子スイッチ
		while (i >= Number) {
			enemy.GetCustomers (myScriptList [i]);
			myScriptList.RemoveAt (i);
			i--;
		}
		audioSource.PlayOneShot (audioClip);
		CustomersNum = myScriptList.Count;
		myListNum = CustomersNum;
	}

	// お客さんを再整列させる
	public void Alignment(){
		myListNum = CustomersNum;
		//ヨコアリくんのタイプ変更
		switch (type) {
		case NORMAL:type = BOXER;break;
		case BOXER:type = GUITAR;break;
		case GUITAR:type = NORMAL;break;
		}
		animator.Play (Down [type]);
	}

	// Hit
	void OnCollisionEnter2D(Collision2D other){
		GameObject obj = other.gameObject;
		if (obj.tag == "Customer" && obj.GetComponent<Customers> ().Induction == false) {
			GetCustomers (obj);
		} else if (obj.tag == "Enemy") {
			Enemy enemy = obj.GetComponent<Enemy> ();
			Hit (1, enemy);
		}
	}

	// ターゲット位置の更新
	void UpdateTarget(){
		float time = Time.deltaTime;
		delay += time;
		topDelay += time;
		if (delay >= 0.3f) {
			move = true;
			delay = 0;
		}
	}

	// アニメーション変更
	void changeAnimation(int type,byte direction){
		switch (direction) {
		case Key.DOWN:
			animator.Play (Down [type]);
			break;

		case Key.UP:
			animator.Play (Up [type]);
			break;

		case Key.LEFT:
			animator.Play (Left [type]);
			spriteRenderer.flipX = false;
			break;

		case Key.RIGHT:
			animator.Play (Left [type]);
			spriteRenderer.flipX = true;
			break;

		default:
			//Debug.Log ("Errer");
			break;
		}
	}

	IEnumerator ManualAnimation(){
		while(true){
			yield return new WaitForSeconds (.3f);
			if (playerDirection == Key.UP || playerDirection == Key.DOWN)
				spriteRenderer.flipX = !spriteRenderer.flipX;
		}
	}

	// お客さんの追加
	void GetCustomers(GameObject obj){
		myScriptList.Add(obj.GetComponent<Customers>());
		myScriptList [CustomersNum].Induction = true;
		myScriptList [CustomersNum].player = gameObject.GetComponent<ArrayCharracter> ();
		CustomersNum = myScriptList.Count;
		myListNum = CustomersNum;
		myScriptList [CustomersNum - 1].CustomerNumber = myListNum;
	}

	// お客さんの削除
	void DeleteCustomers(){
		myScriptList [CustomersNum - 1].KillMe ();
		myScriptList.RemoveAt (CustomersNum - 1);
		CustomersNum = myScriptList.Count;
		myListNum = CustomersNum;
	}

	// 移動関数(switch)
	void selectMoveDirection(int direction){
		Vector3 move = Vector3.zero;
		switch (direction) {
		case Key.UP:
			move += movePosiResult [0];
			break;

		case Key.UPRIGHT:
			move += movePosiResult [1];
			break;

		case Key.RIGHT:
			move += movePosiResult [2];
			break;

		case Key.DOWNRIGHT:
			move += movePosiResult [3];
			break;

		case Key.DOWN:
			move += movePosiResult [4];
			break;

		case Key.DOWNLEFT:
			move += movePosiResult [5];
			break;

		case Key.LEFT:
			move += movePosiResult [6];
			break;
		
		case Key.UPLEFT:
			move += movePosiResult [7];
			break;

		case 0:
		default:
			move = Vector3.zero;
			break;
		}
		transform.position += move;

	}
}
