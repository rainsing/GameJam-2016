using UnityEngine;
using System.Collections;
using Assets.ProgressBars.Scripts;

public class BottomHUD : MonoBehaviour {
	private GuiProgressBar _prograssBar;
	private TextMesh _scoreText;
	private int _totalScore = 0;
	private int _oldScore = 0;
	//static private float _barHeight = 6.0f;
	private CharacterBehaviour gameSetttings;

	private float timebarSpeed = 0.02f;
	public float TimeBarSpeed {
		get { return timebarSpeed; }
		set { timebarSpeed = value; }
	}

	// Use this for initialization
	void Start () {
		_prograssBar = GetComponentInChildren<GuiProgressBar> ();
		_prograssBar.Value = 1.0f;

		_scoreText = GetComponentInChildren<TextMesh> ();
		_scoreText.text = _totalScore.ToString();

		gameSetttings = GameObject.FindGameObjectWithTag ("GameController").GetComponent<CharacterBehaviour>();
	}
	
	// Update is called once per frame
	void Update () {

		float deltaPrograss = Time.deltaTime * timebarSpeed;
		_prograssBar.Value -= deltaPrograss;
		if (_prograssBar.Value < 0) {
			//GAME OVER
			gameSetttings.OnGameOver();
		}

		if (_oldScore != _totalScore) {
			_oldScore = _totalScore;
			_scoreText.text =  _totalScore.ToString();
		}
	}

	public void AddBonus(float bonus)
	{
		_prograssBar.Value += bonus;
		if (_prograssBar.Value > 1)
			_prograssBar.Value = 1.0f;
		else if (_prograssBar.Value < 0) {
			//GAME OVER
			gameSetttings.OnGameOver();
		}		
	}

	public void AddScore(int delta)
	{
		_totalScore += delta;
	}

	public void Reset()
	{
		_totalScore = 0;
		_prograssBar.Value = 1.0f;
	}
}
