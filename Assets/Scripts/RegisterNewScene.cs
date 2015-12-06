using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class RegisterNewScene : MonoBehaviour {

	private PhotonView myPv;
	private GameObject characterButtonList;

	// Use this for initialization
	void Start () {
		myPv = this.GetComponent<PhotonView>();
		characterButtonList = GameObject.Find("CharacterButtonList");
	}
	
	// Update is called once per frame
	void Update () {
	}

	[PunRPC]
	void DisableCharacterId(int id)
	{
		GameObject targetCharacter = characterButtonList.transform.GetChild(id).gameObject;
		targetCharacter.SetActive(false);
	}

	public void OnCharacterClick(int characterId) {
		GameManager.instance.CharacterId = characterId;
		myPv.RPC("DisableCharacterId", PhotonTargets.All, characterId);

		Application.LoadLevel ("Game");
	}
}
