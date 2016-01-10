using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarObject : MonoBehaviour {

	void Start () {
		StartCoroutine(ImageWWWStart());
	}

	IEnumerator ImageWWWStart () {
		string url = "file://" + Application.persistentDataPath + "/me.png";
		WWW www = new WWW(url);
		yield return www;

		if(!string.IsNullOrEmpty(www.error)){
			Debug.LogError("www Error:" + www.error);
			yield break;
		}

		gameObject.GetComponent<Image> ().sprite =
			Sprite.Create (www.texture, new Rect (320, 0, 640, 640), Vector2.zero);	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
