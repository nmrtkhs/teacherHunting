using UnityEngine;
using System.Collections;

public class RegisterNewScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCharacterClick(int characterId) {
		GameManager.instance.CharacterId = characterId;
		Application.LoadLevel ("Game");
	}
}
