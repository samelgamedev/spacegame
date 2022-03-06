using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace spacegame
{
    public static class Init
    {
        public static bool initialized;

        public static void Initialization()
        {
            if (initialized) return;
            initialized = true;

            // initialize logger and speech synthesizer
            Logger.Init();

            // alisonscript initialization
            alisonscript.Interpreter.RegisterKeywords();
            alisonscript.Interpreter.RegisterFunctions();

            // load resources
            BGMPlayer.instance.bgm = Resources.LoadAll<AudioClip>("audio/bgm");
            SFXPlayer.instance.sfx = Resources.LoadAll<AudioClip>("audio/sfx");
            PrefabManager.instance.prefabs = Resources.LoadAll<GameObject>("prefabs");

            // init items
            InventoryManager.InitItems();

            // then go to title screen
            MapManager.ChangeMap("title");

            Logger.WriteLine($"initialized: {Constants.Meta.VERSION}");
        }
    }

}