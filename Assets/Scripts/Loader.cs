using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Loader
{
	#region constants
	
	#endregion
	
	#region vars
	
	#endregion
	
	
	#region init

	public static void LoadTitleMenu()
	{
        SceneManager.LoadScene("Title");
		ScreenManager.Inst.OpenScreen(ScreenID.Title);
	}

	public static void LoadGameplayScene()
	{
        SceneManager.LoadScene("Gameplay");
	}
	
	#endregion

	#region update
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion

	internal static void OnLevelLoaded()
	{
		GameObject world = new GameObject("World");
		world.AddComponent<World>().InitializeGame();
	}
}
