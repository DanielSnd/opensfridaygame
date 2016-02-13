using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class BoardMethod {


    /// <summary>
    /// Get tile reference based on tile position.
    /// </summary>
    /// <param name="tp">position of the tile we're enquiring about</param>
    /// <returns>tile reference on that position</returns>
    public static Tile GetTile(TilePosition tp)
    {
        return GetTile(tp.x, tp.y);
    }



    public static Tile GetTile(int x, int y)
    {
        return isPositionValid(x, y) ? BoardManager.mapTiles[x, y] : null;
    }

    public static bool isPositionValid(TilePosition tp)
    {
        return isPositionValid(tp.x, tp.y);
    }

    /// <summary>
    /// Checks if position X/Y is valid on the board (isn't out of range).
    /// </summary>
    public static bool isPositionValid(int x, int y)
    {
        if (x < 1 || x >= BoardManager.map.GetLength(0)-1)
        {
            return false;
        }
        if (y < 1 || y >= BoardManager.map.GetLength(1)-1)
        {
            return false;
        }
        return true;
    }


    public static int CountTilesAlignedAroundTile(TilePosition tp, int range, TType _type)
    {
        return GetTilesAlignedAroundTile(tp, range, _type).Count;
    }

    public static int CountTilesAroundTile(TilePosition tp, int range, TType _type)
    {
        return GetTilesAroundTile(tp, range, _type).Count;
    }
    
    public static bool isPositionEmpty(TilePosition tp)
    {
        return isPositionEmpty(tp.x, tp.y);
    }

    /// <summary>
    /// Checks if position X/Y is is empty on the board (No walls or actors).
    /// </summary>
    public static bool isPositionEmpty(int x, int y)
    {
        if (!isPositionValid(x, y))
        {
            return false;
        }
        if (BoardManager.map[x, y] != TType.Floor)
        {
            return false;
        }
        if (GetActorInPosition(x, y) != null)
        {
            return false;
        }
        return true;
    }

    public static Actor GetActorInPosition(TilePosition tp)
    {
        return GetActorInPosition(tp.x, tp.y);
    }

    /// <summary>
    /// Get an actor in position X,Y if there is one.
    /// </summary>
    public static Actor GetActorInPosition(int x, int y)
    {
        for (int i = 0; i < Actor.actorList.Count; i++)
        {
            if (Actor.actorList[i].currentPosition.x == x && Actor.actorList[i].currentPosition.y == y)
                return Actor.actorList[i];
        }
        return null;
    }


    public static List<TilePosition> GetTilesAlignedAroundTile (TilePosition centerTile, int range = 1, TType _type = TType.Floor)
    {
        List<TilePosition> returnlist = new List<TilePosition>();
        for (int x = centerTile.x-range; x <= (centerTile.x+range); x++)
        {
            if (isPositionValid(x, centerTile.y) && getTileType(x, centerTile.y) == _type)
            {
                returnlist.Add(new TilePosition(x, centerTile.y));
            }
        }

        for (int y = (centerTile.y - range); y <= (centerTile.y + range); y++)
        {
            if (isPositionValid(centerTile.x, y) && getTileType(centerTile.x, y) == _type)
            {
                returnlist.Add(new TilePosition(centerTile.x, y));
            }
        }

        return returnlist;
    }

    public static List<TilePosition> GetTilesAroundTile(TilePosition centerTile, int range = 1, TType _type = TType.Floor)
    {
        List<TilePosition> returnlist = new List<TilePosition>();
        for (int x = centerTile.x - range; x <= (centerTile.x + range); x++)
        {
            for (int y = (centerTile.y - range); y <= (centerTile.y + range); y++)
            {
                if (isPositionValid(x, y) && getTileType(x,y) == _type)
                {
                    returnlist.Add(new TilePosition(x, y));
                }
            }
        }
        return returnlist;
    }

    public static bool isTileAligned(int tyle1x, int tyle1y, int tyle2x, int tyle2y)
    {
        return (tyle1x == tyle2x || tyle1y == tyle2y);
    }

    public static TType getTileType(int x, int y)
    {
        return BoardManager.map[x, y];
    }

    /// <summary>
    /// Returns a random empty floor spot on the board
    /// </summary>
    /// <returns>Tileposition of a random floor spot.</returns>
    public static TilePosition GetRandomTile(TType _type = TType.Floor)
    {
        List<TilePosition> returnTilesOfType = new List<TilePosition>();
        for (int y = 1; y < BoardManager.map.GetLength(1)-1; y++)
        {
            for (int x = 1; x < BoardManager.map.GetLength(0)-1; x++)
            {
                if ( isPositionValid(x,y) && BoardManager.map[x, y] == _type) returnTilesOfType.Add(new TilePosition(x, y));
            }
        }
        return returnTilesOfType[Random.Range(0, returnTilesOfType.Count)];
    }

    public static ActorPlayer GetClosestPlayer(TilePosition current)
    {
        Dictionary<TilePosition, int> ProcessedTiles = GetTileStepsDictionary(current,current);
        ActorPlayer closestPlayer = null;
        int closestDistance = 99999;
        foreach (ActorPlayer _player in GameManager.playerDictionary.Values)
        {
            if (ProcessedTiles.ContainsKey(_player.currentPosition) && ProcessedTiles[_player.currentPosition] < closestDistance)
            {
                closestDistance = ProcessedTiles[_player.currentPosition];
                closestPlayer = _player;
            }
        }

        return closestPlayer;
    }

    public static Commands GetCommandTowardsTile(TilePosition current, TilePosition goal)
    {
        //GetTile(current).spriteRenderer.color = Color.blue;
        //GetTile(goal).spriteRenderer.color = Color.green;
        //GetTile(goal).textMesh.text = "0";
        TilePosition desiredTile = current;
        var ProcessedTiles = GetTileStepsDictionary(goal, current);

        if (ProcessedTiles.ContainsKey(current))
        {
            int desiredStep = ProcessedTiles[current] - 1;
            List<TilePosition> tilesAroundCurrent = GetTilesAlignedAroundTile(current);
            List<TilePosition> possibleMoves = new List<TilePosition>();
            for (int i = 0; i < tilesAroundCurrent.Count; i++)
            {
                if (ProcessedTiles.ContainsKey(tilesAroundCurrent[i]) &&
                    ProcessedTiles[tilesAroundCurrent[i]] == desiredStep)
                {
                    possibleMoves.Add(tilesAroundCurrent[i]);
                    if (possibleMoves.Count > 1)
                    {
                        break;
                    }
                }
            }
            if (possibleMoves.Count > 0)
            {
                desiredTile = possibleMoves[Random.Range(0, possibleMoves.Count)];
            }

            //GetTile(desiredTile).spriteRenderer.color = Color.yellow;
        }

        return GetCommandToAdjacentTile(current,desiredTile);
    }

    private static Dictionary<TilePosition, int> GetTileStepsDictionary(TilePosition goal, TilePosition current)
    {
        Dictionary<TilePosition, int> TilesAlreadyProcessed = new Dictionary<TilePosition, int>();
        Queue<TilePosition> ProcessTiles = new Queue<TilePosition>();
        ProcessTiles.Enqueue(goal);
        TilesAlreadyProcessed.Add(goal, 0);
        while (ProcessTiles.Count > 0)
        {
            TilePosition processingPosition = ProcessTiles.Dequeue();
            List<TilePosition> tilesAround = GetTilesAlignedAroundTile(processingPosition);
            for (int i = 0; i < tilesAround.Count; i++)
            {
                if (!TilesAlreadyProcessed.ContainsKey(tilesAround[i]))
                {
                    TilesAlreadyProcessed[tilesAround[i]] = TilesAlreadyProcessed[processingPosition] + 1;
                    //GetTile(tilesAround[i]).textMesh.text = TilesAlreadyProcessed[tilesAround[i]].ToString();
                    ProcessTiles.Enqueue(tilesAround[i]);

                    //Only stop if we're specifying a desired end.
                    if (current != goal && tilesAround[i].x == current.x && tilesAround[i].y == current.y)
                    {
                        ProcessTiles.Clear();
                        break;
                    }
                }
            }
        }
        return TilesAlreadyProcessed;
    }

    public static TilePosition GetTileFromCommand(TilePosition desiredPosition, Commands _command)
    {
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
        return desiredPosition;
    }

    public static TilePosition GetTileFromCommand(Actor _actor, Commands _command)
    {
        return GetTileFromCommand(_actor.currentPosition,_command);
    }

    public static Commands GetCommandToAdjacentTile(TilePosition current, TilePosition desired)
    {
        if (current.x == desired.x && current.y == desired.y)
        {
            return Commands.None;
        }
        if (current.x == desired.x)
        {
            if (desired.y > current.y)
            {
                return Commands.Down;
            }
            else
            {
                return Commands.Up;
            }
        }
        else
        {
            if (desired.x < current.x)
            {
                return Commands.Right;
            }
            else
            {
                return Commands.Left;
            }
        }
    }

    public static Commands GetNextCommand(Commands cmd)
    {
        switch (cmd)
        {
            case Commands.Up:
                return Commands.Right;
                break;
            case Commands.Down:
                return Commands.Left;
                break;
            case Commands.Left:
                return Commands.Up;
                break;
            case Commands.Right:
                return Commands.Down;
                break;
        }
        return (Commands) Random.Range(0, 5);
    }

    public static Commands GetPrevCommand(Commands cmd)
    {
        switch (cmd)
        {
            case Commands.Up:
                return Commands.Left;
                break;
            case Commands.Down:
                return Commands.Right;
                break;
            case Commands.Left:
                return Commands.Down;
                break;
            case Commands.Right:
                return Commands.Up;
                break;
        }
        return (Commands)Random.Range(0, 5);
    }

    public static Commands GetBackCommand(Commands cmd)
    {
        return GetPrevCommand(GetPrevCommand(cmd));
    }
}
