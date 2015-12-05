using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyScene : MonoBehaviour {

	// Use this for initialization
	private PhotonManager pm;
	Text[] joinNumText;
	void Start () {
		pm = GameObject.Find ("PhotonManager").GetComponent<PhotonManager> ();	

		joinNumText = new Text[4];
		for (int i = 0; i < 4; ++i) {
			int roomNo = i + 1;
			joinNumText[i] = GameObject.Find ("JoinNum" + roomNo).GetComponent<Text> ();
//			var go = goParent.transform.FindChild ("Text").gameObject;
//			joinNumText[i] = go.GetComponent<Text> ();
			Debug.Log (joinNumText [i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!pm.IsInLobby()) {
			return;
		}

		foreach (RoomInfo room in PhotonNetwork.GetRoomList()) {
			Debug.Log (room);
			
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
			default:
				break;
			}

			joinNumText [roomNo].text = "level:" + roomNo + "\ncount:" + room.playerCount;
		}
	}

	public void onRoomClick(int i) {
		RoomOptions roomOptions = new RoomOptions() { isVisible = true, maxPlayers = 20 };
		PhotonNetwork.JoinOrCreateRoom("level" + i, roomOptions, TypedLobby.Default);
	}
}
