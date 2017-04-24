using UnityEngine;
using System.Collections;

public class ArenaIcon : MonoBehaviour {
	
	struct MYDATA{
		public Vector2 pos;
		public Vector2 ScPos;
		public bool flg;
		public float time;
	}
	MYDATA myData;

	// Use this for initialization
	void Start () {
		myData.pos = new Vector2 (transform.position.x, transform.position.y);
		myData.flg = true;
		myData.time = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (myData.flg)
			myData.pos.y += 0.5f;
		else
			myData.pos.y -= 0.5f;

		transform.position = myData.pos;

		myData.time += Time.deltaTime;
		if (myData.time >= 1) {
			myData.flg = !myData.flg;
			myData.time = 0;
		}
		myData.ScPos = Camera.main.ScreenToWorldPoint (myData.pos);
		if (myData.ScPos.y <= -20)
			gameObject.SetActive (false);
	}
}
