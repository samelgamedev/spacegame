using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace spacegame
{
    public class DebugUI : MonoBehaviour
    {
        public GameObject debugConsole;
        public Text debugText;

        public static string lastLogMessage;
        public static bool displayGameStateFlags;

        // Update is called once per frame
        void Update()
        {
            // toggle on/off debug console
            if (Input.GetKeyDown(KeyCode.Tab))
                debugConsole.SetActive(!debugConsole.activeSelf);

            // set debug text
            // i am so sorry
            debugText.text
                = $"scene: {SceneManager.GetActiveScene().name ?? "<none, somehow>"}\n"
                + $"player world position: {(Player.instance != null ? Player.instance.gameObject.transform.position.ToString() : "<none>")}\n"
                + $"bgm: {(BGMPlayer.instance.playingClip != null ? BGMPlayer.instance.playingClip.name : "<none>")}\n"
                + $"interpreter line: {(alisonscript.Interpreter.runningScript != null ? alisonscript.Interpreter.runningScript.lines[alisonscript.Interpreter.runningScript.lineIndex] : "<none>")}\n"
                + $"cool robot: yup\n"
                + $"delta: {Time.deltaTime}\n"
                + $"rb: v: {(Player.instance != null ? Player.instance.rigidbody2d.velocity.ToString() : "<none>")} "
                + $"av: {(Player.instance != null ? Player.instance.rigidbody2d.angularVelocity.ToString() : "<none>")} "
                + $"d: {(Player.instance != null ? Player.instance.rigidbody2d.drag.ToString() : "<none>")} "
                + $"ad: {(Player.instance != null ? Player.instance.rigidbody2d.angularDrag.ToString() : "<none>")}\n"
                + $"last log message: {lastLogMessage}";
            if (displayGameStateFlags)
            {
                foreach (string key in GameState.GetBooleansDictionary().Keys)
                    debugText.text += $"\n{key}: {GameState.GetBoolean(key)}";
                foreach (string key in GameState.GetIntegersDictionary().Keys)
                    debugText.text += $"\n{key}: {GameState.GetInteger(key)}";
            }
        }
    }
}