using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EditorTools
{
	public static void DestroyInEditor(Object _obj)
	{
		if (Application.isPlaying)
		{
			UnityEngine.MonoBehaviour.Destroy(_obj);
		}
		else
		{
			UnityEngine.MonoBehaviour.DestroyImmediate(_obj);
		}
	}
}
