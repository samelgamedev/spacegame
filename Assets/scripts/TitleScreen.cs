using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace spacegame
{
    public class TitleScreen : MonoBehaviour
    {
        private UINavigateable menu;

        private void Start()
        {
            CreateMainMenu();
            StartCoroutine(DebugCode());
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

                    PartyManager.AddPartyMember(new PartyMemberData
                    {
                        name = "alison the robot"
                    });
                    InventoryManager.GiveItem("test");
                    InventoryManager.GiveItem("cool armour");
                    InventoryManager.GiveItem("cool food");
                    InventoryManager.GiveItem("cool weapon");

                    break;
                case "don't game":
                    UI ui = UIManager.instance.New(new Vector2(0, -140), new Vector2(400, 200));
                    StartCoroutine(ui.PrintText("see ya later!", new Action(() => Application.Quit()),
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
