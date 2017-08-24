using UnityEngine;
using System.Collections;

public class ScrollCamera : MonoBehaviour {
	//******************************************************************//
	//				スコアの結果発表（カウントアップ）制御				//
	//******************************************************************//
	[SerializeField]
	Transform Player;							// プレイヤーの座標
	Vector3 move;								// 移動先を格納する
	[SerializeField]
	float width_Right,width_Left,height_Top,height_Bottom;	// 末端の位置

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	奥行座標の初期化
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Start () {
		move.z = transform.position.z;			// 移動先の奥行を初期化
	}

	//**************************************************************//
	//	関数名　:	Update
	//	機能		:	プレイヤーを中心としてカメラを移動する
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	void Update () {
		Vector3 nowPosition = Player.position;	// プレイヤー座標を取得

		// カメラ位置更新
		if (height_Top >= nowPosition.y && height_Bottom <= nowPosition.y)	// 上下の末端でない限り
			move.y = nowPosition.y;				// y座標を更新
		if (width_Left <= nowPosition.x && width_Right >= nowPosition.x)	// 左右の末端でない限り
			move.x = nowPosition.x;				// x座標を更新 
		transform.position = move;				// 座標の代入
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}
