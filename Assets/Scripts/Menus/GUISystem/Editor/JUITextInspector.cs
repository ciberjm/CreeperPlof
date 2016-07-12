using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(JUIText))]
public class JUITextInspector : Editor
{
	#region constants
	
	#endregion
	
	#region vars
	
	#endregion
	
	#region properties
	
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

	public override void OnInspectorGUI()
	{
		var mObject = (JUIText)target;

		var s = EditorGUILayout.TextField(mObject.Text);
		if (s != mObject.Text)
		{
			mObject.Text = s;
			GUI.changed = true;
		}

		var v = EditorGUILayout.Vector3Field("Background Offset: ", mObject.BGOffset);
		if (v != mObject.BGOffset)
		{
			mObject.BGOffset = v;
			mObject.UpdateMesh();
			GUI.changed = true;
		}

		if (GUILayout.Button("Update mesh"))
		{
			mObject.UpdateMesh();
			GUI.changed = true;
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(mObject);
		}

	}
	
	#endregion
	
	#region protected methods
	
	
	#endregion
	
	#region public methods
	
	#endregion
}
