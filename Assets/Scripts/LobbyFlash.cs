using UnityEngine;
using System.Collections;

public class LobbyFlash : MonoBehaviour {

	public GameObject EventSystem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FlashAnimationFinish() {
		EventSystem.GetComponent<LobbyScene>().LobbyInActvie = true;	
	}
}
