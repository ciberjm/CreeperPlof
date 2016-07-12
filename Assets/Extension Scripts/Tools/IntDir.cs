using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntDir
{
	#region constants

	public enum EnumDir { Up = 0, Down = 1, Left = 2, Right = 3 }
	
	public static readonly IntDir Up	= new IntDir(EnumDir.Up);
	public static readonly IntDir Down	= new IntDir(EnumDir.Down);
	public static readonly IntDir Left	= new IntDir(EnumDir.Left);
	public static readonly IntDir Right	= new IntDir(EnumDir.Right);

	#endregion
	
	#region vars

	#endregion
	
	#region properties

	public EnumDir Enum { get; private set; }
	public Vector2 V2 {	get	{ return ToVector2(); } }
	public Vector3 V3 { get { return ToVector3(); } }

	#endregion
	
	#region init

	private IntDir(EnumDir _val)
	{
		Enum = _val;
	}

	#endregion
	
	#region protected methods
	
	#endregion
	
	#region public methods

	public static IntDir GetRandom()
	{
		var gd = GetDirs();
		return gd[Random.Range(0, gd.Length)];
	}

	public static IntDir[] GetDirs()
	{
		return new IntDir[] { IntDir.Up, IntDir.Right, IntDir.Down, IntDir.Left };
	}

	public static IntDir[] GetDirs(System.Predicate<IntDir> _pred)
	{
		var l = new List<IntDir>();
		var gd = GetDirs();
		for (int i = 0; i < gd.Length; ++i)
		{
			if (_pred(gd[i]))
			{
				l.Add(gd[i]);
			}
		}
		return l.ToArray();
	}

	public static IntDir FromVector2(Vector2 v)
	{
		if (v != Vector2.zero)
		{
			if (Mathf.Abs(v.y) > Mathf.Abs(v.x))
			{
				if (v.y > 0)
				{
					return IntDir.Up;
				}
				else
				{
					return IntDir.Down;
				}
			}
			else
			{
				if (v.x > 0)
				{
					return IntDir.Right;
				}
				else
				{
					return IntDir.Left;
				}
			}
		}

		return null;
	}

	public Vector2 ToVector2()
	{
		switch (Enum)
		{
			case EnumDir.Up: // Up
				return Vector2.up;

			case EnumDir.Down: // Down
				return -Vector2.up;

			case EnumDir.Left: // Left
				return -Vector2.right;

			case EnumDir.Right: // Right
				return Vector2.right;
		}
		return Vector2.zero;
	}

	public Vector3 ToVector3()
	{
		switch (Enum)
		{
			case EnumDir.Up: // Up
				return Vector3.up;

			case EnumDir.Down: // Down
				return Vector3.down;

			case EnumDir.Left: // Left
				return Vector3.left;

			case EnumDir.Right: // Right
				return Vector3.right;
		}
		return Vector3.zero;
	}

	public Coord2D ToCoord2D()
	{
		switch (Enum)
		{
			case EnumDir.Up: // Up
				return Coord2D.down; // Inverted in map

			case EnumDir.Down: // Down
				return Coord2D.up;

			case EnumDir.Left: // Left
				return Coord2D.left;

			case EnumDir.Right: // Right
				return Coord2D.right;
		}
		return Coord2D.zero;
	}

	public IntDir Opposite()
	{
		switch (Enum)
		{
			case EnumDir.Up: // Up
				return IntDir.Down;

			case EnumDir.Down: // Down
				return IntDir.Up;

			case EnumDir.Left: // Left
				return IntDir.Right;

			case EnumDir.Right: // Right
				return IntDir.Left;
		}
		return null;
	}

	public IntDir ClockWiseNext()
	{
		switch (Enum)
		{
			case EnumDir.Up: // Up
				return IntDir.Right;

			case EnumDir.Down: // Down
				return IntDir.Left;

			case EnumDir.Left: // Left
				return IntDir.Up;

			case EnumDir.Right: // Right
				return IntDir.Down;
		}
		return null;
	}


	public IntDir ClockWisePrev()
	{
		switch (Enum)
		{
			case EnumDir.Up: // Up
				return IntDir.Left;

			case EnumDir.Down: // Down
				return IntDir.Right;

			case EnumDir.Left: // Left
				return IntDir.Down;

			case EnumDir.Right: // Right
				return IntDir.Up;
		}
		return null;
	}

	#endregion

}
