using UnityEngine;
using System.Collections;
using Assets.ProgressBars.Scripts;

public class BottomHUD : MonoBehaviour {

	public float timebarSpeed = 0.1f;
	public int scoreSpeed = 100;
	private GuiProgressBar _prograssBar;
	private TextMesh _scoreText;
	private int _totalScore = 0;
	private int _oldScore = 0;
	static private float _barHeight = 6.0f;



	// Use this for initialization
	void Start () {
		_prograssBar = GetComponentInChildren<GuiProgressBar> ();
		_prograssBar.Value = 1.0f;

		_scoreText = GetComponentInChildren<TextMesh> ();
		_scoreText.text = _totalScore.ToString();
	}
	
	// Update is called once per frame
	void Update () {

		float deltaPrograss = Time.deltaTime * timebarSpeed;
		_prograssBar.Value -= deltaPrograss;
		if (_prograssBar.Value < 0) {
			_prograssBar.Value += 1.0f;
			AddScore (scoreSpeed);
		}

		if (_oldScore != _totalScore) {
			_oldScore = _totalScore;
			_scoreText.text =  _totalScore.ToString();
		}
	}

	void AddBonus(float bonus)
	{
		_prograssBar.Value += bonus;
		if (_prograssBar.Value > 1)
			_prograssBar.Value = 1.0f;
	}

	void AddScore(int delta)
	{
		_totalScore += delta;
	}
}
