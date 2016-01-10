using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyScene : MonoBehaviour {

	public bool LobbyInActive;
	public GameObject Flash;

	private int m_Level;
	
	// Use this for initialization
	private PhotonManager pm;
	Text[] joinNumText;
    ExitGames.Client.Photon.Hashtable playerHash;

	void Start () {
		pm = GameObject.Find ("PhotonManager").GetComponent<PhotonManager> ();
        playerHash = new ExitGames.Client.Photon.Hashtable ();
        playerHash.Add ("isInGame", 0);
        playerHash.Add ("characterId", -1);
        PhotonNetwork.player.SetCustomProperties (playerHash);

		joinNumText = new Text[5];
		for (int i = 0; i < 5; ++i) {
			int roomNo = i + 1;
			//joinNumText[i] = GameObject.Find ("JoinNum" + roomNo).GetComponent<Text> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!pm.IsInLobby()) {
			return;
		}
        else
        {
            LobbyInActive = true;
        }

		foreach (RoomInfo room in PhotonNetwork.GetRoomList()) {
			
			int roomNo = 0;
			switch (room.name) {
			case "level1":
				roomNo = 0;
				break;
			case "level2":
				roomNo = 1;
				break;
			case "level3":
				roomNo = 2;
				break;
			case "level4":
				roomNo = 3;
				break;
			case "level5":
				roomNo = 4;
				break;
			default:
				break;
			}
		}
	}
			
	public void onRoomClick(int i) {
        if (!pm.IsInLobby ()) {
            return;
        }
	    Flash.GetComponent<Animator>().SetTrigger("OnClick");
		if(LobbyInActive == true){
			SceneChange(m_Level);
			LobbyInActive = false;
		}
	}

	public void SceneChange(int i) {
		RoomOptions roomOptions = new RoomOptions() { isVisible = true, maxPlayers = 20 };
		GameManager.instance.SelectLevel = i;
		PhotonNetwork.JoinOrCreateRoom("level" + i, roomOptions, TypedLobby.Default);
	}
}
