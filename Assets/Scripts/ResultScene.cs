using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour {

	private PhotonView myPv;
	private SortedDictionary<int, int> memberScore;
	private SortedDictionary<int, int> memberCorrectAnswerNum;
	private SortedDictionary<int, int> memberInCorrectAnswerNum;

	private bool hasSetRankingList = false;

	[PunRPC]	//TODO change characterId to characterName
	void SetMemberScore(int characterId, int score){
		memberScore.Add (score, characterId);
	}

	[PunRPC]	//TODO change characterId to characterName
	void SetMemberAnswerNum(int characterId, int correctAnswerNum, int inCorrectAnswerNum){
		memberCorrectAnswerNum.Add (correctAnswerNum, characterId);
		memberInCorrectAnswerNum.Add (inCorrectAnswerNum, characterId);
	}


	// Use this for initialization
	void Start () {
		GameObject.Find ("MyResultText").GetComponent<Text> ().text = 
			(GameManager.instance.CorrectAnswerNum + GameManager.instance.IncorrectAnswerNum).ToString ()
				+ "もん中、\n" + GameManager.instance.CorrectAnswerNum.ToString() + "もんせいかい！";

		myPv = this.GetComponent<PhotonView>();

		memberScore = new SortedDictionary<int, int>();
		myPv.RPC ("SetMemberScore", PhotonTargets.All, GameManager.instance.CharacterId, GameManager.instance.SelfScore);

		memberCorrectAnswerNum = new SortedDictionary<int, int>();
		memberInCorrectAnswerNum = new SortedDictionary<int, int>();
		myPv.RPC ("SetMemberAnswerNum", PhotonTargets.All, GameManager.instance.CharacterId, GameManager.instance.CorrectAnswerNum, GameManager.instance.IncorrectAnswerNum);

		GameObject.Find("HpGuage").transform.localScale = new Vector3(1,(float)(GameManager.instance.BossHp - GameManager.instance.Score) / GameManager.instance.BossHp,1);
	}
	
	// Update is called once per frame	
	void Update () {
		if (!hasSetRankingList && memberCorrectAnswerNum.Count >= PhotonNetwork.playerList.Length) {
			SetRankingList ();
		}
	}

	void SetRankingList(){
		int playerNum = memberCorrectAnswerNum.Count;
		int i = 0;
		foreach (var member in memberCorrectAnswerNum) {
			if (i == playerNum - 1) {
				GameObject.Find("Ranking").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			} else if (i == playerNum - 2) {
				GameObject.Find("Ranking (1)").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			} else if (i == playerNum - 3) {
				GameObject.Find("Ranking (2)").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			}
			i++;
		}
		hasSetRankingList = true;
	}
	
	public void onLobbySelectClick() {
		PhotonNetwork.Disconnect ();
		Application.LoadLevel ("Lobby");
	}
}
