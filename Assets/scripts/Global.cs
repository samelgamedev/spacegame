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
        public static bool debugMode;

        // Start is called before the first frame update
        void Awake()
        {
            // singleton stuff
            DontDestroyOnLoad(gameObject);
            instance = this;

            Init.Initialization();

            Application.logMessageReceived += LogMessageHandler;
        }

        private void LogMessageHandler(string condition, string stackTrace, LogType type)
        {
            DebugUI.lastLogMessage = $"msg: {condition} type: {type}";

            // exception handler
            if (type == LogType.Exception)
            {
                try
                {
                    Debug.Log($"\n===============================\n" +
                        $"exception: {condition}\n{stackTrace}" +
                        $"\n===============================\n");

                    // get prefab
                    GameObject g = PrefabManager.instance.GetPrefab("exception msg");
                    Instantiate(g,
                        // ui manager is attached to the canvas
                        UIManager.instance.transform.position, Quaternion.identity, UIManager.instance.transform);
                    // set text
                    // clamp the length of the stack trace substring between 0 and 200 so we don't get an index out of range
                    g.GetComponentInChildren<Text>().text = $"an exception was encountered:\n\n{condition}\n{stackTrace.Substring(0, Mathf.Clamp(stackTrace.Length, 0, 200)) + "..."}\n\nthe full stack trace was logged";

                    Debug.Log($"\n===============================\n");

                    // this doesn't work for no reason
                    // get handy dandies to destroy the ui after the select key is pressed unless the handy dandies instance is null
                    StartCoroutine(HandyDandies.instance?.DoAfter(() => Input.GetKeyDown(InputManager.select), () => Destroy(g)));
                }
                catch (Exception ex)
                {
                    Debug.Log("\n===============================\n" +
                        "an exception was encountered while handling an exception:" +
                        "\n===============================\n");

                    Debug.Log($"original exception:\n{condition}\n{stackTrace}" +
                        $"\n===============================\n");

                    Debug.Log($"new exception:\n{ex}\n===============================\n");

                    // yeah i'm not dealing with that, have fun!
                    Application.Quit();
                }
            }
        }

        public void EnterDebugMode()
        {
            debugMode = true;
            GameState.SetBoolean("debug_mode", true);

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
