using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public GameObject TileMapPrefab;

	#endregion
	
	#region properties
	
	#endregion
	
	#region init

	protected virtual void Awake()
	{
		GameObject go = (GameObject)Instantiate(TileMapPrefab);
		go.transform.parent = transform;

		go.name = "TileMap";
	}
	
	// Use this for initialization
	protected virtual void Start()
	{
        if (FindObjectOfType<GameCore>() == null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
			Loader.OnLevelLoaded();
        }
	}
	
	#endregion

	#region update
	
	// Update is called once per frame
	protected virtual void Update()
	{
	
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion
}
