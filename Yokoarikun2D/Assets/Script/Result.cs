using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour {

	static int score = 0;
	public GameObject[] customers;
	public Text ResultText;
	// 集計終わり
	bool totalEnd = false;

	// Audio
	public AudioClip DrumRoll;
	public AudioClip DrumEnd;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		score = 0;
		ResultText.text = "あつめた人数:" + score;
		audioSource = gameObject.GetComponent<AudioSource> ();
		ResultText = GetComponent<Text>();
		StartCoroutine (ScoreUp ());
	}
	
	// Update is called once per frame
	void Update () {
		if (totalEnd) {
			if (Input.GetKeyDown (KeyCode.Space) || Input.anyKeyDown || Input.touchCount > 0) {
				SceneManager.LoadScene ("Rank");
			}
		}
	}

	IEnumerator ScoreUp(){
		score = 0;
		while (score <= ArrayCharracter.Score) {
			if (score >= (ArrayCharracter.Score - 1)) {
				audioSource.Stop ();
				audioSource.PlayOneShot (DrumEnd);
			}
			if (score != 0) {
				ResultText.text = "あつめた人数:" + score;
				Instantiate (customers [GameStatus.GetCustomerCount ()],
					new Vector3 (Random.Range (-3.0f, 3.0f), 
						Random.Range (-4.5f, 0.5f), score), 
					Quaternion.identity);
			}
			score++;
			yield return null;
		}
		totalEnd = true;
	}

}
