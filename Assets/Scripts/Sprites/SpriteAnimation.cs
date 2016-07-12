using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpriteAnimation : MonoBehaviour
{
	#region constants

	public enum AnimState
	{
		None = 0, 
		
		Idle_Up = 11,
		Idle_Down = 12,
		Idle_Left = 13,
		Idle_Right = 14,
		
		Walking_Up = 20, 
		Walking_Down = 21,
		Walking_Left = 22,
		Walking_Right = 23,
		
		Push_Up = 30,
		Push_Down = 31,
		Push_Left = 32,
		Push_Right = 33,
		
		Dying = 40,
		
		Dead = 50,

		Victory = 60,
	}

	[System.Serializable]
	public class AnimDictionary
	{
		[SerializeField]
		public List<AnimStateData> Animations;

		public AnimDictionary()
		{
			Animations = new List<AnimStateData>();
		}

		public AnimStateData this[AnimState _ast]
		{
			get { return GetState(_ast); }
			set { SetState(_ast, value); }
		}

		public void Remove(AnimState _ast)
		{
			Animations.RemoveAll(x => x.AnimState == _ast);
		}

		public bool Contains(AnimState _ast)
		{
			return Animations.Find(x => x.AnimState == _ast) != null;
		}

		private AnimStateData GetState(AnimState _ast)
		{
			return Animations.Find(x => x.AnimState == _ast);
		}

		private void SetState(AnimState _ast, AnimStateData _val)
		{
			Remove(_ast);
			Animations.Add(_val);
		}
	}

	[System.Serializable]
	public class AnimStateData
	{
		[SerializeField]
		public AnimState AnimState;

		[SerializeField]
		public FrameData[] Frames;

		[SerializeField]
		public bool Loop;
	}

	[System.Serializable]
	public class FrameData
	{
		/// <summary>
		/// Sprite frame
		/// </summary>
		[SerializeField]
		public Sprite Sprite;

		/// <summary>
		/// Frame duration in seconds
		/// </summary>
		[SerializeField]
		public float Duration;

		[SerializeField]
		public string MessageName;	
	}

	#endregion
	
	#region vars

	public AnimDictionary AnimationFrames;

	public SpriteRenderer mSpriteRenderer;

	public SpriteAnimation.AnimState InitialState;

	#endregion
	
	#region properties

	public AnimStateData CurrentState { get; protected set; }
	public bool IsPlaying { get; protected set; }

	public FrameData CurrentFrame { get; protected set; }
	public int FrameIndex { get; private set; }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
	}
	
	// Use this for initialization
	protected virtual void Start()
	{
		IsPlaying = false;
		FrameIndex = 0;

		Play(InitialState);
	}
	
	#endregion

	#region update
	
	#endregion
	
	#region protected methods
	
	#endregion
	
	#region public methods
	
	#endregion

	public void Play(AnimState mSASelected)
	{
		if (!IsPlaying || (IsPlaying && (CurrentState != null && mSASelected != CurrentState.AnimState)))
		{
			var NextState = AnimationFrames[mSASelected];

			if (NextState != null)
			{
				CurrentState = NextState;
				Stop();

				FrameIndex = 0;
				StartCoroutine("FrameCoroutine");
			}
		}
	}

	public void Pause(bool _pause)
	{
		IsPlaying = _pause;
	}

	public void Stop()
	{
		StopCoroutine("FrameCoroutine");
		IsPlaying = false;
	}

	private IEnumerator FrameCoroutine()
	{
		IsPlaying = true;
		//Debug.Log("Animation Started: " + CurrentState.AnimState);
		while (true)
		{
			CurrentFrame = CurrentState.Frames[FrameIndex];

			mSpriteRenderer.sprite = CurrentFrame.Sprite;

			if (CurrentFrame.Duration > 0f)
			{
				yield return new WaitForSeconds(CurrentFrame.Duration);
			}
			else
			{
				yield return 0;
			}

			//Debug.Log("Frame " + FrameIndex + " ended");


			FrameIndex++;
			if (FrameIndex >= CurrentState.Frames.Length)
			{
				if (CurrentState.Loop)
				{
					FrameIndex = 0;
				}
				else
				{
					break;
				}
			}
		}

		IsPlaying = false;
		//Debug.Log("Animation finished" + CurrentState.AnimState);
		yield return 0;
	}
}
