using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameplayScreen : ScreenBase
{
	#region constants

	const float MAX_GO_TIMER = 1f;

	#endregion
	
	#region vars

	public JUIText CountDownObj;

	public JUIText P1ScoreObj;
	public JUIText P2ScoreObj;
	public JUIText P3ScoreObj;
	public JUIText P4ScoreObj;

	public JUIText HIScoreObj;
	public JUIText RoundObj;
	public JUIText EnemiesObj;
	public JUIText EscapeObj;

	private float mGoTimer = 0f;
	private float mEscapeTimer = 0f;

	#endregion
	
	#region properties

	public override ScreenID ID
	{
		get { return ScreenID.Gameplay; }
	}

	#endregion
	
	#region init

	protected override void Awake()
	{
		base.Awake();
	}
	
	// Use this for initialization
	protected override void Start()
	{
		base.Start();
	}

	public override void OnOpen()
	{
		base.OnOpen();

		// Set up player scores
		P1ScoreObj.gameObject.SetActive(true);
		P2ScoreObj.gameObject.SetActive(false);
		P3ScoreObj.gameObject.SetActive(false);
		P4ScoreObj.gameObject.SetActive(false);

		EscapeObj.gameObject.SetActive(false);

		UpdateMarkers();
		CountDownObj.gameObject.SetActive(true);
		CountDownObj.Text = "READY?";
		mGoTimer = MAX_GO_TIMER;
		mEscapeTimer = 0f;
	}

	#endregion

	#region update

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
		switch (World.Inst.State)
		{
			case World.GameState.Init:
				break;

			case World.GameState.Running:
				UpdateEnabledMarkers();
				if (mGoTimer > 0f)
				{
					mGoTimer -= Time.deltaTime;
					if (mGoTimer < 0f)
					{
						CountDownObj.gameObject.SetActive(false);
					}
					else
					{
						CountDownObj.Text = "GO!";
					}
				}

				if (World.Inst.EscapeModeOn)
				{
					mEscapeTimer += Time.deltaTime;

					while (mEscapeTimer > 2f)
						mEscapeTimer -= 2f;

					if (mEscapeTimer < 1f)
					{
						if (!EscapeObj.gameObject.activeSelf)
						{
							EscapeObj.gameObject.SetActive(true);
						}
					}
					else
					{
						if (EscapeObj.gameObject.activeSelf)
						{
							EscapeObj.gameObject.SetActive(false);
						}
					}
				}
				break;

			case World.GameState.Paused:
				break;

			case World.GameState.GameOver:
				break;

			case World.GameState.LevelStart:
				UpdateEnabledMarkers();
				//float f = World.Inst.CountDownTimer;
				CountDownObj.Text = "READY?";//Mathf.CeilToInt(f).ToString("0");
				break;

			case World.GameState.LevelCompleted:
				break;

			default:
				break;
		}

	}

	private string SetupScoreObj(GameSessionManager.PlayerSessionData _player)
	{
		return "P" + (_player.IDPlayer + 1).ToString("0") + " X " + _player.Lives.ToString("0") + " - " + _player.Score.ToString("00000000");
	}

	public void UpdateMarkers()
	{
		P1ScoreObj.Text = SetupScoreObj(GameSessionManager.Inst.PlayerData[0]);
		P2ScoreObj.Text = SetupScoreObj(GameSessionManager.Inst.PlayerData[1]);
		P3ScoreObj.Text = SetupScoreObj(GameSessionManager.Inst.PlayerData[2]);
		P4ScoreObj.Text = SetupScoreObj(GameSessionManager.Inst.PlayerData[3]);

		HIScoreObj.Text = "HI - " + GameSessionManager.Inst.HiScore.ToString("00000000");

		EnemiesObj.Text = GameSessionManager.Inst.EnemiesToDefeat.ToString("00") + "/" + GameSessionManager.Inst.TotalEnemies.ToString("00");

		RoundObj.Text = "ROUND " + GameSessionManager.Inst.Round.ToString("00");
	}

	public void UpdateEnabledMarkers()
	{
		P1ScoreObj.Text = SetupScoreObj(GameSessionManager.Inst.PlayerData[0]);
		P2ScoreObj.Text = SetupScoreObj(GameSessionManager.Inst.PlayerData[1]);
		P3ScoreObj.Text = SetupScoreObj(GameSessionManager.Inst.PlayerData[2]);
		P4ScoreObj.Text = SetupScoreObj(GameSessionManager.Inst.PlayerData[3]);

		HIScoreObj.Text = "HI - " + GameSessionManager.Inst.HiScore.ToString("00000000");

		EnemiesObj.Text = GameSessionManager.Inst.EnemiesToDefeat.ToString("00") + "/" + GameSessionManager.Inst.TotalEnemies.ToString("00");
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion
}
