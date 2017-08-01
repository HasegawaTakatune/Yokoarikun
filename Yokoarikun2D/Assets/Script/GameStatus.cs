using UnityEngine;
using System.Collections;

public class GameStatus : MonoBehaviour {

	static public bool stop;

	// ゲーム難易度
	public enum Difficulty : byte {None,Normal,Hard}
	static Difficulty _difficulty;
	public static Difficulty difficulty{ get { return _difficulty; } set { _difficulty = value; } }

	//******************************************************************//
	//			各種お客さんごとの獲得数の保存と吐き出し					//
	//******************************************************************//
	public const int 
	BOY1=0,
	BOY2=1,
	GIRL=2;
	static float customerCount = 000000;//080510; // 獲得数

	/// 各種お客さんの取得数を格納
	/// BOY1 000000～000099
	/// BOY2 000000～009900
	/// GIRL 000000～990000
	public static void AddCustomerCount(int type){
		if (type == BOY1) {
			customerCount += 1;
		} else if (type == BOY2) {
			customerCount += 100;
		} else {// GIRL
			customerCount += 10000;
		}
	}

	/// 各種お客さんの取得数を渡す
	/// BOY1 000000～000099
	/// BOY2 000000～009900
	/// GIRL 000000～990000
	public static int GetCustomerCount(int type){
		if (type == BOY1) {
			return (int)(customerCount % 100);
		} else if (type == BOY2) {
			return (int)(((customerCount%10000)*0.01));
		} else {// GIRL
			return (int)(customerCount*0.0001);
		}
	}

	public static int GetCustomerCount(){
		if (0 < (int)(customerCount % 100)) {
			customerCount -= 1;
			return BOY1;
		} else if (0 < (int)(((customerCount%10000)*0.01))) {
			customerCount -= 100;
			return BOY2;
		} else if(0 < ((int)(customerCount*0.0001))){// GIRL
			customerCount -= 10000;
			return GIRL;
		}
		Debug.Log ("Error : GetCustomerCount");
		return BOY1;
	}
	//******************************************************************//
	//******************************************************************//
	//******************************************************************//

}
