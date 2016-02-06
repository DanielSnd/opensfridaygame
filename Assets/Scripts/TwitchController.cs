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

    // Use this for initialization
    void Start()
    {
        //Get IRC component and register to receive messages from it.
        IRC = this.GetComponent<TwitchIRC>();
        IRC.messageRecievedEvent.AddListener(OnChatMsgRecieved);
    }

    /// <summary>
    /// This method will be called by IRC UnityEvent messageRecievedEvent
    /// </summary>
    /// <param name="msg">long ugly message with unnecessary stuff</param>
    void OnChatMsgRecieved(string msg)
    {
        //Parse the message received and separate it's pieces.
        int msgIndex = msg.IndexOf("PRIVMSG #");
        string msgString = msg.Substring(msgIndex + IRC.channelName.Length + 11);
        string user = msg.Substring(1, msg.IndexOf('!') - 1);

        //If user has a player and message starts with # handle it.
        if (GameManager.get.playerDictionary.ContainsKey(user) && msgString.ToLower().StartsWith("#"))
        {
            HandlePlayerCommand(msgString, user);
        }

        //If there are less than our max number of players in our player dictionary
        //and message is #join or #play spawn player.
        if (GameManager.get.playerDictionary.Count < maxPlayers && (msgString.ToLower().StartsWith("#join") || msgString.ToLower().StartsWith("#play")))
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
    private static void HandlePlayerCommand(string msgString, string user)
    {
        //Get just the command without the # and get a reference to player actor.
        string command = msgString.ToLower().Substring(msgString.LastIndexOf('#') + 1);
        Actor playerActor = GameManager.get.playerDictionary[user];

        if (command.StartsWith("u"))
        {
            playerActor.NextCommand = Commands.Up;
        }
        else if (command.StartsWith("d"))
        {
            playerActor.NextCommand = Commands.Down;
        }
        else if (command.StartsWith("l"))
        {
            playerActor.NextCommand = Commands.Left;
        }
        else if (command.StartsWith("r"))
        {
            playerActor.NextCommand = Commands.Right;
        }
        else if (command.StartsWith("n"))
        {
            playerActor.NextCommand = Commands.None;
        }
    }
}
