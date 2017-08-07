using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnClickButtonSelect : OnClickButton {

	// 消える
	public GameObject[] ToMakeItTransparent;
	// 表れる
	public GameObject[] ToMakeItOpaque;
	// 選択状態にする
	public bool IChoose = false;
	// ランキング呼んだか？
	public static bool CollRank = false;

	// Use this for initialization
	void Start () {
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
			if (IChoose) {
				gameObject.GetComponent<Button> ().Select ();
			}
		}
		CollRank = false;
	}
	// ランキング
	public void OnClickRank(){
		audioSource.PlayOneShot (audioClip);
		CollRank = true;
		StartCoroutine ("MoveToSelect");
	}
	// スタート
	public void OnClickStart(){
		audioSource.PlayOneShot (audioClip);
		CollRank = false;
		StartCoroutine ("MoveToSelect");
	}
	// ストップ
	public void OnClickStop(){
		if (GameStatus.start) {
			audioSource.PlayOneShot (audioClip);
			GameStatus.stop = true;
			StartCoroutine ("MoveToSelectStop");
		}
	}
	// 再開
	public void OnClickResume(){
		audioSource.PlayOneShot (audioClip);
		StartCoroutine ("MoveToSelectResume");
	}
	// Default
	private IEnumerator MoveToSelect(){
		yield return new WaitForSeconds (audioClip.length); // 遅延
		int i = 0;
		while (ToMakeItTransparent.Length != i) {
			ToMakeItTransparent [i].SetActive (false);
			i++;
		}
		i = 0;
		while (ToMakeItOpaque.Length != i) {
			ToMakeItOpaque [i].SetActive (true);
			i++;
		}
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			ToMakeItOpaque[0].GetComponent<Button> ().Select ();
		}
		gameObject.SetActive (false);
	}
	// PlayStop
	private IEnumerator MoveToSelectStop(){
		int i = 0;
		while (ToMakeItTransparent.Length != i) {
			ToMakeItTransparent [i].SetActive (false);
			i++;
		}
		i = 0;
		while (ToMakeItOpaque.Length != i) {
			ToMakeItOpaque [i].SetActive (true);
			i++;
		}
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			ToMakeItOpaque[0].GetComponent<Button> ().Select ();
		}
		yield return new WaitForSeconds (audioClip.length); // 遅延
		gameObject.SetActive (false);
	}
	// PlayResume
	private IEnumerator MoveToSelectResume(){
		yield return new WaitForSeconds (audioClip.length); // 遅延
		int i = 0;
		while (ToMakeItTransparent.Length != i) {
			ToMakeItTransparent [i].SetActive (false);
			i++;
		}
		i = 0;
		while (ToMakeItOpaque.Length != i) {
			ToMakeItOpaque [i].SetActive (true);
			i++;
		}
		if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {
			ToMakeItOpaque[0].GetComponent<Button> ().Select ();
		}
		GameStatus.stop = false;
		gameObject.SetActive (false);
	}
}