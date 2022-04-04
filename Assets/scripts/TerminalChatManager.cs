using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace spacegame
{
    public class TerminalChatManager : MonoBehaviour
    {
        public static TerminalChatManager instance;

        private static List<string> messages = new List<string>(); // the messages that have been sent
        private static int messageOffset; // the messages to display
        // individual name colours when the text is updated
        // e.g: "alison the robot" is blue, "microwave man sam" is yellow, "vancouver" is green
        private static readonly Dictionary<string, string> nameColours = new Dictionary<string, string>
        {
            {"alison the robot", "<color=blue>alison the robot</color>" },
            {"microwave man sam", "<color=yellow>microwave man sam</color>" },
            {"vancouver", "<color=green>vancouver</color>" }
        };
        private static GameObject bg;

        public Text text; // the text to show messages on (to be assigned in inspector)
        public static bool open { get; private set; } // we only need to set this in this class; just use the Open method everywhere else

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (!open) return;

            if (Input.GetKeyDown(InputManager.cancel))
                ToggleOpen(false);

            if (Input.GetKeyDown(InputManager.select))
                AdvanceMessage(1);

            UpdateMessages();
        }

        public static void ToggleOpen(bool value)
        {
            open = value;
            if (value)
                bg = Instantiate(PrefabManager.instance.GetPrefab("terminal chat"), UIManager.instance.canvas.transform.position, Quaternion.identity, UIManager.instance.canvas.transform);
            else
                Destroy(bg);
        }

        public static void AddMessage(string message)
        {
            messages.Add(message);
        }

        public static void AdvanceMessage(int count)
        {
            // make sure the message offset can't exceed the length of the list
            messageOffset = Mathf.Clamp(messageOffset + count, 0, messages.Count);
        }

        // (.*?(?<=\n)) to match everything up to and including the first newline

        public void UpdateMessages()
        {
            text.text = string.Empty;

            for (int i = 0; i < messageOffset; i++)
            {
                string message = messages[i];
                // substitute names with their corresponding colours
                foreach (string key in nameColours.Keys)
                    message = message.Replace(key, nameColours[key]);
                if (message != string.Empty) text.text += $"{message}\n";
            }
        }
    }
}