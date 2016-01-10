using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour {

	public Sprite rankS;
	public Sprite rankA;
	public Sprite rankB;
	public Sprite rankC;
	public Sprite rankD;
	public Sprite rankE;
	public Sprite titleWin;
	public Sprite titleLose;
	public Sprite[] enemyWin;
	public Sprite[] enemyLose;
	public Sprite backgroundWin;
	public Sprite backgroundLose;
	public bool ResultInActvie;
	public GameObject Canvas;
	
	private PhotonView myPv;
	private SortedDictionary<int, string> memberScore;
	private SortedDictionary<int, string> memberCorrectAnswerNum;
	private SortedDictionary<int, string> memberInCorrectAnswerNum;

	private bool hasSetRankingList = false;
	private bool win = true;

	[PunRPC]
	void SetMemberScore(string characterName, int score){
		memberScore.Add (score, characterName);
	}

	[PunRPC]
	void SetMemberAnswerNum(string characterName, int correctAnswerNum, int inCorrectAnswerNum){
		memberCorrectAnswerNum.Add (correctAnswerNum, characterName);
		memberInCorrectAnswerNum.Add (inCorrectAnswerNum, characterName);
	}


	// Use this for initialization
	void Start () {
		GameObject.Find ("Ending").SetActive (false);

		win = (GameManager.instance.BossHp <= GameManager.instance.Score);

		GameObject.Find ("MyResultText").GetComponent<Text> ().text = 
			(GameManager.instance.CorrectAnswerNum + GameManager.instance.IncorrectAnswerNum).ToString ()
				+ "もん中、\n" + GameManager.instance.CorrectAnswerNum.ToString() + "もんせいかい！";

		myPv = this.GetComponent<PhotonView>();

		memberScore = new SortedDictionary<int, string>();
		myPv.RPC ("SetMemberScore", PhotonTargets.All, GameManager.instance.name, GameManager.instance.SelfScore);

		memberCorrectAnswerNum = new SortedDictionary<int, string>();
		memberInCorrectAnswerNum = new SortedDictionary<int, string>();
		myPv.RPC ("SetMemberAnswerNum", PhotonTargets.All, GameManager.instance.name, GameManager.instance.CorrectAnswerNum, GameManager.instance.IncorrectAnswerNum);

		GameObject.Find("HpGuage").transform.localScale = new Vector3(1, (GameManager.instance.BossHp > GameManager.instance.Score)? (float)(GameManager.instance.BossHp - GameManager.instance.Score) / GameManager.instance.BossHp : 0, 1);

		if (win) {
			GameObject.Find ("BackGround").GetComponent<Image> ().sprite = backgroundWin;
			GameObject.Find ("Title").GetComponent<Image> ().sprite = titleWin;
			GameObject.Find ("Enemy").GetComponent<Image> ().sprite = enemyWin[GameManager.instance.SelectLevel];
		} else {
			GameObject.Find ("BackGround").GetComponent<Image> ().sprite = backgroundLose;
			GameObject.Find ("TItle").GetComponent<Image> ().sprite = titleLose;
			GameObject.Find ("Enemy").GetComponent<Image> ().sprite = enemyLose[GameManager.instance.SelectLevel];
		}
	}
	
	// Update is called once per frame	
	void Update () {
		if (!hasSetRankingList && memberCorrectAnswerNum.Count >= PhotonNetwork.playerList.Length) {
			SetRankingList ();
		}

		if(ResultInActvie == true){
			SceneChange();
			ResultInActvie = false;
		}
	}

	void SetRankingList(){
		int playerNum = memberCorrectAnswerNum.Count;
		int sumCorrectAnswerNum = 0;
		int sumInCorrectAnswerNum = 0;

		int i = 0;
		foreach (var member in memberCorrectAnswerNum) {
			if (i == playerNum - 1) {
				GameObject.Find("Ranking").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			} else if (i == playerNum - 2) {
				GameObject.Find("Ranking (1)").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			} else if (i == playerNum - 3) {
				GameObject.Find("Ranking (2)").GetComponentsInChildren<Text>()[1].text = member.Value.ToString();
			}
			sumCorrectAnswerNum += member.Key;
			i++;
		}

		foreach (var member in memberInCorrectAnswerNum) {
			sumInCorrectAnswerNum += member.Key;
		}

		float correctAnswerRate = (sumCorrectAnswerNum + sumInCorrectAnswerNum > 0? (float)sumCorrectAnswerNum / (sumCorrectAnswerNum + sumInCorrectAnswerNum): 0.0f);

		Image playRank = GameObject.Find ("PlayRank").GetComponent<Image> ();
		if (GameManager.instance.BossHp > GameManager.instance.Score) {
			playRank.sprite = rankE;
		} else if (correctAnswerRate < 0.2f) {
			playRank.sprite = rankD;
		} else if (correctAnswerRate < 0.5f) {
			playRank.sprite = rankC;
		} else if (correctAnswerRate < 0.7f) {
			playRank.sprite = rankB;
		} else if (correctAnswerRate < 0.9f) {
			playRank.sprite = rankA;
		} else {
			playRank.sprite = rankS;
		}

		hasSetRankingList = true;
	}
	
	public void onLobbySelectClick() {
		Canvas.GetComponent<Animator>().SetTrigger("OnClick");
	}

	public void SceneChange() {
		PhotonNetwork.Disconnect ();
		Application.LoadLevel ("Lobby");
	}
}
