using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ScreenID
{
	None = 0,
	Title = 1,
	Gameplay = 2,
	GameOver = 3,
	Pause = 4,
	LevelCompleted = 5,
	LevelStart = 6,
	HowToPlay = 7,
}

public class ScreenManager : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public Camera ScreensCamera;

	public ScreenBase[] ScreenPrefabs;

	private Dictionary<ScreenID, ScreenBase> mScreenMap;

	#endregion
	
	#region properties
	
	public static ScreenManager Inst { get; protected set; }

	public ScreenBase CurrentScreen { get; protected set; }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
		Inst = this;
		mScreenMap = new Dictionary<ScreenID, ScreenBase>();

		Transform screenParent = transform.FindChild("Screens");

		for (int i = 0; i < ScreenPrefabs.Length; ++i)
		{
			var screenPrefab = ScreenPrefabs[i];

			// Instantiate
			ScreenBase screen = (ScreenBase)Instantiate(screenPrefab, Vector3.zero, Quaternion.identity);

			screen.gameObject.name = screenPrefab.name;

			// Put under the manager
			screen.transform.parent = screenParent;

			// Deactivate
			screen.gameObject.SetActive(false);

			// Save
			mScreenMap[screen.ID] = screen;
		}
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
	
	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods

	public void OpenScreen(ScreenID _screen)
	{
		if (CurrentScreen != null)
		{
			CurrentScreen.OnExit();
			CurrentScreen.OnClose();
			CurrentScreen.gameObject.SetActive(false);
		}

		if (mScreenMap.ContainsKey(_screen))
		{
			ScreenBase screen = mScreenMap[_screen];
			screen.gameObject.SetActive(true);
			CurrentScreen = screen;
			screen.OnOpen();
		}
		else
		{
			Debug.LogError("Screen: " + _screen + " doesn't exist!");
		}
	}

	#endregion
}
