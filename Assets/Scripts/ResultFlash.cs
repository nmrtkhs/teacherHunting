using UnityEngine;
using System.Collections;

public class ResultFlash : MonoBehaviour {

	public GameObject EventSystem;

	public void FlashAnimationFinish() {
		EventSystem.GetComponent<ResultScene>().BackToLobby();	
	}
}
