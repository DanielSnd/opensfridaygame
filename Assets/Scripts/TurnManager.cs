using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// TurnManager is in charge of anything turn-related.
/// It also keeps track of ticking the actors.
/// </summary>
public class TurnManager : MonoBehaviour {

    /// <summary>
    /// This is where we actually keep the singleton value.
    /// </summary>
    private static TurnManager _instance;

    /// <summary>
    /// TurnManager is a singleton, and this is how we access it.
    /// </summary>
    public static TurnManager get
    {
        get { return _instance; }
        set { _instance = value; }
    }

    /// We store the last turn time here and check for it on Update.
    float lastTurnTime;

    /// <summary>
    /// Stores Turn Duration
    /// </summary>
    public float turnDuration = 25;

    /// <summary>
    /// We use these images to show the turn duration visually.
    /// </summary>
    public Image turnCountdownBar1;
    /// <summary>
    /// We use this second image to also show the turn duration visually.
    /// </summary>
    public Image turnCountdownBar2;

    /// <summary>
    /// Awake is called first when the object is spawned.
    /// </summary>
    void Awake()
    {
        //Here we set our singleton.
        get = this;

        //And we set our first Turntime reference.
        lastTurnTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// Update is called every frame.
    /// </summary>
    void Update()
    {
        //Here we update the fillamount of the images, so they shrink as the turn time passes.
        //We're using 2 images with opposite directions so they both shrink towards the center.
        turnCountdownBar1.fillAmount = Mathf.Clamp(1-(Time.realtimeSinceStartup - lastTurnTime)/turnDuration, 0, 1);
        turnCountdownBar2.fillAmount = Mathf.Clamp(1-(Time.realtimeSinceStartup - lastTurnTime) / turnDuration, 0, 1);
        
        //Check if we should do a turn tick now
        if (CanDoTick())
        {
            //Time to do a new turn tick.
            TurnTick();
        }
    }

    /// <summary>
    /// If the last turn time saved + the turn duration is smaller than the current time
    /// it means that it's time to do another tick.
    /// </summary>
    /// <returns>True/False if can or can't do tick</returns>
    private bool CanDoTick()
    {
        return lastTurnTime + turnDuration < Time.realtimeSinceStartup;
    }

    /// <summary>
    /// This method actually does the Turn Tick.
    /// </summary>
    void TurnTick()
    {
        //Debug log so we can see the tick happening on the Console.
        Debug.Log("DO TICK");

        //We update our last turn time reference with the current time.
        lastTurnTime = Time.realtimeSinceStartup;

        //We go over every actor currently in the board and ask them to Tick.
        for (int i = Actor.actorList.Count - 1; i >= 0; i--)
        {
            Actor.actorList[i].Tick();
        }
    }
}
