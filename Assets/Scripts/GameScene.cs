using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameScene : MonoBehaviour {

    public Sprite[] circleSprites;

	private bool isStart = false;
	private bool isInitialized = false;
    private bool isAnswering;
	private PhotonView myPv;
	private int bossHP;
	private int lastCorrect = 0;
    private int questionIndex;
    private int answerNum;
    private int remainTurn;
    private float remainTime;

	private GameObject playerAttacks;
	private GameObject combo;
	private GameObject hpGuage;
	private GameObject startButton;
    private GameObject targetCircle;
    private GameObject panel;

	Text[] buttonText;
    Text timeText;
    Text turnText;
    Text difficultyText;
    Text questionText;
    Text answerText1;
    Text answerText2;
    Text answerText3;
    Text answerText4;

    Button[] answerButtons;

    GameObject joinedPlayers;
    List<GameObject> joinedPlayerObjectList;
    Dictionary<int, GameObject> characterDictionary;

	private int difficulty;
    private int correctCount;
	private QuestionManager questionManager;

	// Use this for initialization
	void Start () {
		buttonText = new Text[4];
  
		playerAttacks = GameObject.Find ("PlayerAttacks");
        combo = GameObject.Find("Combo");
		hpGuage = GameObject.Find("HpGuage");
        panel = GameObject.Find("Panel");

		startButton = GameObject.Find("StartButton");
		startButton.SetActive(false);

		myPv = this.GetComponent<PhotonView>();
		GameManager.instance.Score = 0;

		playerAttacks.SetActive(false);

		questionManager = this.GetComponent<QuestionManager> ();
		questionManager.LoadQuestion (GameManager.instance.SelectLevel);

        questionText = GameObject.Find ("QuestionText").GetComponent<Text> ();
        questionText.text = "みんなが入るまで待ってね";

        answerText1 = GameObject.Find ("AnswerText1").GetComponent<Text> ();
        answerText2 = GameObject.Find ("AnswerText2").GetComponent<Text> ();
        answerText3 = GameObject.Find ("AnswerText3").GetComponent<Text> ();
        answerText4 = GameObject.Find ("AnswerText4").GetComponent<Text> ();

        answerButtons = new Button[4];
        answerButtons [0] = GameObject.Find ("AnswerButton1").GetComponent<Button> ();
        answerButtons [1] = GameObject.Find ("AnswerButton2").GetComponent<Button> ();
        answerButtons [2] = GameObject.Find ("AnswerButton3").GetComponent<Button> ();
        answerButtons [3] = GameObject.Find ("AnswerButton4").GetComponent<Button> ();

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        questionIndex = 1;
        answerNum = -1;
        remainTurn = 10;
        remainTime = 10.0f;
        isAnswering = false;
        difficulty = GameManager.instance.difficulty + 1;
        difficultyText = GameObject.Find ("DifficultyText").GetComponent<Text>();
        difficultyText.text = "レベル" + difficulty.ToString ();

        turnText = GameObject.Find ("TurnText").GetComponent<Text> ();
        turnText.text = "残り10ターン";
        timeText = GameObject.Find ("TimeText").GetComponent<Text> ();
        timeText.text = "残り10秒";

        joinedPlayers = GameObject.Find ("JoinedPlayers");
        joinedPlayerObjectList = new List<GameObject> ();

        for (int i = 0; i < joinedPlayers.transform.childCount; i++) {
            GameObject targetObject = joinedPlayers.transform.GetChild (i).gameObject;
            joinedPlayerObjectList.Add (targetObject);
            targetObject.transform.GetChild(0).gameObject.SetActive(false);
            targetObject.SetActive(false);
        }
        GameManager.instance.Score = 0;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isStart) {
            int enableStart = 0;
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
                int isInGame = (int)PhotonNetwork.playerList [i].customProperties ["isInGame"];
                if (isInGame == 0) {
                    enableStart = 0;
                    break;
                }
                enableStart = 1;
            }
            if (enableStart == 1) {
                questionText.text = "Startボタンを押してね";
                startButton.SetActive (true);
                SetJoinedPlayer();
                myPv.RPC ("SetPlayerName", PhotonTargets.All, GameManager.instance.CharacterId, GameManager.instance.name);
            }
        }

		if (!isInitialized)
		{
			bossHP = 300 * PhotonNetwork.playerList.Length;
			isInitialized = true;
		}

		if (bossHP - GameManager.instance.Score <= 0)
		{
			AudioManager.Instance.StopBGM ();
			Application.LoadLevel ("Result");
		}

        if (isAnswering)
        {
            remainTime -= Time.deltaTime;
            timeText.text = "残り" + Mathf.Floor(remainTime).ToString() + "秒";

            if (remainTime <= 0)
            {
                isAnswering = false;
                remainTime = 10.0f;
                timeText.text = "おしまい";
                CheckAnswer();
            }
        }
	}

	[PunRPC]
	void addScore(int score){
		GameManager.instance.Score = score;
	}

	[PunRPC]
	void GameStart(){
		AudioManager.Instance.PlayBGM ("battlebgm");
		isStart = true;
        startButton.SetActive (false);
        StartQuestion();
	}

    [PunRPC]
    void SetCorrect(int characterId, int playerDifficulty)
    {
        correctCount++;
        GameObject target = characterDictionary [characterId].transform.GetChild (0).gameObject;
        target.SetActive (true);
        target.GetComponent<Image> ().sprite = circleSprites [playerDifficulty];
        GameManager.instance.Score += (playerDifficulty + 1) * 10;
    }

    [PunRPC]
    void SetPlayerName(int characterId, string playerName)
    {
        characterDictionary[characterId].transform.GetChild(1).gameObject.GetComponent<Text>().text = playerName;
    }

    void StartQuestion()
    {
        playerAttacks.SetActive(false);
        //for (int i = 0; i < joinedPlayerObjectList.Count; i++) {
        //    joinedPlayerObjectList [i].transform.GetChild (0).gameObject.SetActive (false);
        //}

        questionText.text = "第" + questionIndex.ToString() + "問";
        answerNum = -1;
        Invoke ("SetQuestion", 1.0f);
    }

    void SetQuestion()
    {
        questionManager.SetQuestion(difficulty);

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(true);
        } 
        questionText = questionManager.questionText;
        answerText1 = questionManager.choiceText [0];
        answerText2 = questionManager.choiceText [1];
        answerText3 = questionManager.choiceText [2];
        answerText4 = questionManager.choiceText [3];

        questionIndex++;
        isAnswering = true;
    }

    void SetJoinedPlayer()
    {
        characterDictionary = new Dictionary<int, GameObject> ();
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
            int characterId = (int)PhotonNetwork.playerList[i].customProperties["characterId"];
            joinedPlayerObjectList[i].SetActive(true);
            joinedPlayerObjectList[i].GetComponent<Image>().sprite = Resources.Load<Sprite> ("avatar/avatar_" + characterId);
            characterDictionary.Add(characterId, joinedPlayerObjectList[i]);
        }
    }

    void SetAttack()
    {
        int i = 0;
        foreach (GameObject target in characterDictionary.Values)
        {
            GameObject circle = target.transform.GetChild(0).gameObject;
            if (circle.activeSelf)
            {
                iTween.MoveTo(circle, iTween.Hash(
                    "position", panel.transform.position,
                    "time", 0.5f, 
                    "easeType", "linear",
                    "oncomplete", "OnMoveComplete",
                    "oncompletetarget", gameObject,
                    "oncompleteparams", i
                ));

                iTween.ScaleTo(circle, iTween.Hash(
                    "scale", new Vector3(0.4f,0.4f,0.4f),
                    "time", 0.5f,
                    "easeType", "easeOutCubic"
                ));
            }
            else
            {
                OnMoveComplete(i);
            }
            i++;
        }
    }

    public void OnMoveComplete(object index)
    {
        if (int.Parse(index.ToString()) == characterDictionary.Count - 1)
        {
            foreach (GameObject target in joinedPlayerObjectList)
            {
                target.transform.GetChild(0).gameObject.transform.position = target.transform.position;
                iTween.ScaleTo(target.transform.GetChild(0).gameObject, Vector3.one, 0);
                target.transform.GetChild(0).gameObject.SetActive(false);
            }
            playerAttacks.SetActive(true);
            hpGuage.transform.localScale = new Vector3(1,(float)(bossHP - GameManager.instance.Score) / bossHP,1);
            Invoke ("StartQuestion", 2.0f);
        }
    }

	public void onAnswerClick(int buttonNo) {
        answerNum = buttonNo;
        foreach (Button button in answerButtons) {
            button.image.color = Color.white;
        }

        answerButtons [buttonNo].image.color = new Color (0.5f, 1, 1, 1);

//		if (!isStart) {
//			return;
//		}
//
//		if (questionManager.IsCorrectAnswer(buttonNo)) {
//			GameManager.instance.Score += (30 + 30 * lastCorrect / 2);
//			scoreText.text = GameManager.instance.Score.ToString ();
//			myPv.RPC ("addScore", PhotonTargets.All, GameManager.instance.Score);
//			myPv.RPC ("SetAttack", PhotonTargets.All, GameManager.instance.CharacterId, lastCorrect);
//			difficulty++;
//			if (difficulty > 3) {
//				difficulty = 3;
//			}
//
//			lastCorrect = 1;
//		} else {
//			difficulty--;
//			if (difficulty < 1) {
//				difficulty = 1;
//			}
//			lastCorrect = 0;
//		}
//		questionManager.SetQuestion(difficulty);
	}

    public void CheckAnswer()
    {
        correctCount = 0;
        bool isCorrect = questionManager.IsCorrectAnswer (answerNum);
        if (isCorrect) {
            questionText.text = "正解";
            myPv.RPC ("SetCorrect", PhotonTargets.All, GameManager.instance.CharacterId, difficulty - 1);
        } else {
            questionText.text = "不正解";
        }
        foreach (Button button in answerButtons) {
            button.image.color = Color.white;
        }
        remainTurn--;
        turnText.text = "残り" + remainTurn.ToString() + "ターン";
        Invoke ("SetAttack", 1.0f);
    }

	public void onStart() {
		isStart = true;
		myPv.RPC("GameStart",PhotonTargets.All);
	}
}
