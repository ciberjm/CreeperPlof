using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class World : MonoBehaviour
{
	#region constants

	public const float MAX_COUNTDOWN_TIMER = 1f;
	public const float MAX_PLAYERS_DEAD_TIMER = 3.00f;

	public enum GameState
	{
		Init = 0,
		Running = 1,
		Paused = 2,
		GameOver = 3,
		LevelStart = 4,
		LevelCompleted = 5,
		PlayersDead = 6,
	}

	public delegate void DelegateEventStateChanged(GameState _old, GameState _new);

	public static event DelegateEventStateChanged OnStateChanged;

	#endregion
	
	#region vars

	public bool ShowGUI = false;

	#endregion
	
	#region properties

	public float CountDownTimer { get; private set; }
	public float PlayersDeadTimer { get; private set; }

	public GameState LastState { get; private set; }
	public GameState State { get; private set; }

	public bool EscapeModeOn { get; private set; }

	public static World Inst { get; private set; }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
		Inst = this;
		State = GameState.Init;
		DontDestroyOnLoad(this);
	}
	
	// Use this for initialization
	protected virtual void Start()
	{

	}

	public void InitializeGame()
	{
		Debug.Log("Initializating game");
		SetState(GameState.Init);

		// Start game session manager
		GameSessionManager.Inst.InitGame();
	}

	public void ExitGame()
	{
		Debug.Log("Exiting Game...");

		// Remove all objects
		ScorePopupsManager.Inst.ClearAllPopups();

		Destroy(GameSessionManager.Inst.gameObject);
		Destroy(FindObjectOfType<Level>().gameObject);
		Destroy(this.gameObject);
		Loader.LoadTitleMenu();
	}

	public void LoadNextLevel()
	{
		// Clear stuff
		ScorePopupsManager.Inst.ClearAllPopups();

		GameSessionManager.Inst.NextLevel();

		InitializeLevel();
	}

	public void InitializeLevel()
	{
		ScorePopupsManager.Inst.ClearAllPopups();

		SetState(GameState.Init);
		Debug.Log("Initializating level");

		TiledMap.Inst.InitMap();

		// Setup enemies speed
		foreach (var e in GameSessionManager.Inst.Enemies)
		{
			e.SetupDifficultyLevel(GameSessionManager.Inst.Round);
		}

		// Start game session manager
		GameSessionManager.Inst.StartLevel();

		// Open gameplay screen
		ScreenManager.Inst.OpenScreen(ScreenID.Gameplay);

		Debug.Log("Start Game :D!");
		CountDownTimer = MAX_COUNTDOWN_TIMER;
		SetState(GameState.LevelStart);

		EscapeModeOn = false;
	}

	private void SetState(GameState _newState)
	{
		GameState oldState = State;
		State = _newState;
		if (OnStateChanged != null)
		{
			OnStateChanged(oldState, _newState);
		}
	}

	public void RestartLevel()
	{
		ScorePopupsManager.Inst.ClearAllPopups();

		// Reset tiles
		TiledMap.Inst.ResetLevel();

		// Reset GameProgressManager
		GameSessionManager.Inst.ResetLevel();

		SetState(GameState.LevelStart);

		Debug.Log("Restart Game :D!");
		CountDownTimer = MAX_COUNTDOWN_TIMER;
	
		// Reopen gameplay screen
		ScreenManager.Inst.OpenScreen(ScreenID.Gameplay);
	}

	public void FinishGame()
	{
		SetState(GameState.GameOver);
		ScreenManager.Inst.OpenScreen(ScreenID.GameOver);
	}

	public void FinishLevel()
	{
		Debug.Log("Level completed!");

		// Find all trapped and give points
		foreach (var v in GameSessionManager.Inst.Enemies)
		{
			EnemySpawnable es = v as EnemySpawnable;
			if (es != null)
			{
				if (es.BlockedBy == EnemySpawnable.BlockedByType.Player)
				{
					AddScore(1, 1000, es.transform.position, 0f);
				}
				else if (es.BlockedBy == EnemySpawnable.BlockedByType.Block)
				{
					AddScore(1, 500, es.transform.position, 0f);
				}
			}
		}

		SetState(GameState.LevelCompleted);

		GameSessionManager.Inst.FinishLevel();

		// Open game over screen
		ScreenManager.Inst.OpenScreen(ScreenID.LevelCompleted);


	}

	#endregion

	#region update
	
	// Update is called once per frame
	protected virtual void Update()
	{
		LastState = State;
		switch (State)
		{
			case GameState.Init:
				InitializeLevel();
				break;

			case GameState.LevelStart:
				CountDownTimer -= Time.deltaTime;
				if (CountDownTimer < 0f)
				{
					State = GameState.Running;
				}
				break;

			case GameState.LevelCompleted:
				break;

			case GameState.Running:
				if (GameSessionManager.Inst.IsLevelCompleted())
				{
					FinishLevel();
				}	

				break;

			case GameState.Paused:
				break;

			case GameState.GameOver:
				break;

			case GameState.PlayersDead:
				PlayersDeadTimer -= Time.deltaTime;
				if (PlayersDeadTimer <= 0)
				{
					if (GameSessionManager.Inst.CanContinueGame())
					{
						RestartLevel();
					}
					else
					{
						Debug.Log("Game over :(");
						FinishGame();
					}
				}
				break;

			default:
				Debug.LogError("Wrong game state: " + State);
				break;
		}
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods

	public void AddScore(int _playerID, int _score)
	{
		AddScore(_playerID, _score, Vector3.zero, 0, false);
	}

	public void AddScore(int _playerID, int _score, Vector3 _pos, float _delay)
	{
		AddScore(_playerID, _score, _pos, _delay, true);
	}

	private void AddScore(int _playerID, int _score, Vector3 _pos, float _delay, bool _showPopup)
	{
		GameSessionManager.Inst.AddScore(_playerID, _score);
		if (_showPopup)
		{
			ScorePopupsManager.Inst.SpawnPopup(_score, _pos, _delay);
		}
	}

	#endregion

	public static bool IsRunning()
	{
		return Inst.State == GameState.Running;
	}

	#region ongui

	void OnGUI()
	{
		if (ShowGUI)
		{
			if (GUILayout.Button("Escape mode"))
			{
				EnableEscapeMode();
			}

			if (GUILayout.Button("Restart level"))
			{
				RestartLevel();
			}

			if (GUILayout.Button("Kill Player"))
			{
				foreach (var p in GameSessionManager.Inst.Players)
				{
					p.NotifyDead();
				}
			}

			if (GUILayout.Button("Beat level"))
			{
				var v = new List<Enemy>(GameSessionManager.Inst.Enemies.ToArray());

				foreach (var e in v)
				{
					e.NotifyDead();
				}
			}
		}
	}

	#endregion

	public void PlayerDies(Player player)
	{
		SetState(GameState.PlayersDead);
		PlayersDeadTimer = MAX_PLAYERS_DEAD_TIMER;

		player.Data.RemoveLife();
	}

	internal void EnableEscapeMode()
	{
		EscapeModeOn = true;
		Debug.LogWarning("Escape mode!!!");
	}
}
