using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	public int SelectLevel{ get; set; }
	public int Score{ get; set; }

	static public GameManager instance;
	void Awake ()
	{
		if (instance == null) {

			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else {

			Destroy (gameObject);
		}

	}
//	void Awake() {
//		DontDestroyOnLoad(this);
//	}

	void Start () {
	}


	// Update is called once per frame
	void Update () {
	
	}
}
