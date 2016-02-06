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
    public Actor playerPrefab;

    /// <summary>
    /// Dictionary of usernames versus player actor objects.
    /// </summary>
    public Dictionary<string,Actor> playerDictionary = new Dictionary<string, Actor>();

    /// <summary>
    /// Awake gets called when the object is spawned into the scene.
    /// </summary>
    void Awake()
    {
        //Set our singleton to be this object.
        get = this;
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
        Actor spawnedPlayer = playerPrefab.Spawn();

        //Set position.
        spawnedPlayer.currentPosition = BoardManager.get.GetRandomEmptyFloor();
        Tile emptyTile = BoardManager.get.GetTile(spawnedPlayer.currentPosition);
        spawnedPlayer.transform.position = emptyTile.transform.position;

        //Set username.
        spawnedPlayer.actorName = username;
        
        //Add to dictionary so we can reference it later by username.
        playerDictionary.Add(username,spawnedPlayer);
        
        return spawnedPlayer;
    }
}
