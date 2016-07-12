using UnityEngine;
using System.Collections.Generic;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class ExtensionMethods
{
	//Even though they are used like normal methods, extension
	//methods must be declared static. Notice that the first
	//parameter has the 'this' keyword followed by a Transform
	//variable. This variable denotes which class the extension
	//method becomes a part of.
	public static void ResetTransformation(this Transform trans)
	{
		trans.position = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = new Vector3(1, 1, 1);
	}

	public static void SetParentAndResetTrans(this Transform trans, Transform _parent)
	{
		trans.parent = _parent;
		trans.ResetTransformation();
	}

	public static List<GameObject> GetAllChildsGO(this Transform trans)
	{
		var children = new List<GameObject>();
		foreach (Transform child in trans)
			children.Add(child.gameObject);
		return children;
	}

	public static void DestroyAllChilds(this Transform trans)
	{
		var children = new List<GameObject>();
		foreach (Transform child in trans)
			children.Add(child.gameObject);
		children.ForEach(x => EditorTools.DestroyInEditor(x));
	}

	public static Transform GetChildForced(this Transform trans, string _name)
	{
		Transform t = trans.FindChild(_name);
		if (t == null)
		{
			GameObject go = new GameObject(_name);
			t = go.transform;
			t.parent = trans;
		}
		return t;
	}
}