using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour {

	public Text[] choiceText;
	public Text questionText;

	private List<string> questionList;
	private List<List<string>> answerList;
	
	private int currentQuestionIndex;
	private int correctAnswer;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadQuestion (int stage) {
		questionList = new List<string>();
		answerList = new List<List<string>>();

		questionList.Add ("Question0");
		questionList.Add ("Question1");
		//Load data from Resources/csv
	}

	void SetQuestion (int difficulity) {

		int choiceNum = choiceText.Length;	//4

		currentQuestionIndex = Random.Range (0, questionList.Count);
		correctAnswer = Random.Range (0, choiceNum);

		string[] choice = new string[choiceNum];

		for (int i = 0; i < choiceNum && i < answerList[currentQuestionIndex].Count; i++) {
			choice [i] = answerList [currentQuestionIndex] [i];
		}

		string tmpChoice = choice [correctAnswer];
		choice [correctAnswer] = choice[0];
		choice [0] = tmpChoice;


		//set question and choices to textUI

		questionText.text = questionList [currentQuestionIndex];
		for (int i = 0; i < choiceNum; i++) {
			choiceText [i].text = choice [i];
		}

	}

	bool IsCorrectAnswer (int answer_index){
		return (answer_index == correctAnswer);
	}
	
}
