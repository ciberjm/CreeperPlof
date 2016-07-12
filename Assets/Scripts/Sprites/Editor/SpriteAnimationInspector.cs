using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SpriteAnimation))]
public class SpriteAnimationInspector : Editor
{
	#region constants
	
	#endregion
	
	#region vars

	private SpriteAnimation.AnimState mSASelected;
	private SpriteAnimation mObj;

	#endregion
	
	#region properties
	
	#endregion
	
	#region init

	protected virtual void OnEnable()
	{
		mObj = target as SpriteAnimation;
	}
	
	// Use this for initialization
	protected virtual void OnDisable()
	{
	
	}
	
	#endregion

	#region update

	public override void OnInspectorGUI()
	{
		mObj.mSpriteRenderer = (SpriteRenderer)EditorGUILayout.ObjectField("Sprite: ", mObj.mSpriteRenderer, typeof(SpriteRenderer), true);

		mObj.InitialState = (SpriteAnimation.AnimState)EditorGUILayout.EnumPopup("Initial State: ", mObj.InitialState);

		// Header
		EditorGUILayout.BeginHorizontal();
		{
			mSASelected = (SpriteAnimation.AnimState)EditorGUILayout.EnumPopup("State: ", mSASelected);
			if (GUILayout.Button("Generate"))
			{
				if (mObj.AnimationFrames == null)
					mObj.AnimationFrames = new SpriteAnimation.AnimDictionary();

				if (!mObj.AnimationFrames.Contains(mSASelected))
				{
					mObj.AnimationFrames[mSASelected] = new SpriteAnimation.AnimStateData() { AnimState = mSASelected, Loop = true, Frames = new SpriteAnimation.FrameData[1] };
					mObj.AnimationFrames[mSASelected].Frames[0] = new SpriteAnimation.FrameData();
				}
			}
		}
		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Generate all"))
		{
			if (mObj.AnimationFrames == null)
				mObj.AnimationFrames = new SpriteAnimation.AnimDictionary();

			foreach (var v in EnumUtil.GetValues<SpriteAnimation.AnimState>())
			{
				if (!mObj.AnimationFrames.Contains(v))
				{
					mObj.AnimationFrames[v] = new SpriteAnimation.AnimStateData() { AnimState = v, Loop = true, Frames = new SpriteAnimation.FrameData[1] };
					mObj.AnimationFrames[v].Frames[0] = new SpriteAnimation.FrameData();
				}
			}
		}

		EditorGUILayout.BeginHorizontal();
		{
			if (mObj.IsPlaying)
			{
				EditorGUILayout.LabelField("Playing: " + mObj.CurrentState.AnimState + " " + mObj.FrameIndex + " " + mObj.CurrentFrame.Sprite.name);
			}
			else
			{
				EditorGUILayout.LabelField("Not Playing");
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Play"))
			{
				mObj.Play(mSASelected);
			}
			if (GUILayout.Button("Stop"))
			{
				mObj.Stop();
			}
		}
		EditorGUILayout.EndHorizontal();

		// Frames
		if (mObj.AnimationFrames != null)
		{
			foreach (var kp in mObj.AnimationFrames.Animations)
			{
				var val = kp;
				bool b = EditorGUILayout.Foldout(true, val.AnimState.ToString());
				if (b)
				{
					EditorGUI.indentLevel += 2;
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("Options: ");
						if (GUILayout.Button("Add Frame"))
						{
							List<SpriteAnimation.FrameData> l = new List<SpriteAnimation.FrameData>(val.Frames);
							l.Insert(0, new SpriteAnimation.FrameData());
							val.Frames = l.ToArray();
							break;
						}
						if (GUILayout.Button("Remove State"))
						{
							mObj.AnimationFrames.Remove(val.AnimState);
							break;
						}
					}
					EditorGUILayout.EndHorizontal();
					val.Loop = EditorGUILayout.ToggleLeft(" Loop", val.Loop);
					bool b2 = EditorGUILayout.Foldout(true, "Frames");
					if (b2)
					{
						EditorGUI.indentLevel += 2;
						for (int i = 0; i < val.Frames.Length; ++i)
						{
							SpriteAnimation.FrameData fd = val.Frames[i];
							if (fd == null)
							{
								break;
							}
							
							fd.Sprite = (Sprite)EditorGUILayout.ObjectField(i.ToString("00"), fd.Sprite, typeof(Sprite), false);
							fd.Duration = Mathf.Clamp(EditorGUILayout.FloatField("Duration: ", fd.Duration), 0f, 5f);
							fd.MessageName = EditorGUILayout.TextField("Message: ", fd.MessageName);

							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.LabelField("Options: ");
								if (GUILayout.Button("Add"))
								{
									List<SpriteAnimation.FrameData> l = new List<SpriteAnimation.FrameData>(val.Frames);
									l.Insert(i + 1, new SpriteAnimation.FrameData());
									val.Frames = l.ToArray();
									break;
								}
								if (GUILayout.Button("Remove"))
								{
									List<SpriteAnimation.FrameData> l = new List<SpriteAnimation.FrameData>(val.Frames);
									l.RemoveAt(i);
									val.Frames = l.ToArray();
									break;
								}
							}
							EditorGUILayout.EndHorizontal();
						}
						EditorGUI.indentLevel -= 2;
					}
					EditorGUI.indentLevel -= 2;

				}
			}
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(mObj);
		}
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion
}
