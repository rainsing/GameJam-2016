using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	public Sprite normal0;
	public Sprite normal1;
	public Sprite turned0;
	public Sprite turned1;

	public float speed = 2.0f;
	public float animInterval = 0.3f;
	public float turnDuration = 2.0f;
	public float forceBackDuration = 0.5f;

	private SpriteRenderer m_SpriteRenderer;
	private SpriteRenderer m_SpriteFace;
	private float m_AccumulatedTime = 0.0f;
	private bool m_EvenFrame = true;
	private bool m_Turning = false;
	private float m_TurnTimer;

	private GameObject Door;
	private Vector3 _faceDir = Vector3.zero;

	private bool _moveState = false;//false=stop;true=walking;

	private CharacterBehaviour gameSetttings;

	private int _index;
	public int Index
	{
		get { return _index; }
		set { _index = value; }
	}
	private int _faceIndex;
	public int FaceIndex
	{
		get { return _faceIndex; }
		set { _faceIndex = value; }
	}

	private int _queueIndex;
	public int QueueIndex
	{
		get { return _queueIndex; }
		set { _queueIndex = value; }
	}

	void Awake ()
	{
		m_SpriteRenderer = GetComponent<SpriteRenderer> ();
		m_SpriteFace = GetComponentsInChildren<SpriteRenderer> ()[1];
	}

	public void SetFace(Sprite face, int faceIndex)
	{
		m_SpriteFace.sprite = face;
		FaceIndex = faceIndex;
	}

	public void SetBody(Sprite body0, Sprite body1, Sprite body2, Sprite body3)
	{
		normal0 = body0;
		normal1 = body1;
		turned0 = body2;
		turned1 = body3;
	}

	void Start()
	{
		gameSetttings = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CharacterBehaviour>();
		Door = gameSetttings.Door;
		_faceDir = Door.transform.position - transform.position;
		_faceDir.y = 0.0f;
		_faceDir.Normalize ();
		m_SpriteFace.enabled = false;
	}

	void Update ()
	{
		if (_moveState) 
		{
			m_AccumulatedTime += Time.deltaTime;
			if (m_AccumulatedTime > animInterval) 
			{
				m_EvenFrame = !m_EvenFrame;
				m_AccumulatedTime -= animInterval;

				if (m_EvenFrame) 
					this.transform.position += new Vector3 (0.0f, 0.1f, 0.0f);
				else 
					this.transform.position += new Vector3 (0.0f, -0.1f, 0.0f);
			}

			this.transform.LookAt (
				transform.position + Camera.main.transform.rotation * Vector3.forward, 
				Camera.main.transform.rotation * Vector3.up
			);

			this.transform.position += speed * Time.deltaTime * _faceDir;
		}
		else
		{
			m_AccumulatedTime = 0;			
		}

		if (m_Turning) 
		{
			m_TurnTimer -= Time.deltaTime;
			if (m_TurnTimer <= 0.0f) {
				m_Turning = false;
				m_SpriteFace.enabled = false;
			}
		}

		if (m_Turning)
			m_SpriteRenderer.sprite = m_EvenFrame ? turned0 : turned1;
		else
			m_SpriteRenderer.sprite = m_EvenFrame ? normal0 : normal1;

		CheckPos ();
	}

	void OnMouseDown ()
	{
		if (!m_Turning) 
		{
			m_Turning = true;
			m_TurnTimer = turnDuration;
			m_SpriteFace.enabled = true;
			gameSetttings.SetCurPick (_index);
		}
	}

	public void Walk()
	{
		_moveState = true;
	}

	public void Stop()
	{
		_moveState = false;
	}

	public void ForceTurnBack()
	{
		if (m_Turning) {
			m_TurnTimer = forceBackDuration;
		}
	}

	public void CheckPos()
	{
		Vector3 dir0 = gameSetttings.SpawnPoint.transform.position - gameSetttings.EndPoint.transform.position;
		Vector3 dir1 = transform.position - gameSetttings.EndPoint.transform.position;
		if (Vector3.Dot (dir0, dir1) < 0) {
			transform.position -= _faceDir * gameSetttings.SpawnCount * gameSetttings.SpawnDistance;
			gameSetttings.Restart (Index);
		}
	}
}
