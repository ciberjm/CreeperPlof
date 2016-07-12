using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TiledMap : MonoBehaviour
{
	#region constants

	// Separation between layers for the isometric effect
	public const int LayerHeight = 40;

	public static readonly Dictionary<TileType, string> BLOCKS = new Dictionary<TileType, string> 
	{
		{ TileType.Tile_Grass, "BlockGreen" },
		{ TileType.Tile_Plain, "BlockGray" },
		{ TileType.Tile_Water, "BlockBlue" },
	};

	#endregion

	#region BlockMatrix

	private LevelInfo mCurrentLevel;
	public Dictionary<Coord2D, Block> BlockEntities;

	#endregion

	#region vars

	public Coord2D BlockSize;
	public Sprite terrainBlock;

	public Vector3[] LevelOffsets;

	public bool update = false;
	public bool drawGizmos = false;

	public Coord2D TileSize { get { return mCurrentLevel.Size; } }
	public Vector3 mCenteringOffset;

	private Transform EntitiesTransform;

	#endregion
	
	#region properties

	public static TiledMap Inst { get; private set; }

	#endregion
	
	#region init

	protected virtual void Awake()
	{
		Inst = this;
	}
	
	// Use this for initialization
	public void InitMap()
	{
		mCurrentLevel = LevelInfo.GenerateRandomMap(GameSessionManager.Inst.Round + GameCore.Inst.seed, GameSessionManager.Inst.Round);
		CalculateSizes();
		FillTiles();
		PlaceCharacters();
		PlaceOutsiders();
		//GameSessionManager.Inst.TotalEnemies = mCurrentLevel.TotalEnemies;
	}

	public void ResetLevel()
	{
		ResetBlocks();
		ResetCharacters();
	}

	public void ResetBlocks()
	{
		// Blocks
		BlockEntities = new Dictionary<Coord2D, Block>();

		GameObject blockGO = transform.FindChild("Blocks").gameObject;
		blockGO.transform.DestroyAllChilds();

		for (int i = 0; i < TileSize.x; ++i)
		{
			for (int j = 0; j < TileSize.y; ++j)
			{
				if (mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BlockLevel][i][j] != TileType.None)
				{
					GameObject prefab = null;
					switch (mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BlockLevel][i][j])
					{
						case TileType.Block_Normal:
							prefab = PrefabManager.Inst.PrefabsDict["Block"];
							break;

						case TileType.Block_Unbreakable:
							prefab = PrefabManager.Inst.PrefabsDict["BlockUnbreakable"];
							break;

						case TileType.Block_Fragile:
							prefab = PrefabManager.Inst.PrefabsDict["BlockFragile"];
							break;

						case TileType.Block_OnlyOnce:
							prefab = PrefabManager.Inst.PrefabsDict["BlockOnlyOnce"];
							break;

						default:
							Debug.LogError("Wrong type in floor level: " + mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BlockLevel][i][j]);
							break;
					}

					GameObject go = (GameObject)Instantiate(prefab);
					go.name = ("Block " + "(" + i.ToString("00") + ", " + j.ToString("00") + ")");
					go.transform.SetParentAndResetTrans(blockGO.transform);
					go.transform.position = GetPos(i, j, LevelInfo.BlockMatrixLevel.BlockLevel).ToVector3_XY();
				}
			}
		}
	}

	public void ResetCharacters()
	{
		// Players
		EntitiesTransform.DestroyAllChilds();

		for (int i = 0; i < TileSize.x; ++i)
		{
			for (int j = 0; j < TileSize.y; ++j)
			{
				if (mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.FloorLevel][i][j] != TileType.None)
				{
					//Debug.Log("Spawning something...");
					GameObject prefab = null;
					switch (mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.FloorLevel][i][j])
					{
						case TileType.PlayerSpawn_1:
							prefab = PrefabManager.Inst.PrefabsDict["Player"];
							break;

						case TileType.EnemySpawner_Wanderer:
							prefab = PrefabManager.Inst.PrefabsDict["EnemySpawnable"];
							break;

						default:
							Debug.LogError("Wrong type in floor level: " + mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.FloorLevel][i][j]);
							break;
					}

					GameObject go = (GameObject)Instantiate(prefab);
					go.name = prefab.name;
					go.transform.SetParentAndResetTrans(EntitiesTransform);
					go.transform.position = GetCenterPos(i, j, LevelInfo.BlockMatrixLevel.BlockLevel).ToVector3_XY();

					//Debug.Log("Spawned!");
				}
			}
		}

		PlaceOutsiders();
	}

	protected void CalculateSizes()
	{
		mCenteringOffset = ((TileSize * BlockSize) / 2).ToVector2().ToVector3_XY() * -1f;
	}

	protected void FillTiles()
	{
		// Remove all childs
		transform.DestroyAllChilds();

		GameObject tileGO = new GameObject("Tile");
		tileGO.transform.SetParentAndResetTrans(transform);

		// Generate floor level matrix
		for (int i = 0; i < TileSize.x; ++i)
		{
			for (int j = 0; j < TileSize.y; ++j)
			{
				TileType bt = mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BottomLevel][i][j];

				if (bt == TileType.None)
				{
					// Nothing
				}
				else if (bt == TileType.EmptyArea)
				{
					// Nothing
				}
				else if (bt == TileType.OutsideArea)
				{
					// Outside stuff, spawn grass
					GameObject go = new GameObject("OTile " + "(" + i.ToString("00") + ", " + j.ToString("00") + ")");
					go.transform.SetParentAndResetTrans(tileGO.transform);

					SpriteRenderer sr = go.AddComponent<SpriteRenderer>();

					string name = BLOCKS[TileType.Tile_Grass];
					sr.sprite = SpriteFactory.Inst.SpriteDict[name];
					sr.sortingLayerID = SortingLayer.layers[1].id;
					sr.sortingOrder = j;

					go.transform.position = GetPos(i, j, LevelInfo.BlockMatrixLevel.BottomLevel).ToVector3_XY();
				}
				else
				{
					GameObject go = new GameObject("Tile " + "(" + i.ToString("00") + ", " + j.ToString("00") + ")");
					go.transform.SetParentAndResetTrans(tileGO.transform);

					SpriteRenderer sr = go.AddComponent<SpriteRenderer>();

					string name = BLOCKS[bt];
					sr.sprite = SpriteFactory.Inst.SpriteDict[name];
					sr.sortingLayerID = SortingLayer.layers[1].id;
                    sr.sortingOrder = j;

					go.transform.position = GetPos(i, j, LevelInfo.BlockMatrixLevel.BottomLevel).ToVector3_XY();
				}
			}
		}

		// Floor deco

		// Blocks
		BlockEntities = new Dictionary<Coord2D, Block>();

		GameObject blockGO = new GameObject("Blocks");
		blockGO.transform.SetParentAndResetTrans(transform);

		for (int i = 0; i < TileSize.x; ++i)
		{
			for (int j = 0; j < TileSize.y; ++j)
			{
				if (mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BlockLevel][i][j] != TileType.None)
				{
					GameObject prefab = null;
					switch (mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BlockLevel][i][j])
					{
						case TileType.Block_Normal:
							prefab = PrefabManager.Inst.PrefabsDict["Block"];
							break;

						case TileType.Block_Unbreakable:
							prefab = PrefabManager.Inst.PrefabsDict["BlockUnbreakable"];
							break;

						case TileType.Block_Fragile:
							prefab = PrefabManager.Inst.PrefabsDict["BlockFragile"];
							break;

						case TileType.Block_OnlyOnce:
							prefab = PrefabManager.Inst.PrefabsDict["BlockOnlyOnce"];
							break;

						default:
							Debug.LogError("Wrong type in floor level: " + mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BlockLevel][i][j]);
							break;
					}

					GameObject go = (GameObject)Instantiate(prefab);
					go.name = ("Block " + "(" + i.ToString("00") + ", " + j.ToString("00") + ")");
					go.transform.SetParentAndResetTrans(blockGO.transform);
					go.transform.position = GetPos(i, j, LevelInfo.BlockMatrixLevel.BlockLevel).ToVector3_XY();
				}
			}
		}

		// Top deco
	}

	public void PlaceCharacters()
	{
		// Players
		GameObject charGO = new GameObject("Characters");
		charGO.transform.SetParentAndResetTrans(transform);
		EntitiesTransform = charGO.transform;

		for (int i = 0; i < TileSize.x; ++i)
		{
			for (int j = 0; j < TileSize.y; ++j)
			{
				if (mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.FloorLevel][i][j] != TileType.None)
				{
					//Debug.Log("Spawning something...");
					GameObject prefab = null;
					switch (mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.FloorLevel][i][j])
					{
						case TileType.PlayerSpawn_1:
							prefab = PrefabManager.Inst.PrefabsDict["Player"];
							break;

						case TileType.EnemySpawner_Wanderer:
							prefab = PrefabManager.Inst.PrefabsDict["EnemySpawnable"];
							break;

						default:
							Debug.LogError("Wrong type in floor level: " + mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.FloorLevel][i][j]);
							break;
					}

					GameObject go = (GameObject)Instantiate(prefab);
					go.name = prefab.name;
					go.transform.SetParentAndResetTrans(EntitiesTransform);
					go.transform.position = GetCenterPos(i, j, LevelInfo.BlockMatrixLevel.BlockLevel).ToVector3_XY();

					//Debug.Log("Spawned!");
				}
			}
		}
	}

	public void PlaceOutsiders()
	{
		// Spawn outsiders
		int onlyVer = Mathf.Clamp(mCurrentLevel.OutsideTiles.x - mCurrentLevel.OutsideTiles.y, 0, int.MaxValue);
		int onlyHor = Mathf.Clamp(mCurrentLevel.OutsideTiles.y - mCurrentLevel.OutsideTiles.x, 0, int.MaxValue);
		int both = Mathf.Min(mCurrentLevel.OutsideTiles.x, mCurrentLevel.OutsideTiles.y);

		Coord2D tileMin = mCurrentLevel.OutsideTiles;
		Coord2D tileMax = mCurrentLevel.Size - mCurrentLevel.OutsideTiles;

		// Both
		for (int j = 0; j < both; ++j)
		{
			Coord2D start = new Coord2D(tileMin.x - j - 1, tileMin.y - j - 1);
			Coord2D end = new Coord2D(tileMax.x + j, tileMax.y + j);


			if (j % 2 == 0)
			{
				Coord2D initial = new Coord2D(start.x, Mathf.FloorToInt((end.y - start.y) / 2f));
				SpawnOutsider(initial, start, end, false, IntDir.Down);
			}
			else
			{
				Coord2D initial = new Coord2D(end.x, Mathf.FloorToInt((end.y - start.y) / 2f));
				SpawnOutsider(initial, start, end, true, IntDir.Down);
			}
		}

		// Horizontal
		for (int j = 0; j < onlyHor; ++j)
		{
			SpawnOutsider(new Coord2D(0, 0 + j), new Coord2D(0, 0 + j), new Coord2D(mCurrentLevel.Size.x - 1, 0 + j), false, IntDir.Right);
			SpawnOutsider(new Coord2D(mCurrentLevel.Size.x - 1, mCurrentLevel.Size.y - j - 1), new Coord2D(0, mCurrentLevel.Size.y - j - 1), new Coord2D(mCurrentLevel.Size.x - 1, mCurrentLevel.Size.y - j - 1), true, IntDir.Left);
		}

		for (int j = 0; j < onlyVer; ++j)
		{
			SpawnOutsider(new Coord2D(0 + j, 0), new Coord2D(0 + j, 0), new Coord2D(0 + j, mCurrentLevel.Size.y - 1), false, IntDir.Down);
			SpawnOutsider(new Coord2D(mCurrentLevel.Size.x - j - 1, mCurrentLevel.Size.y - 1), new Coord2D(mCurrentLevel.Size.x - j - 1, 0), new Coord2D(mCurrentLevel.Size.x - j - 1, mCurrentLevel.Size.y - 1), true, IntDir.Up);
		}
	}

	public void SpawnOutsider(Coord2D _spawnPos, Coord2D _start, Coord2D _end, bool _clockWise, IntDir _initialDir)
	{
		GameObject prefab = PrefabManager.Inst.PrefabsDict["EnemyOutsider"];
		GameObject go = (GameObject)Instantiate(prefab);
		go.name = prefab.name;
		go.transform.SetParentAndResetTrans(EntitiesTransform);
		go.transform.position = GetCenterPos(_spawnPos.x, _spawnPos.y, LevelInfo.BlockMatrixLevel.BlockLevel).ToVector3_XY();

		var e = go.GetComponent<EnemyOutsider>();
		e.InitialDir = _initialDir;
		e.StartPath = _start;
		e.EndPath = _end;

		e.ClockWise = _clockWise;
	}

	#endregion

	#region update
	
	#endregion
	
	#region protected methods
	
	#endregion
	
	#region public methods

	/*
	 * Sorting layers:
	 *		- Something
	 *		- Block
	 *		- Players/Enemies
	 * 
	 * */
	public int GetSortingLayer(bool _isBlock, int _Cy)
	{
		return (_Cy * 3) + (_isBlock ? 1 : 2);
	}

	public TileType GetFloorData(Coord2D c)
	{
		if (c.x >= 0 && c.y >= 0 && c.x < mCurrentLevel.Size.x && c.y < mCurrentLevel.Size.y)
		{
			return mCurrentLevel.BlockMatrix[(int)LevelInfo.BlockMatrixLevel.BottomLevel][c.x][c.y];
		}
		return TileType.None;
	}

	public void SpawnEnemy(Coord2D c)
	{
		GameObject prefab = PrefabManager.Inst.PrefabsDict["EnemyWanderer"];
		GameObject go = (GameObject)Instantiate(prefab);
		go.name = prefab.name;
		go.transform.SetParentAndResetTrans(EntitiesTransform);
		go.transform.position = GetCenterPos(c.x, c.y, LevelInfo.BlockMatrixLevel.BlockLevel).ToVector3_XY();
	}

	public void UpdateBlock(Block block)
	{
		if (BlockEntities == null)
			BlockEntities = new Dictionary<Coord2D, Block>();
		BlockEntities[block.LastTiledCoordinates] = null;
		BlockEntities[block.TiledCoordinates] = block;
	}

	public void RemoveBlock(Block block)
	{
		BlockEntities[block.TiledCoordinates] = null;
	}

	public Block GetBlockAt(Coord2D newCoord)
	{
		Block b;
		if (BlockEntities.TryGetValue(newCoord, out b))
		{
			return b;
		}
		return null;
	}

	public Vector2 GetCenterPos(Coord2D c, LevelInfo.BlockMatrixLevel layer)
	{
		return GetCenterPos(c.x, c.y, layer);
	}

	public Vector2 GetCenterPos(int x, int y, LevelInfo.BlockMatrixLevel layer)
	{
		return (BlockSize / 2).ToVector2() + GetPos(x, y, layer);
	}

	public Vector2 GetPos(Coord2D c, LevelInfo.BlockMatrixLevel layer)
	{
		return GetPos(c.x, c.y, layer);
	}

	public Vector2 GetPos(int x, int y, LevelInfo.BlockMatrixLevel layer)
	{
		Vector3 offset = LevelOffsets[(int)layer];
		return offset + mCenteringOffset + (new Coord2D(BlockSize.x * x, (TileSize.y - 1 - y) * BlockSize.y)).ToVector2().ToVector3_XY();
	}

	public Coord2D GetCoords(float x, float y)
	{
		return new Coord2D(Mathf.FloorToInt((x - mCenteringOffset.x) / BlockSize.x),
			TileSize.y - 1 - Mathf.FloorToInt((y - mCenteringOffset.y) / BlockSize.y));
	}

	public Coord2D GetCoords(Vector2 _pos)
	{
		return GetCoords(_pos.x, _pos.y);
	}

	public Coord2D GetCoords(Vector3 _pos)
	{
		return GetCoords(_pos.x, _pos.y);
	}
	
	#endregion

	#region OnDrawGizmos

	public void OnDrawGizmos()
	{
		if (drawGizmos)
		{
			Gizmos.color = Color.green;
			for (int i = 0; i <= TileSize.x; ++i)
			{
				Gizmos.DrawLine(mCenteringOffset + new Vector3(BlockSize.x * i, 0f, 0f), mCenteringOffset + new Vector3(BlockSize.x * i, BlockSize.y * TileSize.y, 0f));
			}

			for (int j = 0; j <= TileSize.y; ++j)
			{
				Gizmos.DrawLine(mCenteringOffset + new Vector3(0f, BlockSize.y * j, 0f), mCenteringOffset + new Vector3(BlockSize.x * TileSize.x, BlockSize.y * j, 0f));
			}

			float eps = 0.1f;
			Vector3 pos = mCenteringOffset + new Vector3(0f, 0f, 0f);
			Coord2D cd = new Coord2D(0, 0);
			int f = 0;
			while (f < 10000)
			{
				f++;
				Vector3 prevpos = pos;
				pos.y += eps;

				cd = GetCoords(pos);

				if (cd.y % 2 == 0)
					Gizmos.color = Color.yellow;
				else
					Gizmos.color = Color.blue;
				Gizmos.DrawLine(prevpos, pos);
			}


			if (BlockEntities != null)
			{
				for (int i = 0; i < TileSize.x; ++i)
				{
					for (int j = 0; j < TileSize.y; ++j)
					{
						Coord2D c2d = new Coord2D(i, j);
						Block b = GetBlockAt(c2d);
						if (b != null)
						{
							Gizmos.color = Color.red;
							Gizmos.DrawWireCube(GetCenterPos(c2d, LevelInfo.BlockMatrixLevel.BlockLevel), BlockSize.ToVector2() / 2f);
						}
					}
				}
			}
		}
	}

	#endregion

	public List<BaseEntity> GetEntitiesIn(Coord2D _coords)
	{
		List<BaseEntity> lbe = new List<BaseEntity>();

		foreach (Transform t in EntitiesTransform)
		{
			BaseEntity be = t.GetComponent<BaseEntity>();

			if (be.TiledCoordinates == _coords)
			{
				lbe.Add(be);
			}
		}

		return lbe;
	}
}
