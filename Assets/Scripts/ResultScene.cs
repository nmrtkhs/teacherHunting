using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.Find ("ScoreText").GetComponent<Text> ().text = 
			"Score:" + 
			GameObject.Find ("ScoreObject").GetComponent<ScoreObject> ().Score.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void onWorkClick() {
	}

	public void onLobbySelectClick() {
		Application.LoadLevel ("Lobby");
	}
}
