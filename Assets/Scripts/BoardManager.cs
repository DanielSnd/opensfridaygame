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
    /// Test tile prefab, we'll change this later.
    /// </summary>
    public Tile testTile;
    
    /// <summary>
    /// This actually keeps track of what each piece of the board is (wall/floor for now).
    /// </summary>
    public int[,] map;

    /// <summary>
    /// This holds a reference to each Tile prefab spawned for the board.
    /// </summary>
    public Tile[,] mapTiles;
    
    /// <summary>
    /// Sets up the outer walls and floor (background) of the game board.
    /// </summary>
    void BoardSetup()
    {
        //Set our map/maptiles array size.
        map = new int[rows+1,columns+1];
        mapTiles = new Tile[rows+1, columns+1];

        //Go over each spot on the array and decide whether to make it a floor or wall.
        //In this case I'm making the edges walls, and everything else floors.
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                map[x, y] = (y == 0 || y == map.GetLength(0) - 1 || x == 0 || x == map.GetLength(1) - 1) ? 1 : 0;
            }
        }

        //Go over the whole map and spawn tiles, and assign the tile values based on the map.
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                Tile _tileref = testTile.Spawn();
                mapTiles[x, y] = _tileref;
                _tileref.tilePosition = new TilePosition(x, y);
                _tileref.transform.position = new Vector3((Mathf.Floor(rows/2) * tileSize) - (x *tileSize), (Mathf.Floor(columns/2) * tileSize) -(y*tileSize), 0);
                if (map[x, y] == 0)
                {
                    _tileref.spriteRenderer.color = Color.grey;
                }
                else
                {
                    _tileref.spriteRenderer.color = Color.black;
                }
                _tileref.gameObject.name = "Tile " + x + "/" + y;
                _tileref.transform.SetParent(transform);
            }
        }

    }

    /// <summary>
    /// On awake set our singleton reference.
    /// </summary>
    void Awake()
    {
        get = this;
    }

    /// <summary>
    /// On start call the board setup, to spawn the board.
    /// </summary>
    void Start () {
	    BoardSetup();
	}

    /// <summary>
    /// Get tile reference based on tile position.
    /// </summary>
    /// <param name="tp">position of the tile we're enquiring about</param>
    /// <returns>tile reference on that position</returns>
    public Tile GetTile(TilePosition tp)
    {
        return mapTiles[tp.x, tp.y];
    }

    /// <summary>
    /// Returns a random empty floor spot on the board
    /// </summary>
    /// <returns>Tileposition of a random floor spot.</returns>
    public TilePosition GetRandomEmptyFloor()
    {
        List<TilePosition> returnEmptyFloors = new List<TilePosition>();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if(map[x,y] == 0) returnEmptyFloors.Add(new TilePosition(x,y));
            }
        }
        return returnEmptyFloors[Random.Range(0, returnEmptyFloors.Count)];
    }

    /// <summary>
    /// Tries to move actor based on command.
    /// </summary>
    /// <param name="_actor">Actor to move</param>
    /// <param name="_command">Command to execute</param>
    /// <returns>Success or fail</returns>
    public bool TryMove(Actor _actor, Commands _command)
    {
        if (_actor == null) return false;

        TilePosition desiredPosition = _actor.currentPosition;
        switch (_command)
        {
            case Commands.None:
                break;
            case Commands.Down:
                desiredPosition.y++;
                break;
            case Commands.Up:
                desiredPosition.y--;
                break;
            case Commands.Right:
                desiredPosition.x--;
                break;
            case Commands.Left:
                desiredPosition.x++;
                break;
        }

        if (!isPositionEmpty(desiredPosition.x, desiredPosition.y))
        {
            Debug.Log(_actor.actorName+" Desired position isn't empty");
            return false;
        }

        _actor.currentPosition = desiredPosition;
        Debug.Log(_actor.actorName + " Movement is valid, let's try moving");
        Vector3 _newTilePosition = mapTiles[desiredPosition.x, desiredPosition.y].transform.position;
        _actor.transform.DOMove(_newTilePosition, 0.25f).SetEase(Ease.OutBack);
        return true;
    }

    /// <summary>
    /// Checks if position X/Y is valid on the board (isn't out of range).
    /// </summary>
    public bool isPositionValid(int x, int y)
    {
        if (x < 0 || x >= map.GetLength(0))
        {
            return false;
        }
        if (y < 0 || y >= map.GetLength(1))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if position X/Y is is empty on the board (No walls or actors).
    /// </summary>
    public bool isPositionEmpty(int x, int y)
    {
        if (!isPositionValid(x, y))
        {
            Debug.Log("Position invalid");
            return false;
        }
        if (map[x, y] != 0)
        {
            Debug.Log("Position is not 0");
            return false;
        }
        if (isActorInPosition(x, y) != null)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Get an actor in position X,Y if there is one.
    /// </summary>
    public Actor isActorInPosition(int x, int y)
    {
        for (int i = 0; i < Actor.actorList.Count; i++)
        {
            if (Actor.actorList[i].currentPosition.x == x && Actor.actorList[i].currentPosition.y == y)
                return Actor.actorList[i];
        }
        return null;
    }
}
