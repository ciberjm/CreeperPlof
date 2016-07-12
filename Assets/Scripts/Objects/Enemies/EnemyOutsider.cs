using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyOutsider : Enemy
{
	#region constants
	
	#endregion
	
	#region vars

	private float mTick = 0.2f;
	private float mTimer = 0;

	public Coord2D StartPath;
	public Coord2D EndPath;

	public bool ClockWise = false;
	public IntDir InitialDir = IntDir.Down;
	
	protected List<Vector2> mMoveOptions;

	#endregion
	
	#region properties

	public bool StopCR { get; protected set; }
	public override bool CanBeDefeated { get { return false; } }

	public override float TimeEscapeValue
	{
		get { return 0f; }
	}

	public override Enemy.EnemyType EType
	{
		get { return EnemyType.Outsider; }
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
		Direction = InitialDir;
		mMoveOptions = new List<Vector2>();
		StopCR = false;
	}
	
	#endregion

	#region update
	
	#endregion
	
	#region protected methods

	protected override IEnumerator ExecuteAI()
	{
		StopCR = false;
		ExecutingAI = true;
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

			// Get options
			mMoveOptions.Clear();

			Coord2D newPos = TiledCoordinates + Direction.ToCoord2D();

			if ((newPos.x >= StartPath.x && newPos.y >= StartPath.y && newPos.x <= EndPath.x && newPos.y <= EndPath.y) && CanMoveTo(newPos))
			{
				mMoveOptions.Add(Direction.ToVector2());
			}
			else
			{
				if (ClockWise)
				{
					Direction = Direction.ClockWiseNext();
				}
				else
				{
					Direction = Direction.ClockWisePrev();
				}
			}

			if (mMoveOptions.Count > 0)
			{
				mVelocityInput = mMoveOptions[Random.Range(0, mMoveOptions.Count)];
			}

			yield return 0;

			mVelocityInput = Vector2.zero;

			yield return new WaitForSeconds(mTick);
			mTimer += mTick;

			if (mTimer > 5f)
			{
				mTimer = 0f;
				mTick = Mathf.Clamp(mTick - (mTick / 5f), 0.01f, mTick);
				
				Speed += 100;
				if (Speed > 2000)
					Speed = 2000;
			}

		}

		ExecutingAI = false;
		yield return 0;
	}
	
	#endregion
	
	#region public methods

	protected override void OnEnemyHit(Enemy e)
	{
		base.OnEnemyHit(e);
	}

	public override void EndOfSmashWithBlock(Block block, IntDir _direction)
	{
	}

	#endregion
}
