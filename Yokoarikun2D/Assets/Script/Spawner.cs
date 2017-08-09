using UnityEngine;
using System.Collections;
using Const;

public class Spawner : MonoBehaviour {
	const float interval = 5;
	public GameObject SpawnChar;
	Customers SpawnCharScript;
	public Vector3 MaxPosition;
	public Vector3 MinPosition;

	// Use this for initialization
	IEnumerator Start () {
		SpawnCharScript = SpawnChar.GetComponent<Customers> ();
		SpawnCharScript.Induction = false;
		while (true) {
			if (!Game.stop) {
				// Spawn
				Instantiate (
					SpawnChar,
					new Vector3 ((0 == (Random.Range (0, 2)) ? 5 : -5),Random.Range (MinPosition.y, MaxPosition.y),0),
					transform.rotation);
			}
			// 待ち時間
			yield return new WaitForSeconds(interval);
		}
	}
}