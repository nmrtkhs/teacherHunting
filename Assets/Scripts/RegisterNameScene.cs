using UnityEngine;
using UnityEngine.UI;


public class RegisterNameScene : MonoBehaviour {

	InputField _inputFieldName;
	Text _textWarning;
	// Use this for initialization
	void Start () {
		_inputFieldName = GameObject.Find ("InputFieldName").GetComponent<InputField> ();
		_textWarning = GameObject.Find ("TextWarning").GetComponent<Text> ();
		_textWarning.enabled = false;
		var name = PlayerPrefs.GetString ("PlayerName");
		if (!string.IsNullOrEmpty (name)) {
			UnityEngine.Debug.Log (name);
			_inputFieldName.text = name;
		}
	}

	// Update is called once per frame
	void Update () {
	}

	public void OnNextClicke() {
		if (string.IsNullOrEmpty(_inputFieldName.text)) {
			_textWarning.enabled = true;
			return;
		}
		PlayerPrefs.SetString ("PlayerName", _inputFieldName.text);
		GameManager.instance.name = _inputFieldName.text;
		Application.LoadLevel ("RegisterNew");
	}

	public void OnBackClick() {
		PhotonNetwork.Disconnect ();
		Application.LoadLevel ("Lobby");
	}
}
