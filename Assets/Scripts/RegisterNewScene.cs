using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class RegisterNewScene : MonoBehaviour {

	private PhotonView myPv;
	private GameObject characterButtonList;
	private List<int> disableList;

	// Use this for initialization
	void Start () {
		myPv = this.GetComponent<PhotonView>();
		characterButtonList = GameObject.Find("CharacterButtonList");
		disableList = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
	}

	[PunRPC]
	void DisableCharacterId(int id)
	{
		GameObject targetCharacter = characterButtonList.transform.GetChild(id).gameObject;
		targetCharacter.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f,1);
		disableList.Add(id);
	}

	public void OnCharacterClick(int characterId) {
		if(disableList.Contains(characterId))
		{
			return;
		}
		GameManager.instance.CharacterId = characterId;
		myPv.RPC("DisableCharacterId", PhotonTargets.All, characterId);

		Application.LoadLevel ("Game");
	}
}
