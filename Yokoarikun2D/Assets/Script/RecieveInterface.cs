﻿using UnityEngine.EventSystems;

public interface RecieveInterface : IEventSystemHandler {
	//******************************************************************//
	//	メッセージ処理インターフェイス
	//	同じオブジェクト内でメッセージが発行された時の受け取り口を宣言する
	//******************************************************************//
	void ISnatched ();	// メッセージ : 横取りされた


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}