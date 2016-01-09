using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	public int SelectLevel{ get; set; }
	public int Score{ get; set; }
	public int CharacterId{ get; set; }
    public int difficulty{ get; set; }

	static public GameManager instance;
	void Awake ()
	{
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else
		{
			Destroy (gameObject);
		}
	}
}
