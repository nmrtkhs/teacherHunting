using UnityEngine;
using System.Collections;

public class TitleScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void OnStartClick() {
		Application.LoadLevel ("Register");
	}

	public void OnContinueClick() {
		Application.LoadLevel ("Lobby");
	}
}
