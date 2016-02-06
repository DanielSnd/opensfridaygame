﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Different types of commands.
/// </summary>
public enum Commands
{
    None,
    Up,
    Down,
    Left,
    Right
}

/// <summary>
/// This is our base Actor class, will be extended
/// later to handle players, enemies, and other actors.
/// </summary>
public class Actor : CacheBehaviour
{
    /// <summary>
    /// Current position this actor is occupying on the board.
    /// </summary>
    public TilePosition currentPosition;

    //Actor's name reference
    private string _name;
    
    /// <summary>
    /// Static actorlist so we can access actors from anywhere.
    /// </summary>
    public static List<Actor> actorList = new List<Actor>();

    /// <summary>
    /// Property to get and set the Text reference for the Canvas Text above Actor's head.
    /// </summary>
    public new Text nameText { get { return _nameText ? _nameText : (_nameText = GetComponentInChildren<Text>()); } }
    [HideInInspector, NonSerialized]
    private Text _nameText;

    //Next command to execute on Tick.
    private Commands nextCommand = Commands.None;

    /// <summary>
    /// Property to get the name and set. Whenever the name is set it'll update the text above actor's head.
    /// </summary>
    public string actorName
    {
        get { return _name;}
        set
        {
            _name = value;
            nameText.text = value;
        }
    }

    /// <summary>
    /// Property to set or get the next command for this player's tick.
    /// </summary>
    public Commands NextCommand
    {
        get { return nextCommand; }
        set
        {
            nextCommand = value; 
            //Output the next registered command to the console so we can see it.
            Debug.Log(actorName+" registered nextcommand "+nextCommand.ToString());
        }
    }

    /// <summary>
    /// On Enable we'll add it to the static list.
    /// On Enable is called when the object becomes visible.
    /// </summary>
    public virtual void OnEnable()
    {
        if (!actorList.Contains(this)) actorList.Add(this);
    }

    /// <summary>
    /// On Disable we'll remove it from the static list.
    /// On Disable is called when the object becomes invisible.
    /// </summary>
    public virtual void OnDisable()
    {
        if (actorList.Contains(this)) actorList.Remove(this);
    }

    /// <summary>
    /// Here's where the player is going to do it's stuff on the turn tick.
    /// </summary>
    public virtual void Tick()
    {
        //DO STUFF
        Debug.Log(actorName + " TICK! "+nextCommand.ToString());
        if (nextCommand != Commands.None)
        {
            // Let's attempt to move in that direction
            bool attemptToMove = BoardManager.get.TryMove(this, nextCommand);
        }

        //Reset next command to none after using the next command.
        nextCommand = Commands.None;
    }
}
