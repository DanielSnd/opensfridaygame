using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(TwitchIRC))]
public class TwitchController : MonoBehaviour {
    private TwitchIRC IRC;
    
    /// <summary>
    /// Maximum amount of players to spawn at a time.
    /// </summary>
    public int maxPlayers = 4;

    public static HashSet<string> usersVotedThisTurn = new HashSet<string>();

    // Use this for initialization
    void Start()
    {
        //Get IRC component and register to receive messages from it.
        IRC = this.GetComponent<TwitchIRC>();
        IRC.StartIRC();
        IRC.messageReceivedEvent.AddListener(OnChatMsgReceived);
    }

    /// <summary>
    /// This method will be called by IRC UnityEvent messageRecievedEvent
    /// </summary>
    /// <param name="msg">long ugly message with unnecessary stuff</param>
    void OnChatMsgReceived(string msg)
    {
        //Parse the message received and separate it's pieces.
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);

        string[] param = msgString.Split(' ');

        //If user has a player and message starts with # handle it.
        if (GameManager.playerDictionary.ContainsKey(user) && msgString.ToLower().StartsWith("#"))
        {
            HandlePlayerCommand(msgString, user);
        }

        //If user has a player and message starts with # handle it.
        if (!usersVotedThisTurn.Contains(user) && GameManager.enemyDictionary.ContainsKey(param[0].ToLower()))
        {
            if (param.Length > 1)
            {
                HandleEnemyCommand(param[1].ToLower(), param[0].ToLower(), user);
            }
        }

        //If there are less than our max number of players in our player dictionary
        //and message is #join or #play spawn player.
        if (GameManager.playerDictionary.Count < maxPlayers && (msgString.ToLower().StartsWith("#join") || msgString.ToLower().StartsWith("#play")) && BoardManager.get.boardReady)
        {
            GameManager.get.SpawnPlayer(user);
        }

        //Show the message in the console, so we can see what it was.
        Debug.Log("["+user+"]: "+msgString);
    }

    /// <summary>
    /// This method receives a command and a username and adds a nextcommand
    /// to the username's player actor.
    /// </summary>
    /// <param name="msgString">message</param>
    /// <param name="user">username</param>
    private static void HandleEnemyCommand(string command, string enemyName,string user)
    {
        if (!GameManager.enemyDictionary.ContainsKey(enemyName)) return;

        ActorEnemy desiredEnemy = GameManager.enemyDictionary[enemyName];
        string cmd = command.Replace("#", "");
        Debug.Log(user+" tried to vote for "+enemyName+" to do "+cmd);
        Commands voteCommand = GetCommand(cmd);
        if (!desiredEnemy.votedCommands.ContainsKey(voteCommand))
        {
            Debug.Log("Success added");
            desiredEnemy.votedCommands.Add(voteCommand, 1);
        }
        else
        {
            Debug.Log("Success counted");
            desiredEnemy.votedCommands[voteCommand]++;
        }
        usersVotedThisTurn.Add(user);
    }

    /// <summary>
    /// This method receives a command and a username and adds a nextcommand
    /// to the username's player actor.
    /// </summary>
    /// <param name="msgString">message</param>
    /// <param name="user">username</param>
    private static void HandlePlayerCommand(string msgString, string user)
    {
        //Get just the command without the # and get a reference to player actor.
        string command = msgString.ToLower().Substring(msgString.LastIndexOf('#') + 1);
        Actor playerActor = GameManager.playerDictionary[user];

        usersVotedThisTurn.Add(user);

        playerActor.NextCommand = GetCommand(command);
    }

    public static Commands GetCommand(string cstr)
    {
        if (cstr.StartsWith("u"))
        {
            return Commands.Up;
        }
        else if (cstr.StartsWith("d"))
        {
            return Commands.Down;
        }
        else if (cstr.StartsWith("l"))
        {
            return Commands.Left;
        }
        else if (cstr.StartsWith("r"))
        {
            return Commands.Right;
        }
        else if (cstr.StartsWith("n"))
        {
            return Commands.None;
        }

        return Commands.None;
    }
}
