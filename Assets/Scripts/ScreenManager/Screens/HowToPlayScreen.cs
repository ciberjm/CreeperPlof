using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HowToPlayScreen : ScreenBase
{
	#region constants

	#endregion
	
	#region vars

	public override ScreenID ID
	{
		get { return ScreenID.HowToPlay; }
	}

	public JUIText TitleObj;
	public GameObject Player;
	public GameObject Enemy;
	public GameObject Block;

	#endregion
	
	#region properties
	
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
	}
	
	#endregion

	#region update

	public override void OnOpen()
	{
 		base.OnOpen();

		Player.SetActive(false);
		Enemy.SetActive(false);
		Block.SetActive(false);
	}
	
	// Update is called once per frame
	protected override void Update()
	{
		CheckInput();
		base.Update();

		if (TimeOpened > 8f)
		{
			Loader.LoadGameplayScene();
		}
		else if (TimeOpened > 3f)
		{
			Block.SetActive(true);
		}
		else if (TimeOpened > 2f)
		{
			Enemy.SetActive(true);
		} 
		else if (TimeOpened > 1f)
		{
			Player.SetActive(true);
			Player.GetComponentInChildren<SpriteAnimation>().Play(Player.GetComponentInChildren<SpriteAnimation>().InitialState);
		}
		
	}

	protected virtual void CheckInput()
	{
		if (Input.GetButtonDown("Push"))
		{
			Loader.LoadGameplayScene();
		}
	}

	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion
}
