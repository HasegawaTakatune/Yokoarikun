using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Const;

public class OnClickButtonSceneManager : OnClickButton {

	public Game.Difficulty setDifficulty;

	// Use this for initialization
	void OnEnable () {
		if (OnClickButtonSelect.CollRank) {
			SceneName = "Rank";
			Game.score = 0;
			Rank.FromTitle = true;
		} else
			SceneName = sceneName;
	}

	public void OnClickSceneManager(){
		Game.stop = false;
		Game.difficulty = setDifficulty;
		OnClick ();
	}
}
