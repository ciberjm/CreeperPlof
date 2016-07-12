using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawnable : Enemy
{
	#region constants

	private class AIController
	{
		public int MinRound = 0;
		public float ProbChasePlayer = 0f;
		public float LineOfSight = 0f;
		public float JumpBlockProb = 0f;
		public Color Color = Color.white;

		public int DistToMorph;
		public float MorphProb;

		public float TimeEscapeValue;
	}

	private readonly List<AIController> AITypes = new List<AIController>()
	{
		new AIController() { Color = Color.white,		MinRound = 0,	LineOfSight = 1,	ProbChasePlayer = 0.10f,	JumpBlockProb = 0.00f,	DistToMorph = 3,	MorphProb = 0.20f,	TimeEscapeValue = 0f, },
		new AIController() { Color = Color.magenta,		MinRound = 2,	LineOfSight = 4,	ProbChasePlayer = 0.50f,	JumpBlockProb = 0.20f,	DistToMorph = 7,	MorphProb = 0.15f,	TimeEscapeValue = 5f, },
		new AIController() { Color = Color.blue,		MinRound = 4,	LineOfSight = 4,	ProbChasePlayer = 1.00f,	JumpBlockProb = 0.40f,	DistToMorph = 8,	MorphProb = 0.10f,	TimeEscapeValue = 15f, },
		new AIController() { Color = Color.red,			MinRound = 5,	LineOfSight = 100,	ProbChasePlayer = 1.00f,	JumpBlockProb = 0.90f,	DistToMorph = 9,	MorphProb = 0.05f,	TimeEscapeValue = 60f, },
	};

	public enum BlockedByType { None = 0, Player = 1, Block = 2, Enemy = 3 }

	private const float MAX_SPAWN_TIMER = 10f;
	private const float MIN_SPAWN_TIMER = 4f;

	#endregion

	#region vars

	public GameObject HiddenGO;
	public GameObject SpawnedGO;

	protected float mMaxSpawnTimer;
	protected float mTimer;

	public float SecondsToNextMovement = 1f;

	protected List<IntDir> mMoveOptions;

	private IntDir mOverBlockDir;
	private IntDir mEscapeDir;

	private AIController mAI;

	#endregion

	#region properties

	public BlockedByType BlockedBy;// { get; protected set; }

	public bool IsSpawned { get; protected set; }
	public bool StopCR { get; protected set; }
	public override bool CanBeDefeated { get { return true; } }

	public override Enemy.EnemyType EType
	{
		get { return EnemyType.Spawnable; }
	}

	public override float TimeEscapeValue
	{
		get
		{
			if (IsSpawned)
				return mAI.TimeEscapeValue;
			else
				return 0f;
		}
	}


	#endregion

	#region init

	protected override void Awake()
	{
		base.Awake();

		Hide();
	}

	// Use this for initialization
	protected override void Start()
	{
		base.Start();
		mMoveOptions = new List<IntDir>();
		StopCR = false;
		BlockedBy = BlockedByType.None;
	}

	public override void SetupDifficultyLevel(int _level)
	{
		base.SetupDifficultyLevel(_level);

		SecondsToNextMovement = (Mathf.Clamp(10f - _level, 0f, 10f)) / 10f;
	}

	#endregion

	#region update

	#endregion

	#region protected methods

	protected override IEnumerator ExecuteAI()
	{
		StopCR = false;
		ExecutingAI = true;
		mMaxSpawnTimer = Random.Range(MIN_SPAWN_TIMER, MAX_SPAWN_TIMER);
		mTimer = mMaxSpawnTimer;
		while (ExecutingAI)
		{
			if (StopCR)
			{
				yield break;
			}

			while (!World.IsRunning())
			{
				yield return 0;
			}

			if (IsSpawned)
			{
				yield return StartCoroutine(SpawnedExecution());
			}
			else
			{
				yield return StartCoroutine(HiddenExecution());
			}

		}

		ExecutingAI = false;
		yield return 0;
	}

	protected virtual IEnumerator SpawnedExecution()
	{
		// Get options
		mMoveOptions.Clear();

		Coord2D target = GameSessionManager.Inst.Players[0].TiledCoordinates;
		int distToTarget = TiledCoordinates.distTo(target);

		bool moveRandomly = false;

		if (World.Inst.EscapeModeOn)
		{
			if (mEscapeDir == null)
			{
				int minDist = int.MaxValue;
				if (TiledCoordinates.x < minDist)
				{
					minDist = TiledCoordinates.x;
					mEscapeDir = IntDir.Left;
				}

				if (TiledCoordinates.y < minDist)
				{
					minDist = TiledCoordinates.y;
					mEscapeDir = IntDir.Up;
				}

				if (TiledMap.Inst.TileSize.x - TiledCoordinates.x < minDist)
				{
					minDist = TiledMap.Inst.TileSize.x - TiledCoordinates.x;
					mEscapeDir = IntDir.Right;
				}

				if (TiledMap.Inst.TileSize.y - TiledCoordinates.y < minDist)
				{
					minDist = TiledMap.Inst.TileSize.y - TiledCoordinates.y;
					mEscapeDir = IntDir.Down;
				}
			}

			target = TiledCoordinates + mEscapeDir;
			CanGoOutsideTile = true;

			var ft = TiledMap.Inst.GetFloorData(target);
			if (ft == TileType.None || ft == TileType.EmptyArea)
			{
				// Escaped!
				NotifyEscaped();
			}
		}
		else
		{
			CanGoOutsideTile = false;
		}

		if (!World.Inst.EscapeModeOn && distToTarget >= mAI.DistToMorph && Random.Range(0f, 1f) <= mAI.MorphProb)
		{
			var ai = GetRandomAIController();
			if (ai.Color != mAI.Color)
			{
				// Spawn particle
				SpawnParticles(ParticleSpawnerSystem.ParticleType.EnemySpawn);
				SetAI(GetRandomAIController());
			}
		}
		else
		{
			if (mOverBlockDir == null)
			{
				if (World.Inst.EscapeModeOn || (distToTarget <= mAI.LineOfSight && Random.Range(0f, 1f) <= mAI.ProbChasePlayer))
				{
					// Get directions to target
					CanGoOutsideTile = true;
					if (target.x < TiledCoordinates.x)
					{
						if (CanMoveTo(TiledCoordinates + IntDir.Left.ToCoord2D()))
						{
							mMoveOptions.Add(IntDir.Left);
						}
					}
					else if (target.x > TiledCoordinates.x)
					{
						if (CanMoveTo(TiledCoordinates + IntDir.Right.ToCoord2D()))
						{
							mMoveOptions.Add(IntDir.Right);
						}
					}

					if (target.y < TiledCoordinates.y)
					{
						if (CanMoveTo(TiledCoordinates + IntDir.Up.ToCoord2D()))
						{
							mMoveOptions.Add(IntDir.Up);
						}
					}
					else if (target.y > TiledCoordinates.y)
					{
						if (CanMoveTo(TiledCoordinates + IntDir.Down.ToCoord2D()))
						{
							mMoveOptions.Add(IntDir.Down);
						}
					}

					if (mMoveOptions.Count == 0)
					{
						CanGoOutsideTile = false;
						// No directions found, try to jump over block
						if ((Random.Range(0f, 1f) <= mAI.JumpBlockProb) || World.Inst.EscapeModeOn)
						{
							// Jump to block logic
							mMovePosOffset = TiledMap.Inst.LevelOffsets[(int)LevelInfo.BlockMatrixLevel.TopLevel];
							mOverBlockDir = null;
							CanMoveInsideBlocks = true;

							if (target.x < TiledCoordinates.x)
							{
								if (CanMoveTo(TiledCoordinates + IntDir.Left.ToCoord2D()))
								{
									mOverBlockDir = IntDir.Left;
								}
							}
							else if (target.x > TiledCoordinates.x)
							{
								if (CanMoveTo(TiledCoordinates + IntDir.Right.ToCoord2D()))
								{
									mOverBlockDir = IntDir.Right;
								}
							}
							else if (target.y < TiledCoordinates.y)
							{
								if (CanMoveTo(TiledCoordinates + IntDir.Up.ToCoord2D()))
								{
									mOverBlockDir = IntDir.Up;
								}
							}
							else if (target.y > TiledCoordinates.y)
							{
								if (CanMoveTo(TiledCoordinates + IntDir.Down.ToCoord2D()))
								{
									mOverBlockDir = IntDir.Down;
								}
							}
						}
						else
						{
							moveRandomly = true;
						}
					}
				}
				else
				{
					moveRandomly = true;
				}
			}
			else
			{
				// Moving over block
				mMoveOptions.Add(mOverBlockDir);

				// Check if we can go there or not
				var ft = TiledMap.Inst.GetFloorData(TiledCoordinates + mOverBlockDir.ToCoord2D());
				if ((ft == TileType.OutsideArea || ft == TileType.EmptyArea || ft == TileType.None) && !World.Inst.EscapeModeOn)
				{
					IntDir[] ids = IntDir.GetDirs(x => CanMoveTo(TiledCoordinates + x));

					mOverBlockDir = ids[Random.Range(0, ids.Length)];
					mMoveOptions.Clear();
					mMoveOptions.Add(mOverBlockDir);
				}
				else if (TiledMap.Inst.GetBlockAt(TiledCoordinates + mOverBlockDir.ToCoord2D()) == null)
				{
					mOverBlockDir = null;
					CanMoveInsideBlocks = false;
					mMovePosOffset = Vector2.zero;
				}
			}

			if (moveRandomly)
			{
				// Random wandering
				mMoveOptions.AddRange(IntDir.GetDirs(x => CanMoveTo(TiledCoordinates + x.ToCoord2D())));
			}

			if (mMoveOptions.Count > 0)
			{
				mVelocityInput = mMoveOptions[Random.Range(0, mMoveOptions.Count)].ToVector2();
			}

			yield return 0;
		}

		mVelocityInput = Vector2.zero;

		yield return new WaitForSeconds(SecondsToNextMovement);
	}

	protected virtual IEnumerator HiddenExecution()
	{
		// Look if someone is on top of it
		BlockedBy = BlockedByType.None;
		var l = TiledMap.Inst.GetEntitiesIn(TiledCoordinates);
		if (l.FindAll(x => x.EntityType == ENTITY_TYPE.Player).Count > 0)
		{
			BlockedBy = BlockedByType.Player;
		}
		else if (l.FindAll(x => x.EntityType == ENTITY_TYPE.Enemy).Count > 1)
		{
			BlockedBy = BlockedByType.Enemy;
		}
		else if (l.FindAll(x => x.EntityType == ENTITY_TYPE.Block).Count > 0)
		{
			BlockedBy = BlockedByType.Block;
		}

		// Check blocks
		if (BlockedBy == BlockedByType.None && TiledMap.Inst.GetBlockAt(TiledCoordinates) != null)
			BlockedBy = BlockedByType.Block;

		if (BlockedBy != BlockedByType.None)
		{
			SetDefeated(true);

			SpriteAnim.Play(SpriteAnimation.AnimState.Idle_Up);

			mMaxSpawnTimer = Random.Range(MIN_SPAWN_TIMER, MAX_SPAWN_TIMER);
			mTimer = mMaxSpawnTimer;
		}
		else
		{
			SetDefeated(false);
			//if (!MainSprite.gameObject.activeSelf)
			//	MainSprite.gameObject.SetActive(true);
		}

		if (mTimer <= 1.2f)
		{
			SpriteAnim.Play(SpriteAnimation.AnimState.Idle_Left);
		}
		else if (mTimer < (MIN_SPAWN_TIMER + (MAX_SPAWN_TIMER - MIN_SPAWN_TIMER) / 2f))
		{
			SpriteAnim.Play(SpriteAnimation.AnimState.Idle_Down);
		}

		MainSprite.color = Color.Lerp(Color.white, Color.gray, mTimer / mMaxSpawnTimer);

		if (mTimer <= 0)
		{
			Spawn();
		}

		yield return new WaitForSeconds(0.2f);

		mTimer -= 0.2f;
	}

	protected virtual void Spawn()
	{
		SpriteAnim.Stop();

		HiddenGO.SetActive(false);
		SpawnedGO.SetActive(true);

		MainSprite = SpawnedGO.GetComponentInChildren<SpriteRenderer>();
		Collider = SpawnedGO.GetComponentInChildren<Collider2D>();
		Anim = SpawnedGO.GetComponentInChildren<Animator>();
		SpriteAnim = SpawnedGO.GetComponentInChildren<SpriteAnimation>();

		BlockedBy = BlockedByType.None;

		mOverBlockDir = null;
		IsSpawned = true;

		SetAI(GetRandomAIController());

		// Spawn particle
		SpawnParticles(ParticleSpawnerSystem.ParticleType.EnemySpawn);
	
		UpdateSortingOrder();
	}

	protected virtual void Hide()
	{
		HiddenGO.SetActive(true);
		SpawnedGO.SetActive(false);

		MainSprite = HiddenGO.GetComponentInChildren<SpriteRenderer>();
		Collider = HiddenGO.GetComponentInChildren<Collider2D>();
		Anim = HiddenGO.GetComponentInChildren<Animator>();

		MainSprite.color = Color.gray;

		IsSpawned = false;
		UpdateSortingOrder();
	}

	public override void UpdateSortingOrder()
	{
		if (mOverBlockDir == null || Defeated)
			base.UpdateSortingOrder();
		else
			MainSprite.sortingOrder = TiledMap.Inst.GetSortingLayer(false, TiledCoordinates.y + 1);
	}

	private AIController GetRandomAIController()
	{
		int currentRound = GameSessionManager.Inst.Round;
		var filtered = AITypes.FindAll(x => x.MinRound <= currentRound);
		return filtered[Random.Range(0, filtered.Count)];
	}

	private void SetAI(AIController _ai)
	{
		mAI = _ai;
		MainSprite.color = mAI.Color;
	}

	#endregion

	#region public methods

	protected override void OnEnemyHit(Enemy e)
	{
		base.OnEnemyHit(e);
		if (e != null)
		{
			if (e.EType == EnemyType.Outsider)
			{
				ParticleSpawnerSystem.SpawnParticle(ParticleSpawnerSystem.ParticleType.EnemySmashedByOutsider, transform.position);
				NotifyDead();
			}
		}
	}

	public override void EndOfSmashWithBlock(Block block, IntDir _direction)
	{
		if (_direction == IntDir.Right || _direction == IntDir.Left)
		{
			SpawnParticles(ParticleSpawnerSystem.ParticleType.EnemySmashed).transform.rotation = Quaternion.Euler(0f, 0f, 90f);
		}
		else
		{
			SpawnParticles(ParticleSpawnerSystem.ParticleType.EnemySmashed);
		}

		NotifyDead();
	}

	private ParticleObj SpawnParticles(ParticleSpawnerSystem.ParticleType _type)
	{
		var po = ParticleSpawnerSystem.SpawnParticle(_type, transform.position);
		po.GetComponent<ParticleSystem>().GetComponent<Renderer>().material.color = mAI.Color;
		return po;
	}

	#endregion
}
