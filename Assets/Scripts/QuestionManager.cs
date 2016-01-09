using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Parse;

public class QuestionManager : MonoBehaviour {

	public Text [] choiceText;
	public Text questionText;

	private List<List<string>> questionList;		//Difficulty,QuestionIndex
	private List<List<List<string>>> answerList;	//Difficulty,QuestionIndex,Choices

	private int currentQuestionIndex;
	private int correctAnswer;
	private int currentStage;
	private int currentDifficulty;

	// Use this for initialization
	void Awake () {
		questionList = new List<List<string>>();
		questionList.Add(new List<string>());
		questionList.Add(new List<string>());
		questionList.Add(new List<string>());
		
		answerList = new List<List<List<string>>>();
		answerList.Add(new List<List<string>>());
		answerList.Add(new List<List<string>>());
		answerList.Add(new List<List<string>>());
	}

	// Update is called once per frame
	void Update () {

	}

	public void LoadQuestion (int stage) {

		currentStage = stage; 
		string fileName = "question/tsv/test";

		switch (stage){
		case 0:
			fileName = "question/tsv/0_2keta";
			questionText.fontSize = 50;
			break;
		case 1:
			fileName = "question/tsv/1_over100";
			questionText.fontSize = 40;
			break;
		case 2:
			fileName = "question/tsv/2_3keta2keta";
			questionText.fontSize = 50;
			break;
		case 3:
			fileName = "question/tsv/3_kuku";
			questionText.fontSize = 50;
			break;
		case 4:
			fileName = "question/tsv/4_till10000";
			questionText.fontSize = 34;
			break;
		case 5:		//all_stage
			for (int i = 0; i < 5; i++) { 
				LoadQuestion(i);
			}
			questionText.fontSize = 34;
			currentStage = 5; 
			return;
		default:
			Debug.LogErrorFormat("errorStageNum:{0}", stage);
			break;
		}

		TextAsset csv = Resources.Load(fileName) as TextAsset;
		StringReader reader = new StringReader(csv.text);
		int line = 0;

		while (reader.Peek() > -1) {
			string[] values = reader.ReadLine().Split('\t');

			if (line > 0){
				int difficulty = int.Parse(values[0]) - 1;	//csv...1~3 -> data...0~2
				questionList[difficulty].Add(values[1]);
				var answers = new List<string>();
				answers.Add(values[2]);
				answers.Add(values[3]);
				answers.Add(values[4]);
				answers.Add(values[5]);
				answerList[difficulty].Add(answers);
			}
			line++;
    	}

		Debug.LogFormat("QuestionLoaded :" + fileName + "  line:{0}", line);
	}

	public void SetQuestion (int difficulty) {

		int choiceNum = choiceText.Length;	//4
		difficulty--;
		currentDifficulty = difficulty;

		if (difficulty < 0 || difficulty > 3) {
			Debug.LogWarning ("Wrong Difficulty");
		}

		currentQuestionIndex = Random.Range (0, questionList[difficulty].Count);
		correctAnswer = Random.Range (0, choiceNum);

		string[] choice = new string[choiceNum];

		for (int i = 0; i < choiceNum && i < answerList[difficulty][currentQuestionIndex].Count; i++) {
			choice [i] = answerList[difficulty][currentQuestionIndex][i];
		}

		string tmpChoice = choice [correctAnswer];
		choice [correctAnswer] = choice[0];
		choice [0] = tmpChoice;

		//set question and choices to textUI	
		questionText.text = questionList[difficulty][currentQuestionIndex];
		for (int i = 0; i < choiceNum; i++) {
			choiceText [i].text = choice [i];
		}

	}

	public bool IsCorrectAnswer (int answer_index){
        bool isCorrect;
        if (answer_index == -1) {
            isCorrect = false;
        } else if (answer_index == correctAnswer) {
            isCorrect = true;
        } else {
            isCorrect = false;
        }
		SendToParse (isCorrect);
		return isCorrect;
	}

	void SendToParse (bool isCorrect){
//		var answerData = new Dictionary<string, string>{
//			{"stage", currentStage.ToString()},
//			{"difficulty", (currentDifficulty + 1).ToString()},
//			{"question", questionList[currentDifficulty][currentQuestionIndex]},
//			{"answer", isCorrect.ToString()}
//		};
//	
//		ParseAnalytics.TrackEventAsync ("Answer", answerData);
		
		ParseObject parseObject = new ParseObject("AnswerLog");
		parseObject["stage"] = currentStage;
		parseObject["difficulty"] = currentDifficulty + 1;
		parseObject["question"] = questionList[currentDifficulty][currentQuestionIndex];
		parseObject["isCorrect"] = isCorrect;

		parseObject.SaveAsync ().ContinueWith(task =>  {
			if(CheckTask("Save", task)){	
				//保存が成功するとObjectIdに一意の文字列が設定される
				Debug.Log("ObjectId : " + parseObject.ObjectId);
			}
		});

	}

	//タスクのチェックを行い、タスク成功時のみTrueを返す
	bool CheckTask(string taskName, System.Threading.Tasks.Task task){
		if (task.IsCanceled)
		{
			Debug.Log(taskName + "キャンセル");
		}
		else if (task.IsFaulted)
		{
			Debug.Log(taskName + "失敗");
			
			//エラーメッセージ
			using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator()) {
				if (enumerator.MoveNext()) {
					ParseException error = (ParseException) enumerator.Current;
					Debug.Log(error.Message);
				}
			}
		}
		else
		{
			Debug.Log(taskName + "成功");
			return true;
		}
		
		return false;
	}
}
