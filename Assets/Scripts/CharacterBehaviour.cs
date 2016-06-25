using UnityEngine;
using System.Collections;

public class CharacterBehaviour : MonoBehaviour {

	public GameObject characterPrefab;
	GameObject[] characters;
	public float[] waitTime;
	public float[] walkTime;
	public int level = 0;
	bool _moveOrWait = true;
	bool _forceWait = false;
	float _accTime = 0.0f;
	bool _levelInitialized = false;
	float curWaitTime;
	float curWalkTime;

	[System.Serializable]
	public class BodySprites
	{
		public Sprite bodySprite0;
		public Sprite bodySprite1;
		public Sprite bodySprite2;
		public Sprite bodySprite3;
	}
	public BodySprites[] bodyArray;

	public Sprite[] faceArray;
	public Vector3 faceOffset;

	public GameObject SpawnPoint;
	public GameObject EndPoint;
	public GameObject Door;
	public float SpawnDistance;
	public int SpawnCount;

	private Vector3 spawnDir;

	private int _oldCharacterIndex = -1;

	private int[] _realQueue;


	// Use this for initialization
	void Start () {
		Vector3 startPos = Door.transform.position;
		spawnDir = startPos - SpawnPoint.transform.position;
		spawnDir.y = 0;
		spawnDir.Normalize ();

		characters = new GameObject[SpawnCount];
		_realQueue = new int[SpawnCount];
		Vector3 spawnPos = startPos + new Vector3(0,3.03f, 0);
		for (int i = 0; i < SpawnCount; i++) {
			spawnPos -= SpawnDistance * spawnDir;
			characters[i] = GameObject.Instantiate (characterPrefab, spawnPos, characterPrefab.transform.rotation) as GameObject;
			characters [i].GetComponent<Character> ().Index = i;
			Restart (i);
			_realQueue [i] = i;
			characters [i].GetComponent<Character> ().QueueIndex = i;
			characters [i].GetComponent<Character> ().Walk ();
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

	public void Restart(int index)
	{
		int faceID = Mathf.FloorToInt(Random.value * faceArray.Length - 0.0001f);
		Character curCharacter = characters [index].GetComponent<Character> ();
		curCharacter.SetFace (faceArray [faceID], faceID);
		int bodyID = Mathf.FloorToInt (Random.value * bodyArray.Length - 0.0001f);
		BodySprites bodySprites = bodyArray [bodyID];
		curCharacter.SetBody (bodySprites.bodySprite0, bodySprites.bodySprite1, bodySprites.bodySprite2, bodySprites.bodySprite3);
	}

	public void RestartPos(int index)
	{
		Character curCharacter = characters [index].GetComponent<Character> ();
		int queueIndex = curCharacter.QueueIndex;
		for (int i = queueIndex + 1; i < SpawnCount; i++) {
			_realQueue [i - 1] = _realQueue [i];
		}
		_realQueue [SpawnCount] = curCharacter.Index;		
	}

	public void SetCurPick(int index)
	{
		if (_oldCharacterIndex == -1) {
			_oldCharacterIndex = index;
			return;
		} 

		Character curCharacter = characters [index].GetComponent<Character> ();
		Character oldCharacter = characters [_oldCharacterIndex].GetComponent<Character> ();
		if (curCharacter.FaceIndex == oldCharacter.FaceIndex) {
			OnCorrect (_oldCharacterIndex, index);
			_oldCharacterIndex = -1;
		} else {
			OnWrong (_oldCharacterIndex, index);
			_oldCharacterIndex = -1;
		}
	}

	void OnCorrect(int oldIndex, int newIndex)
	{
		Debug.Log ("Correct!!!!!");
	}

	void OnWrong(int oldIndex, int newIndex)
	{
		Debug.Log ("Wrong!!!!!");
		Character curCharacter = characters [newIndex].GetComponent<Character> ();
		Character oldCharacter = characters [oldIndex].GetComponent<Character> ();
		curCharacter.ForceTurnBack ();
		oldCharacter.ForceTurnBack ();
	}
}
