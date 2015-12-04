using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameScene : MonoBehaviour {

	public float GameTime = 30.0f;
	private float currentTime = .0f;
	private int totalScore = 0;
	private float correctAnswer = .0f;
	private bool isStart = false;
	private PhotonView myPv;

	Text timeText;
	Text[] buttonText;
	Text scoreText;
	Text questionText;
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

		myPv = PhotonView.Get(this);
		if(!myPv.isMine){
			this.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isStart) {
			Debug.Log("nanto");
			return;
		}

		// time update
		currentTime += Time.deltaTime;
		float leftTime = GameTime - currentTime;
		if (leftTime <= 0) {
			leftTime = .0f;
		}
		Debug.Log (leftTime);
		timeText.text = ((int)leftTime).ToString ();
	}

	[PunRPC]
	void addScore(int score){
		totalScore = score;
		scoreText.text = totalScore.ToString ();
	}

	private void updateQuesttion() {
		int arg1 = Random.Range (1, 100);
		int arg2 = Random.Range (1, 100);
		correctAnswer = arg1 + arg2;

		questionText.text = arg1.ToString () + " + " + arg2 + " = ?";

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
			totalScore += 30;
			scoreText.text = totalScore.ToString ();
			myPv.RPC("addScore",PhotonTargets.All,totalScore);
		}
		updateQuesttion ();
	}

	public void onStart() {
		isStart = true;
		updateQuesttion ();
		Debug.Log (PhotonNetwork.room.playerCount);
		//GameObject.Find ("StartButton").SetActive (false);
	}
}
