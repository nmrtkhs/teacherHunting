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
	public bool ResultActvie;
	public GameObject Canvas;
	public GameObject Ending;
	
	private PhotonView myPv;
	private Dictionary<string, int> memberCorrectAnswerNum;
	private Dictionary<string, int> memberInCorrectAnswerNum;

	private bool hasSetRankingList = false;
	private bool win = true;

	[PunRPC]
	void SetMemberAnswerNum(string characterName, int correctAnswerNum, int inCorrectAnswerNum){
		memberCorrectAnswerNum.Add (characterName, correctAnswerNum);
		memberInCorrectAnswerNum.Add (characterName, inCorrectAnswerNum);
	}


	// Use this for initialization
	void Start () {
		memberCorrectAnswerNum = new Dictionary<string, int>();
		memberInCorrectAnswerNum = new Dictionary<string, int>();

		win = (GameManager.instance.BossHp <= GameManager.instance.Score);

		if (win && GameManager.instance.SelectLevel == 5) {
			AudioManager.Instance.PlayBGM("endingbgm", 0.5f);
			Ending.SetActive(true);
			ResultActvie = false;
		} else {
			ResultOn();
		}
	}

	public void ResultOn() {
		ResultActvie = true;
		Ending.SetActive (false);
		Canvas.GetComponent<Animator>().SetTrigger("OnResultStart");

		AudioManager.Instance.PlayBGM(win ? "resultclearbgm" : "resultfailbgm", 0.5f);

		GameObject.Find ("MyResultText").GetComponent<Text> ().text = 
			(GameManager.instance.CorrectAnswerNum + GameManager.instance.IncorrectAnswerNum).ToString ()
				+ "もん中、\n" + GameManager.instance.CorrectAnswerNum.ToString() + "もんせいかい！";

		myPv = this.GetComponent<PhotonView>();

		memberCorrectAnswerNum = new Dictionary<string, int>();
		memberInCorrectAnswerNum = new Dictionary<string, int>();
		myPv.RPC ("SetMemberAnswerNum", PhotonTargets.All, GameManager.instance.name, GameManager.instance.CorrectAnswerNum, GameManager.instance.IncorrectAnswerNum);

		GameObject.Find("HpGuage").transform.localScale = new Vector3(1, (GameManager.instance.BossHp > GameManager.instance.Score)? (float)(GameManager.instance.BossHp - GameManager.instance.Score) / GameManager.instance.BossHp : 0, 1);

		if (win) {
			GameObject.Find ("BackGround").GetComponent<Image> ().sprite = backgroundWin;
			GameObject.Find ("Title").GetComponent<Image> ().sprite = titleWin;
			GameObject.Find ("Enemy").GetComponent<Image> ().sprite = enemyWin[GameManager.instance.SelectLevel];
		} else {
			GameObject.Find ("BackGround").GetComponent<Image> ().sprite = backgroundLose;
			GameObject.Find ("Title").GetComponent<Image> ().sprite = titleLose;
			GameObject.Find ("Enemy").GetComponent<Image> ().sprite = enemyLose[GameManager.instance.SelectLevel];
		}
	}

	
	// Update is called once per frame	
	void Update () {

		if (ResultActvie) {
			UpdateRankingList ();
		}
	}

	void UpdateRankingList(){
		int playerNum = memberCorrectAnswerNum.Count;
		int sumCorrectAnswerNum = 0;
		int sumInCorrectAnswerNum = 0;

		var rank = new List< KeyValuePair <string, int> >();
		rank.Add(new KeyValuePair<string, int> ("", -1));
		rank.Add(new KeyValuePair<string, int> ("", -1));
		rank.Add(new KeyValuePair<string, int> ("", -1));

		foreach (var member in memberCorrectAnswerNum) {
			if (member.Value > rank [0].Value) {
				rank [2] = rank [1];
				rank [1] = rank [0];
				rank [0] = member;
			} else if (member.Value > rank [1].Value) {
				rank [2] = rank [1];
				rank [1] = member;
			} else if (member.Value > rank [2].Value) {
				rank [2] = member;
			}

			sumCorrectAnswerNum += member.Value;
		}

		GameObject.Find("Ranking").GetComponentsInChildren<Text>()[1].text = rank[0].Key;
		GameObject.Find("Ranking (1)").GetComponentsInChildren<Text>()[1].text = rank[1].Key;
		GameObject.Find("Ranking (2)").GetComponentsInChildren<Text> () [1].text = rank [2].Key;

		foreach (var member in memberInCorrectAnswerNum) {
			sumInCorrectAnswerNum += member.Value;
		}

		float correctAnswerRate = (sumCorrectAnswerNum + sumInCorrectAnswerNum > 0? (float)sumCorrectAnswerNum / (sumCorrectAnswerNum + sumInCorrectAnswerNum): 0.0f);

		Image playRank = GameObject.Find ("PlayRank").GetComponent<Image> ();
		if (!win) {
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
	
	public void OnLobbySelectClick() {
		Canvas.GetComponent<Animator>().SetTrigger("OnLobbySelect");
	}

	public void BackToLobby() {
		PhotonNetwork.Disconnect ();
		AudioManager.Instance.PlayBGM ("stratbgm", 0.5f);
		Application.LoadLevel ("Lobby");
	}
}
