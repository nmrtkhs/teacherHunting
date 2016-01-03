using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.Find ("MyResultText").GetComponent<Text> ().text = 
			(GameManager.instance.CorrectAnswerNum + GameManager.instance.IncorrectAnswerNum).ToString ()
				+ "もん中、\n" + GameManager.instance.CorrectAnswerNum.ToString() + "もんせいかい！";
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
