using UnityEngine;
using System.Collections;

public class CharacterBehaviour : MonoBehaviour {

	public GameObject bottomUI;
	BottomHUD bottomHUD;
	public int scoreSpeed = 100;
	public float timebarBonus = 0.2f;

	[System.Serializable]
	public class LevelSettings
	{
		public float timebarSpeed = 0.02f;
		public float wallSpeed = 0.02f;
		public float waitTime = 3.0f;
		public float walkTime = 1.0f;
	}
	public LevelSettings[] levelSettings;

	public GameObject characterPrefab;
	GameObject[] characters;
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
		bottomHUD = bottomUI.GetComponent<BottomHUD> ();

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

			if (level < levelSettings.Length) {
				curWaitTime = levelSettings [level].waitTime;
				curWalkTime = levelSettings [level].walkTime;
				bottomHUD.TimeBarSpeed = levelSettings [level].timebarSpeed;
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

		UpdateQueue ();
	}

	void UpdateQueue()
	{
		//check head return
		Vector3 dir0 = SpawnPoint.transform.position - EndPoint.transform.position;

		int index = _realQueue [0];
		GameObject curCharacter = characters [index];

		Vector3 dir1 = curCharacter.transform.position - EndPoint.transform.position;

		if (Vector3.Dot (dir0, dir1) < 0) {
			//int lastIndex = _realQueue [SpawnCount - 1];
			//curCharacter.transform.position = characters[lastIndex].transform.position - SpawnDistance * faceDir;
			KickBack(index);
			Restart (index);
			RestartPos (index);
		}

		//check refilling
		if (!_moveOrWait) {
			for (int i = 1; i < SpawnCount; i++) {
				int frontId = _realQueue [i - 1];
				int curId = _realQueue [i];
				GameObject frontObj = characters [frontId];
				GameObject curObj = characters [curId];
				Vector3 vDist = frontObj.transform.position - curObj.transform.position;
				if (vDist.sqrMagnitude > SpawnDistance * SpawnDistance) {
					//move back obj forward
					for (int j = i; j < SpawnCount; j++) {
						int curIndex = _realQueue [j];
						characters [curIndex].GetComponent<Character> ().Walk ();
					}
					break;
				} else {
					characters [curId].GetComponent<Character> ().Stop ();
				}
			}
		}
	}

	public void KickBack(int index)
	{
		Vector3 faceDir = Door.transform.position - SpawnPoint.transform.position;
		faceDir.y = 0.0f;
		faceDir.Normalize ();

		int lastIndex = _realQueue [SpawnCount - 1];
		GameObject curObj = characters [index];
		curObj.transform.position = characters[lastIndex].transform.position - SpawnDistance * faceDir;
		Restart (index);
		RestartPos (index);
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
			int moveIndex = _realQueue[i];
			Character moveCharacter = characters [moveIndex].GetComponent<Character> ();
			moveCharacter.QueueIndex = i - 1;			 
		}
		_realQueue [SpawnCount - 1] = curCharacter.Index;
		curCharacter.QueueIndex = SpawnCount - 1;
	}

	public void SetCurPick(int index)
	{
		if (_oldCharacterIndex == -1 || _oldCharacterIndex == index) {
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
		//after short time with face, return back
		Character curCharacter = characters [newIndex].GetComponent<Character> ();
		Character oldCharacter = characters [oldIndex].GetComponent<Character> ();

		curCharacter.PrepareForKick ();
		oldCharacter.PrepareForKick ();

		bottomHUD.AddBonus (timebarBonus);
		bottomHUD.AddScore (scoreSpeed);

		if (level < levelSettings.Length)
			Global.WallProgress += levelSettings [level].wallSpeed;
	}

	void OnWrong(int oldIndex, int newIndex)
	{
		Debug.Log ("Wrong!!!!!");
		Character curCharacter = characters [newIndex].GetComponent<Character> ();
		Character oldCharacter = characters [oldIndex].GetComponent<Character> ();
		curCharacter.ForceTurnBack ();
		oldCharacter.ForceTurnBack ();

		bottomHUD.AddBonus (-0.25f * timebarBonus);
	}
}
