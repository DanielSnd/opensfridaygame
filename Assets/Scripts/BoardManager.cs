using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using DG.Tweening;
using Random = UnityEngine.Random;

/// <summary>
/// Struct to hold row/column positions
/// </summary>
public struct TilePosition
{
    public int x;
    public int y;

    public TilePosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(TilePosition a, TilePosition b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(TilePosition c1, TilePosition c2)
    {
        return !c1.Equals(c2);
    }
}

public enum TType
{
    Floor = 0,
    Wall = 1
}

public class BoardManager : MonoBehaviour
{
    //Holds the singleton reference
    private static BoardManager _instance;
    /// <summary>
    /// Gets and sets the singleton reference.
    /// </summary>
    public static BoardManager get
    {
        get { return _instance; }
        set { _instance = value; }
    }

    /// <summary>
    /// Amount of columns on the board.
    /// </summary>
    public int columns = 8;

    /// <summary>
    /// Amount of rows on the board.
    /// </summary>
    public int rows = 8;

    /// <summary>
    /// TileSize for our tile sprites.
    /// </summary>
    public int tileSize = 32;

    /// <summary>
    /// Percentage of tiles our Tunneler should dig before stopping.
    /// </summary>
    public float digTilePct = 0.4f;

    /// <summary>
    /// Test tile prefab, we'll change this later.
    /// </summary>
    public Tile testTile;
    
    /// <summary>
    /// This actually keeps track of what each piece of the board is (wall/floor for now).
    /// </summary>
    public static TType[,] map;

    /// <summary>
    /// This holds a reference to each Tile prefab spawned for the board.
    /// </summary>
    public static Tile[,] mapTiles;

    public bool boardReady = true;

    public void BoardSetup()
    {
        if (!boardReady)
        {
            StartCoroutine(DOBoardSetup());
        }
    }

    /// <summary>
    /// Sets up the outer walls and floor (background) of the game board.
    /// </summary>
    public IEnumerator DOBoardSetup()
    {
        boardReady = false;
        GameManager.ResetAll();

        //Set our map/maptiles array size.
        map = new TType[rows+1,columns+1];
        mapTiles = new Tile[rows+1, columns+1];

        //Go over each spot on the array and make them all walls.
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                map[x, y] = TType.Wall;
            }
        }

        //Go over the whole map and spawn tiles, and assign the tile values based on the map.
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                Tile _tileref = testTile.Spawn();
                mapTiles[x, y] = _tileref;
                _tileref.tilePosition = new TilePosition(x, y);
                _tileref.transform.position = new Vector3((Mathf.Floor(rows/2) * tileSize) - (x *tileSize), (Mathf.Floor(columns/2) * tileSize) -(y*tileSize), 0);
                PaintTile(x, y);
                _tileref.gameObject.name = "Tile " + x + "/" + y;
                _tileref.transform.SetParent(transform);
            }
        }

        yield return null;

        //Get a random position to start our tunneler.
        TilePosition centerPos = new TilePosition(Mathf.FloorToInt(map.GetLength(0)*0.5f),Mathf.FloorToInt(map.GetLength(1)*0.5f));

        List<TilePosition> aroundCenterPos = BoardMethod.GetTilesAroundTile(centerPos, 4, TType.Wall);
        
        TilePosition tunnelerPos = aroundCenterPos[Random.RandomRange(0,aroundCenterPos.Count)];

        int amountToDig = Mathf.FloorToInt((map.GetLength(0) - 2) * (map.GetLength(1) - 2) * digTilePct);
        yield return null;
        Commands tunnelerCommand = (Commands)Random.Range(1, 5);
        while (amountToDig>0)
        {
            yield return null;
            if (BoardMethod.getTileType(tunnelerPos.x, tunnelerPos.y) == TType.Wall)
            {
                float roomChance = 0.05f;
                if (BoardMethod.CountTilesAlignedAroundTile(tunnelerPos, 3, TType.Floor) == 2)
                {
                    roomChance = 0.55f;
                }
                if (Random.value < roomChance)
                {
                    var tilesAllAround = BoardMethod.GetTilesAroundTile(tunnelerPos, 1, TType.Wall);
                    amountToDig -= 3;
                    foreach (TilePosition _tilePosition in tilesAllAround)
                    {
                        TurnToFloor(_tilePosition);
                    }
                }
                else
                {
                    amountToDig--;
                    TurnToFloor(tunnelerPos);
                }
            }
            TilePosition possibleTunnelerPos = BoardMethod.GetTileFromCommand(tunnelerPos, tunnelerCommand);
            if (BoardMethod.isPositionValid(possibleTunnelerPos))
            {
                tunnelerPos = possibleTunnelerPos;
            }
            else
            {
                tunnelerCommand = Random.value < 0.5
                    ? BoardMethod.GetNextCommand(tunnelerCommand)
                    : BoardMethod.GetPrevCommand(tunnelerCommand);
            }

            if (Random.value < 0.25f)
            {
                tunnelerCommand = Random.value < 0.5
                    ? BoardMethod.GetNextCommand(tunnelerCommand)
                    : BoardMethod.GetPrevCommand(tunnelerCommand);
            }
        }

        boardReady = true;

        for (int i = 0; i < 3; i++)
        {
           GameManager.get.SpawnEnemy();
        }
    }

    public void TurnToFloor(TilePosition tp)
    {
        map[tp.x,tp.y] = TType.Floor;
        PaintTile(tp);
    }

    private static void PaintTile(TilePosition tp)
    {
        PaintTile(tp.x, tp.y);
    }

    private static void PaintTile(int x, int y)
    {
        //if (!BoardMethod.isPositionValid(x, y)) return;
        if (map[x, y] == TType.Floor)
        {
            if (mapTiles[x,y] != null)
                mapTiles[x, y].spriteRenderer.color = Color.grey;
        }
        else
        {
            if(mapTiles[x, y] != null)
                mapTiles[x, y].spriteRenderer.color = Color.black;
        }
    }

    /// <summary>
    /// On awake set our singleton reference.
    /// </summary>
    void Awake()
    {
        get = this;
        map = new TType[rows + 1, columns + 1];
        mapTiles = new Tile[rows + 1, columns + 1];
    }

    /// <summary>
    /// On start call the board setup, to spawn the board.
    /// </summary>
    void Start ()
    {
        StartCoroutine(DOBoardSetup());
    }
    
    /// <summary>
    /// Tries to move actor based on command.
    /// </summary>
    /// <param name="_actor">Actor to move</param>
    /// <param name="_command">Command to execute</param>
    /// <returns>Success or fail</returns>
    public bool TryCommand(Actor _actor, Commands _command)
    {
        if (_actor == null) return false;

        TilePosition desiredPosition = BoardMethod.GetTileFromCommand(_actor, _command);

        if (!BoardMethod.isPositionEmpty(desiredPosition))
        {
            Debug.Log(_actor.actorName+" Desired position isn't empty");
            Actor _actorInPosition = BoardMethod.GetActorInPosition(desiredPosition);
            if (_actorInPosition != null)
            {
                _actor.Attack(_actorInPosition);
                return true;
            }
            return false;
        }

        _actor.currentPosition = desiredPosition;
        Debug.Log(_actor.actorName + " command is valid, let's try moving");
        Vector3 _newTilePosition = mapTiles[desiredPosition.x, desiredPosition.y].transform.position;
        _actor.transform.DOMove(_newTilePosition, 0.25f).SetEase(Ease.OutBack);
        return true;
    }
}
