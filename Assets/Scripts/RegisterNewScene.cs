using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class RegisterNewScene : MonoBehaviour {

	private PhotonView myPv;
	private GameObject characterButtonList;
	private List<int> disableList;
    ExitGames.Client.Photon.Hashtable playerHash;
    Dropdown dropdown;

	// Use this for initialization
	void Start () {
		myPv = this.GetComponent<PhotonView>();
		characterButtonList = GameObject.Find("CharacterButtonList");
		disableList = new List<int>();
        dropdown = GameObject.Find ("Dropdown").GetComponent<Dropdown> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	[PunRPC]
	void DisableCharacterId(int id)
	{
		GameObject targetCharacter = characterButtonList.transform.GetChild(id - 1).gameObject;
		targetCharacter.GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f,1);
		disableList.Add(id);
	}

	public void OnBackButtonClicke() {
		Application.LoadLevel ("RegisterName");
	}

	public void OnCharacterClick(int characterId) {
		if(disableList.Contains(characterId))
		{
			return;
		}
		GameManager.instance.CharacterId = characterId;
		myPv.RPC("DisableCharacterId", PhotonTargets.All, characterId);
        playerHash = new ExitGames.Client.Photon.Hashtable ();
        playerHash.Add ("isInGame", 1);
        playerHash.Add ("characterId", characterId);
        SetDifficulty ();
        PhotonNetwork.player.SetCustomProperties (playerHash);
		Application.LoadLevel ("Game");
	}

    void SetDifficulty()
    {
        GameManager.instance.difficulty = dropdown.value;
    }
}
