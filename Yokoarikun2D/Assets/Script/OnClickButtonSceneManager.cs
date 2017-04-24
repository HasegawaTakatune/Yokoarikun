using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class OnClickButtonSceneManager : OnClickButton {

	public enum Difficulty : byte {None,Normal,Hard}
	public Difficulty setDifficulty;
	public static Difficulty difficulty;

	public void OnClickSceneManager(){
		if (OnClickButtonSelect.CollRank) {
			SceneName = "Rank";
			ArrayCharracter.Score = 0;
			Rank.FromTitle = true;
		}
		GameStatus.stop = false;
		difficulty = setDifficulty;
		OnClick ();
	}

	// Call Rank Scene
	public void CallRankFromTitle(){
		ArrayCharracter.Score = 0;
		Rank.FromTitle = true;
	}
}
