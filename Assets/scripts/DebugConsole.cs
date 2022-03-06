using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

namespace spacegame
{
    public class DebugConsole : MonoBehaviour
    {
        // the text in the ui and the command we've written
        private Text text;
        private string command = string.Empty;
        private string lastText; // the text in the console prior to the execution of the command

        // singleton instance
        private static DebugConsole instance;
        
        private void Awake()
        {
            text = GetComponentInChildren<Text>();
            instance = this;

            text.text = "[alison]$";
        }

        // dictionary of keycodes and delegates for navigating around the console 
        private Dictionary<KeyCode, Action> navigationKeybinds = new Dictionary<KeyCode, Action>
        {
            {KeyCode.Backspace, () => instance.Backspace()},
            {KeyCode.Return, () => instance.Enter() },
        };

        private bool WrappedKeyDown(KeyCode input, out KeyCode output)
        {
            output = input;
            return Input.GetKeyDown(input);
        }

        private void ClearCommand()
        {
            command = string.Empty;
            lastText = text.text;
        }

        private void Update()
        {
            // add keyboard input to command string
            switch (Input.inputString)
            {
                // don't do anything if it's a special character like carriage return
                case "\r":
                    break;
                default:
                    command += Input.inputString;
                    break;
            }
            text.text = $"{lastText}[alison]$ {command}";

            // OK so this is a bit dumb but if i just write a hundred ifs then i don't satiate my massive programmer ego
            // basically we have all of the funky navigation stuff like backspace and moving back and forward through the string
            // in a dictionary with KeyCode keys and Action values
            // THEN we wrap KeyDown and skadoobly woobly and that's how we do it i am awesome
            KeyCode output = KeyCode.None;
            if (navigationKeybinds.Keys.Any(k => WrappedKeyDown(k, out output)))
            {
                navigationKeybinds[output].Invoke();
            }
        }

        public void Out(string input)
        {
            text.text += $"\n{input}\n";
        }

        private void Backspace()
        {
            if (command.Length > 1)
                command = command.Remove(command.Length - 2);
        }

        // TODO: comment this
        private void Enter()
        {
            if (command != string.Empty)
            {
                string commandName = command.Contains(' ') ? command.Substring(0, command.IndexOf(' ')) : command;
                string[] args = command.Replace($"{commandName}", string.Empty).Split(' ');

                Commands.Call(commandName, args);
            }
            ClearCommand();
        }

        public static class Commands
        {
            public static Dictionary<string, Delegate> commands = new Dictionary<string, Delegate>();
            public delegate void Command(string[] args);

            public static void RegisterCommands()
            {
                foreach (MethodInfo m in typeof(Commands).GetMethods())
                {
                    // try to get the command attribute from the method
                    CommandAttribute command = (CommandAttribute)m.GetCustomAttribute(typeof(CommandAttribute));

                    // if it exists, add it to the commands dictionary
                    if (command != null)
                    {
                        commands.Add(command.name, m.CreateDelegate(typeof(Command)));
                        Logger.WriteLine($"registered debug command: {command.name} ({m.Name})");
                    }
                }

                Logger.WriteLine("finished registering debug commands");
            }

            public static void Call(string command, string[] args)
            {
                try
                {
                    args = args.Skip(1).ToArray();

                    foreach (string s in commands.Keys)
                    {
                        if (s == command)
                        {
                            CommandAttribute c = (CommandAttribute)commands[s].GetMethodInfo().GetCustomAttribute(typeof(CommandAttribute));
                            // catch insufficient args
                            if (args.Length < c.minimumArgs)
                            {
                                instance.Out($"insufficient args for command \"{command}\" ({c.minimumArgs} expected, received {args.Length})");
                                return;
                            }

                            // Logger.WriteLine($"exception: {ex}");
                            // instance.Out(ex.Message);
                            commands[command].DynamicInvoke(new object[] { args });
                            return;
                        }
                    }
                    instance.Out($"no debug command \"{command}\"");
                }
                catch (Exception ex)
                {
                    Logger.WriteLine($"exception: {ex}");
                    instance.Out(ex.Message);
                }
            }

            [Command("help")]
            public static void Help(string[] args)
            {
                instance.Out($"commands:\n{string.Join(",", commands.Keys)}");
            }

            [Command("clear")]
            public static void Clear(string[] args)
            {
                instance.text.text = string.Empty;
            }

            [Command("change_map", 1)]
            public static void ChangeMap(string[] args)
            {
                MapManager.ChangeMap(args[0], args.Length > 1 ? int.Parse(args[1]) : 0);
            }

            [Command("maps")]
            public static void MapList(string[] args)
            {
                string[] sceneNames = new string[SceneManager.sceneCount];

                for (int i = 0; i < SceneManager.sceneCount; i++)
                    sceneNames[i] = SceneManager.GetSceneAt(i).name;

                instance.Out(string.Join(",", sceneNames));
            }

            [Command("cam_size", 1)]
            public static void CameraSize(string[] args)
            {
                MainCamera.instance.GetComponent<Camera>().orthographicSize = float.Parse(args[0]);
            }

            [Command("scripts")]
            public static void ScriptsList(string[] args)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(Application.streamingAssetsPath + "/alisonscript", "*.alisonscript", SearchOption.AllDirectories);
                List<string> filesList = files.ToList();
                filesList.ForEach((string file) => file = file.Remove(0, file.LastIndexOf("\\")));

                instance.Out(string.Join(",", filesList));
            }

            [Command("give_item", 1)]
            public static void GiveItem(string[] args)
            {
                InventoryManager.GiveItem(args[0]);
            }

            [Command("noclip")]
            public static void ToggleNoclip(string[] args)
            {
                // noclip already on?
                if (GameState.GetBoolean("debug_noclip"))
                {
                    // reset gravity back to 1 and remove vertical movement
                    Player.instance.SetGravity(1);
                    InputManager.instance.RemoveEvent(Constants.Input.VERTICAL_KEY_HELD, Player.instance.VerticalMovement);
                }
                else
                {
                    // set gravity to 0 and allow vertical movement
                    Player.instance.SetGravity(0);
                    InputManager.instance.AddEvent(Constants.Input.VERTICAL_KEY_HELD, Player.instance.VerticalMovement);

                    // not really sure what'd happen if you tried enabling noclip on a ladder but i'm like 98% sure it'd break
                    // so i'll just do this for now
                    Player.instance.onLadder = false;
                }

                // toggle collider enabled and noclip state boolean
                Player.instance.coll.enabled = !Player.instance.coll.enabled;
                GameState.SetBoolean("debug_noclip", !GameState.GetBoolean("debug_noclip"));

                instance.Out($"noclip is now {GameState.GetBoolean("debug_noclip")}");
            }

            [Command("reset_velocity")]
            public static void ResetVelocity(string[] args)
            {
                Player.instance.rigidbody2d.velocity = Vector2.zero;
            }

            [Command("logs")]
            public static void OpenLogs(string[] args)
            {
                Process.Start(Logger.logsPath);
            }

            [Command("gsvis")]
            public static void GameStateVisualizer(string[] args)
            {
                DebugUI.displayGameStateFlags = !DebugUI.displayGameStateFlags;
            }

            [Command("script", 1)]
            public static void RunScript(string[] args)
            {
                alisonscript.Interpreter.Run(args[0]);
            }
        }

        private class CommandAttribute : Attribute
        {
            public string name;
            public int minimumArgs;

            public CommandAttribute(string name) : this(name, 0) { }
            public CommandAttribute(string name, int minimumArgs)
            {
                this.name = name;
                this.minimumArgs = minimumArgs;
            }
        }
    }
}
