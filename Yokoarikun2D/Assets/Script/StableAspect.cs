using UnityEngine;
using System.Collections;

public class StableAspect : MonoBehaviour {
	//******************************************************************//
	//					カメラのアスペクト比を設定する					//
	//******************************************************************//
	private Camera cam;					// カメラコンポーネント
	private float width  = 	720;		// カメラの横幅
	private float height = 1280;		// カメラの縦幅
	private float pixelPerUni=100f;		// ユニット当たりのピクセル(Pixel Per Unit)

	void Awake(){
		float aspect = (float)Screen.height / (float)Screen.width;	// 現在のアスペクト比
		float bgAcpect = height / width;							// 設定したいアスペクト比

		cam = GetComponent<Camera>();								// カメラコンポーネントを取得する
		cam.orthographicSize = height / 2f / pixelPerUni;			// カメラのrthographicSizeを設定


		if (bgAcpect > aspect) {
			float bgScale = height / Screen.height;							// 倍率
			float camWidth = width / (Screen.width * bgScale);				// viewport rectの幅
			cam.rect = new Rect ((1f - camWidth) / 2f, 0f, camWidth, 1f);	// viewport rectを設定
		} else {
			float bgScale=width/Screen.width;								// 倍率
			float camHeight=height/(Screen.height*bgScale);					// viewport rectの幅
			cam.rect = new Rect(0f, (1f - camHeight) / 2f, 1f, camHeight);	// viewport rectを設定
		}

		//Screen.autorotateToLandscapeLeft = false;
		//Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortrait = true;
		//Screen.SetResolution ((int)width, (int)height, true);
		Application.targetFrameRate = 60;
	}


	//******************************************************************//
	//								End of class						//
	//******************************************************************//
}
