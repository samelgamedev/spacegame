using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

namespace spacegame
{
    // Project Settings > Script Execution Order for all of this mayhem
    public class Global : MonoBehaviour
    {
        public static Global instance;
        // wraps the game state bool
        public static bool debugMode
        {
            get
            {
                return GameState.GetBoolean("debug_mode");
            }
            set
            {
                GameState.SetBoolean("debug_mode", value);
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            // singleton stuff
            DontDestroyOnLoad(gameObject);
            instance = this;

            Init.Initialization();

            // hook log message received and low memory callbacks
            Application.logMessageReceived += LogMessageHandler;
            Application.lowMemory += new Application.LowMemoryCallback(() => SceneManager.LoadScene("low_memory")); // please never happen ever please never happen ever please never 
        }

        private void LogMessageHandler(string condition, string stackTrace, LogType type)
        {
            DebugUI.lastLogMessage = $"msg: {condition} type: {type}";

            // exception handler
            if (type == LogType.Exception)
            {
                try
                {
                    Logger.WriteLine($"\n===============================\n" +
                        $"exception: {condition}\n{stackTrace}" +
                        $"\n===============================\n");

                    // get prefab
                    GameObject g = PrefabManager.instance.GetPrefab("exception msg");
                    GameObject msg = Instantiate(g,
                        // ui manager is attached to the canvas
                        UIManager.instance.transform.position, Quaternion.identity, UIManager.instance.transform);
                    // set text
                    // clamp the length of the stack trace substring between 0 and 200 so we don't get an index out of range
                    msg.GetComponentInChildren<Text>().text = $"an exception was encountered:\n\n{condition}\n{stackTrace.Substring(0, Mathf.Clamp(stackTrace.Length, 0, 200)) + "..."}\n\nthe full stack trace was logged";

                    Logger.WriteLine($"\n===============================\n");

                    // this doesn't work for no reason
                    // get handy dandies to destroy the ui after the select key is pressed unless the handy dandies instance is null
                    StartCoroutine(HandyDandies.instance?.DoAfter(() => Input.GetKeyDown(InputManager.select), () => Destroy(msg)));
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("\n===============================\n" +
                        "an exception was encountered while handling an exception:" +
                        "\n===============================\n");

                    Logger.WriteLine($"original exception:\n{condition}\n{stackTrace}" +
                        $"\n===============================\n");

                    Logger.WriteLine($"new exception:\n{ex}\n===============================\n");

                    // yeah i'm not dealing with that, have fun!
                    Application.Quit();
                }
            }
        }

        public void EnterDebugMode()
        {
            debugMode = true;

            // create debug ui
            Instantiate(PrefabManager.instance.GetPrefab("debug ui"), UIManager.instance.transform);

            // register debug commands
            DebugConsole.Commands.RegisterCommands();
        }

        // the purpose of this coroutine is to give the init scene a moment to load so the loading text appears
        // as Resources.LoadAll is pretty slow
        // this is commented out for now because i don't load many resources but i might need it in the future
        /*
        IEnumerator Thing()
        {
            yield return new WaitForSeconds(0.1f);
            Init.Initialization();
        }*/
    }
}
