﻿using UnityEngine;
using System.Collections;

public class GameStatus : MonoBehaviour {
	//******************************************************************//
	//	各種お客さんごとの獲得数の保存と吐き出し	
	//******************************************************************//
	public const int 
	Boy_01 	= 0,	// 男の子_01
	Boy_02 	= 1,	// 男の子_02
	Girl_01 = 2;	// 女の子_01
	static float customerCount = 000000;// 獲得数

	//**************************************************************//
	//	関数名　:	AddCustomerCount
	//	機能		:	各種お客さんの取得数を格納
	//				 Boy_01 	000000～000099
	//				 Boy_02		000000～009900
	//				 Girl_01 	000000～990000
	//	引数		:	int type	お客さんの種類	Boy_01:0	Boy_02:1	Girl_01:2
	//	戻り値	:	なし
	//**************************************************************//
	public static void AddCustomerCount(int type){
		if (type == Boy_01)			customerCount += 1;		// Boy_01に加算
		else if (type == Boy_02)	customerCount += 100;	// Boy_02に加算
		else 						customerCount += 10000;	// Girl_01に加算
		
	}

	//**************************************************************//
	//	関数名　:	GetCustomerCount
	//	機能		:	各種お客さんの取得数を渡す(指定したタイプを取得)
	//				 Boy_01 	000000～000099
	//				 Boy_02		000000～009900
	//				 Girl_01 	000000～990000
	//	引数		:	int type	お客さんの種類	Boy_01:0	Boy_02:1	Girl_01:2
	//	戻り値	:	選択した種類の総数を返す
	//**************************************************************//
	public static int GetCustomerCount(int type){
		if (type == Boy_01) 		return (int)(customerCount % 100);					// Boy_01の総数を取得
		else if (type == Boy_02) 	return (int)(((customerCount % 10000) * 0.01));		// Boy_02の総数を取得
		else 						return (int)(customerCount * 0.0001);				// Girl_01の総数を取得
	}

	//**************************************************************//
	//	関数名　:	GetCustomerCount
	//	機能		:	各種お客さんの取得数を渡す(Boy_01 → Boy_02 → Girl_01の順に取得して減算をする)
	//				 Boy_01 	000000～000099
	//				 Boy_02		000000～009900
	//				 Girl_01 	000000～990000
	//	引数		:	なし
	//	戻り値	:	保持されているお客さんの種類を返す		Boy_01:0	Boy_02:1	Girl_01:2
	//**************************************************************//
	public static int GetCustomerCount(){
		if (0 < (int)(customerCount % 100)) {
			customerCount -= 1;			// Boy_01の総数を減算
			return Boy_01;				// Boy_01を返す
		} else if (0 < (int)(((customerCount % 10000) * 0.01))) {
			customerCount -= 100;		// Boy_02の総数を減算
			return Boy_02;				// Boy_02を返す
		} else if(0 < ((int)(customerCount * 0.0001))){
			customerCount -= 10000;		// Girl_01の総数を減算
			return Girl_01;				// Girl_01を返す
		}
		Debug.Log ("Error : GetCustomerCount");	// この関数でエラーが起きた
		return Boy_01;							// 処理を続行させるためのダミー
	}
	//******************************************************************//
	//								End of class						//
	//******************************************************************//

}

//******************************************************************//
//	ゲーム制御定数群	
//******************************************************************//
namespace Const{
	
	//キーボード入力のステータス	
	public static class Key{
		public const byte 
		UP = 1,				// ↑
		RIGHT = 2,			// →
		DOWN = 4,			// ↓
		LEFT = 8,			// ←
		UPRIGHT = 3,		// ↗
		DOWNRIGHT = 6,		// ↘
		UPLEFT = 9,			// ↖
		DOWNLEFT = 12;		// ↙
	}
		
	//ゲームステータス
	public static class Game{
		public static bool stop;	// ストップ（true : 停止  false : 稼働）

		static bool _start = false;	// ゲームスタート　（true : 開始  false : 停止）
		public static bool start{ get { return _start; } set { _start = value; } }

		static int _score = 0;		// ゲームスコア
		public static int score{ get { return _score; } set { _score = value; } }

		// ゲーム難易度
		public enum Difficulty : byte {None,Normal,Hard}
		static Difficulty _difficulty;
		public static Difficulty difficulty{ get { return _difficulty; } set { _difficulty = value; } }
		public static bool IsItHard(){return _difficulty == Difficulty.Hard;}	// 難易度を調べる　（true : ハード   false : ノーマル）
	}
}
//******************************************************************//
//							End of namespace						//
//******************************************************************//

//******************************************************************//
//　画面サイズの設定													//
//******************************************************************//
public class GameInitial{
	[RuntimeInitializeOnLoadMethod]
	static void OnRuntimeMethodLoad(){
		// スクリーンサイズ、フルスクリーン、フレームレートの設定
		Screen.SetResolution (720, 1028, false,60);
		Screen.autorotateToLandscapeLeft = false;	// 画面を左に自動回転させるのを無効化
		Screen.autorotateToLandscapeRight = false;	// 画面を右に自動回転させるのを無効化
		Screen.autorotateToPortrait = true;			// 画面を上に自動回転させるのを有効化
	}
}
//******************************************************************//
//							End of class 							//
//******************************************************************//