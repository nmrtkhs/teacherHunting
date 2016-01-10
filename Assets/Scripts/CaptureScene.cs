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

		string deviceName = devices [0].name;
		foreach (var device in WebCamTexture.devices) {
			if (device.isFrontFacing) {
				deviceName = device.name;
			}	
		}

		webcamTexture = new WebCamTexture(deviceName, Width, Height, FPS);

		if (webcamTexture == null) {
			Debug.Log (webcamTexture);
		}
		webcamTexture.Play();

		imageObj.GetComponent<RawImage>().texture = webcamTexture;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCaptureClick () {
		color32 = webcamTexture.GetPixels32();
		Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
		texture.SetPixels32(color32);
		texture.Apply();

//		Sprite sprite = Sprite.Create (texture, new Rect (320, 0, 640, 640), Vector2.zero);
//		Texture2D outputTexture = sprite.texture;

		var bytes = texture.EncodeToPNG();
//		var bytes = outputTexture.EncodeToPNG();

//		File.WriteAllBytes(Application.persistentDataPath + "/me.png", bytes);
//		File.WriteAllBytes(Application.dataPath + "/me.png", bytes);

		webcamTexture.Stop();

		Application.LoadLevel ("Register");
	}
}
