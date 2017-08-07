using UnityEngine;
using System.Collections;

public class ArenaIcon : MonoBehaviour {
	Vector2 pos;
	Vector2 ScPos;
	bool flg = true;
	const float speed = 0.5f;

	// Use this for initialization
	void Start () {
		pos = new Vector2 (transform.position.x, transform.position.y);
		StartCoroutine (Change ());
	}
	
	// Update is called once per frame
	void Update () {

		pos.y += (flg) ? speed : -speed;
		transform.position = pos;

		ScPos = Camera.main.ScreenToWorldPoint (pos);
		if (ScPos.y <= -20)
			gameObject.SetActive (false);
	}
		
	IEnumerator Change(){
		while (true) {
			yield return new WaitForSeconds (1f);
			flg = !flg;
		}
	}
}
