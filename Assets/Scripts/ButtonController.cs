using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour {

	public GameObject gameobject;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick() {
		Debug.Log ("OnClickAnimation");
		gameobject.GetComponent<Animator>().SetTrigger("OnClick");
	}
}
