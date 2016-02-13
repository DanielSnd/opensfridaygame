using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
    /// <summary>
    /// Reference to store the singleton instance.
    /// </summary>
    private static GameManager _instance;

    /// <summary>
    /// This is how we get the singleton reference.
    /// </summary>
    public static GameManager get
    {
        get { return _instance; }
        set { _instance = value; }
    }

    /// <summary>
    /// Player prefab to spawn.
    /// </summary>
    public ActorPlayer playerPrefab;

    public ActorEnemy enemyPrefab;

    /// <summary>
    /// Dictionary of usernames versus player actor objects.
    /// </summary>
    public static Dictionary<string, ActorPlayer> playerDictionary = new Dictionary<string, ActorPlayer>();

    /// <summary>
    /// Dictionary of usernames versus enemy actor objects.
    /// </summary>
    public static Dictionary<string, ActorEnemy> enemyDictionary = new Dictionary<string, ActorEnemy>();
    
    public const int alphabettotal = 25;

    /// <summary>
    /// Awake gets called when the object is spawned into the scene.
    /// </summary>
    void Awake()
    {
        //Set our singleton to be this object.
        get = this;
    }

    public Actor SpawnEnemy()
    {
        //Spawn Player Object.
        ActorEnemy spawnedPlayer = enemyPrefab.Spawn();

        //Set position.
        spawnedPlayer.currentPosition = BoardMethod.GetRandomTile();
        Tile emptyTile = BoardMethod.GetTile(spawnedPlayer.currentPosition);
        spawnedPlayer.transform.position = emptyTile.transform.position;

        spawnedPlayer.spriteRenderer.color = Color.red;

        //Set username.
        spawnedPlayer.actorName = GetEnemyName();
        
        //Add to dictionary so we can reference it later by username.
        enemyDictionary.Add(spawnedPlayer.actorName, spawnedPlayer);

        return spawnedPlayer;
    }

    /// <summary>
    /// Spawn player for the desired username using ObjectPool pooling system.
    /// Find a random empty spot on the board and move the newly spawned player there.
    /// </summary>
    /// <param name="username">desired username for player</param>
    /// <returns>Spawned Player Prefab.</returns>
    public Actor SpawnPlayer(string username)
    {
        //If there is already a player with that username don't spawn it.
        if (playerDictionary.ContainsKey(username))
        {
            return null;
        }
        
        //Spawn Player Object.
        ActorPlayer spawnedPlayer = playerPrefab.Spawn();

        //Set position.
        spawnedPlayer.currentPosition = BoardMethod.GetRandomTile();
        Tile emptyTile = BoardMethod.GetTile(spawnedPlayer.currentPosition);
        spawnedPlayer.transform.position = emptyTile.transform.position;

        spawnedPlayer.spriteRenderer.color = Color.white;

        //Set username.
        spawnedPlayer.actorName = username;
        
        //Add to dictionary so we can reference it later by username.
        playerDictionary.Add(username,spawnedPlayer);
        
        return spawnedPlayer;
    }

    char RandomLetter()
    {
        int letterNum = Random.Range(0, alphabettotal+1);
        return (char) ('a' + letterNum);
    }

    string GetEnemyName()
    {
        for (int i = 0; i < 10; i++)
        {
            string randomName = "#" + RandomLetter() + RandomLetter() + RandomLetter();
            if (!enemyDictionary.ContainsKey(randomName)) return randomName;
        }
        return "#" + RandomLetter() + RandomLetter() + RandomLetter();
    }

    public static void ResetAll()
    {
        if (BoardManager.mapTiles != null && BoardManager.mapTiles.Length>0)
        {
            foreach (Tile _tile in BoardManager.mapTiles)
            {
                if (_tile != null)
                {
                    _tile.Recycle();
                }
            }
        }
        if (Actor.actorList.Count > 0)
        {
            foreach (Actor _actor in Actor.actorList)
            {
                if (_actor != null)
                {
                    _actor.Recycle();
                }
            }
        }
        Actor.actorList.Clear();
        enemyDictionary.Clear();
        playerDictionary.Clear();
    }
}
