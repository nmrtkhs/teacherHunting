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
		if (PhotonNetwork.inRoom) {
			Room room = PhotonNetwork.room;
			if (room == null) {
					return;
			}
			Hashtable cp = room.customProperties;
			List<int> selectedIds = cp["selectedIds"];
			foreach(int selectedId in selectedIds)
			{
				myPv.RPC("DisableCharacterId", PhotonTargets.All, selectedId);
			}
		}
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

		Room room = PhotonNetwork.room;
		Hashtable cp = room.customProperties;
		List<int> selectedIds = cp["selectedIds"];
		List <int> newIds = selectedIds.Add(characterId);
		cp["selectedIds"] = newIds;
		room.SetCustomProperties(cp);

		Application.LoadLevel ("Game");
	}
}
