using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PrefabManager : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public GameObject[] Prefabs;

	public Dictionary<string, GameObject> PrefabsDict;

	#endregion
	
	#region properties

	public static PrefabManager Inst { get; private set; }

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
			if (PrefabsDict == null || Prefabs == null || Prefabs.Length != PrefabsDict.Count)
			{
				RebuildFactory();
			}
		}
	}
	
	#endregion
	
	#region protected methods

	protected void RebuildFactory()
	{
		PrefabsDict = new Dictionary<string, GameObject>();
		foreach (var v in Prefabs)
		{
			PrefabsDict[v.name] = v;
		}
	}
	
	#endregion
	
	#region public methods
	
	#endregion
}
