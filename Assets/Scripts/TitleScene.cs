using UnityEngine;
using System.Collections;

public class TitleScene : MonoBehaviour {
	public bool TitleActvie;
	public bool TitleInActvie;
	public GameObject Panel;
	public GameObject Opneing;
	public GameObject Flash;
	public GameObject Button;

	// Use this for initialization
	void Start () {
		AudioManager.Instance.PlayBGM ("stratbgm", 0.5f);
	}

	// Update is called once per frame
	void Update () {
		if(TitleActvie == true){
			TitleOn();
			TitleActvie = false;
		}

		if(TitleInActvie == true){
			SceneChange();
			TitleInActvie = false;
		}
		
	}

	public void TitleOn() {
		Panel.SetActive (true);
		Opneing.SetActive (false);
	}

	public void OnStartClick() {
		Debug.Log ("OnStartClick");
		Flash.GetComponent<Animator>().SetTrigger("OnClick");
		Button.GetComponent<Animator>().SetTrigger("OnClick");
	}

	public void SceneChange() {
		Debug.Log ("SceneChange");
		
		Application.LoadLevel ("Lobby");
	}
//	public void OnContinueClick() {
//		Application.LoadLevel ("Lobby");
//	}
}
