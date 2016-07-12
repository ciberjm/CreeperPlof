using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameCore : MonoBehaviour
{
	#region constants

	#endregion
	
	#region vars

	public GameObject[] Prefabs;

	public int seed = 0;

	#endregion
	
	#region properties
	
	public static GameCore Inst { get; protected set; }
	
	#endregion
	
	#region init

	protected virtual void Awake()
	{
		Inst = this;
		DontDestroyOnLoad(this);

		// Load prefabs
		foreach (var pref in Prefabs)
		{
			GameObject go = (GameObject)Instantiate(pref);
			go.transform.parent = transform;
			go.name = "[" + pref.name + "]";
		}

		// Load start of the game
		Loader.LoadTitleMenu();
	}
	
	// Use this for initialization
	protected virtual void Start()
	{
	
	}
	
	#endregion

	#region update
	
	#endregion
	
	#region protected methods
	
	#endregion
	
	#region public methods
	
	#endregion
}
