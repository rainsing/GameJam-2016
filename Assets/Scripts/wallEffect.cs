using UnityEngine;
using System.Collections;

public class wallEffect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		Global.WallProgress += 0.003f;
		if (Global.WallProgress >= 1.0f) {
			Global.WallProgress = 0.0f;
		}
		GetComponent<SpriteRenderer> ().material.SetFloat ("_FinalScale", Global.WallProgress);
	}
}
