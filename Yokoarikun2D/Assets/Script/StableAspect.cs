using UnityEngine;
using System.Collections;

public class StableAspect : MonoBehaviour {

	private Camera cam;
	// 画面サイズ
	private float width;
	private float height;
	// 画像のixel Per Unit
	private float pixelPerUni=100f;

	void Awake(){
		width = 720;
		height = 1280;

		float aspect = (float)Screen.height / (float)Screen.width;
		float bgAcpect = height / width;

		// カメラコンポーネントを取得する
		cam=GetComponent<Camera>();
		// カメラのrthographicSizeを設定
		cam.orthographicSize=height/2f/pixelPerUni;


		if (bgAcpect > aspect) {
			// 倍率
			float bgScale = height / Screen.height;
			// viewport rectの幅
			float camWidth = width / (Screen.width * bgScale);
			// viewportRectを設定
			cam.rect = new Rect ((1f - camWidth) / 2f, 0f, camWidth, 1f);
		} else {
			// 倍率
			float bgScale=width/Screen.width;
			// viewport rectの幅
			float camHeight=height/(Screen.height*bgScale);
			// viewportRectを設定
			cam.rect=new Rect(0f,(1f-camHeight)/2f,1f,camHeight);
		}
	}

		void Start(){
		// 画面コントロール
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortrait = true;
		Screen.SetResolution ((int)width, (int)height, true);
		Application.targetFrameRate = 60;
	}
}
