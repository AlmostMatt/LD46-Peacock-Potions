using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventState
{
    // populate this data and the event UI will pull from it
    public static GameEvent currentEvent;
    public static string currentEventText = "DEFAULT TEXT!";
    public static string[] currentEventOptions = new string[]{ "uninitialized option 1", "uninitialized option 2" };

    // could track history here too, maybe. or somewhere else, idk

}
