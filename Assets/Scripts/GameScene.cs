using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameScene : MonoBehaviour {

	private float GameTime = 120f;
	private float currentTime = .0f;
	private float attackingTime = .0f;
	private int correctAnswer = -1;
	private bool isStart = false;
	private bool isInitialized = false;
	private PhotonView myPv;
	private int bossHP;
	private int lastCorrect = 0;

	private GameObject playerAttacks;
	private Image attackIcon;
	private GameObject combo;
	private GameObject hpGuage;

	Text playerCountText;
	Text timeText;
	Text[] buttonText;
	Text scoreText;
	Text questionText;

	Text bossHPText;
	private int difficulty = 1;

	private QuestionManager questionManager;

	// Use this for initialization
	void Start () {
		buttonText = new Text[4];
		timeText = GameObject.Find ("TimeText").GetComponent<Text> ();
		for (int i = 0; i < 4; ++i) {
			int buttonNo = i + 1;
			var goParent = GameObject.Find ("AnswerButton" + buttonNo);
			var go = goParent.transform.FindChild ("Text").gameObject;
			buttonText[i] = go.GetComponent<Text> ();
		}
		scoreText = GameObject.Find ("ScoreText").GetComponent<Text> ();
		questionText = GameObject.Find ("QuestionText").GetComponent<Text> ();
		playerCountText = GameObject.Find ("PlayerCountText").GetComponent<Text> ();

		playerAttacks = GameObject.Find ("PlayerAttacks");
		attackIcon = GameObject.Find("AttackIcon").GetComponent<Image>();
		combo = GameObject.Find("Combo");
		hpGuage = GameObject.Find("HpGuage");

		myPv = this.GetComponent<PhotonView>();
		GameManager.instance.Score = 0;

		playerAttacks.SetActive(false);

		questionManager = this.GetComponent<QuestionManager> ();
		questionManager.LoadQuestion (GameManager.instance.SelectLevel);
	}
	
	// Update is called once per frame
	void Update () {

		if (!isStart) {
			playerCountText.text = "PlayerCount:" + PhotonNetwork.playerList.Length;	
			return;
		}

		if (!isInitialized)
		{
			bossHP = 300 * PhotonNetwork.playerList.Length;
			isInitialized = true;
		}

		if(bossHP - GameManager.instance.Score <= 0)
		{
			PhotonNetwork.Disconnect ();
			Application.LoadLevel ("Result");
		}

		hpGuage.transform.localScale = new Vector3(1,(float)(bossHP - GameManager.instance.Score) / bossHP,1);

		// time update
		currentTime += Time.deltaTime;
		float leftTime = GameTime - currentTime;
		if (leftTime <= 0) {
			leftTime = .0f;
			timeText.text = ((int)leftTime).ToString ();
			PhotonNetwork.Disconnect ();
			Application.LoadLevel ("Result");
		}
		timeText.text = ((int)leftTime).ToString ();

		if(playerAttacks.activeSelf)
		{
			attackingTime += Time.deltaTime;
			if(attackingTime > 1.0f)
			{
				playerAttacks.SetActive(false);
				attackingTime = 0;
			}
		}
	}

	[PunRPC]
	void addScore(int score){
		GameManager.instance.Score = score;
		scoreText.text = GameManager.instance.Score.ToString ();
	}

	[PunRPC]
	void start(){
		isStart = true;
		questionManager.SetQuestion(difficulty);
	}

	[PunRPC]
	void SetAttack(int id, int correct)
	{
		attackIcon.sprite = Resources.LoadAll<Sprite> ("avatar")[id];
		if(correct == 1)
		{
			combo.SetActive(true);
		} else {
			combo.SetActive(false);
		}
		playerAttacks.SetActive(true);
		attackingTime = .0f;
	}


	public void onAnswerClick(int buttonNo) {
		if (!isStart) {
			return;
		}

		if (questionManager.IsCorrectAnswer(buttonNo)) {
			GameManager.instance.Score += (30 + 30 * lastCorrect / 2);
			GameManager.instance.SelfScore += (30 + 30 * lastCorrect / 2);
			scoreText.text = GameManager.instance.Score.ToString ();
			myPv.RPC ("addScore", PhotonTargets.All, GameManager.instance.Score);
			myPv.RPC ("SetAttack", PhotonTargets.All, GameManager.instance.CharacterId, lastCorrect);
			difficulty++;
			if (difficulty > 3) {
				difficulty = 3;
			}

			lastCorrect = 1;
		} else {
			difficulty--;
			if (difficulty < 1) {
				difficulty = 1;
			}
			lastCorrect = 0;
		}
		questionManager.SetQuestion(difficulty);
	}

	public void onStart() {
		isStart = true;
		myPv.RPC("start",PhotonTargets.All);
	}
}
