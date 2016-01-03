using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour {

	private PhotonView myPv;
	private SortedDictionary<int, int> memberScore;

	[PunRPC]	//TODO change characterId to characterName
	void SetMemberScore(int characterId, int score){
		memberScore.Add (score, characterId);
	}

	// Use this for initialization
	void Start () {
		GameObject.Find ("MyResultText").GetComponent<Text> ().text = 
			(GameManager.instance.CorrectAnswerNum + GameManager.instance.IncorrectAnswerNum).ToString ()
				+ "もん中、\n" + GameManager.instance.CorrectAnswerNum.ToString() + "もんせいかい！";

		memberScore = new SortedDictionary<int, int>();
		myPv = this.GetComponent<PhotonView>();
		myPv.RPC ("SetMemberScore", PhotonTargets.All, GameManager.instance.CharacterId, GameManager.instance.SelfScore);
	}
	
	// Update is called once per frame	
	void Update () {
		Debug.Log (memberScore.Count);
		int i = 0;
		foreach (var member in memberScore) {
			if (i == memberScore.Count - 1) {
				GameObject.Find("Ranking").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			} else if (i == memberScore.Count - 2) {
				GameObject.Find("Ranking (1)").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			} else if (i == memberScore.Count - 3) {
				GameObject.Find("Ranking (2)").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			}
			i++;
		}
	}
	
	public void onLobbySelectClick() {
		PhotonNetwork.Disconnect ();
		Application.LoadLevel ("Lobby");
	}
}
