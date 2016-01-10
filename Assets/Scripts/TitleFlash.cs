using UnityEngine;
using System.Collections;

public class TitleFlash : MonoBehaviour {

	public GameObject EventSystem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FlashAnimationFinish() {
		EventSystem.GetComponent<TitleScene>().TitleInActvie = true;
	}
}
