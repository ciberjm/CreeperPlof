using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSessionManager : MonoBehaviour
{
	#region constants

	public class GameConfiguration
	{
		public int nPlayers { get; set; }
		public bool PlayerOneEnabled;
	}

	public class PlayerSessionData
	{
		public int IDPlayer { get; set; }

		public bool Enabled { get; set; }

		public int Score { get; set; }
		public int Lives { get; private set; }

		public bool Dead { get { return !Enabled || mDead; } }

		private bool mDead = false;

		public PlayerSessionData(int _idPlayer, int _lives, bool _enabled, int _score)
		{
			IDPlayer = _idPlayer;
			Lives = _lives;
			Enabled = _enabled;
			Score = _score;

			mDead = false;
		}

		public void RemoveLife()
		{
			Lives -= 1;
			mDead = Lives >= 0;
		}
	}

	#endregion
	
	#region vars

	public List<BaseEntity> AllEntities;
	public List<Player> Players;
	public List<Enemy> Enemies;
	public List<Enemy> InvincibleEnemies;
	public List<Block> Blocks;

	public List<Enemy> EnemiesDestroyed;

	public int EnemiesToDefeat = 0;

	// Game info
	public PlayerSessionData[] PlayerData { get; private set; }

	public int HiScore { get; private set; }

	public int TotalEnemies { get; set; }

	public int Round { get; private set; }

	private float mTimerToEscape;

	#endregion
	
	#region properties

	public static GameSessionManager Inst { get; protected set; }

	public GameConfiguration GameConfig { get; protected set; }

	public int EnemiesKilled { get; set; }

	public int EnemiesEscaped { get; set; }

	public int EnemiesTrapped { get; set; }

	public float TimeInLevel { get; private set; }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
		Inst = this;
	}
	
	// Use this for initialization
	protected virtual void Start()
	{

	}

	public void InitGame()
	{
		Players = new List<Player>();
		Enemies = new List<Enemy>();
		InvincibleEnemies = new List<Enemy>();
		AllEntities = new List<BaseEntity>();
		Blocks = new List<Block>();
		EnemiesDestroyed = new List<Enemy>();

		// Reset variables
		EnemiesKilled = 0;
		EnemiesTrapped = 0;
		EnemiesEscaped = 0;
		TimeInLevel = 0f;

		CheckEnemiesDefeated();

		PlayerData = new PlayerSessionData[4];
		PlayerData[0] = new PlayerSessionData(0, 3, true, 0);
		PlayerData[1] = new PlayerSessionData(0, 3, false, 0);
		PlayerData[2] = new PlayerSessionData(0, 3, false, 0);
		PlayerData[3] = new PlayerSessionData(0, 3, false, 0);

		Round = 1;
		TotalEnemies = Enemies.Count;
		HiScore = 1000;
	}

	public void StartLevel()
	{
		// Reset variables
		EnemiesKilled = 0;
		EnemiesTrapped = 0;
		EnemiesEscaped = 0;
		TimeInLevel = 0f;

		CheckEnemiesDefeated();

		//Round++;
		TotalEnemies = Enemies.Count;
	}

	public void ClearLevel()
	{
		Players = new List<Player>();
		Enemies = new List<Enemy>();
		InvincibleEnemies = new List<Enemy>();
		AllEntities = new List<BaseEntity>();
		Blocks = new List<Block>();
		EnemiesDestroyed = new List<Enemy>();
	}

	public void NextLevel()
	{
		ClearLevel();
		Round++;
	}
	
	public void ResetLevel()
	{
		// Reset characters
		//foreach (var player in Players)
		//{
		//	player.ResetEntity();
		//}


		//foreach (var enemy in Enemies)
		//{
		//	enemy.ResetEntity();
		//}


		CheckEnemiesDefeated();
	}

	public void PauseGame()
	{

	}

	public void ResumeGame()
	{

	}

	public void FinishLevel()
	{
		CheckEnemiesDefeated();
	}

	#endregion

	#region update

	protected virtual void CheckEnemiesDefeated()
	{
		EnemiesToDefeat = Enemies.FindAll(x => !x.Defeated && !x.Escaped).Count;
		EnemiesTrapped = Enemies.FindAll(x => x.Defeated && !x.Escaped).Count;
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		switch (World.Inst.State)
		{
			case World.GameState.Running:
				TimeInLevel += Time.deltaTime;
				mTimerToEscape -= Time.deltaTime;

				if ((TimeInLevel > 120f || (mTimerToEscape <= 0 && EnemiesToDefeat < 3)) && !World.Inst.EscapeModeOn)
				{
					World.Inst.EnableEscapeMode();
				}
				break;

			default:
				break;
		}
	}
	
	#endregion
	
	#region protected methods
	
	#endregion
	
	#region public methods

	public void AddScore(int _playerID, int _score)
	{
		int score = GameSessionManager.Inst.PlayerData[_playerID - 1].Score + _score;
		GameSessionManager.Inst.PlayerData[_playerID - 1].Score = score;
		if (HiScore < score)
		{
			HiScore = score;
		}
	}

	public bool CanContinueGame()
	{
		for (int i = 0; i < PlayerData.Length; ++i)
		{
			if (PlayerData[i].Enabled && PlayerData[i].Lives > 0)
			{
				return true;
			}
		}
		return false;
	}

	public void RegisterEntity(BaseEntity be)
	{
		//Debug.Log("Registering entity");
		AllEntities.Add(be);

		switch (be.EntityType)
		{
			case BaseEntity.ENTITY_TYPE.Player:
				Player p = (Player)be;
				Players.Add(p);
				p.PlayerID = Players.Count;
				break;
			
			case BaseEntity.ENTITY_TYPE.Enemy:
				Enemy e = (Enemy)be;
				if (e.CanBeDefeated)
				{
					Enemies.Add(e);
				}
				else
				{
					InvincibleEnemies.Add(e);
				}
				break;
			
			case BaseEntity.ENTITY_TYPE.Block:
				Blocks.Add((Block)be);
				break;
			
			case BaseEntity.ENTITY_TYPE.Collectable:
				break;

			default:
				Debug.LogError("Type not implemented: " + be.EntityType);
				break;
		}
	}

	public void DeregisterEntity(BaseEntity be)
	{
		AllEntities.Remove(be);

		switch (be.EntityType)
		{
			case BaseEntity.ENTITY_TYPE.Player:
				Players.Remove((Player)be);
				break;

			case BaseEntity.ENTITY_TYPE.Enemy:
				Enemy e = (Enemy)be;
				if (e.CanBeDefeated)
				{
					Enemies.Remove(e);
				}
				else
				{
					InvincibleEnemies.Remove(e);
				}
				break;

			case BaseEntity.ENTITY_TYPE.Block:
				Blocks.Remove((Block)be);
				break;

			case BaseEntity.ENTITY_TYPE.Collectable:
				break;

			default:
				Debug.LogError("Type not implemented: " + be.EntityType);
				break;
		}
	}

	public static void SetupGame()
	{

	}

	#endregion

	#region ongui

	//void OnGUI()
	//{
	//	GUILayout.BeginHorizontal();
	//	{
	//		GUILayout.Button("\nCreeper Plof! v0.1\nby Juan Miguel Lechuga Pérez\n");

	//		GUILayout.Button("Entities: " + AllEntities.Count);
	//		GUILayout.Button("Players:  " + Players.Count);
	//		GUILayout.Button("Enemies:  " + Enemies.FindAll(x => !x.Defeated).Count + " / " + Enemies.Count);
	//		GUILayout.Button("Blocks:   " + Blocks.Count);
	//		GUILayout.Button("Enemies to defeat: " + EnemiesToDefeat);
	//		if (GUILayout.Button("Restart"))
	//		{
	//			Application.LoadLevel(0);
	//		}
	//	} 
	//	GUILayout.EndHorizontal();
	//}

	#endregion

	private void SetTimerToEscape()
	{
		mTimerToEscape = Random.Range(0f, 10f);
		foreach (var enemy in Enemies)
		{
			mTimerToEscape += enemy.TimeEscapeValue;
		}
	}

	internal bool IsLevelCompleted()
	{
		return EnemiesToDefeat <= 0;
	}

	internal void DefeatEnemy(Enemy enemy)
	{
		CheckEnemiesDefeated();
		SetTimerToEscape();
	}

	internal void UndefeatEnemy(Enemy enemy)
	{
		// Update defeated list
		CheckEnemiesDefeated();
	}

	internal void EscapeEnemy(Enemy enemy)
	{
		CheckEnemiesDefeated();
	}

	public void DestroyEnemy(Enemy _e)
	{
		EnemiesDestroyed.Add(_e);
	}
}
