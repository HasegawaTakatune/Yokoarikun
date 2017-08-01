using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class OnClickButtonSceneManager : OnClickButton {

	public GameStatus.Difficulty setDifficulty;

	// Use this for initialization
	void OnEnable () {
		if (OnClickButtonSelect.CollRank) {
			SceneName = "Rank";
			ArrayCharracter.Score = 0;
			Rank.FromTitle = true;
		} else
			SceneName = sceneName;
	}

	public void OnClickSceneManager(){
		GameStatus.stop = false;
		GameStatus.difficulty = setDifficulty;
		OnClick ();
	}

	// Call Rank Scene
	public void CallRankFromTitle(){
		ArrayCharracter.Score = 0;
		Rank.FromTitle = true;
	}
}
