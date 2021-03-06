using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace spacegame
{
    public class TitleScreen : MonoBehaviour
    {
        private UINavigateable menu;
        public Text versionText;

        private void Start()
        {
            CreateMainMenu();
            StartCoroutine(DebugCode());

            versionText.text = Constants.Meta.VERSION;

            foreach (Follower f in Player.followers)
                Destroy(f.gameObject);
            Player.followers.Clear();
        }

        private void CreateMainMenu()
        {
            menu = UIManager.instance.NewNavigateable(new Vector2(0, -140), new Vector2(400, 100),
                UI.PrintTextOptions.CallbackAfterInput | UI.PrintTextOptions.Instant); // don't destroy the ui after callback
            menu.SetOptions(new string[] { "new game", "load game", "configure game", "don't game" },
                new Action(() => TitleInput(menu.selectedOption)));
        }

        private void TitleInput(string selectedOption)
        {
            switch (selectedOption)
            {
                case "new game":
                    // force the ui to be ready to pop from the input queue (we don't want it to destroy when the quit confirmation appears)
                    menu.readyToDequeue = true;
                    MapManager.ChangeMap("ship_alison_intro");

                    // reset the game state dictionaries
                    // we want debug_mode to stay though, so we check if it's there
                    Dictionary<string, bool> dict = new Dictionary<string, bool>();
                    if (Global.debugMode) dict.Add("debug_mode", true);
                    GameState.ResetBooleansDictionary(dict);
                    GameState.ResetIntegersDictionary(new Dictionary<string, int>());
                    break;
                case "load game":
                    menu.readyToDequeue = true;
                    SaveLoadManager.instance.Load();
                    break;
                case "don't game":
                    UI ui = UIManager.instance.New(new Vector2(0, -140), new Vector2(400, 200));
                    StartCoroutine(ui.PrintText("see ya later!", () => Application.Quit(),
                        UI.PrintTextOptions.CallbackAfterInput));
                    break;
            }
            SFXPlayer.instance.Play("sfx_menu_confirm");
        }

        private IEnumerator DebugCode()
        {
            // MOLLY
            foreach (KeyCode k in new KeyCode[] { KeyCode.M, KeyCode.O, KeyCode.L, KeyCode.L, KeyCode.Y })
                yield return new WaitUntil(() => Input.GetKeyDown(k));

            Global.instance.EnterDebugMode();

            UI ui = UIManager.instance.New(new Vector2(-220, -60), new Vector2(440, 240));
            StartCoroutine(ui.PrintText("wow it's debug mode", 
                options: UI.PrintTextOptions.DestroyUIAfterCallback | UI.PrintTextOptions.CallbackAfterInput));
        }
    }
}
