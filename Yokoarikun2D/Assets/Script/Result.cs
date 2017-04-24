using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour {

	static int score = 0;
	public GameObject[] SpwawnCust;
	public Text ResultText;
	public string RankScene = "Rank";
	bool buttonflg = false;
	Quaternion quaternion = Quaternion.identity;

	// Audio
	public AudioClip DrumRoll;
	public AudioClip DrumEnd;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		audioSource = gameObject.GetComponent<AudioSource> ();
		ResultText = GetComponent<Text>();
		StartCoroutine("ScoreUp");
		score = 0;
		ResultText.text = "あつめた人数:" + score;
	}
	
	// Update is called once per frame
	void Update () {
		if (buttonflg == true) {
			if (Input.GetKeyDown (KeyCode.Space) || Input.anyKeyDown || Input.touchCount > 0) {
				SceneManager.LoadScene (RankScene);
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
				Instantiate (SpwawnCust [GameStatus.GetCustomerCount()],//[Mathf.FloorToInt (Random.Range (0, 3))],
					new Vector3 (Random.Range (-3.0f, 3.0f), 
						Random.Range (-4.5f, 0.5f), -1.0f), 
					quaternion);
			}
			score++;
			yield return null;
		}
		buttonflg = true;
	}

}
