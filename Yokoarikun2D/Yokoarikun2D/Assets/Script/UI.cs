using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI : MonoBehaviour {

	public Text UITxt;

	// Use this for initialization
	void Start () {
		if (Application.platform == RuntimePlatform.Android) {
			UITxt.text = "タッチ";
		} else {
			UITxt.text = "スペース";
		}
	}
}
