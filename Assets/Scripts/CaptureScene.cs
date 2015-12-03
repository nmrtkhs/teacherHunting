using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class CaptureScene : MonoBehaviour {

	public int Width = 1920;
	public int Height = 1080;
	public int FPS = 30;
	WebCamTexture webcamTexture;
	public Color32[] color32;

	private GameObject imageObj;
	// Use this for initialization
	void Start () {
		WebCamDevice[] devices = WebCamTexture.devices;
		// display all cameras
		for (var i = 0; i < devices.Length; i++) {
			Debug.Log (devices [i].name);
		}

		imageObj = GameObject.Find("RawImage").gameObject as GameObject;

		webcamTexture = new WebCamTexture(devices[0].name, Width, Height, FPS);
		if (webcamTexture == null) {
			Debug.Log (webcamTexture);
		}
		webcamTexture.Play();

//		GetComponent<Renderer> ().material.mainTexture = webcamTexture;
//		color32 = webcamTexture.GetPixels32();
//		Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
//		texture.SetPixels32(color32);
//		texture.Apply();

//		imageObj.GetComponent<Image> ().sprite = Sprite.Create (texture, new Rect (0, 0, 320, 240), Vector2.zero);
		imageObj.GetComponent<RawImage>().texture = webcamTexture;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCaptureClick () {
//		var bytes = webcamTexture.EncodeToPNG();
		color32 = webcamTexture.GetPixels32();
		Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
		texture.SetPixels32(color32);
		texture.Apply();
		var bytes = texture.EncodeToPNG();

		File.WriteAllBytes(Application.dataPath + "/me.png", bytes);

		Application.LoadLevel ("Register");
	}
}
