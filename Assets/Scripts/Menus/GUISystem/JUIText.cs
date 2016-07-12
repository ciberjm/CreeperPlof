using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class JUIText : MonoBehaviour
{
	#region constants
	
	#endregion
	
	#region vars

	public Vector3 BGOffset;

	public SpriteRenderer BackgroundGO;
	public TextMesh TextGO;

	#endregion
	
	#region properties

	public string Text { get { return GetText(); } set { SetText(value); } }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
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

	protected void SetText(string s)
	{
		TextGO.text = s;

		UpdateMesh();
	}

	protected string GetText()
	{
		return TextGO.text;
	}

	#endregion
	
	#region public methods

	public void UpdateMesh()
	{
		// Update background
		Bounds b = TextGO.GetComponent<Renderer>().bounds;
		BackgroundGO.transform.localScale = (b.size / 2f).CoordinateDivide(TextGO.transform.lossyScale).CoordinateProduct(TextGO.transform.localScale) + BGOffset;
	}

	#endregion
}
