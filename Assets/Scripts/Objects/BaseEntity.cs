using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseEntity : MonoBehaviour
{
	#region constants
	
	public enum ENTITY_TYPE { Undef = 0, Player = 1, Enemy = 2, Block = 3, Collectable = 4 }

	#endregion
	
	#region vars

	public SpriteRenderer MainSprite;
	protected Collider2D Collider;
	protected Animator Anim;
	protected SpriteAnimation SpriteAnim;

	// Setup vars
	public float Speed = 1f;
	public bool CanGoOutsideTile = true;
	public bool CanMoveOverEmptyAreas = false;
	public bool CanMoveInsideTile = true;
	public bool CanEscapeMap = false;
	public bool CanMoveInsideBlocks = false;

	// Input
	protected Vector2 mVelocityInput;
	protected float mMovementTimer;
	protected Vector3 mStartPos;
	protected Vector3 mTargetPos;

	// Status
	public Coord2D TiledCoordinates;
	public bool isMoving;
	public bool isTryingToMove;
	public bool hasStartedMoving;
	public bool hasStoppedMoving;
	public bool hasMovedFromBlockCenter;

	// Tracking
	protected Vector3 mPrevPos;
	protected Vector3 mLastMove;

	// Coroutine
	protected bool mForceStopMoveCR = false;

	protected Vector2 mMovePosOffset;

	#endregion
	
	#region properties

	public abstract ENTITY_TYPE EntityType { get; }

	public virtual bool CanMove { get { return true; } }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
		MainSprite = gameObject.GetComponentInChildren<SpriteRenderer>();
		Collider = gameObject.GetComponentInChildren<Collider2D>();
		Anim = gameObject.GetComponentInChildren<Animator>();
		SpriteAnim = gameObject.GetComponentInChildren<SpriteAnimation>();
	}

	protected virtual void World_OnStateChanged(World.GameState _old, World.GameState _new)
	{
	}
	
	// Use this for initialization
	protected virtual void Start()
	{
		mMovePosOffset = Vector2.zero;
		mVelocityInput = Vector3.zero;
		isMoving = false;
		hasStartedMoving = false;
		hasStoppedMoving = false;
		hasMovedFromBlockCenter = false;

		PostMovement();
	}

	protected virtual void OnEnable()
	{
		GameSessionManager.Inst.RegisterEntity(this);
		World.OnStateChanged += World_OnStateChanged;
	}

	protected virtual void OnDisable()
	{
		GameSessionManager.Inst.DeregisterEntity(this);
		World.OnStateChanged -= World_OnStateChanged;
	}

	public virtual void ResetEntity()
	{
		StopMove();
		mVelocityInput = Vector3.zero;
		isMoving = false;
		hasStartedMoving = false;
		hasStoppedMoving = false;
		hasMovedFromBlockCenter = false;

		PostMovement();
	}

	#endregion

	#region update

	protected virtual void FixedUpdate()
	{

	}

	// Update is called once per frame
	protected virtual void Update()
	{
		switch (World.Inst.State)
		{
			case World.GameState.Init:
				OnWorldInit();
				break;
			
			case World.GameState.Running:
				OnWorldRunning();
				break;
			
			case World.GameState.Paused:
				OnWorldPaused();
				break;
			
			case World.GameState.GameOver:
				OnWorldGameOver();
				break;
			
			case World.GameState.LevelStart:
				OnWorldLevelStart();
				break;
	
			case World.GameState.LevelCompleted:
				OnWorldLevelCompleted();
				break;

			case World.GameState.PlayersDead:
				OnWorldPlayersDead();
				break;

			default:
				Debug.LogError("Case not implemented! " + World.Inst.State);
				break;
		}
	}

	protected virtual void OnWorldInit()
	{

	}

	protected virtual void OnWorldPaused()
	{

	}

	protected virtual void OnWorldRunning()
	{
		GetInput();

		PreMove();
		CheckInputVelocity();
		PostMove();
	}

	protected virtual void OnWorldLevelCompleted()
	{
		SpriteAnim.Play(SpriteAnimation.AnimState.Victory);
	}

	protected virtual void OnWorldLevelStart()
	{

	}

	protected virtual void OnWorldGameOver()
	{

	}

	protected virtual void OnWorldPlayersDead()
	{

	}


	protected abstract void GetInput();

	protected virtual void PreMove()
	{
		
	}

	protected virtual void PostMove()
	{
	}

	protected virtual void CheckInputVelocity()
	{
		hasStartedMoving = false;
		hasStoppedMoving = false;
		hasMovedFromBlockCenter = false;

		if (mVelocityInput != Vector2.zero)
		{
			if (!isTryingToMove)
				hasStartedMoving = true;

			isTryingToMove = true;

			if (!isMoving)
			{
				if (Mathf.Abs(mVelocityInput.x) > Mathf.Abs(mVelocityInput.y))
				{
					mVelocityInput.y = 0;
				}
				else
				{
					mVelocityInput.x = 0;
				}

				// Check if movement is possible
				Coord2D newCoord = new Coord2D(TiledCoordinates.x + (System.Math.Sign(mVelocityInput.x) * 1),
												TiledCoordinates.y - (System.Math.Sign(mVelocityInput.y) * 1));

				hasMovedFromBlockCenter = true;
				if (CanMoveTo(newCoord) && CanMove)
				{
					mForceStopMoveCR = false;
					//Debug.LogWarning("Starting move: " + mVelocityInput.Dump());
					StartCoroutine(Move(transform));
				}
				else
				{
					// Cannot move
					OnMovementFailed();
				}
			}
		}
		else
		{
			if (isTryingToMove)
				hasStoppedMoving = true;

			isTryingToMove = false;
		}
	}

	protected virtual bool CanMoveTo(Coord2D _newCoordinates)
	{
		TileType tileType = TiledMap.Inst.GetFloorData(_newCoordinates);

		// It is inside of the map?
		if ((_newCoordinates.x < 0 || _newCoordinates.x >= (TiledMap.Inst.TileSize.x) ||
			_newCoordinates.y < 0 || _newCoordinates.y >= (TiledMap.Inst.TileSize.y)))
		{
			//Debug.Log("Outside!: " + _newCoordinates.Dump() + " " + TiledCoordinates.Dump() + TiledMap.Inst.mTileSize.Dump());

			return false;
		}
		
		// Is there anything?
		if (tileType == TileType.None)
		{
			return false;
		}

		if (tileType == TileType.EmptyArea)
		{
			return CanMoveOverEmptyAreas;
		}

		// Can go outside if it is the case?
		if (tileType == TileType.OutsideArea) 
		{
			return CanGoOutsideTile;
		}
		else // Check blocks
		{
			//Debug.Log("Inside!: " + _newCoordinates.Dump() + " " + TiledCoordinates.Dump() + " "  + TiledMap.Inst.mTileSize.Dump());
			// Is there any block nearby?
			Block b = TiledMap.Inst.GetBlockAt(_newCoordinates);
			if (b == null || CanMoveInsideBlocks)
			{
				return CanMoveInsideTile;
			}
		}
		return false;
	}

	public virtual void PostMovement()
	{
		TiledCoordinates = TiledMap.Inst.GetCoords(transform.position);
		UpdateSortingOrder();
	}

	public virtual void UpdateSortingOrder()
	{
		MainSprite.sortingOrder = TiledMap.Inst.GetSortingLayer(false, TiledCoordinates.y);
	}

	protected virtual void OnMovementFailed()
	{

	}

	#endregion
	
	#region protected methods

	protected IEnumerator Move(Transform transform)
	{
		isMoving = true;
		mStartPos = transform.position;
		mMovementTimer = 0;

		float gridSize = Mathf.Abs(mVelocityInput.x) > Mathf.Abs(mVelocityInput.y) ? TiledMap.Inst.BlockSize.x : TiledMap.Inst.BlockSize.y;

		if (EntityType == ENTITY_TYPE.Block)
		{
			mTargetPos = TiledMap.Inst.GetPos(new Coord2D(TiledCoordinates.x + System.Math.Sign(mVelocityInput.x), TiledCoordinates.y - System.Math.Sign(mVelocityInput.y)), LevelInfo.BlockMatrixLevel.BlockLevel);
		}
		else
		{
			mTargetPos = TiledMap.Inst.GetCenterPos(new Coord2D(TiledCoordinates.x + System.Math.Sign(mVelocityInput.x), TiledCoordinates.y - System.Math.Sign(mVelocityInput.y)), LevelInfo.BlockMatrixLevel.BlockLevel);
		}
		//mTargetPos = new Vector3((mStartPos.x + System.Math.Sign(mVelocityInput.x) * TiledMap.Inst.BlockSize.x),
		//	(mStartPos.y + System.Math.Sign(mVelocityInput.y) * TiledMap.Inst.BlockSize.y), mStartPos.z);

		while (mMovementTimer < 1f)
		{
			while (!World.IsRunning())
			{
				yield return 0;
			}

			mMovementTimer += Time.deltaTime * (Speed / gridSize);

			mPrevPos = transform.position;
			transform.position = Vector3.Lerp(mStartPos, mTargetPos + mMovePosOffset.ToVector3_XY(), mMovementTimer);
			mLastMove = transform.position - mPrevPos;

			PostMovement();
			yield return 0;

			if (mForceStopMoveCR)
			{
				//Debug.LogError("Breaking coroutine!");
				yield break;
			}
		}

		isMoving = false;
		PostMovement();
		yield return 0;
	}

	protected void StopMove()
	{
		mForceStopMoveCR = true;
		isMoving = false;

		mPrevPos = transform.position;
		transform.position = TiledMap.Inst.GetCenterPos(TiledCoordinates, LevelInfo.BlockMatrixLevel.BlockLevel);
		mLastMove = transform.position - mPrevPos;

		PostMovement();
	}

	protected virtual void Freeze()
	{
		mForceStopMoveCR = true;

		mVelocityInput = Vector2.zero;
		isMoving = false;

		SpriteAnim.Stop();
		Anim.enabled = false;
		Anim.speed = 0;
	}

	#endregion
	
	#region public methods
	
	#endregion

	public virtual void NotifyDead()
	{
		SpriteAnim.Play(SpriteAnimation.AnimState.Dying);
	}

	public abstract void EndOfSmashWithBlock(Block block, IntDir _direction);
}
