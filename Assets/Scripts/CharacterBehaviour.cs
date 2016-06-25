using UnityEngine;
using System.Collections;

public class CharacterBehaviour : MonoBehaviour {

	public GameObject characterPrefab;
	GameObject[] characters;
	public float[] waitTime;
	public float[] walkTime;
	public int level = 0;
	bool _moveOrWait = false;
	bool _forceWait = false;
	float _accTime = 0.0f;
	bool _levelInitialized = false;
	float curWaitTime;
	float curWalkTime;

	public Sprite[] faceArray;
	public Vector3 faceOffset;

	public GameObject SpawnPoint;
	public GameObject EndPoint;
	public GameObject Door;
	public float SpawnDistance;
	public int SpawnCount;

	private Vector3 spawnDir;


	// Use this for initialization
	void Start () {
		Vector3 startPos = Door.transform.position;
		spawnDir = startPos - SpawnPoint.transform.position;
		spawnDir.y = 0;
		spawnDir.Normalize ();

		characters = new GameObject[SpawnCount];
		Vector3 spawnPos = startPos + new Vector3(0,3.03f, 0);
		for (int i = 0; i < SpawnCount; i++) {
			spawnPos -= SpawnDistance * spawnDir;
			characters[i] = GameObject.Instantiate (characterPrefab, spawnPos, characterPrefab.transform.rotation) as GameObject;
		}


		
		//characters = GameObject.FindGameObjectsWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (!_levelInitialized) {
			_accTime = 0.0f;
			_forceWait = false;
			_moveOrWait = false;

			if (level < waitTime.Length && level < walkTime.Length) {
				curWaitTime = waitTime [level];
				curWalkTime = walkTime [level];
			}
			_levelInitialized = true;

		}


		_accTime += Time.deltaTime;

		if (_moveOrWait == false && !_forceWait && _accTime > curWaitTime) {
			//transform to walk
			foreach (GameObject character in characters) {
				Character curCharacter = character.GetComponent<Character> ();
				curCharacter.Walk ();
			}
			_accTime = 0.0f;
			_moveOrWait = true;
		}
		else if (_moveOrWait == true && (_forceWait || _accTime > curWalkTime))
			{
				//transform to wait
				foreach (GameObject character in characters) {
					Character curCharacter = character.GetComponent<Character> ();
					curCharacter.Stop ();
				}
				_accTime = 0.0f;
				_moveOrWait = false;
			}
	}
}
