using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	public int SelectLevel{ get; set; }
	void Awake() {
		DontDestroyOnLoad(this);
	}

	void Start () {
	}


	// Update is called once per frame
	void Update () {
	
	}
}
