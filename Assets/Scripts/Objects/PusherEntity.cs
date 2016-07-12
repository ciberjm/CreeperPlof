using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PusherEntity : DirectionalEntity
{
	#region constants
	
	#endregion
	
	#region vars

	#endregion
	
	#region properties

	public bool EnablePush { get; protected set; }

	public bool IsPushing { get; protected set; }

	public float mPushTimer = 0f;

	private bool mHasPushed = false;

	protected override bool CanUpdateDirection
	{
		get
		{
			return base.CanUpdateDirection && !IsPushing;
		}
	}

	protected override bool CanUpdateAnim
	{
		get
		{
			return base.CanUpdateAnim && !IsPushing;
		}
	}

	public override bool CanMove
	{
		get
		{
			return base.CanMove && !IsPushing && !EnablePush;
		}
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

	public override void ResetEntity()
	{
		base.ResetEntity();
		IsPushing = false;
	}

	#endregion

	#region update
	
	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if (World.IsRunning())
		{
			if (EnablePush)
			{
				EnablePush = false;
				Push();
			}
			else if (IsPushing)
			{
				//Debug.LogError("PUSSHING!!!" + TiledCoordinates.Dump());
				mVelocityInput = Vector2.zero;
				mPushTimer += Time.deltaTime;
				if (mPushTimer > 0.3f && !mHasPushed)
				{
					//Debug.Log("Pushing...");
					mHasPushed = true;
					OnAnimEventPush();
				}
				else if (mPushTimer > 0.33f)
				{
					mHasPushed = false;
					IsPushing = false;
				}
			}
		}

	}
	
	#endregion
	
	#region protected methods

	protected virtual void Push()
	{
		IsPushing = true;
		mHasPushed = false;
		mPushTimer = 0f;
		switch (Direction.Enum)
		{
			case IntDir.EnumDir.Up:
				SpriteAnim.Play(SpriteAnimation.AnimState.Push_Up);
				Anim.SetTrigger("PushUp");
				break;

			case IntDir.EnumDir.Down:				
				SpriteAnim.Play(SpriteAnimation.AnimState.Push_Down);
				Anim.SetTrigger("PushDown");
				break;

			case IntDir.EnumDir.Left:
				SpriteAnim.Play(SpriteAnimation.AnimState.Push_Left);
				Anim.SetTrigger("PushLeft");
				break;

			case IntDir.EnumDir.Right:				
				SpriteAnim.Play(SpriteAnimation.AnimState.Push_Right);
				Anim.SetTrigger("PushRight");
				break;

			default:
				break;
		}
	}

	#endregion
	
	#region public methods

	public void OnAnimEventPush()
	{
		Coord2D newCoords = TiledCoordinates + Direction.ToCoord2D();
		Block b = TiledMap.Inst.GetBlockAt(newCoords);
		if (b != null)
		{
			b.GetPushTo(Direction);
		}
	}

	public void OnAnimEventPushEnded()
	{
		IsPushing = false;
	}

	#endregion

	#region physics

	protected virtual void OnTriggerEnter2D(Collider2D c)
	{
		if (World.IsRunning())
		{
			GameObject go = c.gameObject.transform.parent.gameObject;
			if (go.CompareTag("Player"))
			{
				OnPlayerHit(go.GetComponent<Player>());
			}
			else if (go.CompareTag("Block"))
			{
				Block b = go.GetComponent<Block>();
				if (b.isMoving) // Check that moves
				{
					//if (b.TiledCoordinates.x == TiledCoordinates.x || b.TiledCoordinates.y == TiledCoordinates.y)
					//{
					OnMovingBlockHit(b);
					//}
					//else
					//{
					//	Debug.Log((b.TiledCoordinates + Direction).Dump() + TiledCoordinates.Dump());
					//}
				}
			}
			else if (go.CompareTag("Enemy"))
			{
				OnEnemyHit(go.GetComponent<Enemy>());
			}
			else
			{
				Debug.LogError("No idea what I'm colliding with :/" + go.name, go);
			}
		}
	}

	protected virtual void OnPlayerHit(Player p)
	{
	}

	protected virtual void OnMovingBlockHit(Block b)
	{
	}

	protected virtual void OnEnemyHit(Enemy e)
	{
	}

	#endregion

	#region gizmos



	#endregion
}
