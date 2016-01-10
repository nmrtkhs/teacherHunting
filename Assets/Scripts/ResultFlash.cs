using UnityEngine;
using System.Collections;

public class ResultFlash : MonoBehaviour {

	public GameObject EventSystem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FlashAnimationFinish() {
		EventSystem.GetComponent<ResultScene>().ResultInActvie = true;	
	}
}
