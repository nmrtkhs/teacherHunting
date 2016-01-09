using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarNewObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Image>().sprite = Resources.Load<Sprite> ("avatar/avatar_" + GameManager.instance.CharacterId);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
