using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region tile type

public enum TileType
{
	None = 0,
	OutsideArea = 1,
	EmptyArea = 2,

	Tile_Plain = 100,
	Tile_Grass = 101,
	Tile_Wood = 102,
	Tile_Dirt = 103,
	Tile_DirtStone = 104,
	Tile_Water = 105,
	Tile_WoodStone = 106,

	PlayerSpawn_1 = 1001,
	PlayerSpawn_2 = 1002,
	PlayerSpawn_3 = 1003,
	PlayerSpawn_4 = 1004,

	Block_Normal = 5001,
	Block_Unbreakable = 5002,
	Block_Fragile = 5003,
	Block_OnlyOnce = 5004,

	EnemySpawner_Wanderer = 10001,
}

#endregion

#region level info

public class LevelInfo
{
	#region consts/types

	private const int MIN_WIDTH = 8;
	private const int MAX_WIDTH = 10;
	private const int MIN_HEIGHT = 6;
	private const int MAX_HEIGHT = 7;

	public enum BlockMatrixLevel { BottomLevel = 0, FloorLevel = 1, BlockLevel = 2, TopLevel = 3, Max = 4 }
	
	#endregion

	#region vars

	public Coord2D Size { get; private set; }

	public int TotalEnemies { get; private set; }

	public TileType[][][] BlockMatrix { get; private set; }

	public Coord2D OutsideTiles { get; private set; }

	#endregion

	#region static funcs

	public static LevelInfo GenerateRandomMap(int _seed, int _difficulty)
	{
		LevelInfo li = new LevelInfo();
		System.Random rand = new System.Random(_seed);

		float probHole = rand.Next(0, 5 + Mathf.FloorToInt(_difficulty / 5f)) / 100f;

		li.Size = new Coord2D(rand.Next(MIN_WIDTH, MAX_WIDTH) * 2, rand.Next(MIN_HEIGHT, MAX_HEIGHT) * 2);

		li.OutsideTiles = new Coord2D(rand.Next(Mathf.Clamp(2 - (_difficulty / 2), 0, 2), li.Size.x / 4), rand.Next(Mathf.Clamp(2 - (_difficulty / 2), 0, 2), li.Size.y / 4));

		li.BlockMatrix = new TileType[(int)LevelInfo.BlockMatrixLevel.Max][][];

		// This is for spawning stuff INSIDE of the tiles
		List<Coord2D> insideC2Ds = new List<Coord2D>();

		// Bottom placement
		li.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BottomLevel] = new TileType[li.Size.x][];

		var bottomMatrix = li.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BottomLevel];

		for (int i = 0; i < bottomMatrix.Length; ++i)
		{
			bottomMatrix[i] = new TileType[li.Size.y];
			for (int j = 0; j < bottomMatrix[i].Length; ++j)
			{
				if (((float)rand.NextDouble()) < probHole)
				{
					bottomMatrix[i][j] = TileType.EmptyArea;
				}
				else if (i < li.OutsideTiles.x || j < li.OutsideTiles.y || i >= (li.Size.x - li.OutsideTiles.x) || j >= (li.Size.y - li.OutsideTiles.y))
				{
					bottomMatrix[i][j] = TileType.OutsideArea;
				}
				else
				{
					bottomMatrix[i][j] = TileType.Tile_Plain;
					insideC2Ds.Add(new Coord2D(i, j));
				}
			}
		}

		// Floor deco
		li.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.FloorLevel] = new TileType[li.Size.x][];
		var floorMatrix = li.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.FloorLevel];
		for (int i = 0; i < floorMatrix.Length; ++i)
		{
			floorMatrix[i] = new TileType[li.Size.y];
			for (int j = 0; j < floorMatrix[i].Length; ++j)
			{
				floorMatrix[i][j] = TileType.None;
			}
		}

		// Player and enemy placement
		int nPlayers = 1;
		for (int i = 0; i < nPlayers; ++i)
		{
			int pos = rand.Next(0, insideC2Ds.Count);
			Coord2D c = insideC2Ds[pos];
			insideC2Ds.RemoveAt(pos);

			floorMatrix[c.x][c.y] = TileType.PlayerSpawn_1 + i;
		}

		li.TotalEnemies = 0;
		int nEnemies = Mathf.Clamp(rand.Next(3 + _difficulty, 6 + _difficulty), 5, insideC2Ds.Count - 4);
		for (int i = 0; i < nEnemies && insideC2Ds.Count > 0; ++i)
		{
			int pos = rand.Next(0, insideC2Ds.Count);
			Coord2D c = insideC2Ds[pos];
			insideC2Ds.RemoveAt(pos);

			floorMatrix[c.x][c.y] = TileType.EnemySpawner_Wanderer;
			li.TotalEnemies++;
		}

		// Block placement
		li.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BlockLevel] = new TileType[li.Size.x][];

		// Fill with empty values
		var blockLevelMatrix = li.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BlockLevel];
		for (int i = 0; i < blockLevelMatrix.Length; ++i)
		{
			blockLevelMatrix[i] = new TileType[li.Size.y];
			for (int j = 0; j < blockLevelMatrix[i].Length; ++j)
			{
				blockLevelMatrix[i][j] = TileType.None;
			}
		}

		foreach (var c in insideC2Ds)
		{
			TileType t = TileType.None;
			if (rand.NextDouble() < 0.5)
			{
				t = TileType.Block_Normal;
			}
			//else if (rand.NextDouble() < 0.1)
			//{
			//	t = TileType.Block_Fragile;
			//}
			else if (rand.NextDouble() < 0.05)
			{
				t = TileType.Block_Unbreakable;
			}
			//else if (rand.NextDouble() < 0.05)
			//{
			//	t = TileType.Block_OnlyOnce;
			//}

			blockLevelMatrix[c.x][c.y] = t;
		}

		return li;
	}

	#endregion
}



#endregion
