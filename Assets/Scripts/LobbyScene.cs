using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LobbyScene : MonoBehaviour {

	// Use this for initialization
	private PhotonManager pm;
	Text[] joinNumText;
	void Start () {
		pm = GameObject.Find ("PhotonManager").GetComponent<PhotonManager> ();	

		joinNumText = new Text[5];
		for (int i = 0; i < 5; ++i) {
			int roomNo = i + 1;
			joinNumText[i] = GameObject.Find ("JoinNum" + roomNo).GetComponent<Text> ();
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

			joinNumText [roomNo].text = "さんか人数:" + room.playerCount;
		}
	}

	public void onRoomClick(int i) {
		RoomOptions roomOptions = new RoomOptions() { isVisible = true, maxPlayers = 20, customRoomProperties = new Hashtable(){"selectedIds",new List<int>()}};
		GameManager.instance.SelectLevel = i;
		PhotonNetwork.JoinOrCreateRoom("level" + i, roomOptions, TypedLobby.Default);
	}
}
