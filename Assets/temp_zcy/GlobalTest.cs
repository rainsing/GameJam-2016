using UnityEngine;
using System.Collections;

public class GlobalTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Global.Level = 100;
		Debug.Log ("Level:" + Global.Level);

		Global.TimeBar = 5.6f;
		Debug.Log ("TimeBar:" + Global.TimeBar);

		Global.Score = 500;
		Debug.Log ("Score:" + Global.Score);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
