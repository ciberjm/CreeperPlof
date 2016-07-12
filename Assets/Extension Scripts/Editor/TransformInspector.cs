using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Transform))]
public class TransformInspector : Editor
{
	public float DrawFloatControl(string _label, float _val, float _defaultVal)
	{
		float ret = 0;
		EditorGUILayout.BeginHorizontal();
		{
			ret = EditorGUILayout.FloatField(_label, _val);
			if (GUILayout.Button("R", GUILayout.MaxWidth(25f)))
			{
				ret = _defaultVal;
			}
		}
		EditorGUILayout.EndHorizontal();
		return ret;
	}

	public Vector3 DrawVector3Control(string _label, Vector3 _val, Vector3 _defaultVal)
	{
		Vector3 ret;
		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button(_label, GUILayout.MaxWidth(35f)))
			{
				ret = _defaultVal;
			}
			else
			{
				ret.x = DrawFloatControl("X", _val.x, _defaultVal.x);
				ret.y = DrawFloatControl("Y", _val.y, _defaultVal.y);
				ret.z = DrawFloatControl("Z", _val.z, _defaultVal.z);				
			}
		}
		EditorGUILayout.EndVertical();
		return ret;
	}

	private static bool msLocalCoords = true;

	public override void OnInspectorGUI()
	{
		Vector3 position;
		Vector3 eulerAngles;
		Vector3 scale;

		Transform t = (Transform)target;

        // Replicate the standard transform inspector gui
        EditorGUIUtility.labelWidth = 0;
        EditorGUIUtility.fieldWidth = 0;
		EditorGUI.indentLevel = 0;
		EditorGUIUtility.labelWidth = 15f;

		if (GUILayout.Button(msLocalCoords ? "Local Space" : "World Space"))
		{
			msLocalCoords = !msLocalCoords;
		}

		if (msLocalCoords)
		{
			position	= DrawVector3Control("LP: ", t.localPosition, Vector3.zero);
			eulerAngles	= DrawVector3Control("LR: ", t.localEulerAngles, Vector3.zero);
			scale		= DrawVector3Control("LS: ", t.localScale, Vector3.one);
		}
		else
		{
			position	= DrawVector3Control("WP: ", t.position, Vector3.zero);
			eulerAngles	= DrawVector3Control("WR: ", t.eulerAngles, Vector3.zero);
			scale		= DrawVector3Control("WS: ", t.lossyScale, Vector3.one);
		}

		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Reset transform"))
			{
				position = Vector3.zero;
				eulerAngles = Vector3.zero;
				scale = Vector3.one;
			}
		}
		EditorGUILayout.EndHorizontal();

		//EditorGUIUtility.LookLikeInspector();

		if (GUI.changed)
		{
			//Undo.RegisterUndo(t, "Transform Change");

			Undo.RecordObject(t, "Transform Change");
			if (msLocalCoords)
			{
				t.localPosition = FixIfNaN(position);
				t.localEulerAngles = FixIfNaN(eulerAngles);
				t.localScale = FixIfNaN(scale);
			}
			else
			{
				t.position = FixIfNaN(position);
				t.eulerAngles = FixIfNaN(eulerAngles);
				//t.lossyScale = FixIfNaN(scale);
			}
		}
	}

	private Vector3 FixIfNaN(Vector3 v)
	{
		if (float.IsNaN(v.x))
		{
			v.x = 0;
		}
		if (float.IsNaN(v.y))
		{
			v.y = 0;
		}
		if (float.IsNaN(v.z))
		{
			v.z = 0;
		}
		return v;
	}

}