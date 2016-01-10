using UnityEngine;
using System.Collections;

public class ScoreObject : MonoBehaviour {

	// Use this for initialization
	public int Score = 0;
	void Awake() {
		DontDestroyOnLoad(this);
	}
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
