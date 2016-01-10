using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarNewObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Image>().sprite = Resources.LoadAll<Sprite> ("avatar")[GameManager.instance.CharacterId];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
