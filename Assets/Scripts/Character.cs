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

	private SpriteRenderer m_SpriteRenderer;
	private SpriteRenderer m_SpriteFace;
	private float m_AccumulatedTime = 0.0f;
	private bool m_EvenFrame = true;
	private bool m_Turning = false;
	private float m_TurnTimer;

	private GameObject Door;
	private Vector3 _faceDir = Vector3.zero;

	private int _moveState = 0;//0=stop;1=walking;2=turn;

	private CharacterBehaviour gameSetttings;

	private int _index;
	public int Index
	{
		get { return _index; }
		set { _index = value; }
	}



	void Awake ()
	{
		m_SpriteRenderer = GetComponent<SpriteRenderer> ();
		m_SpriteFace = GetComponentsInChildren<SpriteRenderer> ()[1];
	}

	public void SetFace(Sprite face)
	{
		m_SpriteFace.sprite = face;
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
		if (_moveState == 0) 
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
		else if (_moveState == 1)
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
		}
	}

	public void Walk()
	{
		_moveState = 1;
	}

	public void Stop()
	{
		_moveState = 0;
	}

	void ForceTurnBack()
	{
		if (m_Turning) {
			m_SpriteRenderer.sprite = m_EvenFrame ? turned0 : turned1;
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
