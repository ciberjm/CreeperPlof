using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class DirectionalEntity : BaseEntity
{
	#region constants
	
	#endregion
	
	#region vars

	public Coord2D SpawnPoint { get; protected set; }

	public IntDir Direction = IntDir.Down;

	protected virtual bool CanUpdateDirection { get { return true; } }
	protected virtual bool CanUpdateAnim { get { return true; } }

	#endregion
	
	#region properties
	
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
		Direction = IntDir.Down;
		SpawnPoint = TiledMap.Inst.GetCoords(transform.position.ToVector2_XY());
	}

	public override void ResetEntity()
	{
		base.ResetEntity();
		Direction = IntDir.Down;
		transform.position = TiledMap.Inst.GetCenterPos(SpawnPoint, LevelInfo.BlockMatrixLevel.FloorLevel);
	}

	#endregion

	#region update
	
	#endregion
	
	#region protected methods

	protected override void PreMove()
	{
		base.PreMove();
	}

	protected override void CheckInputVelocity()
	{
		base.CheckInputVelocity();

		if (mVelocityInput != Vector2.zero && hasMovedFromBlockCenter && CanUpdateDirection)
		{
			IntDir newDir = IntDir.FromVector2(mVelocityInput);
			if (newDir != null && newDir != Direction)
			{
				Direction = newDir;
				OnDirectionChanged();
			}
		}
	}

	protected override void PostMove()
	{
		base.PostMove();

		if (CanUpdateAnim)
		{
			if (isTryingToMove)
			{
				switch (Direction.Enum)
				{
					case IntDir.EnumDir.Up:
						SpriteAnim.Play(SpriteAnimation.AnimState.Walking_Up);
						break;

					case IntDir.EnumDir.Down:
						SpriteAnim.Play(SpriteAnimation.AnimState.Walking_Down);
						break;

					case IntDir.EnumDir.Left:
						SpriteAnim.Play(SpriteAnimation.AnimState.Walking_Left);
						break;

					case IntDir.EnumDir.Right:
						SpriteAnim.Play(SpriteAnimation.AnimState.Walking_Right);
						break;

					default:
						break;
				}
			}
			else if (hasStoppedMoving)
			{
				switch (Direction.Enum)
				{
					case IntDir.EnumDir.Up:
						SpriteAnim.Play(SpriteAnimation.AnimState.Idle_Up);
						break;

					case IntDir.EnumDir.Down:
						SpriteAnim.Play(SpriteAnimation.AnimState.Idle_Down);
						break;

					case IntDir.EnumDir.Left:
						SpriteAnim.Play(SpriteAnimation.AnimState.Idle_Left);
						break;

					case IntDir.EnumDir.Right:
						SpriteAnim.Play(SpriteAnimation.AnimState.Idle_Right);
						break;

					default:
						break;
				}
			}

			Anim.SetBool("Moving", isTryingToMove);
		}
	}

	protected virtual void OnDirectionChanged()
	{

	}

	#endregion
	
	#region public methods
	
	#endregion

	#region gizmos

	void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Coord2D c2d = TiledCoordinates;
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(TiledMap.Inst.GetCenterPos(c2d, LevelInfo.BlockMatrixLevel.BlockLevel), TiledMap.Inst.BlockSize.ToVector2() / 2f);

			Gizmos.color = Color.blue;
			Coord2D newCoords = TiledCoordinates + Direction.ToCoord2D();
			//Debug.Log("Dir: " + Direction.Enum + " " + Direction.ToCoord2D().Dump());
			Gizmos.DrawWireCube(TiledMap.Inst.GetCenterPos(newCoords, LevelInfo.BlockMatrixLevel.BlockLevel), TiledMap.Inst.BlockSize.ToVector2() * 0.75f);
		}
	}

	#endregion
}
