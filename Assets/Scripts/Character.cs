using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	public Sprite normal0;
	public Sprite normal1;
	public Sprite turned0;
	public Sprite turned1;

	public float speed = -2.0f;
	public float animInterval = 0.3f;
	public float turnDuration = 2.0f;

	private SpriteRenderer m_SpriteRenderer;
	private float m_AccumulatedTime = 0.0f;
	private bool m_EvenFrame = true;
	private bool m_Turning = false;
	private float m_TurnTimer;

	void Awake ()
	{
		m_SpriteRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update ()
	{
		m_AccumulatedTime += Time.deltaTime;
		if (m_AccumulatedTime > animInterval) 
		{
			m_EvenFrame = !m_EvenFrame;
			m_AccumulatedTime -= animInterval;

			if (m_EvenFrame) {
				this.transform.position += new Vector3 (0.0f, 0.1f, 0.0f);
			} 
			else 
			{
				this.transform.position += new Vector3 (0.0f, -0.1f, 0.0f);
			}
		}

		if (m_Turning) 
		{
			m_TurnTimer -= Time.deltaTime;
			if (m_TurnTimer <= 0.0f)
				m_Turning = false;
		}

		this.transform.LookAt (
			transform.position + Camera.main.transform.rotation * Vector3.forward, 
			Camera.main.transform.rotation * Vector3.up
		);
			
		this.transform.position += new Vector3 (speed * Time.deltaTime, 0.0f, 0.0f);

		if (m_Turning)
			m_SpriteRenderer.sprite = m_EvenFrame ? turned0 : turned1;
		else
			m_SpriteRenderer.sprite = m_EvenFrame ? normal0 : normal1;
	}

	void OnMouseDown ()
	{
		if (!m_Turning) 
		{
			m_Turning = true;
			m_TurnTimer = turnDuration;
		}
	}
}
