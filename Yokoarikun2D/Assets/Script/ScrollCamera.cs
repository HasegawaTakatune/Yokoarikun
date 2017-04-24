using UnityEngine;
using System.Collections;

public class ScrollCamera : MonoBehaviour {

	public GameObject player;
	Vector3 playerPosi,position;
	float posiZ;

	public float width_Right,width_Left,height_Top,height_Bottom;

	// Use this for initialization
	void Start () {
		position.z = transform.position.z;
	}
	
	// Update is called once per frame
	void Update () {
		playerPosi = player.transform.position;

		// カメラ位置更新
		if (height_Top >= playerPosi.y && height_Bottom <= playerPosi.y) {
			position.y = playerPosi.y;
		}
		if (width_Left <= playerPosi.x && width_Right >= playerPosi.x) {
			position.x = playerPosi.x;
		}
		transform.position = position;
	}
}
