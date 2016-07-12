using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TitleScreen : ScreenBase
{
	#region constants

	private static readonly string[] TEXTS = { 
												 "Random text like in Minecraft, but shittier",
												 "Now with more hats!",
												 "Hi Mom!",
												 "This is what you do when you are bored at home",
												 "I should stop doing these messages and go to sleep",
												 "Nevermind",
												 "As good as... nothing, actually",
												 "The next update will include steamworks",
												 "I should go back and complete The Witcher 2",
												 "I love writting these messages",
												 "Someday JJJJJJ will return!",
												 "The flickering text was a bug but it was so fun to watch that I kept it",
												 "Hello world",
												 "Not powered by Unreal Engine 4",
												 "Pollobot isn't dead!!",
												 "Based on Don't Pull which was based on Pengo",
											 };

	#endregion

	#region vars

	public override ScreenID ID
	{
		get { return ScreenID.Title; }
	}

	public JUIText TitleObj;
	public JUIText InsertCoinObj;
	public JUIText RandomTextObj;

	private float mTextAnimationTimer;

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
		RandomTextObj.gameObject.SetActive(false);
	}

	// Update is called once per frame
	protected override void Update()
	{
		CheckInput();
		base.Update();

		if (Mathf.FloorToInt(TimeOpened) % 4 == 0)
		{
			UpdateText();
		}
	}

	protected virtual void CheckInput()
	{
		if (Input.GetButtonDown("Push"))
		{
			ScreenManager.Inst.OpenScreen(ScreenID.HowToPlay);
		}
	}

	protected virtual void UpdateText()
	{
		RandomTextObj.gameObject.SetActive(true);
		RandomTextObj.Text = TEXTS[Random.Range(0, TEXTS.Length)];
	}

	#endregion

	#region protected methods


	#endregion

	#region public methods

	#endregion
}
