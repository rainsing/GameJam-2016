using UnityEngine;
using System.Collections;

public class UIEvent : MonoBehaviour {

	private CharacterBehaviour gameSettings;

	// Use this for initialization
	void Start () {
		gameSettings = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CharacterBehaviour>();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnRetry()
	{
		gameSettings.OnRetry ();
	}
}
