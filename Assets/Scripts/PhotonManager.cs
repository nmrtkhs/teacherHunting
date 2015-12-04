using UnityEngine;
using System.Collections;

public class PhotonManager : Photon.MonoBehaviour {

	void Start ()
	{
		Debug.Log ("photonStart");
		//photonを利用するための初期設定 ロビーを作成して入る？
		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	//PhotonNetwork.ConnectUsingSettingsを行うと呼ばれる
	void OnJoinedLobby()
	{
		Debug.Log ("photonJoinedLobby");
		//ランダムにルームに入る
		PhotonNetwork.JoinRandomRoom();
	}

	//ランダムにルームに入れなかった
	void OnPhotonRandomJoinFailed()
	{
		Debug.Log ("photonRandomJoinFailed");
		//部屋を自分で作って入る
		RoomOptions roomOptions = new RoomOptions() { isVisible = false, maxPlayers = 4 };
		PhotonNetwork.JoinOrCreateRoom("teacherHunting", roomOptions, TypedLobby.Default);
	}

	//ルームに入室成功
	void OnJoinedRoom()
	{
		//キャラクター作成
		//PlayerMake ();
		Debug.Log("aa-");
	}
}
