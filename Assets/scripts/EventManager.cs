using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace spacegame
{
    public static class EventManager
    {
        // this method processes events, which are usually only in interactables
        public static void ProcessEvent(Event ev)
        {
            switch (ev?.eventType)
            {
                case Event.EventType.RunScript:
                    // run a script
                    // event argument 0 should be the name of the script and every argument afterward should be additional script args
                    alisonscript.Interpreter.Run(ev.args[0], ev.args.Skip(1).ToArray());
                    Player.instance.StopHorizontalAnimation();
                    Player.instance.canMove = false;
                    break;
                case Event.EventType.ChangeMap:
                    // change the map
                    // event argument 0 should be the name of the map and argument 1 should be the transfer point
                    MapManager.ChangeMap(ev.args[0], ev.args.Length > 1 ? int.Parse(ev.args[1]) : 0);
                    break;
                case Event.EventType.PlaySfx:
                    // play sfx
                    // event argument 0 should be the name of the sound effect
                    SFXPlayer.instance.Play(ev.args[0]);
                    break;
            }
        }

        // process an array of events instead
        public static void ProcessEvents(Event[] events)
        {
            foreach (Event e in events) ProcessEvent(e);
        }
    }
}
