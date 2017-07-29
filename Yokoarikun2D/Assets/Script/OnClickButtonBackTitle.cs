using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnClickButtonBackTitle : OnClickButton {

	// Use this for initialization
	void Start () {
		// Select Button
		if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
			gameObject.GetComponent<Button> ().Select ();
		}
		SceneName = sceneName;
	}
}
