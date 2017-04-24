/************************************************
各OnClickButtonクラスの大本
オーディオの制御
シーン移動制御を受け持つ

************************************************/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnClickButton : MonoBehaviour {
	public string SceneName;
	// Audio
	public AudioClip audioClip;
	public AudioSource audioSource;

	// Use this for initialization
	void Start () {
		// Audio
		audioSource = GetComponent<AudioSource> ();
		audioSource.clip = audioClip;
	}
	
	// Button Click
	public void OnClick(){
		//audioSource.PlayOneShot (audioClip);
		StartCoroutine ("MoveToScene");
	}

	private IEnumerator MoveToScene(){
		yield return new WaitForSeconds (audioClip.length);
		SceneManager.LoadScene (SceneName);
	}
}
