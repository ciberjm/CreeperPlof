using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SpriteFactory : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public Sprite[] SpritePrefabs;

	public Dictionary<string, Sprite> SpriteDict;

	#endregion
	
	#region properties

	public static SpriteFactory Inst { get; private set; }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
		Inst = this;
		RebuildFactory();
	}
	
	// Use this for initialization
	protected virtual void Start()
	{

	}
	
	#endregion

	#region update
	
	// Update is called once per frame
	protected virtual void Update()
	{
		if (!Application.isPlaying) 
		{
			Inst = this;
			if (SpriteDict == null || SpritePrefabs == null || SpritePrefabs.Length != SpriteDict.Count)
			{
				RebuildFactory();
			}
		}
	}
	
	#endregion
	
	#region protected methods

	protected void RebuildFactory()
	{
		SpriteDict = new Dictionary<string, Sprite>();
		foreach (var v in SpritePrefabs)
		{
			SpriteDict[v.name] = v;
		}
	}
	
	#endregion
	
	#region public methods
	
	#endregion
}
