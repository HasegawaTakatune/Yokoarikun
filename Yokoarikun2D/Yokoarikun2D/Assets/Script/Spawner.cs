using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	const byte DOWN = 0;
	public float interval = 5;
	public GameObject SpawnChar;
	Customers SpawnCharScript;
	public Vector3 MaxPosition;
	public Vector3 MinPosition;

	// Use this for initialization
	IEnumerator Start () {
		SpawnCharScript = SpawnChar.GetComponent<Customers> ();

		SpawnCharScript.Induction = false;
		while (true) {
			if (!GameStatus.stop) {
				// Set Sprite and Animator
				SpawnCharScript.type = (byte)Mathf.Floor (Random.Range (0, 3));
				// Spawn
				Instantiate (
					SpawnChar,
					new Vector3 ((0 == (Random.Range (0, 2)) ? 5 : -5),Random.Range (MinPosition.y, MaxPosition.y),0),
					transform.rotation);
			}
			// エラーがあったらintervalの時間分待つ
			yield return new WaitForSeconds(interval);
		}
	}
}