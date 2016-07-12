using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct Coord2D
{
	#region constants

	public static readonly Coord2D zero		= new Coord2D(+0, +0);
	public static readonly Coord2D one		= new Coord2D(+1, +1);

	public static readonly Coord2D up		= new Coord2D(+0, +1);
	public static readonly Coord2D left		= new Coord2D(-1, +0);
	public static readonly Coord2D down		= new Coord2D(+0, -1);
	public static readonly Coord2D right	= new Coord2D(+1, +0);

	#endregion
	
	#region vars

	[SerializeField]
	public int x;

	[SerializeField]
	public int y;

	#endregion
	
	#region properties
	
	#endregion
	
	#region init

	public Coord2D(int _x, int _y)
	{
		x = _x;
		y = _y;
	}

	public Coord2D(Vector2 _v)
	{
		x = (int)_v.x;
		y = (int)_v.y;
	}

	#endregion

	#region static methods

	#endregion
	
	#region overloading methods

	public string Dump()
	{
		return "(" + x + ", " + y + ")";
	}

	public Vector2 ToVector2()
	{
		return new Vector2(x, y);
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator+(Coord2D c1, Coord2D c2)
	{
		return new Coord2D(c1.x + c2.x, c1.y + c2.y);
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator +(Coord2D c1, IntDir c2)
	{
		return c1 + c2.ToCoord2D();
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator-(Coord2D c1, Coord2D c2)
	{
		return new Coord2D(c1.x - c2.x, c1.y - c2.y);
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator*(Coord2D c1, Coord2D c2)
	{
		return new Coord2D(c1.x * c2.x, c1.y * c2.y);
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator *(Coord2D c1, float c2)
	{
		return new Coord2D(c1.x * (int)c2, c1.y * (int)c2);
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator *(Coord2D c1, int c2)
	{
		return new Coord2D(c1.x * c2, c1.y * c2);
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator/(Coord2D c1, Coord2D c2)
	{
		return new Coord2D(c1.x / c2.x, c1.y / c2.y);
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator /(Coord2D c1, float c2)
	{
		return new Coord2D(c1.x / (int)c2, c1.y / (int)c2);
	}

	// Declare which operator to overload (+), the types 
	// that can be added (two Complex objects), and the 
	// return type (Complex):
	public static Coord2D operator /(Coord2D c1, int c2)
	{
		return new Coord2D(c1.x / c2, c1.y / c2);
	}

	public override bool Equals(object obj)
	{
		// If parameter is null return false.
		if (obj == null)
		{
			return false;
		}

		// If parameter cannot be cast to Point return false.
		Coord2D p = (Coord2D)obj;
		if ((object)p == null)
		{
			return false;
		}

		// Return true if the fields match:
		return (x == p.x) && (y == p.y);
	}

	public bool Equals(Coord2D p)
	{
		// If parameter is null return false:
		if ((object)p == null)
		{
			return false;
		}

		// Return true if the fields match:
		return (x == p.x) && (y == p.y);
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public static bool operator ==(Coord2D a, Coord2D b)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b))
		{
			return true;
		}

		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null))
		{
			return false;
		}

		// Return true if the fields match:
		return a.x == b.x && a.y == b.y;
	}

	public static bool operator !=(Coord2D a, Coord2D b)
	{
		return !(a == b);
	}

	#endregion

	public int distTo(Coord2D target)
	{
		return Mathf.Abs(target.x - this.x) + Mathf.Abs(target.y - this.y);
	}
}
