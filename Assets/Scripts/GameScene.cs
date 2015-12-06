using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameScene : MonoBehaviour {

	public float GameTime = 120f;
	private float currentTime = .0f;
	private float attackingTime = .0f;
	private float correctAnswer = .0f;
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
		updateQuesttion ();
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

	private void updateQuesttion() {
		int digit = (int)Mathf.Pow(10, difficulty);

		int arg1 = Random.Range (1, digit);
		int arg2 = Random.Range (1, digit);

		switch (GameManager.instance.SelectLevel) {
		case 1:
			calcQuestionDigit2 ();
			break;
		case 2:
			correctAnswer = arg1 - arg2;
			questionText.text = arg1.ToString () + " - " + arg2 + " = ?";
			break;
		case 3:
			correctAnswer = arg1 * arg2;
			questionText.text = arg1.ToString () + " x " + arg2 + " = ?";
			break;
		case 4:
			correctAnswer = arg1 + arg2;
			questionText.text = arg1.ToString () + " + " + arg2 + " = ?";
			break;
		default:
			correctAnswer = arg1 + arg2;
			questionText.text = arg1.ToString () + " + " + arg2 + " = ?";
			break;
		}

		int answerNo = Random.Range (0, 4);
		for (int i = 0; i < buttonText.Length; ++i) {
			buttonText [i].text = (correctAnswer - (answerNo - i)).ToString ();
		}
	}

	private void calcQuestionDigit2() {
		int digit = (int)Mathf.Pow(10, difficulty);

		int arg1Digit1 = Random.Range (0, 10);
		int arg1Digit2 = 0;
		bool isPlus = (int)Random.Range (0, 2) == 0 ? true : false;
		if (isPlus) {
			arg1Digit2 = Random.Range(0, 9);
		} else {
			arg1Digit2 = Random.Range(1, 10);
		}

		int arg2Digit1 = Random.Range (0, 10);
		switch (difficulty) {
		case 1:
			if (isPlus) {
				arg2Digit1 = Random.Range (0, 10 - arg1Digit2);
			} else {
				arg2Digit1 = Random.Range (0, arg1Digit2 - 1);
			}
			break;
		case 2:
			arg2Digit1 = Random.Range (0, 10);
			break;
		case 3:
			break;
		default:
			break;
		}
		int arg1 = int.Parse(arg1Digit1.ToString() + arg1Digit2.ToString());
		int arg2 = arg2Digit1;

		correctAnswer = arg1 + arg2;
		questionText.text = arg1.ToString () + " + " + arg2 + " = ?";
	}

	public void onAnswerClick(int buttonNo) {
		if (!isStart) {
			return;
		}

		float answer = float.Parse (buttonText [buttonNo].text);
		if (correctAnswer == answer) {
			GameManager.instance.Score += (30 + 30 * lastCorrect / 2);
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
		updateQuesttion ();
	}

	public void onStart() {
		isStart = true;
		myPv.RPC("start",PhotonTargets.All);
	}
}
