using UnityEngine;
using System.Collections;

public class ArenaIcon : MonoBehaviour {
	
		public Vector2 pos;
		public Vector2 ScPos;
		public bool flg;
		public float time;

	// Use this for initialization
	void Start () {
		pos = new Vector2 (transform.position.x, transform.position.y);
		flg = true;
		time = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (flg)
			pos.y += 0.5f;
		else
			pos.y -= 0.5f;

		transform.position = pos;

		time += Time.deltaTime;
		if (time >= 1) {
			flg = !flg;
			time = 0;
		}
		ScPos = Camera.main.ScreenToWorldPoint (pos);
		if (ScPos.y <= -20)
			gameObject.SetActive (false);
	}
}
