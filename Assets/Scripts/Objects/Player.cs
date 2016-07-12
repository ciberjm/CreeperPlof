using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : PusherEntity
{
	#region constants
	
	#endregion
	
	#region vars

	public int PlayerID;

	#endregion
	
	#region properties

	public override BaseEntity.ENTITY_TYPE EntityType
	{
		get { return ENTITY_TYPE.Player; }
	}

	public GameSessionManager.PlayerSessionData Data { get { return GameSessionManager.Inst.PlayerData[PlayerID - 1]; } }

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
	
	#endregion

	#region update

	protected override void PreMove()
	{
		base.PreMove();
	}

	protected override void PostMove()
	{
		base.PostMove();
	}
	
	#endregion
	
	#region protected methods

	protected override void GetInput()
	{
		//Debug.Log("IsPushing " + IsPushing);
		if (IsPushing)
		{
			mVelocityInput = Vector3.zero;
		}
		else
		{
			float hor = Mathf.Clamp(Input.GetAxis("Horizontal"), -1f, 1f);
			float ver = Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 1f);

			mVelocityInput = new Vector2(hor, ver);
	
			bool pushPressed = Input.GetButtonDown("Push");
			if (pushPressed && !IsPushing)
			{
				EnablePush = true;
				//Debug.Log("Button Pressed");
				StopMove();
				//Push();
			}
			else
			{

			}
		}
	}
	
	#endregion
	
	#region public methods

	#endregion

	#region physics

	protected override void World_OnStateChanged(World.GameState _old, World.GameState _new)
	{
		base.World_OnStateChanged(_old, _new);

		if (_new == World.GameState.LevelCompleted)
		{
			SpriteAnim.Play(SpriteAnimation.AnimState.None);
			Anim.SetTrigger("VictoryTrigger");
		}
	}

	protected override void OnEnemyHit(Enemy e)
	{
		base.OnEnemyHit(e);
		NotifyDead();

	}

	protected override void Freeze()
	{
		Debug.Log("Freezed");
		base.Freeze();
		Collider.enabled = false;
	}

	public override void NotifyDead()
	{
		Freeze();
		base.NotifyDead();


		// Kill the player
		World.Inst.PlayerDies(this);
		//Destroy(gameObject);
	}

	public override void EndOfSmashWithBlock(Block block, IntDir _direction)
	{
	}

	#endregion

	#region gizmos

	#endregion
}
