using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : PusherEntity
{
	#region constants

	public enum EnemyType { Outsider = 0, Spawnable = 1 }

	#endregion

	#region vars

	protected bool ExecutingAI = false;

	protected Block BlockKiller = null;

	public bool Defeated { get; protected set; }

	public bool Escaped { get; protected set; }

	public abstract bool CanBeDefeated { get; }

	#endregion

	#region properties

	public override BaseEntity.ENTITY_TYPE EntityType
	{
		get { return ENTITY_TYPE.Enemy; }
	}

	public abstract float TimeEscapeValue { get; }

	public abstract EnemyType EType { get; }

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
		Escaped = false;
		SetDefeated(false);
		ExecutingAI = false;
	}

	public override void ResetEntity()
	{
		base.ResetEntity();
		SetDefeated(false);
		ExecutingAI = false;
	}

	public virtual void SetupDifficultyLevel(int _level)
	{

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
		if (BlockKiller != null)
		{
		}
		else if (!ExecutingAI)
		{
			StartCoroutine("ExecuteAI");
			ExecutingAI = true;
		}
	}

	protected abstract IEnumerator ExecuteAI();

	#endregion

	#region public methods

	public void SetDefeated(bool b)
	{
		if (Defeated != b)
		{
			Defeated = b;
			if (b)
			{
				GameSessionManager.Inst.DefeatEnemy(this);
			}
			else
			{
				GameSessionManager.Inst.UndefeatEnemy(this);
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		StopAI();
	}

	public void StopAI()
	{
		StopCoroutine("ExecuteAI");
		ExecutingAI = false;
	}

	public virtual void NotifyEscaped()
	{
		gameObject.SetActive(false);
		Escaped = true;
		GameSessionManager.Inst.EnemiesEscaped++;
		GameSessionManager.Inst.EscapeEnemy(this);
	}

	public override void NotifyDead()
	{
		base.NotifyDead();

		SetDefeated(true);
		GameSessionManager.Inst.DestroyEnemy(this);
		gameObject.SetActive(false);
		
		//Destroy(gameObject);

		GameSessionManager.Inst.EnemiesKilled++;
	}

	#endregion

	#region collision

	protected override void OnMovingBlockHit(Block b)
	{
		//Debug.LogError("HEY");
		base.OnMovingBlockHit(b);

		//Debug.Log("Hit block: " + b.name);

		// Stop movement
		Freeze();

		// Attach to the block
		BlockKiller = b;

		b.AttachEntity(this);
	}

	protected override void Freeze()
	{
		base.Freeze();
		StopAI();
		Collider.enabled = false;
	}

	#endregion

	#region state events

	#endregion
}
