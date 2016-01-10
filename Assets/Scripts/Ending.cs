using UnityEngine;
using System.Collections;

public class Ending: MonoBehaviour {

	public GameObject EventSystem;

	public void EndingAnimationFinish() {
		EventSystem.GetComponent<ResultScene>().ResultOn();	
	}

}
