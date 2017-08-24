using UnityEngine;
using System.Collections;
using Const;

public class Spawner : MonoBehaviour {
	//******************************************************************//
	//	キャラクターの生成を制御	
	//******************************************************************//
	[SerializeField]
	float interval = 5;			// 次の生成までの間隔
	[SerializeField]
	GameObject SpawnCharacter;	// 生成キャラクター
	[SerializeField]
	Vector3 MaxPosition;		// 生成の最大座標
	[SerializeField]
	Vector3 MinPosition;		// 生成の最小座標

	//**************************************************************//
	//	関数名　:	Start
	//	機能		:	ゲーム稼働中、キャラクターの生成をする（コルーチン）
	//	引数		:	なし
	//	戻り値	:	なし
	//**************************************************************//
	IEnumerator Start () {
		while (true) {
			if (!Game.stop) {	// ゲーム稼働中
				Instantiate (	// キャラクターの生成
					SpawnCharacter,
					new Vector3 ((0 == (Random.Range (0, 2)) ? 5 : -5),Random.Range (MinPosition.y, MaxPosition.y),0),
					transform.rotation);
			}
			yield return new WaitForSeconds(interval);	// 次の生成までの間隔をあける
		}
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}