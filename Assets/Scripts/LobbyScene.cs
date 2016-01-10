using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyScene : MonoBehaviour {

	public bool LobbyInActvie;
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

//			joinNumText [roomNo].text = "さんか人数:" + room.playerCount;
		}
	}
			
	public void onRoomClick(int i) {
        if (!pm.IsInLobby ()) {
            return;
        }
		if(LobbyInActvie == true){
			SceneChange(m_Level);
			LobbyInActvie = false;
		}
	}
	public void OnRoomClick(int level) {
		Debug.Log ("OnRoomClick");
		m_Level = level;
	Flash.GetComponent<Animator>().SetTrigger("OnClick");
	}
	
	public void SceneChange(int i) {
		RoomOptions roomOptions = new RoomOptions() { isVisible = true, maxPlayers = 20 };
		GameManager.instance.SelectLevel = i;
		PhotonNetwork.JoinOrCreateRoom("level" + i, roomOptions, TypedLobby.Default);
	}
}
