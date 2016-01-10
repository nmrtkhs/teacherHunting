using UnityEngine;
using System.Collections;

public class Ending: MonoBehaviour {

	public GameObject EventSystem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpeningAnimationFinish() {
		EventSystem.GetComponent<ResultScene>().ResultActvie = true;	
	}

}
