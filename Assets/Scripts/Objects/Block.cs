using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : BaseEntity
{
	#region constants

	public const int INFINITE_CELLS = -1;

	#endregion
	
	#region vars

	// Configuration
	public bool BreaksOnPush = false;
	public bool BreaksOnHit = false;
	public bool BreaksOnPushBlocked = true;
	public int CellsToMove = INFINITE_CELLS;

	public Coord2D LastTiledCoordinates;
	public IntDir DirectionPushed;
	
	private int mCellsMoved = 0;

	protected List<AttachedEntity> mAttachedEntities;


	protected class AttachedEntity
	{
		public BaseEntity Entity { get; protected set; }
		public Vector2 Offset { get; protected set; }

		public AttachedEntity(BaseEntity be, Vector3 CurrentPos)
		{
			Entity = be;
			Offset = CurrentPos - be.transform.position;
		}
	}

	#endregion
	
	#region properties

	public override BaseEntity.ENTITY_TYPE EntityType
	{
		get { return ENTITY_TYPE.Block; }
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
		LastTiledCoordinates = TiledCoordinates;
		DirectionPushed = null;
		TiledMap.Inst.UpdateBlock(this);

		mAttachedEntities = new List<AttachedEntity>();
	}
	
	#endregion

	#region entities

	public void AttachEntity(BaseEntity be)
	{
		mAttachedEntities.Add(new AttachedEntity(be, transform.position.ToVector2_XY()));
	}

	private void UpdateAttachedEntities()
	{
		if (mAttachedEntities != null && mAttachedEntities.Count > 0)
		{
			foreach (var aentity in mAttachedEntities)
			{
				aentity.Entity.transform.position = transform.position - aentity.Offset.ToVector3_XY();
				aentity.Entity.TiledCoordinates = TiledMap.Inst.GetCoords(aentity.Entity.transform.position);

				// Top or down?
				int val = 0;
				if (transform.position.y < aentity.Entity.transform.position.y)
				{
					val = -1;
				}
				else
				{
					val = 1;
				}
				aentity.Entity.MainSprite.sortingOrder = TiledMap.Inst.GetSortingLayer(false, TiledCoordinates.y) + val;
				//Debug.Log("Moving entity: " + aentity.Entity.name + " for: " + (transform.position + aentity.Offset.ToVector3_XY()).Dump());
			}
		}
	}

	#endregion

	#region update

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();
	}

	public override void UpdateSortingOrder()
	{
		MainSprite.sortingOrder = TiledMap.Inst.GetSortingLayer(true, TiledCoordinates.y);
	}

	#endregion
	
	#region protected methods

	protected override void PreMove()
	{
		base.PreMove();
	}

	public override void PostMovement()
	{
		LastTiledCoordinates = TiledCoordinates;

		base.PostMovement();

		UpdateAttachedEntities();

		// Update tiled map
		if (LastTiledCoordinates != TiledCoordinates)
		{
			TiledMap.Inst.UpdateBlock(this);
			mCellsMoved++;

			if (!isMoving)
			{
				if (!CanGoNextBlock())
				{
					if (DirectionPushed != null)
					{
						Block b = TiledMap.Inst.GetBlockAt(TiledCoordinates + DirectionPushed.ToCoord2D());
						OnBlockPushEnds(DirectionPushed, b, mCellsMoved);
					}
				}
			}
		}
	}

	public virtual void OnBlockPushStarts(IntDir _direction)
	{
		//Debug.Log("Block starts moving");

		if (BreaksOnPush)
		{
			BreakBlock();
		}
	}

	public virtual void OnBlockPushEnds(IntDir _direction, bool _hitWall, int _cellsMoved)
	{
		DirectionPushed = null;
		//Debug.Log("Block finished moving: " + _cellsMoved);

		// Clear array of entities
		int i = 0;
		foreach (var ae in mAttachedEntities)
		{
			ae.Entity.EndOfSmashWithBlock(this, _direction);

			World.Inst.AddScore(1, 100 * (i + 1), ae.Entity.transform.position, 0.5f * i);
			i++;
		}

		mAttachedEntities.Clear();

		if (BreaksOnPushBlocked && _cellsMoved < 1)
		{
			BreakBlock();
		}

		if (BreaksOnHit)
		{
			BreakBlock();
		}
	}

	protected override void OnMovementFailed()
	{
		base.OnMovementFailed();
		if (DirectionPushed != null)
		{
			Block b = TiledMap.Inst.GetBlockAt(TiledCoordinates + DirectionPushed.ToCoord2D());
			OnBlockPushEnds(DirectionPushed, b, mCellsMoved);
		}
	}

	public void BreakBlock()
	{
		TiledMap.Inst.RemoveBlock(this);
		Destroy(gameObject);
		ParticleSpawnerSystem.SpawnParticle(ParticleSpawnerSystem.ParticleType.BlockDestroyed, TiledMap.Inst.GetCenterPos(TiledCoordinates, LevelInfo.BlockMatrixLevel.BlockLevel));
	}

	public void GetPushTo(IntDir _direction)
	{
		if (DirectionPushed != null)
		{
			// Direction override!
		}

		DirectionPushed = _direction;

		OnBlockPushStarts(DirectionPushed);
		mCellsMoved = 0;
	}

	protected override void GetInput()
	{
		if (DirectionPushed != null)
		{
			mVelocityInput = DirectionPushed.ToVector2();
		}
		else
		{
			mVelocityInput = Vector2.zero;
		}
	}

	protected bool CanGoNextBlock()
	{
		if (DirectionPushed != null)
			return CanMoveTo(TiledCoordinates + DirectionPushed.ToCoord2D());

		return false;
	}

	protected override void PostMove()
	{
		base.PostMove();
	}
	
	#endregion
	
	#region public methods

	public override void EndOfSmashWithBlock(Block block, IntDir _direction)
	{
	}

	#endregion
}
