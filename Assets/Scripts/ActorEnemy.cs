using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActorEnemy : Actor {
    
    public Dictionary<Commands,int> votedCommands = new Dictionary<Commands, int>();

    public override void Tick()
    {
        if (votedCommands.Count > 0)
        {
            NextCommand = votedCommands.Keys.OrderByDescending(x => votedCommands[x]).First();
            votedCommands.Clear();
        }
        else
        {
            if (GameManager.playerDictionary.Count > 0)
            {
                ActorPlayer desiredPlayer = BoardMethod.GetClosestPlayer(currentPosition);
                if (desiredPlayer != null)
                {
                    NextCommand = BoardMethod.GetCommandTowardsTile(currentPosition, desiredPlayer.currentPosition);
                }
            }
            else
            {
                NextCommand = (Commands)Random.Range(0, 5);
            }
        }

        base.Tick();
    }
}
