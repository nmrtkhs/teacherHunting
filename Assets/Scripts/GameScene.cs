using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameScene : MonoBehaviour {

	public float GameTime = 60f;
	private float currentTime = .0f;
	private float correctAnswer = .0f;
	private bool isStart = false;
	private PhotonView myPv;

	Text timeText;
	Text[] buttonText;
	Text scoreText;
	Text questionText;
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

		myPv = this.GetComponent<PhotonView>();
		GameManager.instance.Score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isStart) {
			return;
		}

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

	private void updateQuesttion() {
		int digit = (int)Mathf.Pow(10, difficulty);

		int arg1 = Random.Range (1, digit);
		int arg2 = Random.Range (1, digit);

		switch (GameManager.instance.SelectLevel) {
		bool isPlus = (int)Random.Range (0, 2) == 0 ? true : false;

		switch (gm.SelectLevel) {
		case 1:
			correctAnswer = arg1 + arg2;
			questionText.text = arg1.ToString () + " + " + arg2 + " = ?";
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

	public void onAnswerClick(int buttonNo) {
		if (!isStart) {
			return;
		}

		float answer = float.Parse (buttonText [buttonNo].text);
		if (correctAnswer == answer) {
			GameManager.instance.Score += 30;
			scoreText.text = GameManager.instance.Score.ToString ();
			myPv.RPC ("addScore", PhotonTargets.All, GameManager.instance.Score);
			difficulty++;
		} else {
			difficulty--;
			if (difficulty < 1) {
				difficulty = 1;
			}
		}
		updateQuesttion ();
	}

	public void onStart() {
		isStart = true;
		myPv.RPC("start",PhotonTargets.All);
	}
}
