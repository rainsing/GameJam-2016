using UnityEngine;
using System.Collections;

public class CharacterBehaviour : MonoBehaviour {

	public GameObject bottomUI;
	BottomHUD bottomHUD;
	public int scoreSpeed = 100;
	public float timebarBonus = 0.2f;

	public GameObject levelUI;

	public GameObject gameoverUI;


	[System.Serializable]
	public class LevelSettings
	{
		public float timebarSpeed = 0.02f;
		public float wallSpeed = 0.02f;
		public float waitTime = 3.0f;
		public float walkTime = 1.0f;
		public int faceCount = 2;
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
	public float SpawnDistance;		// a.k.a start walking distance
	public float StopWalkingDistance = 1.8f;
	public int SpawnCount;

	private Vector3 spawnDir;

	private int _oldCharacterIndex = -1;

	private int[] _realQueue;

	private bool gameOvering = false;
	public bool GameOvering
	{
		get { return gameOvering; }
		set { gameOvering = value; }
	}


	// Use this for initialization
	void Start () 
	{
		SpawnPoint.GetComponent<MeshRenderer> ().enabled = false;

		bottomHUD = bottomUI.GetComponent<BottomHUD> ();

		TextMesh levelText = levelUI.GetComponentsInChildren<TextMesh> ()[1];
		levelText.text = level.ToString ();

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
		if (gameOvering)
			return;
		
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

		if (_moveOrWait == false && !_forceWait && _accTime > curWaitTime) 
		{
			// Only the first one in the queue moves according to current level's wait time / walk time.
			characters [_realQueue [0]].GetComponent<Character> ().Walk ();

			_accTime = 0.0f;
			_moveOrWait = true;
		}
		else if (_moveOrWait == true && (_forceWait || _accTime > curWalkTime))
		{
			// Only the first one in the queue moves according to current level's wait time / walk time.
			characters [_realQueue [0]].GetComponent<Character> ().Stop ();

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
		for (int i = 1; i < SpawnCount; i++) 
		{
			int frontId = _realQueue [i - 1];
			int curId = _realQueue [i];
			GameObject frontObj = characters [frontId];
			GameObject curObj = characters [curId];
			Vector3 vDist = frontObj.transform.position - curObj.transform.position;
			Character curChar = curObj.GetComponent<Character> ();

			if (!curChar.IsWalking() && vDist.sqrMagnitude > SpawnDistance * SpawnDistance) 
				curChar.Walk ();

			if (curChar.IsWalking() && vDist.sqrMagnitude < StopWalkingDistance * StopWalkingDistance)
				curChar.Stop ();
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

		// Make sure kicked back characters remain on the ground.
		Vector3 leveledPosition = curObj.transform.position;
		leveledPosition.y = SpawnPoint.transform.position.y + 3.03f;
		curObj.transform.position = leveledPosition;

		Restart (index);
		RestartPos (index);
	}

	public void Restart(int index)
	{
		int faceCount = Mathf.Min (faceArray.Length, levelSettings [level].faceCount);
		int faceID = Mathf.FloorToInt(Random.value * faceCount - 0.0001f);
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
		Character curCharacter = characters [index].GetComponent<Character> ();
		Global.ChangeWallFace = true;
		Global.WallFace = faceArray[curCharacter.FaceIndex];

		if (_oldCharacterIndex == -1 || _oldCharacterIndex == index) {
			_oldCharacterIndex = index;
			return;
		} 


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

		if (level < levelSettings.Length) {
			Global.WallProgress += levelSettings [level].wallSpeed;
			if (Global.WallProgress >= 1.0f)
				OnLevelUp ();
		}

		GameObject.Find ("SoundCorrect").GetComponent<AudioSource> ().Play ();
			
	}

	void OnWrong(int oldIndex, int newIndex)
	{
		Debug.Log ("Wrong!!!!!");
		Character curCharacter = characters [newIndex].GetComponent<Character> ();
		Character oldCharacter = characters [oldIndex].GetComponent<Character> ();
		curCharacter.ForceTurnBack ();
		oldCharacter.ForceTurnBack ();

		bottomHUD.AddBonus (-0.25f * timebarBonus);

		GameObject.Find ("SoundWrong").GetComponent<AudioSource> ().Play ();
	}

	void OnLevelUp()
	{
		//reset
		Global.WallProgress = 0.0f;
		level++;
		if (level >= levelSettings.Length)
			level = levelSettings.Length - 1;
		_levelInitialized = false;

		TextMesh levelText = levelUI.GetComponentsInChildren<TextMesh> ()[1];
		levelText.text = level.ToString ();
	}

	public void OnGameOver()
	{
		if (!gameOvering) {
			gameoverUI.SetActive (true);
			gameOvering = true;
		}
	}

	public void OnRetry()
	{
		if (gameOvering) {
			gameOvering = false;
			level = 0;
			_levelInitialized = false;
			gameoverUI.SetActive (false);
		}
	}
}
