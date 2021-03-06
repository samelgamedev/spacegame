using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Reflection;

namespace spacegame.alisonscript
{
    public class Functions : MonoBehaviour
    {
        public static Functions instance;
        public Dictionary<string, string> functions = new Dictionary<string, string>();

        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            instance = this;
        }

        // this is only used in the Call method to keep it tidy so it's private
        private static MemberInfo GetMemberInfoForFunction(string name)
        {
            foreach (MemberInfo m in typeof(Functions).GetMethods())
                if (m.Name == name)
                    return m;
            throw new Exception($"no alisonscript function called {name} " +
                $"(make sure you're referring to the name of the coroutine rather than the function)");
        }

        public void Call(string functionName, Action callback, params string[] args)
        {
            foreach (string s in functions.Keys)
            {
                if (s == functionName)
                {
                    // get function attribute
                    FunctionAttribute f = (FunctionAttribute)Attribute.GetCustomAttribute(GetMemberInfoForFunction(functions[s]), typeof(FunctionAttribute));

                    // if there isn't enough arguments, get outta here
                    if (args.Length < f.minimumArgs)
                        throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), 
                            $"{f.name} requires a minimum of {f.minimumArgs} arguments (only {args.Length} were given)");

                    // call function coroutine
                    StartCoroutine(functions[s], new FunctionArgs(callback, args));
                    return;
                }
            }
            throw new AlisonscriptSyntaxError(Interpreter.runningScript.GetCurrentLine(), $"{functionName} didn't match any registered alisonscript functions");
        }

        [Function("log", 1)]
        public IEnumerator Log(FunctionArgs args)
        {
            foreach (string s in args.args) 
                Logger.WriteLine(s);
            args.callback.Invoke();

            yield break;
        }

        // functions are coroutines instead of delegates solely because of wait. yup
        [Function("wait", 1)] 
        public IEnumerator Wait(FunctionArgs args)
        {
            yield return new WaitForSeconds(int.Parse(args[0]));
            args.callback.Invoke();
        }

        [Function("spk", 1)]
        public IEnumerator Speak(FunctionArgs args)
        {
            string fullText = args[0];
            UI speakerBox = null;

            // start by getting the speaker, which we do with a regex
            Regex searchForSpeaker = new Regex(@"(?<=\|)(.+?)(?=\|)");
            if (searchForSpeaker.IsMatch(fullText)) // nice! we have a speaker! let's go!
            {
                // create speaker box
                speakerBox = UIManager.instance.New(new Vector2(-152, 328), new Vector2(500, 100));

                // set alignment to middle left so it doesn't go all abjkdgfjls mnczxivpyppio89aysuid mzn.oixczxnycvluz.xcv like that
                speakerBox.SetTextAlignment(TextAnchor.MiddleLeft);

                // print the speaker onto the speaker box
                speakerBox.StartCoroutine(speakerBox.PrintText(searchForSpeaker.Match(fullText).Value, 
                    options: UI.PrintTextOptions.Instant | UI.PrintTextOptions.DontCallback));

                // then replace the contents of the speaker with an empty string
                // this regex includes the bars, eg "|guy| what a lovely day!!!" matches "|guy|"
                Regex speaker = new Regex(@"\|.*?\|");
                fullText = speaker.Replace(fullText, string.Empty);

                // i normally like adding an extra whitespace after the speaker so it's more readable so this checks for that and removes it
                // you can disable this check by including "-ndws" (no delete white space) as an argument

                if (fullText[0] == ' ' && !args.args.Contains("-ndws"))
                    fullText = fullText.Substring(1);
            }

            // create textbox
            UI textbox = UIManager.instance.New(new Vector2(-52, 169), new Vector2(700, 200));
            if (speakerBox != null)
                textbox.alsoDestroy.Add(speakerBox);
            textbox.StartCoroutine(textbox.PrintText(fullText, callback: args.callback, 
                options: UI.PrintTextOptions.CallbackAfterInput | UI.PrintTextOptions.DestroyUIAfterCallback));

            yield break;
        }

        [Function("can_move", 1)] 
        public IEnumerator ToggleMove(FunctionArgs args)
        {
            if (bool.TryParse(args[0], out bool result))
                Player.instance.canMove = result;
            else
                throw new ArgumentException($"failed to parse {args[0]} to boolean");
            args.callback.Invoke();
            yield break;
        }

        [Function("jump", 1)]
        public IEnumerator Jump(FunctionArgs args)
        {
            Interpreter.runningScript.lineIndex = int.Parse(args[0]);
            args.callback.Invoke();
            yield break;
        }

        [Function("goto", 1)]
        public IEnumerator GotoLabel(FunctionArgs args) 
        {
            foreach (Line label in from line in Interpreter.runningScript.lines where line.labelData.isLabel select line)
            {
                if (label.labelData.labelName == args[0])
                {
                    Interpreter.lineIndex = label.index;
                    Interpreter.EvaluateLine(label);
                }
            }
            args.callback.Invoke();
            yield break;
        }

        [Function("choice", 2)]
        public IEnumerator DialogueChoice(FunctionArgs args)
        {
            UINavigateable ui = UIManager.instance.NewNavigateable(new Vector2(200, 0), new Vector2(400, 50));
            ui.SetOptions(args.args.Skip(1).ToArray(), // first element of the array is the string that the navigateable ui is hovering over
                new Action(() =>
                {
                    Interpreter.runningScript.AddObject(args[0], ui.selectedOption);
                    args.callback.Invoke();
                    SFXPlayer.instance.Play("sfx_menu_confirm");
                })) ;
            yield break;
        }

        [Function("end_processing")]
        public IEnumerator EndProcessing(FunctionArgs args)
        {
            Interpreter.runningScript.Finished();
            Interpreter.DisposeRunningScript();
            yield break;
        }

        [Function("change_map", 2)]
        public IEnumerator ChangeMap(FunctionArgs args)
        {
            MapManager.ChangeMap(args[0], int.Parse(args[1]));
            args.callback.Invoke();
            yield break;
        }

        [Function("bgm", 1)]
        public IEnumerator PlayBgm(FunctionArgs args)
        {
            BGMPlayer.instance.Play(args[0]);
            args.callback.Invoke();
            yield break;
        }

        [Function("sfx", 1)]
        public IEnumerator PlaySfx(FunctionArgs args)
        {
            SFXPlayer.instance.Play(args[0]);
            args.callback.Invoke();
            yield break;
        }

        [Function("npc_move", 3)]
        public IEnumerator NpcMove(FunctionArgs args)
        {
            // get the npc we're requesting
            GameObject npc = (from g in GameObject.FindGameObjectsWithTag("npc") 
                              where g.name == args[0]
                              select g).ToArray()[0];
            // move them
            npc.GetComponent<NPC>().MoveTo(new Vector2(float.Parse(args[1]), float.Parse(args[2])));
            args.callback.Invoke();
            yield break;
        }

        [Function("foll_from_npc", 1)]
        public IEnumerator FollowerFromNpc(FunctionArgs args)
        {
            // get the npc we're requesting
            GameObject npc = (from g in GameObject.FindGameObjectsWithTag("npc")
                              where g.name == args[0]
                              select g).ToArray()[0];
            // make them a follower
            Follower follower = npc.GetComponent<NPC>().BecomeFollower();
            Player.followers.Add(follower);

            args.callback.Invoke();
            yield break;
        }

        [Function("set_gsb", 2)]
        public IEnumerator SetGameStateBool(FunctionArgs args)
        {
            GameState.SetBoolean(args[0], bool.Parse(args[1]));
            args.callback.Invoke();
            yield break;
        }

        [Function("tchat", 1)]
        public IEnumerator TerminalChat(FunctionArgs args)
        {
            TerminalChatManager.AddMessage(args[0]);
            args.callback.Invoke();
            yield break;
        }

        [Function("adv_tchat", 1)]
        public IEnumerator AdvanceTerminalChat(FunctionArgs args)
        {
            TerminalChatManager.AdvanceMessage(int.Parse(args[0]));
            args.callback.Invoke();
            yield break;
        }

        [Function("show_tchat")]
        public IEnumerator ShowTerminalChat(FunctionArgs args)
        {
            TerminalChatManager.ToggleOpen(true);
            
            // don't invoke the callback until the chat screen is closed
            yield return new WaitUntil(() => !TerminalChatManager.open);
            args.callback.Invoke();
        }

        public struct FunctionArgs
        {
            public Action callback;
            public string[] args;

            public FunctionArgs(Action callback, params string[] args)
            {
                this.callback = callback;
                this.args = args;
            }
            
            // allow accessing the args array by just mentioning the instance of the struct
            // so we can just use args[0] instead of args.args[0]
            public string this[int index]
            {
                get => args[index];
                set => args[index] = value;
            }
        }
    }
}
