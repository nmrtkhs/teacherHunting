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
		Debug.LogFormat ("Q_level:{0}, difficulty:{1}", GameManager.instance.SelectLevel, difficulty);
		
		switch (GameManager.instance.SelectLevel) {
		case 0:
			calcQuestionDigit2 ();
			break;
		case 2:
			calcQuestionDigit3 ();
			break;
		case 1:
			calcQuestionOver100();
			break;
		case 4:
			calcQuestionOver1000();
			break;
		default:
			Debug.LogWarning("wrong at switch");
			break;
		}
		
		Debug.LogFormat("CorrectAnswer: {0}", correctAnswer);
		
		int answerNo = Random.Range (0, 4);
		for (int i = 0; i < buttonText.Length; ++i) {
			buttonText [i].text = (correctAnswer - (answerNo - i)).ToString ();
		}
	}
	
	private void calcQuestionDigit2() {
		int qType = -1;
		int arg1Digit10 = 1000;
		int arg1Digit1 = 1000;
		int arg2Digit10 = 1000;
		int arg2Digit1 = 1000;
		
		int arg1 = -1;
		int arg2 = -1;
		
		switch (difficulty) {
		case 1:
			qType = Random.Range (1, 5);
			Debug.LogFormat ("calcQuestionDigit2: q_type:{0}", qType);
			
			switch(qType){
			case 1:		//oo+o
				arg1Digit10 = Random.Range(1, 10);
				arg1Digit1 = Random.Range(1, 9);
				arg2Digit10 = 0;
				arg2Digit1 = Random.Range(1, 10-arg1Digit1);
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				arg2 = arg2Digit10*10 + arg2Digit1;
				correctAnswer = arg1 + arg2;
				questionText.text = arg1.ToString () + " + " + arg2.ToString() + " = ?";
				break;
			case 2:
				arg1Digit10 = Random.Range(1, 10);
				arg1Digit1 = Random.Range(1, 10);
				arg2Digit10 = Random.Range(1, 10-arg1Digit10);
				arg2Digit1 = Random.Range(0, 10-arg1Digit1);
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				arg2 = arg2Digit10*10 + arg2Digit1;
				correctAnswer = arg1 + arg2;
				questionText.text = arg1.ToString () + " + " + arg2.ToString() + " = ?";
				break;
			case 3:
				arg1Digit10 = Random.Range(1, 10);
				arg1Digit1 = Random.Range(1, 10);
				arg2Digit10 = 0;
				arg2Digit1 = Random.Range(0, arg1Digit1);
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				arg2 = arg2Digit10*10 + arg2Digit1;
				correctAnswer = arg1 - arg2;
				questionText.text = arg1.ToString () + " - " + arg2.ToString() + " = ?";
				break;
			case 4:
				arg1Digit10 = Random.Range(1, 10);
				arg1Digit1 = Random.Range(1, 10);
				arg2Digit10 = Random.Range(1, arg1Digit10+1);
				arg2Digit1 = Random.Range(0, arg1Digit1);
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				arg2 = arg2Digit10*10 + arg2Digit1;
				correctAnswer = arg1 - arg2;
				questionText.text = arg1.ToString () + " - " + arg2.ToString() + " = ?";
				break;
				
			default:
				Debug.LogWarning("wrong at switch");
				break;
			}
			break;
			
		case 2:
			qType = Random.Range (1, 3);
			Debug.LogFormat ("calcQuestionDigit2 q_type:{0}", qType);
			switch(qType){
			case 1:
				arg1Digit10 = Random.Range(1, 9);
				arg1Digit1 = Random.Range(0, 10);
				arg2Digit10 = 0;
				arg2Digit1 = Random.Range(0, 10);
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				arg2 = arg2Digit10*10 + arg2Digit1;
				correctAnswer = arg1 + arg2;
				questionText.text = arg1.ToString () + " + " + arg2.ToString() + " = ?";
				break;
			case 2:				
				arg1Digit10 = Random.Range(1, 9);
				arg1Digit1 = Random.Range(0, 10);
				arg1 = arg1Digit10*10 + arg1Digit10;
				arg2 = Random.Range(10, arg1);
				
				correctAnswer = arg1 + arg2;
				questionText.text = arg1.ToString () + " + " + arg2.ToString() + " = ?";
				break;
			default:
				Debug.LogWarning("wrong at switch");
				break;
			}
			break;
			
		case 3:
			qType = Random.Range (1, 5);
			Debug.LogFormat ("calcQuestionDigit2 q_type:{0}", qType);
			switch(qType){
			case 1:
				arg1Digit10 = Random.Range(1, 10);
				arg1Digit1 = ((arg1Digit10!=9) ? Random.Range(0, 10) : Random.Range(0, 9));
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				arg2 = arg1 + Random.Range(1, 10);
				correctAnswer = arg2 - arg1;
				questionText.text = arg1.ToString () + " + ? = " + arg2.ToString();
				break;
			case 2:
				arg1Digit10 = Random.Range(1, 10);
				arg1Digit1 = Random.Range(0, 10);
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				correctAnswer = Random.Range(0, arg1);
				arg2 = arg1 - correctAnswer;
				questionText.text = arg1.ToString () + " - ? = " + arg2.ToString();
				break;
			case 3:
				arg1Digit10 = Random.Range(1, 9);
				arg1Digit1 = Random.Range(0, 10);
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				correctAnswer = Random.Range(10, 100-arg1);
				arg2 = arg1 + correctAnswer;
				questionText.text = arg1.ToString () + " + ? = " + arg2.ToString();
				break;
			case 4:
				arg1Digit10 = Random.Range(1, 9);
				arg1Digit1 = Random.Range(1, 10);
				
				arg1 = arg1Digit10*10 + arg1Digit1;
				correctAnswer = Random.Range(10, arg1);
				arg2 = arg1 - correctAnswer;
				questionText.text = arg1.ToString () + " - ? = " + arg2.ToString();
				break;
				
			default:
				Debug.LogWarning("wrong at switch");
				break;
			}
			break;
		default:
			Debug.LogWarning("wrong at switch");
			break;
		}
		
		if (correctAnswer < 0)	Debug.LogWarningFormat("correctAnswer:{0}"	, correctAnswer); 
		if (arg1 < 0) 	Debug.LogWarningFormat("arg1:{0}"	, arg1);
		if (arg2 < 0) 	Debug.LogWarningFormat("arg2:{0}"	, arg2);
	}
	
	private void calcQuestionDigit3() {
		int qType = -1;
		int arg1Digit100 = 1000;
		int arg1Digit10 = 1000;
		int arg1Digit1 = 1000;
		int arg2Digit10 = 1000;
		int arg2Digit1 = 1000;
		
		int arg1 = -1;
		int arg2 = -1;
		
		switch (difficulty) {
		case 1:
			qType = Random.Range (1, 3);
			Debug.LogFormat ("calcQuestionDigit3: q_type:{0}", qType);
			
			switch(qType){
			case 1:	
				arg1Digit100 = Random.Range(1, 10);
				arg1Digit10 = Random.Range(0, 10);
				arg1Digit1 = Random.Range(1, 9);
				arg2Digit10 = Random.Range(1, 10-arg1Digit10);
				arg2Digit1 = Random.Range(0, 10-arg1Digit1);
				
				arg1 = arg1Digit100*100 + arg1Digit10*10 + arg1Digit1;
				arg2 = arg2Digit10*10 + arg2Digit1;
				correctAnswer = arg1 + arg2;
				questionText.text = arg1.ToString () + " + " + arg2.ToString() + " = ?";
				break;
			case 2 :
				arg1Digit100 = Random.Range(1, 10);
				arg1Digit10 = Random.Range(1, 10);
				arg1Digit1 = Random.Range(1, 9);
				arg2Digit10 = Random.Range(1, arg1Digit10 + 1);
				arg2Digit1 = Random.Range(1, arg1Digit1 + 1);
				
				arg1 = arg1Digit100*100 + arg1Digit10*10 + arg1Digit1;
				arg2 = arg2Digit10*10 + arg2Digit1;
				correctAnswer = arg1 - arg2;
				questionText.text = arg1.ToString () + " - " + arg2.ToString() + " = ?";
				break;
			default:
				Debug.LogWarning("wrong at switch");
				break;
			}
			break;
			
		case 2:
			qType = Random.Range (1, 3);
			Debug.LogFormat ("calcQuestionDigit3: q_type:{0}", qType);
			
			switch(qType){
			case 1:	
				arg1Digit100 = Random.Range(1, 9);
				arg1Digit10 = Random.Range(0, 10);
				arg1Digit1 = Random.Range(0, 10);
				arg1 = arg1Digit100*100 + arg1Digit10*10 + arg1Digit1;
				arg2 = Random.Range(10, 1000-arg1);
				
				correctAnswer = arg1 + arg2;
				questionText.text = arg1.ToString () + " + " + arg2.ToString() + " = ?";
				break;
			case 2 :
				arg1Digit100 = Random.Range(1, 10);
				arg1Digit10 = Random.Range(0, 10);
				arg1Digit1 = Random.Range(0, 10);
				arg1 = arg1Digit100*100 + arg1Digit10*10 + arg1Digit1;
				arg2 = Random.Range(10, arg1);
				correctAnswer = arg1 - arg2;
				questionText.text = arg1.ToString () + " - " + arg2.ToString() + " = ?";
				break;
			default:
				Debug.LogWarning("wrong at switch");
				break;
			}
			break;
		case 3:
			qType = Random.Range (1, 3);
			Debug.LogFormat ("calcQuestionDigit3: q_type:{0}", qType);
			
			switch(qType){
			case 1:	
				arg1Digit100 = Random.Range(1, 9);
				arg1Digit10 = Random.Range(0, 10);
				arg1Digit1 = Random.Range(0, 10);
				arg1 = arg1Digit100*100 + arg1Digit10*10 + arg1Digit1;
				correctAnswer = Random.Range(10, 1000-arg1);
				
				arg2 = arg1 + correctAnswer;
				questionText.text = arg1.ToString () + " + ? = " + arg2.ToString();
				break;
			case 2:
				arg1Digit100 = Random.Range(1, 10);
				arg1Digit10 = Random.Range(0, 10);
				arg1Digit1 = Random.Range(0, 10);
				arg1 = arg1Digit100*100 + arg1Digit10*10 + arg1Digit1;
				correctAnswer = Random.Range(10, arg1);
				
				arg2 = arg1 - correctAnswer;
				questionText.text = arg1.ToString () + " - ? = " + arg2.ToString();
				break;
			default:
				Debug.LogWarning("wrong at switch");
				break;
			}
			break;
		default:
			Debug.LogWarning("wrong at switch");
			break;
		}
		
		if (correctAnswer < 0)	Debug.LogErrorFormat("correctAnswer:{0}"	, correctAnswer); 
		if (arg1 < 0) 	Debug.LogErrorFormat("arg1:{0}"	, arg1);
		if (arg2 < 0) 	Debug.LogErrorFormat("arg2:{0}"	, arg2);
	}
	
	private void calcQuestionOver100() {
		int qType = -1;
		int arg1 = -1;
		int arg2 = -1;
		int arg3 = -1;
		
		switch (difficulty) {
		case 1:
			qType = Random.Range (1, 3);
			Debug.LogFormat ("calcQuestionOver100: q_type:{0}", qType);
			
			switch (qType) {
			case 1:	
				arg1 = Random.Range (0, 10);
				arg2 = Random.Range (0, 10);
				arg3 = Random.Range (0, 10);
				correctAnswer = arg1 * 100 + arg2 * 10 + arg3;
				questionText.text = "100を" + arg1.ToString () + "こ，10を" + arg2.ToString () + "こ，1を" + arg3.ToString () + "こあつめた数は?です";
				break;
			case 2:
				if (Random.Range (0, 2) == 0) {
					arg1 = Random.Range (1, 6) * 100;
					correctAnswer = arg1 + 400;
					questionText.text= arg1.ToString () + "-" + (arg1 + 100).ToString () + "-" + (arg1 + 200).ToString () + "-" + (arg1 + 300).ToString () + "-?";
				} else {
					arg1 = Random.Range (1, 7) * 100 + 50;
					correctAnswer = arg1 + 100;
					questionText.text = arg1.ToString () + "-" + (arg1 + 50).ToString () + "-?-" + (arg1 + 150).ToString () + "-" + (arg1 + 200).ToString ();
				}
				break;
			default:
				Debug.LogWarning ("wrong at switch");
				break;
			}
			break;
		case 2:
			qType = Random.Range (1, 3);
			Debug.LogFormat ("calcQuestionOver100: q_type:{0}", qType);
			
			switch (qType) {
			case 1:	
				arg1 = (int) Mathf.Pow (10, Random.Range(0, 3));
				arg2 = Random.Range (10, 100);
				correctAnswer = arg1 * arg2;
				questionText.text = arg1.ToString () + "を" + arg2.ToString () + "こ、あつめたかずは?";
				break;
			case 2:	
				arg1 = Random.Range (1, 10) * 100 + Random.Range (0, 10) * 10;
				correctAnswer = arg1 / 10;
				questionText.text = arg1.ToString () + "は10を?こあつめたかずです";
				break;
			default:
				Debug.LogWarning ("wrong at switch");
				break;
			}
			break;
			
		case 3:
			qType = Random.Range (1, 5);
			Debug.LogFormat ("calcQuestionOver100: q_type:{0}", qType);
			
			switch (qType) {
			case 1:	
				arg1 = Random.Range (100, 1000);
				arg2 = Random.Range (10, 100);
				correctAnswer = arg1 - arg2;
				questionText.text = arg1.ToString () + "より" + arg2.ToString () + "小さいかずは?";
				break;
			case 2:	
				arg1 = Random.Range (100, 1000);
				arg2 = Random.Range (10, 100);
				correctAnswer = arg1 + arg2;
				questionText.text = arg1.ToString () + "より" + arg2.ToString () + "大きいかずは?";
				break;
			case 3:
				arg1 = Random.Range (100, 1000);
				arg2 = Random.Range (10, 100);
				arg3 = Random.Range (10, arg1 + arg2);
				correctAnswer = arg1 + arg2 - arg3;
				questionText.text = arg1.ToString () + "より" + arg2.ToString () + "大きく" + arg3.ToString () + "小さいかずは?";
				break;
			case 4:
				arg1 = Random.Range (100, 964);	//1000-36
				arg2 = Random.Range (2, 10);
				correctAnswer = arg1 + arg2 * 4;
				questionText.text = arg1.ToString () + "-" + (arg1 + arg2).ToString() + "-" + (arg1 + arg2 * 2).ToString() + "-" + (arg1 + arg2 * 3).ToString() + "-?";
				break;
			default:
				Debug.LogWarning ("wrong at switch");
				break;
			}
			break;
			
		default:
			Debug.LogWarning ("wrong at switch");
			break;	
		}
		
		if (correctAnswer < 0)	Debug.LogErrorFormat("correctAnswer:{0}"	, correctAnswer); 
		
	}
	
	private void calcQuestionOver1000() {
		int qType = -1;
		int arg0 = -1;
		int arg1 = -1;
		int arg2 = -1;
		int arg3 = -1;
		
		switch (difficulty) {
		case 1:
			qType = Random.Range (1, 3);
			Debug.LogFormat ("calcQuestionOver1000: q_type:{0}", qType);
			
			switch (qType) {
			case 1:	
				arg0 = Random.Range (0, 10);
				arg1 = Random.Range (0, 10);
				arg2 = Random.Range (0, 10);
				arg3 = Random.Range (0, 10);
				correctAnswer = arg0*1000 + arg1 * 100 + arg2 * 10 + arg3;
				questionText.text = "1000を" + arg0.ToString () + "こ，100を" + arg1.ToString () + "こ，10を" + arg2.ToString () + "こ，1を" + arg3.ToString () + "こあつめた数は?です";
				break;
			case 2:
				if (Random.Range (0, 2) == 0) {
					arg1 = Random.Range (1, 6) * 1000;
					correctAnswer = arg1 + 4000;
					questionText.text= arg1.ToString () + "-" + (arg1 + 1000).ToString () + "-" + (arg1 + 2000).ToString () + "-" + (arg1 + 3000).ToString () + "-?";
				} else {
					arg1 = Random.Range (1, 7) * 1000 + 500;
					correctAnswer = arg1 + 1000;
					questionText.text = arg1.ToString () + "-" + (arg1 + 500).ToString () + "-?-" + (arg1 + 1500).ToString () + "-" + (arg1 + 2000).ToString ();
				}
				break;
			default:
				Debug.LogWarning ("wrong at switch");
				break;
			}
			break;
		case 2:
			qType = Random.Range (1, 3);
			Debug.LogFormat ("calcQuestionOver1000: q_type:{0}", qType);
			
			switch (qType) {
			case 1:	
				arg1 = (int) Mathf.Pow (10, Random.Range(0, 4));
				arg2 = Random.Range (10, 100);
				correctAnswer = arg1 * arg2;
				questionText.text = arg1.ToString () + "を" + arg2.ToString () + "こ、あつめたかずは?";
				break;
			case 2:	
				arg1 = Random.Range (1, 10) * 1000 + Random.Range (0, 10) * 100;
				correctAnswer = arg1 / 100;
				questionText.text = arg1.ToString () + "は100を?こあつめたかずです";
				break;
			default:
				Debug.LogWarning ("wrong at switch");
				break;
			}
			break;
			
		case 3:
			qType = Random.Range (1, 5);
			Debug.LogFormat ("calcQuestionOver1000: q_type:{0}", qType);
			
			//			switch (qType) {
			//			case 1:	
			//				arg1 = Random.Range (100, 1000);
			//				arg2 = Random.Range (10, 100);
			//				correctAnswer = arg1 - arg2;
			//				questionText.text = arg1.ToString () + "より" + arg2.ToString () + "小さいかずは?";
			//				break;
			//			case 2:	
			//				arg1 = Random.Range (100, 1000);
			//				arg2 = Random.Range (10, 100);
			//				correctAnswer = arg1 + arg2;
			//				questionText.text = arg1.ToString () + "より" + arg2.ToString () + "大きいかずは?";
			//				break;
			//			case 3:
			//				arg1 = Random.Range (100, 1000);
			//				arg2 = Random.Range (10, 100);
			//				arg3 = Random.Range (10, arg1 + arg2);
			//				correctAnswer = arg1 + arg2 - arg3;
			//				questionText.text = arg1.ToString () + "より" + arg2.ToString () + "大きく" + arg3.ToString () + "小さいかずは?";
			//				break;
			//			case 4:
			//				arg1 = Random.Range (100, 964);	//1000-36
			//				arg2 = Random.Range (2, 10);
			//				correctAnswer = arg1 + arg2 * 4;
			//				questionText.text = arg1.ToString () + "-" + (arg1 + arg2).ToString() + "-" + (arg1 + arg2 * 2).ToString() + "-" + (arg1 + arg2 * 3).ToString() + "-?";
			//				break;
			//			default:
			//				Debug.LogWarning ("wrong at switch");
			//				break;
			//			}
			break;
			
		default:
			Debug.LogWarning ("wrong at switch");
			break;	
		}
		
		if (correctAnswer < 0)	Debug.LogErrorFormat("correctAnswer:{0}"	, correctAnswer); 
		
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
